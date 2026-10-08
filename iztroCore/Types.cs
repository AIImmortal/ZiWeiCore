namespace iztroCore
{
    public partial class ZiWeiPan
    {
        public required string Gender { get; init; }
        public required string LunarDate { get; init; }
        public required string ChineseDate { get; init; }
        public required string Time { get; init; }
        public required string TimeRange { get; init; }
        public required string Sign { get; init; }
        public required string Zodiac { get; init; }
        public required string EarthlyBranchOfSoulPalace { get; init; }
        public required string EarthlyBranchOfBodyPalace { get; init; }
        public required string Soul { get; init; }
        public required string Body { get; init; }
        public required string FiveElementsClass { get; init; }
        public required Palace[] Palaces { get; init; }
    }

    public class Palace
    {
        public required int Index { get; init; }
        public required string Name { get; init; }
        public required bool IsBodyPalace { get; init; }
        public required bool IsOriginalPalace { get; init; }
        public required string HeavenlyStem { get; init; }
        public required string EarthlyBranch { get; init; }
        public required Star[] MajorStars { get; init; }
        public required Star[] MinorStars { get; init; }
        public required Star[] AdjectiveStars { get; init; }
        public required string Changsheng12 { get; init; }
        public required string Boshi12 { get; init; }
        public required string Jiangqian12 { get; init; }
        public required string Suiqian12 { get; init; }
        public required Stage Decadal { get; init; }
        public required int[] Ages { get; init; }
    }

    public class Star
    {
        public required string Name { get; init; }
        public required string Type { get; init; }
        public string? Brightness { get; init; }
        public string? Mutagen { get; init; }
    }

    public class Stage
    {
        public required int[] Range { get; init; }
    }

    public class HoroscopeInfo
    {
        public required HoroscopeItem Decadal { get; init; }
        public required HoroscopeItem Age { get; init; }
        public required HoroscopeItem Yearly { get; init; }
        public required HoroscopeItem Monthly { get; init; }
        public required HoroscopeItem Daily { get; init; }
        public required HoroscopeItem Hourly { get; init; }
    }

    public class HoroscopeItem
    {
        public required int Index { get; init; }
        public required string Name { get; init; }
        public required string HeavenlyStem { get; init; }
        public required string EarthlyBranch { get; init; }
        public required string[] PalaceNames { get; init; }
        public Star[][]? Stars { get; init; }
        public required string[] Mutagen { get; init; }
    }
}