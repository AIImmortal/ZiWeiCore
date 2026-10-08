using tyme.solar;

namespace iztroCore
{
    /// <summary>
    /// 1582-10-15(格里历纪元) ~ 2500-12-31 逐阳历日缓存表：每阳历日存农历(年/月/天/该月天数) + 日干支(60甲子)。
    /// 由同一套 tyme4net 预计算生成，内容与逐次计算完全一致，只是把 O(历法计算) 变成 O(1) 数组读取。
    /// 1582-10-15 之前为儒略历→格里历改革(1582-10-05~14 缺失)，.NET 投影格里历与 tyme4net 不一致，
    /// 故该区间（非紫微斗数实际使用范围）不建表，回退到 tyme4net 计算。
    /// </summary>
    public static class LunarTable
    {
        /// <summary>格里历纪元（1582-10-15），此前儒略历与格里历日号不一致</summary>
        public static readonly DateTime Base = new(1582, 10, 15);

        /// <summary>覆盖天数：1582-10-15 ~ 2500-12-31</summary>
        public static readonly int Length = (new DateTime(2500, 12, 31) - Base).Days + 1;

        private static readonly ushort[] Y = new ushort[Length];
        private static readonly byte[] M = new byte[Length];    // bit7 置位表示闰月
        private static readonly byte[] D = new byte[Length];
        private static readonly byte[] DC = new byte[Length];  // 该农历月的天数
        private static readonly byte[] Gz = new byte[Length];  // 日干支 60 甲子索引

        static LunarTable()
        {
            // 逐行用 FromYmd 填充：与 GetLunarDate 的参考公式同源，按构造必然一致。
            // （不能用 SolarDay.FromYmd(Base).Next(i)：tyme4net 的 Next 在 1500 等远年存在漂移。）
            for (var i = 0; i < Length; i++)
            {
                var dt = Base.AddDays(i);
                var sd = SolarDay.FromYmd(dt.Year, dt.Month, dt.Day);
                var l = sd.GetLunarDay();
                Y[i] = (ushort)l.Year;
                var m = Math.Abs(l.Month);
                M[i] = (byte)(l.Month < 0 ? 0x80 | m : m);
                D[i] = (byte)l.Day;
                DC[i] = (byte)l.LunarMonth.DayCount;
                Gz[i] = (byte)sd.GetSixtyCycleDay().Day.Index;
            }
        }

        /// <summary>阳历日期 -> 表索引；超出覆盖范围返回 -1</summary>
        public static int IndexOf(DateTime date)
        {
            var idx = (date.Date - Base).Days;
            return (uint)idx < Length ? idx : -1;
        }

        public static int LunarYear(int idx) => Y[idx];
        public static int LunarMonth(int idx) => M[idx] & 0x7f;
        public static bool IsLeap(int idx) => (M[idx] & 0x80) != 0;
        public static int LunarDay(int idx) => D[idx];
        public static int DayCount(int idx) => DC[idx];
        public static int DayGanZhi(int idx) => Gz[idx];
    }
}