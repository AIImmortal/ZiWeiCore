namespace iztroCore
{
    /// <summary>通用工具函数，移植自 iztro utils/index.ts</summary>
    public static class Util
    {
        /// <summary>天干 key 数组（供计算用）</summary>
        public static string[] Stems => Heaven.HeavenlyStems;
        /// <summary>地支 key 数组（供计算用）</summary>
        public static string[] Branches => Heaven.EarthlyBranches;

        /// <summary>天干 key -> 索引（0~9）</summary>
        public static int StemIdx(string key) => Array.IndexOf(Stems, key);

        /// <summary>地支 key -> 索引（0~11）</summary>
        public static int BranchIdx(string key) => Array.IndexOf(Branches, key);

        /// <summary>索引归一化，锁定在 0~max-1 范围内（iztro fixIndex）</summary>
        public static int FixIndex(int index, int max = 12)
        {
            while (index < 0) index += max;
            while (index > max - 1) index -= max;
            return index;
        }

        /// <summary>地支相对于寅宫的索引（寅宫=0）（iztro fixEarthlyBranchIndex）</summary>
        public static int FixEarthlyBranchIndex(int earthlyBranchIndex) => FixIndex(earthlyBranchIndex - 2);

        /// <summary>地支 key 相对寅宫的索引（iztro fixEarthlyBranchIndex）</summary>
        public static int FixEarthlyBranchIndex(string earthlyBranchKey) => FixIndex(BranchIdx(earthlyBranchKey) - 2);

        /// <summary>
        /// 调整农历月份索引（正月建寅）。闰月后 15 天（timeIndex!=12 时）+1 月。
        /// 返回 0~11。
        /// </summary>
        public static int FixLunarMonthIndex(int lunarMonth, int timeIndex, bool isLeap, bool fixLeap, int lunarDay)
        {
            var needToAdd = isLeap && fixLeap && lunarDay > 15 && timeIndex != 12;
            return FixIndex(lunarMonth - 1 + (needToAdd ? 1 : 0));
        }

        /// <summary>农历日期【天】的索引，晚子时不下调（iztro fixLunarDayIndex）</summary>
        public static int FixLunarDayIndex(int lunarDay, int timeIndex) => timeIndex >= 12 ? lunarDay : lunarDay - 1;

        /// <summary>小时 -> 时辰索引（iztro timeToIndex）</summary>
        public static int TimeToIndex(int hour)
        {
            if (hour == 0) return 0;       // 早子时
            if (hour == 23) return 12;     // 晚子时
            return (hour + 1) / 2;
        }

        /// <summary>
        /// 起小限（iztro getAgeIndex）。年金三合 → 起始宫位索引。
        /// 寅午戌->辰 申子辰->戌 巳酉丑->未 亥卯未->丑。
        /// </summary>
        public static int GetAgeIndex(string earthlyBranchKey)
        {
            return earthlyBranchKey switch
            {
                "yinEarthly" or "wuEarthly" or "xuEarthly" => FixEarthlyBranchIndex("chenEarthly"),
                "shenEarthly" or "ziEarthly" or "chenEarthly" => FixEarthlyBranchIndex("xuEarthly"),
                "siEarthly" or "youEarthly" or "chouEarthly" => FixEarthlyBranchIndex("weiEarthly"),
                "haiEarthly" or "maoEarthly" or "weiEarthly" => FixEarthlyBranchIndex("chouEarthly"),
                _ => -1
            };
        }

        /// <summary>翻译：iztro key -> 简体中文</summary>
        public static string T(string key)
        {
            if (string.IsNullOrEmpty(key)) return key ?? string.Empty;
            if (Locale.Star.TryGetValue(key, out var v)) return v;
            if (Locale.Palace.TryGetValue(key, out v)) return v;
            if (Locale.HeavenlyStem.TryGetValue(key, out v)) return v;
            if (Locale.EarthlyBranch.TryGetValue(key, out v)) return v;
            if (Locale.Brightness.TryGetValue(key, out v)) return v;
            if (Locale.Mutagen.TryGetValue(key, out v)) return v;
            if (Locale.Gender.TryGetValue(key, out v)) return v;
            if (Locale.FiveElementsClass.TryGetValue(key, out v)) return v;
            if (Locale.Common.TryGetValue(key, out v)) return v;
            return key;
        }

        private static readonly IReadOnlyDictionary<string, string> Reverse =
            BuildReverse();

        private static IReadOnlyDictionary<string, string> BuildReverse()
        {
            var d = new Dictionary<string, string>();
            void add(IReadOnlyDictionary<string, string> src)
            {
                foreach (var kv in src) d[kv.Value] = kv.Key;
            }
            add(Locale.Star);
            add(Locale.Palace);
            add(Locale.HeavenlyStem);
            add(Locale.EarthlyBranch);
            add(Locale.Brightness);
            add(Locale.Mutagen);
            add(Locale.Gender);
            add(Locale.FiveElementsClass);
            add(Locale.Common);

            IReadOnlyDictionary<string, string> result = d;
            return result;
        }

        /// <summary>反查：简体中文 -> iztro key（未命中返回原值）</summary>
        public static string KeyOf(string chinese)
        {
            if (string.IsNullOrEmpty(chinese)) return chinese;
            return Reverse.TryGetValue(chinese, out var key) ? key : chinese;
        }

        /// <summary>翻译干支日期为「庚辰 甲申 丙午 庚寅」格式</summary>
        public static string TranslateChineseDate(GanzhiDate gz)
        {
            return $"{Pillar(gz.Yearly)} {Pillar(gz.Monthly)} {Pillar(gz.Daily)} {Pillar(gz.Hourly)}";
        }
        private static string Pillar((int S, int B) p) => T(Stems[p.S]) + T(Branches[p.B]);
        /// <summary>星耀亮度：starKey + 宫位索引(node) -> 中文亮度</summary>
        public static string GetBrightness(string starKey, int index)
        {
            if (StarBrightness.Table.TryGetValue(starKey, out var arr))
            {
                return T(arr[FixIndex(index)]);
            }

            return string.Empty;
        }

        /// <summary>星耀四化：starKey + 年干 -> 中文四化（无则空）</summary>
        public static string GetMutagen(string starKey, string heavenlyStemKey)
        {
            var target = HeavenlyStemInfo.Info[heavenlyStemKey].Mutagen;
            var idx = Array.IndexOf(target, starKey);
            return idx >= 0 ? T(Heaven.Mutagen[idx]) : string.Empty;
        }

        /// <summary>某天干的四化星曜列表（中文名）</summary>
        public static string[] GetMutagensByHeavenlyStem(string heavenlyStemKey)
        {
            var target = HeavenlyStemInfo.Info[heavenlyStemKey].Mutagen;
            var res = new string[target.Length];
            for (var i = 0; i < target.Length; i++) res[i] = T(target[i]);
            return res;
        }
    }
}