namespace iztroCore
{
    /// <summary>排盘公共参数上下文</summary>
    public class PanContext
    {
        public DateTime Solar;
        public int TimeIndex;
        public bool FixLeap = true;
        public required GanzhiDate Ganzhi;
        public required LunarDate Lunar;

        /// <summary>农历月索引（正月=0，fixLunarMonthIndex）</summary>
        public int LunarMonthIndex;

        /// <summary>农历日索引（fixLunarDayIndex）</summary>
        public int LunarDayIndex;
        public string GenderKey = "male";
    }

    /// <summary>星曜落宫索引函数，移植自 iztro star/location.ts</summary>
    public static class Location
    {
        // ---- 内部小助手 ----
        /// <summary>地支 key 相对寅宫的宫位索引（0~11）</summary>
        private static int P(string branchKey) => Util.FixEarthlyBranchIndex(branchKey);

        /// <summary>地支 key 完整索引（0~11，子=0）</summary>
        private static int B(string branchKey) => Util.BranchIdx(branchKey);

        /// <summary>天干 key 索引（0~9）</summary>
        private static int S(string stemKey) => Util.StemIdx(stemKey);

        /// <summary>起紫微星诀：返回(ziweiIndex, tianfuIndex)</summary>
        public static (int ziweiIndex, int tianfuIndex) GetStartIndex(
            int lunarDay, int maxDays, int timeIndex, string soulStemKey, string soulBranchKey,
            string? fromStemKey, string? fromBranchKey, string dayDivide = "forward")
        {
            var baseStem = fromStemKey ?? soulStemKey;
            var baseBranch = fromBranchKey ?? soulBranchKey;
            var fiveElements = PalaceUtil.GetFiveElementsClass(baseStem, baseBranch);
            var fiveElementsValue = Heaven.FiveElementsClassValue[fiveElements];

            var _day = timeIndex == 12 && dayDivide != "current" ? lunarDay + 1 : lunarDay;
            if (_day > maxDays) _day -= maxDays;

            var offset = -1;
            int quotient = 0, remainder = -1;
            do
            {
                offset++;
                var divisor = _day + offset;
                quotient = divisor / fiveElementsValue;
                remainder = divisor % fiveElementsValue;
            } while (remainder != 0);

            quotient %= 12;
            var ziweiIndex = quotient - 1;
            ziweiIndex = offset % 2 == 0 ? ziweiIndex + offset : ziweiIndex - offset;
            ziweiIndex = Util.FixIndex(ziweiIndex);
            var tianfuIndex = Util.FixIndex(12 - ziweiIndex);
            return (ziweiIndex, tianfuIndex);
        }

        /// <summary>禄存、擎羊、陀罗、天马（按年干支）</summary>
        public static (int luIndex, int maIndex, int yangIndex, int tuoIndex) GetLuYangTuoMaIndex(string heavenlyStemKey, string earthlyBranchKey)
        {
            int luIndex = -1, maIndex = -1;

            switch (earthlyBranchKey)
            {
                case "yinEarthly": case "wuEarthly": case "xuEarthly": maIndex = P("shenEarthly"); break;
                case "shenEarthly": case "ziEarthly": case "chenEarthly": maIndex = P("yinEarthly"); break;
                case "siEarthly": case "youEarthly": case "chouEarthly": maIndex = P("haiEarthly"); break;
                case "haiEarthly": case "maoEarthly": case "weiEarthly": maIndex = P("siEarthly"); break;
            }

            switch (heavenlyStemKey)
            {
                case "jiaHeavenly": luIndex = P("yinEarthly"); break;
                case "yiHeavenly": luIndex = P("maoEarthly"); break;
                case "bingHeavenly":
                case "wuHeavenly": luIndex = P("siEarthly"); break;
                case "dingHeavenly":
                case "jiHeavenly": luIndex = P("wuEarthly"); break;
                case "gengHeavenly": luIndex = P("shenEarthly"); break;
                case "xinHeavenly": luIndex = P("youEarthly"); break;
                case "renHeavenly": luIndex = P("haiEarthly"); break;
                case "guiHeavenly": luIndex = P("ziEarthly"); break;
            }

            return (luIndex, maIndex, Util.FixIndex(luIndex + 1), Util.FixIndex(luIndex - 1));
        }

