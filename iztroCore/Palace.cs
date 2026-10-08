namespace iztroCore
{
    /// <summary>命身宫、五行局、宫名、大限小限，移植自 iztro astro/palace.ts</summary>
    public static class PalaceUtil
    {
        /// <summary>结果：命宫身宫及命宫干支</summary>
        public class SoulAndBody
        {
            public int SoulIndex;
            public int BodyIndex;
            public string HeavenlyStemOfSoul = "";
            public string EarthlyBranchOfSoul = "";
        }

        /// <summary>获取命宫、身宫及命宫干支（五虎遁定寅宫天干）</summary>
        public static SoulAndBody GetSoulAndBody(PanContext ctx)
        {
            var timeBranchIdx = ctx.TimeIndex % 12;
            var heavenlyStemOfYear = Util.Stems[ctx.Ganzhi.Yearly.Stem];

            var monthIndex = ctx.LunarMonthIndex;
            var soulIndex = Util.FixIndex(monthIndex - timeBranchIdx);
            var bodyIndex = Util.FixIndex(monthIndex + timeBranchIdx);

            var startStem = TigerRule.Rule[heavenlyStemOfYear];
            var heavenlyStemOfSoulIdx = Util.FixIndex(Util.StemIdx(startStem) + soulIndex, 10);
            var heavenlyStemOfSoul = Util.Stems[heavenlyStemOfSoulIdx];
            var earthlyBranchOfSoul = Util.Branches[Util.FixIndex(soulIndex + 2)];

            return new SoulAndBody
            {
                SoulIndex = soulIndex,
                BodyIndex = bodyIndex,
                HeavenlyStemOfSoul = heavenlyStemOfSoul,
                EarthlyBranchOfSoul = earthlyBranchOfSoul,
            };
        }

        /// <summary>定五行局法（以命宫天干地支而定）。返回 key，如 wood3rd。</summary>
        public static string GetFiveElementsClass(string heavenlyStemKey, string earthlyBranchKey)
        {
            string[] table = ["wood3rd", "metal4th", "water2nd", "fire6th", "earth5th"];
            var heavenlyStemNumber = Util.StemIdx(heavenlyStemKey) / 2 + 1;
            var earthlyBranchNumber = Util.FixIndex(Util.BranchIdx(earthlyBranchKey), 6) / 2 + 1;
            var index = heavenlyStemNumber + earthlyBranchNumber;
            while (index > 5) index -= 5;
            return table[index - 1];
        }

        /// <summary>获取从寅宫开始的各个宫名（key）</summary>
        public static string[] GetPalaceNames(int fromIndex)
        {
            var names = new string[12];
            for (var i = 0; i < 12; i++)
            {
                names[i] = Util.T(Heaven.Palaces[Util.FixIndex(i - fromIndex)]);
            }

            return names;
        }

        /// <summary>大限段数据</summary>
        public class Decadal
        {
            public int Start;
            public int End;
            public string HeavenlyStem = "";
            public string EarthlyBranch = "";
        }

        /// <summary>起大限与小限：返回 (decadals[12], ages[12])</summary>
        public static (Decadal[] decadals, int[][] ages) GetHoroscope(PanContext ctx)
        {
            var genderKey = ctx.GenderKey;
            var heavenlyStem = Util.Stems[ctx.Ganzhi.Yearly.Stem];
            var earthlyBranch = Util.Branches[ctx.Ganzhi.Yearly.Branch];

            var soul = GetSoulAndBody(ctx);
            var fiveElementsClass = GetFiveElementsClass(soul.HeavenlyStemOfSoul, soul.EarthlyBranchOfSoul);
            var startAge = Heaven.FiveElementsClassValue[fiveElementsClass];

            var startHeavenlyStem = TigerRule.Rule[heavenlyStem];
            var yinYang = EarthlyBranchInfo.Info[earthlyBranch].YinYang;
            var forward = Heaven.Gender[genderKey] == yinYang;

            var decadals = new Decadal[12];
            for (var i = 0; i < 12; i++)
            {
                var idx = forward ? Util.FixIndex(soul.SoulIndex + i) : Util.FixIndex(soul.SoulIndex - i);
                var start = startAge + 10 * i;
                var heavenlyStemIndex = Util.FixIndex(Util.StemIdx(startHeavenlyStem) + idx, 10);
                var earthlyBranchIndex = Util.FixIndex(2 + idx);

                decadals[idx] = new Decadal
                {
                    Start = start,
                    End = start + 9,
                    HeavenlyStem = Util.Stems[heavenlyStemIndex],
                    EarthlyBranch = Util.Branches[earthlyBranchIndex],
                };
            }

            var ageIdx = Util.GetAgeIndex(earthlyBranch);
            var ages = new int[12][];
            for (var i = 0; i < 12; i++)
            {
                var age = new List<int>();
                for (var j = 0; j < 10; j++) age.Add(12 * j + i + 1);
                var idx = genderKey == "male" ? Util.FixIndex(ageIdx + i) : Util.FixIndex(ageIdx - i);
                ages[idx] = [.. age];
            }

            return (decadals, ages);
        }
    }
}