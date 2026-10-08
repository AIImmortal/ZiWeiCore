namespace iztroCore
{
    /// <summary>
    /// 紫微斗数公开 API（输出与 iztro 简体中文一致）。
    /// </summary>
    public static class ZiWei
    {
        /// <summary>
        /// 通过阳历获取星盘。
        /// </summary>
        /// <param name="solar">出生阳历（含时辰，时辰由 hour 推导）</param>
        /// <param name="gender">性别：1=男，0=女</param>
        public static ZiWeiPan BySolar(DateTime solar, int gender)
        {
            var genderKey = gender == 1 ? "male" : "female";
            var timeIndex = Util.TimeToIndex(solar.Hour);
            var lunar = TymeAdapter.GetLunarDate(solar);

            var ctx = new PanContext
            {
                Solar = solar,
                TimeIndex = timeIndex,
                FixLeap = true,
                GenderKey = genderKey,
                Lunar = lunar,
                Ganzhi = TymeAdapter.GetGanzhi(solar, timeIndex, "normal", "forward"),
                LunarMonthIndex = Util.FixLunarMonthIndex(lunar.Month, timeIndex, lunar.IsLeap, true, lunar.Day),
                LunarDayIndex = Util.FixLunarDayIndex(lunar.Day, timeIndex)
            };

            var soulBody = PalaceUtil.GetSoulAndBody(ctx);
            var palaceNames = PalaceUtil.GetPalaceNames(soulBody.SoulIndex);
            var majorStars = Stars.GetMajorStar(ctx, soulBody.HeavenlyStemOfSoul, soulBody.EarthlyBranchOfSoul);
            var minorStars = Stars.GetMinorStar(ctx);
            var adjectiveStars = Stars.GetAdjectiveStar(ctx, soulBody.SoulIndex, soulBody.BodyIndex);
            var changsheng12 = Stars.GetChangsheng12(ctx);
            var boshi12 = Stars.GetBoShi12(ctx);
            var (suiqian12, jiangqian12) = Stars.GetYearly12(ctx);
            var (decadals, ages) = PalaceUtil.GetHoroscope(ctx);

            var yearStemKey = Util.Stems[ctx.Ganzhi.Yearly.Stem];
            var yearBranchKey = Util.Branches[ctx.Ganzhi.Yearly.Branch];

            var palaces = new Palace[12];
            for (var i = 0; i < 12; i++)
            {
                var heStemOfPalace = Util.Stems[Util.FixIndex(Util.StemIdx(soulBody.HeavenlyStemOfSoul) - soulBody.SoulIndex + i, 10)];
                var ebPalace = Util.Branches[Util.FixIndex(2 + i)];
                palaces[i] = new Palace
                {
                    Index = i,
                    Name = Util.T(palaceNames[i]),
                    IsBodyPalace = soulBody.BodyIndex == i,
                    IsOriginalPalace = ebPalace != "ziEarthly" && ebPalace != "chouEarthly" && heStemOfPalace == yearStemKey,
                    HeavenlyStem = Util.T(heStemOfPalace),
                    EarthlyBranch = Util.T(ebPalace),
                    MajorStars = majorStars[i],
                    MinorStars = minorStars[i],
                    AdjectiveStars = adjectiveStars[i],
                    Changsheng12 = changsheng12[i],
                    Boshi12 = boshi12[i],
                    Jiangqian12 = jiangqian12[i],
                    Suiqian12 = suiqian12[i],
                    Decadal = new Stage { Range = [decadals[i].Start, decadals[i].End] },
                    Ages = ages[i],
                };
            }

            var earthlyBranchOfSoulPalace = Util.Branches[Util.FixIndex(soulBody.SoulIndex + 2)];
            var earthlyBranchOfBodyPalace = Util.Branches[Util.FixIndex(soulBody.BodyIndex + 2)];

            var pan = new ZiWeiPan
            {
                Gender = Locale.Gender[genderKey],
                LunarDate = ctx.Lunar.ToStringCN(),
                ChineseDate = Util.TranslateChineseDate(ctx.Ganzhi),
                Time = Util.T(ChineseTime.Keys[timeIndex]),
                TimeRange = ChineseTime.Ranges[timeIndex],
                Sign = Util.T(TymeAdapter.GetSign(solar)),
                Zodiac = Util.T(TymeAdapter.GetZodiacKeyFromEarthlyBranchKey(yearBranchKey)),
                EarthlyBranchOfSoulPalace = Util.T(earthlyBranchOfSoulPalace),
                EarthlyBranchOfBodyPalace = Util.T(earthlyBranchOfBodyPalace),
                Soul = Util.T(EarthlyBranchInfo.Info[earthlyBranchOfSoulPalace].Soul),
                Body = Util.T(EarthlyBranchInfo.Info[yearBranchKey].Body),
                FiveElementsClass = Util.T(PalaceUtil.GetFiveElementsClass(soulBody.HeavenlyStemOfSoul, soulBody.EarthlyBranchOfSoul)),
                Palaces = palaces,
            };
            pan.SetBirthState(ctx.Lunar, ctx.Ganzhi.Hourly.Branch);

            return pan;
        }
    }

    /// <summary>
    /// ZiWeiPan 的运限（Horoscope）部分：状态挂在实例上，天然线程安全。
    /// </summary>
    public partial class ZiWeiPan
    {
        private LunarDate _birthLunar = new();
        private int _birthHourBranch;

        internal void SetBirthState(LunarDate birthLunar, int birthHourBranch)
        {
            _birthLunar = birthLunar;
            _birthHourBranch = birthHourBranch;
        }

        /// <summary>
        /// 获取运限数据（综合大限/小限/流年/流月/流日/流时）。
        /// </summary>
        /// <param name="target">运限目标阳历日期</param>
        public HoroscopeInfo Horoscope(DateTime target)
        {
            var _date = TymeAdapter.GetLunarDate(target);
            var targetTimeIndex = Util.TimeToIndex(target.Hour);
            var gz = TymeAdapter.GetGanzhi(target, targetTimeIndex, "normal", "forward");
            var yearStemKey = Util.Stems[gz.Yearly.Stem];
            var yearBranchKey = Util.Branches[gz.Yearly.Branch];

            // 虚岁（以自然年为界）
            var nominalAge = _date.Year - _birthLunar.Year + 1;

            // 大限索引
            int decadalIndex = -1;
            string decStemKey = "jiaHeavenly", decBranchKey = "ziEarthly";
            for (var i = 0; i < 12; i++)
            {
                var r = Palaces[i].Decadal.Range;
                if (nominalAge >= r[0] && nominalAge <= r[1])
                {
                    decadalIndex = i;
                    decStemKey = Util.KeyOf(Palaces[i].HeavenlyStem);
                    decBranchKey = Util.KeyOf(Palaces[i].EarthlyBranch);
                    break;
                }
            }

            var isChildhood = false;
            if (decadalIndex < 0 && nominalAge >= 1 && nominalAge <= 6)
            {
                string[] childPalaces = ["命宫", "财帛", "疾厄", "夫妻", "福德", "官禄"];
                var targetName = childPalaces[nominalAge - 1];
                for (var i = 0; i < 12; i++)
                {
                    if (Palaces[i].Name == targetName)
                    {
                        decadalIndex = i;
                        decStemKey = Util.KeyOf(Palaces[i].HeavenlyStem);
                        decBranchKey = Util.KeyOf(Palaces[i].EarthlyBranch);
                        break;
                    }
                }

                isChildhood = true;
            }

            // 小限索引
            int ageIndex = -1;
            string ageStemKey = "jiaHeavenly", ageBranchKey = "ziEarthly";
            for (var i = 0; i < 12; i++)
            {
                if (Array.IndexOf(Palaces[i].Ages, nominalAge) >= 0)
                {
                    ageIndex = i;
                    ageStemKey = Util.KeyOf(Palaces[i].HeavenlyStem);
                    ageBranchKey = Util.KeyOf(Palaces[i].EarthlyBranch);
                    break;
                }
            }

            var yearlyIndex = Util.FixEarthlyBranchIndex(yearBranchKey);
            var leapAddition = _birthLunar.IsLeap && _birthLunar.Day > 15 ? 1 : 0;
            var dateLeapAddition = _date.IsLeap && _date.Day > 15 ? 1 : 0;
            var monthlyIndex = Util.FixIndex(yearlyIndex - (_birthLunar.Month + leapAddition) + _birthHourBranch + (_date.Month + dateLeapAddition));
            var dailyIndex = Util.FixIndex(monthlyIndex + _date.Day - 1);
            var hourlyIndex = Util.FixIndex(dailyIndex + gz.Hourly.Branch);

            return new HoroscopeInfo
            {
                Decadal = new HoroscopeItem
                {
                    Index = decadalIndex,
                    Name = isChildhood ? Util.T("childhood") : Util.T("decadal"),
                    HeavenlyStem = decadalIndex < 0 ? "jia" : Util.T(decStemKey),
                    EarthlyBranch = decadalIndex < 0 ? "子" : Util.T(decBranchKey),
                    PalaceNames = PalaceUtil.GetPalaceNames(decadalIndex),
                    Stars = Stars.GetHoroscopeStar(decStemKey, decBranchKey, "decadal"),
                    Mutagen = Util.GetMutagensByHeavenlyStem(decStemKey),
                },
                Age = new HoroscopeItem
                {
                    Index = ageIndex,
                    Name = Util.T("turn"),
                    HeavenlyStem = ageIndex < 0 ? "jia" : Util.T(ageStemKey),
                    EarthlyBranch = ageIndex < 0 ? "zi" : Util.T(ageBranchKey),
                    PalaceNames = PalaceUtil.GetPalaceNames(ageIndex),
                    Stars = null,
                    Mutagen = Util.GetMutagensByHeavenlyStem(ageStemKey),
                },
                Yearly = new HoroscopeItem
                {
                    Index = yearlyIndex,
                    Name = Util.T("yearly"),
                    HeavenlyStem = Util.T(yearStemKey),
                    EarthlyBranch = Util.T(yearBranchKey),
                    PalaceNames = PalaceUtil.GetPalaceNames(yearlyIndex),
                    Stars = Stars.GetHoroscopeStar(yearStemKey, yearBranchKey, "yearly"),
                    Mutagen = Util.GetMutagensByHeavenlyStem(yearStemKey),
                },
                Monthly = BuildFlow(gz.Monthly, "monthly", monthlyIndex),
                Daily = BuildFlow(gz.Daily, "daily", dailyIndex),
                Hourly = BuildFlow(gz.Hourly, "hourly", hourlyIndex),
            };
        }
        private static HoroscopeItem BuildFlow((int Stem, int Branch) pillar, string scope, int index)
        {
            var stemKey = Util.Stems[pillar.Stem];
            var branchKey = Util.Branches[pillar.Branch];
            return new HoroscopeItem
            {
                Index = index,
                Name = Util.T(scope),
                HeavenlyStem = Util.T(stemKey),
                EarthlyBranch = Util.T(branchKey),
                PalaceNames = PalaceUtil.GetPalaceNames(index),
                Stars = Stars.GetHoroscopeStar(stemKey, branchKey, scope),
                Mutagen = Util.GetMutagensByHeavenlyStem(stemKey),
            };
        }
    }
}