        /// <summary>天魁、天钺（按年干）</summary>
        public static (int kuiIndex, int yueIndex) GetKuiYueIndex(string heavenlyStemKey)
        {
            return heavenlyStemKey switch
            {
                "jiaHeavenly" or "wuHeavenly" or "gengHeavenly" => (P("chouEarthly"), P("weiEarthly")),
                "yiHeavenly" or "jiHeavenly" => (P("ziEarthly"), P("shenEarthly")),
                "xinHeavenly" => (P("wuEarthly"), P("yinEarthly")),
                "bingHeavenly" or "dingHeavenly" => (P("haiEarthly"), P("youEarthly")),
                "renHeavenly" or "guiHeavenly" => (P("maoEarthly"), P("siEarthly")),
                _ => (0, 0),
            };
        }

        /// <summary>左辅、右弼（按生月，lunarMonth 1~12）</summary>
        public static (int zuoIndex, int youIndex) GetZuoYouIndex(int lunarMonth)
            => (Util.FixIndex(P("chenEarthly") + (lunarMonth - 1)), Util.FixIndex(P("xuEarthly") - (lunarMonth - 1)));

        /// <summary>文昌、文曲（按时间）</summary>
        public static (int changIndex, int quIndex) GetChangQuIndex(int timeIndex)
        {
            var fixedTime = Util.FixIndex(timeIndex);
            return (Util.FixIndex(P("xuEarthly") - fixedTime), Util.FixIndex(P("chenEarthly") + fixedTime));
        }

        /// <summary>三台、八座、恩光、天贵（日系星）</summary>
        public static (int santaiIndex, int bazuoIndex, int enguangIndex, int tianguiIndex) GetDailyStarIndex(int lunarDayIndex, int timeIndex, int lunarMonthIndex)
        {
            var (zuoIndex, youIndex) = GetZuoYouIndex(lunarMonthIndex + 1);
            var (changIndex, quIndex) = GetChangQuIndex(timeIndex);
            var dayIndex = Util.FixLunarDayIndex(lunarDayIndex, timeIndex);
            var santaiIndex = Util.FixIndex((zuoIndex + dayIndex) % 12);
            var bazuoIndex = Util.FixIndex((youIndex - dayIndex) % 12);
            var enguangIndex = Util.FixIndex(((changIndex + dayIndex) % 12) - 1);
            var tianguiIndex = Util.FixIndex(((quIndex + dayIndex) % 12) - 1);
            return (santaiIndex, bazuoIndex, enguangIndex, tianguiIndex);
        }

        /// <summary>台辅、封诰（时系星）</summary>
        public static (int taifuIndex, int fenggaoIndex) GetTimelyStarIndex(int timeIndex)
        {
            var fixedTime = Util.FixIndex(timeIndex);
            return (Util.FixIndex(P("wuEarthly") + fixedTime), Util.FixIndex(P("yinEarthly") + fixedTime));
        }

        /// <summary>地空、地劫（按时支）</summary>
        public static (int kongIndex, int jieIndex) GetKongJieIndex(int timeIndex)
        {
            var fixedTime = Util.FixIndex(timeIndex);
            var hai = P("haiEarthly");
            return (Util.FixIndex(hai - fixedTime), Util.FixIndex(hai + fixedTime));
        }

        /// <summary>火星、铃星（按年支及时支）</summary>
        public static (int huoIndex, int lingIndex) GetHuoLingIndex(string earthlyBranchKey, int timeIndex)
        {
            var fixedTime = Util.FixIndex(timeIndex);
            return earthlyBranchKey switch
            {
                "yinEarthly" or
                "wuEarthly" or
                "xuEarthly"
                   => (Util.FixIndex(P("chouEarthly") + fixedTime), Util.FixIndex(P("maoEarthly") + fixedTime)),
                "shenEarthly" or
                "ziEarthly" or
                "chenEarthly"
                   => (Util.FixIndex(P("yinEarthly") + fixedTime), Util.FixIndex(P("xuEarthly") + fixedTime)),
                "siEarthly" or
                "youEarthly" or
                "chouEarthly"
                   => (Util.FixIndex(P("maoEarthly") + fixedTime), Util.FixIndex(P("xuEarthly") + fixedTime)),
                "haiEarthly" or
                "weiEarthly" or
                "maoEarthly"
                   => (Util.FixIndex(P("youEarthly") + fixedTime), Util.FixIndex(P("xuEarthly") + fixedTime)),
                _ => (0, 0)
            };
        }

        /// <summary>红鸾、天喜（按年支）</summary>
        public static (int hongluanIndex, int tianxiIndex) GetLuanXiIndex(string earthlyBranchKey)
        {
            var hongluanIndex = Util.FixIndex(P("maoEarthly") - B(earthlyBranchKey));
            return (hongluanIndex, Util.FixIndex(hongluanIndex + 6));
        }

