using tyme.solar;

namespace iztroCore
{
    /// <summary>农历日期（由 tyme4net 计算）</summary>
    public class LunarDate
    {
        /// <summary>农历年</summary>
        public int Year { get; init; }
        /// <summary>农历月（1~12）</summary>
        public int Month { get; init; }
        /// <summary>农历日</summary>
        public int Day { get; init; }
        /// <summary>是否闰月</summary>
        public bool IsLeap { get; init; }
        /// <summary>该农历月最大天数</summary>
        public int DayCount { get; init; }

        /// <summary>农历月名，lunar-lite 用 冬月/腊月（tyme4net 用 十一/十二月）</summary>
        private static readonly string[] Months = ["正", "二", "三", "四", "五", "六", "七", "八", "九", "十", "冬", "腊"];

        /// <summary>农历日名，lunar-lite 风格</summary>
        private static readonly string[] Days = ["初一", "初二", "初三", "初四", "初五", "初六", "初七", "初八", "初九", "初十", "十一", "十二", "十三", "十四", "十五", "十六", "十七", "十八", "十九", "二十", "廿一", "廿二", "廿三", "廿四", "廿五", "廿六", "廿七", "廿八", "廿九", "三十"];

        /// <summary>输出 zh-CN 农历字符串，如「二〇〇〇年七月十七」</summary>
        public string ToStringCN()
        {
            const string digits = "〇一二三四五六七八九";
            var yearStr = string.Empty;
            foreach (var c in Year.ToString())
            {
                yearStr += digits[c - '0'];
            }

            return $"{yearStr}年{(IsLeap ? "闰" : "")}{Months[Month - 1]}月{Days[Day - 1]}";
        }
    }

    /// <summary>干支五柱（年干支、月干支、日干支、时干支）</summary>
    public class GanzhiDate
    {
        /// <summary>年柱 (天干索引, 地支索引)</summary>
        public (int Stem, int Branch) Yearly;
        /// <summary>月柱 (天干索引, 地支索引)</summary>
        public (int Stem, int Branch) Monthly;
        /// <summary>日柱 (天干索引, 地支索引)</summary>
        public (int Stem, int Branch) Daily;
        /// <summary>时柱 (天干索引, 地支索引)</summary>
        public (int Stem, int Branch) Hourly;

        public (int Stem, int Branch) this[string part] => part switch
        {
            "yearly" => Yearly,
            "monthly" => Monthly,
            "daily" => Daily,
            "hourly" => Hourly,
            _ => (0, 0),
        };
    }

    /// <summary>
    /// tyme4net 适配层，用于替代 lunar-lite。
    /// 提供农历日期、干支五柱、星座、生肖等计算。
    /// </summary>
    public static class TymeAdapter
    {
        /// <summary>生肖（按地支索引，0=子）</summary>
        public static readonly string[] Zodiac = ["rat", "ox", "tiger", "rabbit", "dragon", "snake", "horse", "sheep", "monkey", "rooster", "dog", "pig"];

        /// <summary>由农历缓存表直接推算干支五柱（normal 年分界；表外或到达边界返回 false）</summary>
        private static bool TryGanzhiFromTable(int idx, int timeIndex, string dayDivide, out GanzhiDate gz)
        {
            var lunarYear = LunarTable.LunarYear(idx);
            var lunarMonth = LunarTable.LunarMonth(idx);
            var isLeap = LunarTable.IsLeap(idx);
            var lunarDay = LunarTable.LunarDay(idx);

            // 年柱
            var yearIdx = (lunarYear - 1984) % 60;
            if (yearIdx < 0) yearIdx += 60;
            var yearStemIdx = yearIdx % 10;
            var yearBranchIdx = yearIdx % 12;

            // 月柱：以农历月定月支（正月建寅），闰月当月且日>15 时月序+1
            var leapFix = isLeap && lunarDay > 15 ? 1 : 0;
            var effMonth = lunarMonth + leapFix;
            var monthBranchIdx = (effMonth + 1) % 12;
            var monthM = Util.FixIndex(monthBranchIdx - 2);
            var tiger = Util.StemIdx(TigerRule.Rule[Heaven.HeavenlyStems[yearStemIdx]]);
            var monthStemIdx = Util.FixIndex(tiger + monthM, 10);

            // 日柱（晚子时按次日计）
            var dayIdx = idx;
            if (timeIndex >= 12 && dayDivide != "current")
            {
                if (dayIdx + 1 >= LunarTable.Length) { gz = null!; return false; }
                dayIdx++;
            }
            var dailyIdx = LunarTable.DayGanZhi(dayIdx);
            var dailyStemIdx = dailyIdx % 10;
            var dailyBranchIdx = dailyIdx % 12;

            // 时柱（五鼠遁）
            var hourBranchIdx = timeIndex % 12;
            var rat = Util.StemIdx(RatRule.Rule[Heaven.HeavenlyStems[dailyStemIdx]]);
            var hourStemIdx = Util.FixIndex(rat + hourBranchIdx, 10);

            gz = new GanzhiDate
            {
                Yearly = (yearStemIdx, yearBranchIdx),
                Monthly = (monthStemIdx, monthBranchIdx),
                Daily = (dailyStemIdx, dailyBranchIdx),
                Hourly = (hourStemIdx, hourBranchIdx),
            };
            return true;
        }

