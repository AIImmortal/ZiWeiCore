namespace iztroCore
{
    /// <summary>十天干 key（按 iztro 命名）</summary>
    public static class Heaven
    {
        public static readonly string[] HeavenlyStems = ["jiaHeavenly", "yiHeavenly", "bingHeavenly", "dingHeavenly", "wuHeavenly", "jiHeavenly", "gengHeavenly", "xinHeavenly", "renHeavenly", "guiHeavenly"];
        public static readonly string[] EarthlyBranches = ["ziEarthly", "chouEarthly", "yinEarthly", "maoEarthly", "chenEarthly", "siEarthly", "wuEarthly", "weiEarthly", "shenEarthly", "youEarthly", "xuEarthly", "haiEarthly"];
        public static readonly string[] Zodiac = ["rat", "ox", "tiger", "rabbit", "dragon", "snake", "horse", "sheep", "monkey", "rooster", "dog", "pig"];
        /// <summary>十二宫 key</summary>
        public static readonly string[] Palaces = ["soulPalace", "parentsPalace", "spiritPalace", "propertyPalace", "careerPalace", "friendsPalace", "surfacePalace", "healthPalace", "wealthPalace", "childrenPalace", "spousePalace", "siblingsPalace"];
        /// <summary>四化 key，顺序【禄，权，科，忌】</summary>
        public static readonly string[] Mutagen = ["sihuaLu", "sihuaQuan", "sihuaKe", "sihuaJi"];
        /// <summary>性别 -> 阴阳（男为阳，女为阴）</summary>
        public static readonly Dictionary<string, string> Gender = new() { ["male"] = "阳", ["female"] = "阴" };
        /// <summary>五行局 -> 数值（几岁起运）</summary>
        public static readonly Dictionary<string, int> FiveElementsClassValue = new() { ["water2nd"] = 2, ["wood3rd"] = 3, ["metal4th"] = 4, ["earth5th"] = 5, ["fire6th"] = 6 };
    }

    /// <summary>时辰（00:00~01:00 为早子时，23:00~00:00 为晚子时）</summary>
    public static class ChineseTime
    {
        public static readonly string[] Keys = ["earlyRatHour", "oxHour", "tigerHour", "rabbitHour", "dragonHour", "snakeHour", "horseHour", "goatHour", "monkeyHour", "roosterHour", "dogHour", "pigHour", "lateRatHour"];
        public static readonly string[] Ranges = ["00:00~01:00", "01:00~03:00", "03:00~05:00", "05:00~07:00", "07:00~09:00", "09:00~11:00", "11:00~13:00", "13:00~15:00", "15:00~17:00", "17:00~19:00", "19:00~21:00", "21:00~23:00", "23:00~00:00"];
    }

    /// <summary>五虎遁：从年干算月干（正月建寅）</summary>
    public static class TigerRule
    {
        public static readonly Dictionary<string, string> Rule = new()
        {
            ["jiaHeavenly"] = "bingHeavenly",
            ["yiHeavenly"] = "wuHeavenly",
            ["bingHeavenly"] = "gengHeavenly",
            ["dingHeavenly"] = "renHeavenly",
            ["wuHeavenly"] = "jiaHeavenly",
            ["jiHeavenly"] = "bingHeavenly",
            ["gengHeavenly"] = "wuHeavenly",
            ["xinHeavenly"] = "gengHeavenly",
            ["renHeavenly"] = "renHeavenly",
            ["guiHeavenly"] = "jiaHeavenly"
        };
    }

    /// <summary>五鼠遁：以日干算时干</summary>
    public static class RatRule
    {
        public static readonly Dictionary<string, string> Rule = new()
        {
            ["jiaHeavenly"] = "jiaHeavenly",
            ["yiHeavenly"] = "bingHeavenly",
            ["bingHeavenly"] = "wuHeavenly",
            ["dingHeavenly"] = "gengHeavenly",
            ["wuHeavenly"] = "renHeavenly",
            ["jiHeavenly"] = "jiaHeavenly",
            ["gengHeavenly"] = "bingHeavenly",
            ["xinHeavenly"] = "wuHeavenly",
            ["renHeavenly"] = "gengHeavenly",
            ["guiHeavenly"] = "renHeavenly"
        };
    }

    /// <summary>十二地支信息</summary>
    public static class EarthlyBranchInfo
    {
        /// <summary>支 key -> (yinYang, fiveElements, crash, soul星key, body星key)</summary>
        public static readonly Dictionary<string, (string YinYang, string FiveElements, string Crash, string Soul, string Body)> Info = new()
        {
            ["ziEarthly"] = ("阳", "水", "wuEarthly", "tanlangMaj", "huoxingMin"),
            ["chouEarthly"] = ("阴", "土", "weiEarthly", "jumenMaj", "tianxiangMaj"),
            ["yinEarthly"] = ("阳", "木", "shenEarthly", "lucunMin", "tianliangMaj"),
            ["maoEarthly"] = ("阴", "木", "youEarthly", "wenquMin", "tiantongMaj"),
            ["chenEarthly"] = ("阳", "土", "xuEarthly", "lianzhenMaj", "wenchangMin"),
            ["siEarthly"] = ("阴", "火", "haiEarthly", "wuquMaj", "tianjiMaj"),
            ["wuEarthly"] = ("阳", "火", "ziEarthly", "pojunMaj", "huoxingMin"),
            ["weiEarthly"] = ("阴", "土", "chouEarthly", "wuquMaj", "tianxiangMaj"),
            ["shenEarthly"] = ("阳", "金", "yinEarthly", "lianzhenMaj", "tianliangMaj"),
            ["youEarthly"] = ("阴", "金", "maoEarthly", "wenquMin", "tiantongMaj"),
            ["xuEarthly"] = ("阳", "土", "chenEarthly", "lucunMin", "wenchangMin"),
            ["haiEarthly"] = ("阴", "水", "siEarthly", "jumenMaj", "tianjiMaj")
        };
    }

    /// <summary>十天干信息（四化顺序【禄，权，科，忌】）</summary>
    public static class HeavenlyStemInfo
    {
        public static readonly Dictionary<string, (string YinYang, string FiveElements, string? Crash, string[] Mutagen)> Info = new()
        {
            ["jiaHeavenly"] = ("阳", "木", "gengHeavenly", ["lianzhenMaj", "pojunMaj", "wuquMaj", "taiyangMaj"]),
            ["yiHeavenly"] = ("阴", "木", "xinHeavenly", ["tianjiMaj", "tianliangMaj", "ziweiMaj", "taiyinMaj"]),
            ["bingHeavenly"] = ("阳", "火", "renHeavenly", ["tiantongMaj", "tianjiMaj", "wenchangMin", "lianzhenMaj"]),
            ["dingHeavenly"] = ("阴", "火", "guiHeavenly", ["taiyinMaj", "tiantongMaj", "tianjiMaj", "jumenMaj"]),
            ["wuHeavenly"] = ("阳", "土", null, ["tanlangMaj", "taiyinMaj", "youbiMin", "tianjiMaj"]),
            ["jiHeavenly"] = ("阴", "土", null, ["wuquMaj", "tanlangMaj", "tianliangMaj", "wenquMin"]),
            ["gengHeavenly"] = ("阳", "金", "jiaHeavenly", ["taiyangMaj", "wuquMaj", "taiyinMaj", "tiantongMaj"]),
            ["xinHeavenly"] = ("阴", "金", "yiHeavenly", ["jumenMaj", "taiyangMaj", "wenquMin", "wenchangMin"]),
            ["renHeavenly"] = ("阳", "水", "bingHeavenly", ["tianliangMaj", "ziweiMaj", "zuofuMin", "wuquMaj"]),
            ["guiHeavenly"] = ("阴", "水", "dingHeavenly", ["pojunMaj", "jumenMaj", "taiyinMaj", "tanlangMaj"])
        };
    }

    /// <summary>星耀亮度表（按宫位地支排序，自寅宫起）</summary>
    public static class StarBrightness
    {
        public static readonly Dictionary<string, string[]> Table = new()
        {
            ["ziweiMaj"] = ["wang", "wang", "de", "wang", "miao", "miao", "wang", "wang", "de", "wang", "ping", "miao"],
            ["tianjiMaj"] = ["de", "wang", "li", "ping", "miao", "xian", "de", "wang", "li", "ping", "miao", "xian"],
            ["taiyangMaj"] = ["wang", "miao", "wang", "wang", "wang", "de", "de", "ping", "bu", "xian", "xian", "bu"],
            ["wuquMaj"] = ["de", "li", "miao", "ping", "wang", "miao", "de", "li", "miao", "ping", "wang", "miao"],
            ["tiantongMaj"] = ["li", "ping", "ping", "miao", "xian", "bu", "wang", "ping", "ping", "miao", "wang", "bu"],
            ["lianzhenMaj"] = ["miao", "ping", "li", "xian", "ping", "li", "miao", "ping", "li", "xian", "ping", "li"],
            ["tianfuMaj"] = ["miao", "de", "miao", "de", "wang", "miao", "de", "wang", "miao", "de", "miao", "miao"],
            ["taiyinMaj"] = ["wang", "xian", "xian", "xian", "bu", "bu", "li", "wang", "wang", "miao", "miao", "miao"],
            ["tanlangMaj"] = ["ping", "li", "miao", "xian", "wang", "miao", "ping", "li", "miao", "xian", "wang", "miao"],
            ["jumenMaj"] = ["miao", "miao", "xian", "wang", "wang", "bu", "miao", "miao", "xian", "wang", "wang", "bu"],
            ["tianxiangMaj"] = ["miao", "xian", "de", "de", "miao", "de", "miao", "xian", "de", "de", "miao", "miao"],
            ["tianliangMaj"] = ["miao", "miao", "miao", "xian", "miao", "wang", "xian", "de", "miao", "xian", "miao", "wang"],
            ["qishaMaj"] = ["miao", "wang", "miao", "ping", "wang", "miao", "miao", "wang", "miao", "ping", "wang", "miao"],
            ["pojunMaj"] = ["de", "xian", "wang", "ping", "miao", "wang", "de", "xian", "wang", "ping", "miao", "wang"],
            ["wenchangMin"] = ["xian", "li", "de", "miao", "xian", "li", "de", "miao", "xian", "li", "de", "miao"],
            ["wenquMin"] = ["ping", "wang", "de", "miao", "xian", "wang", "de", "miao", "xian", "wang", "de", "miao"],
            ["huoxingMin"] = ["miao", "li", "xian", "de", "miao", "li", "xian", "de", "miao", "li", "xian", "de"],
            ["lingxingMin"] = ["miao", "li", "xian", "de", "miao", "li", "xian", "de", "miao", "li", "xian", "de"],
            ["qingyangMin"] = ["", "xian", "miao", "", "xian", "miao", "", "xian", "miao", "", "xian", "miao"],
            ["tuoluoMin"] = ["xian", "", "miao", "xian", "", "miao", "xian", "", "miao", "xian", "", "miao"]
        };
    }
}