        /// <summary>华盖、咸池（按年支）</summary>
        public static (int huagaiIndex, int xianchiIndex) GetHuagaiXianchiIndex(string earthlyBranchKey)
        {
            int huagai, xianchi;
            switch (earthlyBranchKey)
            {
                case "yinEarthly":
                case "wuEarthly":
                case "xuEarthly":
                    huagai = P("xuEarthly"); xianchi = P("maoEarthly"); break;
                case "shenEarthly":
                case "ziEarthly":
                case "chenEarthly":
                    huagai = P("chenEarthly"); xianchi = P("youEarthly"); break;
                case "siEarthly":
                case "youEarthly":
                case "chouEarthly":
                    huagai = P("chouEarthly"); xianchi = P("wuEarthly"); break;
                default:
                    huagai = P("weiEarthly"); xianchi = P("ziEarthly"); break;
            }

            return (Util.FixIndex(huagai), Util.FixIndex(xianchi));
        }

        /// <summary>孤辰、寡宿（按年支）</summary>
        public static (int guchenIndex, int guasuIndex) GetGuGuaIndex(string earthlyBranchKey)
        {
            int gu, gua;
            switch (earthlyBranchKey)
            {
                case "yinEarthly":
                case "maoEarthly":
                case "chenEarthly":
                    gu = P("siEarthly"); gua = P("chouEarthly"); break;
                case "siEarthly":
                case "wuEarthly":
                case "weiEarthly":
                    gu = P("shenEarthly"); gua = P("chenEarthly"); break;
                case "shenEarthly":
                case "youEarthly":
                case "xuEarthly":
                    gu = P("haiEarthly"); gua = P("weiEarthly"); break;
                default:
                    gu = P("yinEarthly"); gua = P("xuEarthly"); break;
            }

            return (Util.FixIndex(gu), Util.FixIndex(gua));
        }

        /// <summary>劫杀（按年支）</summary>
        public static int GetJieshaAdjIndex(string earthlyBranchKey)
        {
            return earthlyBranchKey switch
            {
                "shenEarthly" or "ziEarthly" or "chenEarthly" => 3,
                "haiEarthly" or "maoEarthly" or "weiEarthly" => 6,
                "yinEarthly" or "wuEarthly" or "xuEarthly" => 9,
                _ => 0
            };
        }

        /// <summary>大耗（按年支，阴阳移位过一宫）</summary>
        public static int GetDahaoIndex(string earthlyBranchKey)
        {
            string[] matched = ["weiEarthly", "wuEarthly", "youEarthly", "shenEarthly", "haiEarthly", "xuEarthly", "chouEarthly", "ziEarthly", "maoEarthly", "yinEarthly", "siEarthly", "chenEarthly"];
            var m = matched[B(earthlyBranchKey)];
            return Util.FixIndex(B(m) - 2);
        }

        /// <summary>天伤、天使索引</summary>
        public static (int tianshangIndex, int tianshiIndex) GetTianshiTianshangIndex(string genderKey, string earthlyBranchKey, int soulIndex)
        {
            var yinyang = B(earthlyBranchKey) % 2;
            var genderYinyang = genderKey == "male" ? 0 : 1;
            var tianshangIndex = Util.FixIndex(Array.IndexOf(Heaven.Palaces, "friendsPalace") + soulIndex);
            var tianshiIndex = Util.FixIndex(Array.IndexOf(Heaven.Palaces, "healthPalace") + soulIndex);
            return (tianshangIndex, tianshiIndex);
        }

        /// <summary>年解（按年支，解神从戌上起子逆数至生年支）</summary>
        public static int GetNianjieIndex(string earthlyBranchKey)
        {
            string[] arr = ["xuEarthly", "youEarthly", "shenEarthly", "weiEarthly", "wuEarthly", "siEarthly", "chenEarthly", "maoEarthly", "yinEarthly", "chouEarthly", "ziEarthly", "haiEarthly"];
            return Util.FixIndex(P(arr[B(earthlyBranchKey)]));
        }