        /// <summary>星座 key（Aries 索引0）</summary>
        public static readonly string[] Constellations = ["aries", "taurus", "gemini", "cancer", "leo", "virgo", "libra", "scorpio", "sagittarius", "capricorn", "aquarius", "pisces"];

        /// <summary>由阳历日期获取农历日期（1582-10-15~2500-12-31 走缓存表，表外回退 tyme4net）</summary>
        public static LunarDate GetLunarDate(DateTime dt)
        {
            var idx = LunarTable.IndexOf(dt);
            if (idx >= 0)
            {
                return new LunarDate
                {
                    Year = LunarTable.LunarYear(idx),
                    Month = LunarTable.LunarMonth(idx),
                    Day = LunarTable.LunarDay(idx),
                    IsLeap = LunarTable.IsLeap(idx),
                    DayCount = LunarTable.DayCount(idx),
                };
            }

            var l0 = SolarDay.FromYmd(dt.Year, dt.Month, dt.Day).GetLunarDay();
            return new LunarDate
            {
                Year = l0.Year,
                Month = Math.Abs(l0.Month),
                Day = l0.Day,
                IsLeap = l0.Month < 0,
                DayCount = l0.LunarMonth.DayCount,
            };
        }

        /// <summary>
        /// 由阳历日期与时辰索引计算干支五柱。
        /// 年分界：
        ///   normal —— 正月初一（年柱 = (农历年 - 1984) % 60）
        ///   exact —— 立春（年柱 = tyme4net 干支日年柱）
        /// 月柱以节令为准，时柱以五鼠遁推时干。
        /// 晚子时（timeIndex=12）且 dayDivide=forward 时，日柱与时柱按次日计。
        /// </summary>
        public static GanzhiDate GetGanzhi(DateTime dt, int timeIndex, string yearDivide = "normal", string dayDivide = "forward")
        {
            var idx = LunarTable.IndexOf(dt);
            if (idx >= 0 && yearDivide == "normal" && TryGanzhiFromTable(idx, timeIndex, dayDivide, out var t))
            {
                return t;
            }

            var solarDay = SolarDay.FromYmd(dt.Year, dt.Month, dt.Day);
            var scOriginal = solarDay.GetSixtyCycleDay();
            var lunar = solarDay.GetLunarDay();

            // 年柱
            int yearIdx;
            if (yearDivide == "exact")
            {
                yearIdx = scOriginal.Year.Index;
            }
            else
            {
                yearIdx = (lunar.Year - 1984) % 60;
                if (yearIdx < 0) yearIdx += 60;
            }

            var yearStemIdx = yearIdx % 10;
            var yearBranchIdx = yearIdx % 12;

            // 月柱：iztro/lunar-lite 默认以农历月定月支（正月建寅），闰月当月且日>15 时月序+1
            // 月支 idx = (有效月 + 1) % 12（正月->寅(2)，腊月->丑(1)）
            var lunarMonth = Math.Abs(lunar.Month);
            var leapFix = lunar.Month < 0 && lunar.Day > 15 ? 1 : 0;
            var effMonth = lunarMonth + leapFix;
            var monthBranchIdx = (effMonth + 1) % 12;
            var monthM = Util.FixIndex(monthBranchIdx - 2);
            var tigerStart = Util.StemIdx(TigerRule.Rule[Heaven.HeavenlyStems[yearStemIdx]]);
            var monthStemIdx = Util.FixIndex(tigerStart + monthM, 10);

            // 日柱与时柱（晚子时按次日计）
            var daySolar = solarDay;
            if (timeIndex >= 12 && dayDivide != "current")
            {
                daySolar = solarDay.Next(1);
            }

            var scAdjusted = daySolar.GetSixtyCycleDay();
            var dailyIdx = scAdjusted.Day.Index;
            var dailyStemIdx = dailyIdx % 10;
            var dailyBranchIdx = dailyIdx % 12;

            var hourBranchIdx = timeIndex % 12;
            var ratStart = Util.StemIdx(RatRule.Rule[Heaven.HeavenlyStems[dailyStemIdx]]);
            var hourStemIdx = Util.FixIndex(ratStart + hourBranchIdx, 10);

            return new GanzhiDate
            {
                Yearly = (yearStemIdx, yearBranchIdx),
                Monthly = (monthStemIdx, monthBranchIdx),
                Daily = (dailyStemIdx, dailyBranchIdx),
                Hourly = (hourStemIdx, hourBranchIdx),
            };
        }

        /// <summary>由阳历日期获取星座 key</summary>
        public static string GetSign(DateTime dt)
        {
            var y = dt.Month * 100 + dt.Day;
            var idx = y > 1221 || y < 120 ? 9 : y < 219 ? 10 : y < 321 ? 11 : y < 420 ? 0 : y < 521 ? 1 : y < 622 ? 2 : y < 723 ? 3 : y < 823 ? 4 : y < 923 ? 5 : y < 1024 ? 6 : y < 1123 ? 7 : 8;
            return Constellations[idx];
        }

        /// <summary>由地支 key 获取生肖 key，如 wuEarthly -> horse</summary>
        public static string GetZodiacKeyFromEarthlyBranchKey(string earthlyBranchKey)
        {
            var idx = Array.IndexOf(Heaven.EarthlyBranches, earthlyBranchKey);
            return idx >= 0 ? Zodiac[idx] : earthlyBranchKey;
        }
    }
}