        /// <summary>年系星索引全集</summary>
        public static YearlyStarIndexes GetYearlyStarIndex(PanContext ctx, int soulIndex, int bodyIndex)
        {
            var heavenlyStem = Util.Stems[ctx.Ganzhi.Yearly.Stem];
            var earthlyBranch = Util.Branches[ctx.Ganzhi.Yearly.Branch];
            var heavenlyStemIdx = ctx.Ganzhi.Yearly.Stem;
            var earthlyBranchIdx = ctx.Ganzhi.Yearly.Branch;

            var (huagaiIndex, xianchiIndex) = GetHuagaiXianchiIndex(earthlyBranch);
            var (guchenIndex, guasuIndex) = GetGuGuaIndex(earthlyBranch);
            var tiancaiIndex = Util.FixIndex(soulIndex + earthlyBranchIdx);
            var tianshouIndex = Util.FixIndex(bodyIndex + earthlyBranchIdx);

            string[] tianchuArr = ["siEarthly", "wuEarthly", "ziEarthly", "siEarthly", "wuEarthly", "shenEarthly", "yinEarthly", "wuEarthly", "youEarthly", "haiEarthly"];
            var tianchuIndex = Util.FixIndex(P(tianchuArr[heavenlyStemIdx]));

            string[] posuiArr = ["siEarthly", "chouEarthly", "youEarthly"];
            var posuiIndex = Util.FixIndex(P(posuiArr[earthlyBranchIdx % 3]));

            string[] feilianArr = ["shenEarthly", "youEarthly", "xuEarthly", "siEarthly", "wuEarthly", "weiEarthly", "yinEarthly", "maoEarthly", "chenEarthly", "haiEarthly", "ziEarthly", "chouEarthly"];
            var feilianIndex = Util.FixIndex(P(feilianArr[earthlyBranchIdx]));

            var longchiIndex = Util.FixIndex(P("chenEarthly") + earthlyBranchIdx);
            var fenggeIndex = Util.FixIndex(P("xuEarthly") - earthlyBranchIdx);
            var tiankuIndex = Util.FixIndex(P("wuEarthly") - earthlyBranchIdx);
            var tianxuIndex = Util.FixIndex(P("wuEarthly") + earthlyBranchIdx);

            string[] tianguanArr = ["weiEarthly", "chenEarthly", "siEarthly", "yinEarthly", "maoEarthly", "youEarthly", "haiEarthly", "youEarthly", "xuEarthly", "wuEarthly"];
            var tianguanIndex = Util.FixIndex(P(tianguanArr[heavenlyStemIdx]));

            string[] tianfuArr = ["youEarthly", "shenEarthly", "ziEarthly", "haiEarthly", "maoEarthly", "yinEarthly", "wuEarthly", "siEarthly", "wuEarthly", "siEarthly"];
            var tianfuIndex = Util.FixIndex(P(tianfuArr[heavenlyStemIdx]));

            var tiandeIndex = Util.FixIndex(P("youEarthly") + earthlyBranchIdx);
            var yuedeIndex = Util.FixIndex(P("siEarthly") + earthlyBranchIdx);
            var tiankongIndex = Util.FixIndex(P(earthlyBranch) + 1);

            string[] jieluArr = ["shenEarthly", "wuEarthly", "chenEarthly", "yinEarthly", "ziEarthly"];
            var jieluIndex = Util.FixIndex(P(jieluArr[heavenlyStemIdx % 5]));

            string[] kongwangArr = ["youEarthly", "weiEarthly", "siEarthly", "maoEarthly", "chouEarthly"];
            var kongwangIndex = Util.FixIndex(P(kongwangArr[heavenlyStemIdx % 5]));

            int xunkongIndex = Util.FixIndex(P(earthlyBranch) + S("guiHeavenly") - heavenlyStemIdx + 1);
            var yinyang = earthlyBranchIdx % 2;
            if (yinyang != xunkongIndex % 2) xunkongIndex = Util.FixIndex(xunkongIndex + 1);

            var jiekongIndex = yinyang == 0 ? jieluIndex : kongwangIndex;
            var jieshaAdjIndex = GetJieshaAdjIndex(earthlyBranch);
            var nianjieIndex = GetNianjieIndex(earthlyBranch);
            var dahaoAdjIndex = GetDahaoIndex(earthlyBranch);
            var (tianshangIndex, tianshiIndex) = GetTianshiTianshangIndex(ctx.GenderKey, earthlyBranch, soulIndex);

            return new YearlyStarIndexes
            {
                XianchiIndex = xianchiIndex,
                HuagaiIndex = huagaiIndex,
                GuchenIndex = guchenIndex,
                GuasuIndex = guasuIndex,
                TiancaiIndex = tiancaiIndex,
                TianshouIndex = tianshouIndex,
                TianchuIndex = tianchuIndex,
                PosuiIndex = posuiIndex,
                FeilianIndex = feilianIndex,
                LongchiIndex = longchiIndex,
                FenggeIndex = fenggeIndex,
                TiankuIndex = tiankuIndex,
                TianxuIndex = tianxuIndex,
                TianguanIndex = tianguanIndex,
                TianfuIndex = tianfuIndex,
                TiandeIndex = tiandeIndex,
                YuedeIndex = yuedeIndex,
                TiankongIndex = tiankongIndex,
                JieluIndex = jieluIndex,
                KongwangIndex = kongwangIndex,
                XunkongIndex = xunkongIndex,
                TianshangIndex = tianshangIndex,
                TianshiIndex = tianshiIndex,
                JiekongIndex = jiekongIndex,
                JieshaAdjIndex = jieshaAdjIndex,
                NianjieIndex = nianjieIndex,
                DahaoAdjIndex = dahaoAdjIndex,
            };
        }

        /// <summary>月系星索引（解神、天姚、天刑、阴煞、天月、天巫）</summary>
        public static (int yuejieIndex, int tianyaoIndex, int tianxingIndex, int yinshaIndex, int tianyueIndex, int tianwuIndex) GetMonthlyStarIndex(int lunarMonthIndex)
        {
            string[] jieshenArr = ["shenEarthly", "xuEarthly", "ziEarthly", "yinEarthly", "chenEarthly", "wuEarthly"];
            var yuejieIndex = Util.FixIndex(P(jieshenArr[lunarMonthIndex / 2]));
            var tianyaoIndex = Util.FixIndex(P("chouEarthly") + lunarMonthIndex);
            var tianxingIndex = Util.FixIndex(P("youEarthly") + lunarMonthIndex);
            string[] yinshaArr = ["yinEarthly", "ziEarthly", "xuEarthly", "shenEarthly", "wuEarthly", "chenEarthly"];
            var yinshaIndex = Util.FixIndex(P(yinshaArr[lunarMonthIndex % 6]));
            string[] tianyueArr = ["xuEarthly", "siEarthly", "chenEarthly", "yinEarthly", "weiEarthly", "maoEarthly", "haiEarthly", "weiEarthly", "yinEarthly", "wuEarthly", "xuEarthly", "yinEarthly"];
            var tianyueIndex = Util.FixIndex(P(tianyueArr[lunarMonthIndex]));
            string[] tianwuArr = ["siEarthly", "shenEarthly", "yinEarthly", "haiEarthly"];
            var tianwuIndex = Util.FixIndex(P(tianwuArr[lunarMonthIndex % 4]));

            return (yuejieIndex, tianyaoIndex, tianxingIndex, yinshaIndex, tianyueIndex, tianwuIndex);
        }

        /// <summary>流昌流曲（按天干）</summary>
        public static (int changIndex, int quIndex) GetChangQuIndexByHeavenlyStem(string heavenlyStemKey)
        {
            return heavenlyStemKey switch
            {
                "jiaHeavenly" => (P("siEarthly"), P("youEarthly")),
                "yiHeavenly" => (P("wuEarthly"), P("shenEarthly")),
                "bingHeavenly" or
                "wuHeavenly" => (P("shenEarthly"), P("wuEarthly")),
                "dingHeavenly" or
                "jiHeavenly" => (P("youEarthly"), P("siEarthly")),
                "gengHeavenly" => (P("haiEarthly"), P("maoEarthly")),
                "xinHeavenly" => (P("ziEarthly"), P("yinEarthly")),
                "renHeavenly" => (P("yinEarthly"), P("ziEarthly")),
                "guiHeavenly" => (P("maoEarthly"), P("haiEarthly")),
                _ => (0, 0)
            };
        }
    }

    /// <summary>年系星索引结果</summary>
    public class YearlyStarIndexes
    {
        public int XianchiIndex, HuagaiIndex, GuchenIndex, GuasuIndex, TiancaiIndex, TianshouIndex,
            TianchuIndex, PosuiIndex, FeilianIndex, LongchiIndex, FenggeIndex, TiankuIndex, TianxuIndex,
            TianguanIndex, TianfuIndex, TiandeIndex, YuedeIndex, TiankongIndex, JieluIndex, KongwangIndex,
            XunkongIndex, TianshangIndex, TianshiIndex, JiekongIndex, JieshaAdjIndex, NianjieIndex, DahaoAdjIndex;
    }
}