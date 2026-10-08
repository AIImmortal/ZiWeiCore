namespace iztroCore
{
    /// <summary>星曜安放，移植自 iztro star 系列文件</summary>
    public static class Stars
    {
        /// <summary>建 12 个空宫列表</summary>
        private static List<Star>[] Empty() => new List<Star>[12];

        private static Star NewStar(string nameKey, string type, string? brightness = null, string? mutagen = null)
            => new()
            {
                Name = Util.T(nameKey),
                Type = type,
                Brightness = string.IsNullOrEmpty(brightness) ? null : brightness,
                Mutagen = string.IsNullOrEmpty(mutagen) ? null : mutagen,
            };

        /// <summary>14 主星（亮度 + 四化）</summary>
        public static Star[][] GetMajorStar(PanContext ctx, string soulStemKey, string soulBranchKey)
        {
            var (ziweiIndex, tianfuIndex) = Location.GetStartIndex(ctx.Lunar.Day, ctx.Lunar.DayCount, ctx.TimeIndex, soulStemKey, soulBranchKey, null, null);
            var yearStem = Util.Stems[ctx.Ganzhi.Yearly.Stem];
            var stars = Empty();

            string[] ziweiGroup = ["ziweiMaj", "tianjiMaj", "", "taiyangMaj", "wuquMaj", "tiantongMaj", "", "", "lianzhenMaj"];
            for (var i = 0; i < ziweiGroup.Length; i++)
            {
                if (ziweiGroup[i] == "") continue;
                var idx = Util.FixIndex(ziweiIndex - i);
                (stars[idx] ??= []).Add(NewStar(ziweiGroup[i], "major",
                    Util.GetBrightness(ziweiGroup[i], idx), Util.GetMutagen(ziweiGroup[i], yearStem)));
            }

            string[] tianfuGroup = ["tianfuMaj", "taiyinMaj", "tanlangMaj", "jumenMaj", "tianxiangMaj", "tianliangMaj", "qishaMaj", "", "", "", "pojunMaj"];
            for (var i = 0; i < tianfuGroup.Length; i++)
            {
                if (tianfuGroup[i] == "") continue;
                var idx = Util.FixIndex(tianfuIndex + i);
                (stars[idx] ??= []).Add(NewStar(tianfuGroup[i], "major",
                    Util.GetBrightness(tianfuGroup[i], idx), Util.GetMutagen(tianfuGroup[i], yearStem)));
            }

            return ToArr(stars);
        }

        /// <summary>14 辅星</summary>
        public static Star[][] GetMinorStar(PanContext ctx)
        {
            var (Stem, Branch) = ctx.Ganzhi.Yearly;
            var yearStem = Util.Stems[Stem];
            var yearBranch = Util.Branches[Branch];
            var monthIndex = ctx.LunarMonthIndex;

            var (zuoIndex, youIndex) = Location.GetZuoYouIndex(monthIndex + 1);
            var (changIndex, quIndex) = Location.GetChangQuIndex(ctx.TimeIndex);
            var (kuiIndex, yueIndex) = Location.GetKuiYueIndex(yearStem);
            var (huoIndex, lingIndex) = Location.GetHuoLingIndex(yearBranch, ctx.TimeIndex);
            var (kongIndex, jieIndex) = Location.GetKongJieIndex(ctx.TimeIndex);
            var (luIndex, maIndex, yangIndex, tuoIndex) = Location.GetLuYangTuoMaIndex(yearStem, yearBranch);

            var stars = Empty();
            void at(int i, string key, string type, bool withBright = true, string? mutagen = null)
                => (stars[i] ??= []).Add(NewStar(key, type, withBright ? Util.GetBrightness(key, i) : null, mutagen));

            at(zuoIndex, "zuofuMin", "soft", mutagen: Util.GetMutagen("zuofuMin", yearStem));
            at(youIndex, "youbiMin", "soft", mutagen: Util.GetMutagen("youbiMin", yearStem));
            at(changIndex, "wenchangMin", "soft", mutagen: Util.GetMutagen("wenchangMin", yearStem));
            at(quIndex, "wenquMin", "soft", mutagen: Util.GetMutagen("wenquMin", yearStem));
            at(kuiIndex, "tiankuiMin", "soft");
            at(yueIndex, "tianyueMin", "soft");
            at(luIndex, "lucunMin", "lucun");
            at(maIndex, "tianmaMin", "tianma");
            at(kongIndex, "dikongMin", "tough");
            at(jieIndex, "dijieMin", "tough");
            at(huoIndex, "huoxingMin", "tough");
            at(lingIndex, "lingxingMin", "tough");
            at(yangIndex, "qingyangMin", "tough");
            at(tuoIndex, "tuoluoMin", "tough");

            return ToArr(stars);
        }

        /// <summary>38 杂曜</summary>
        public static Star[][] GetAdjectiveStar(PanContext ctx, int soulIndex, int bodyIndex)
        {
            var y = Location.GetYearlyStarIndex(ctx, soulIndex, bodyIndex);
            var (yuejieIndex, tianyaoIndex, tianxingIndex, yinshaIndex, tianyueIndex, tianwuIndex) = Location.GetMonthlyStarIndex(ctx.LunarMonthIndex);
            var (santaiIndex, bazuoIndex, enguangIndex, tianguiIndex) = Location.GetDailyStarIndex(ctx.Lunar.Day, ctx.TimeIndex, ctx.LunarMonthIndex);
            var (taifuIndex, fenggaoIndex) = Location.GetTimelyStarIndex(ctx.TimeIndex);
            var (hongluanIndex, tianxiIndex) = Location.GetLuanXiIndex(Util.Branches[ctx.Ganzhi.Yearly.Branch]);

            var stars = Empty();
            void at(int i, string key, string type) => (stars[i] ??= []).Add(NewStar(key, type));

            at(hongluanIndex, "hongluan", "flower");
            at(tianxiIndex, "tianxi", "flower");
            at(tianyaoIndex, "tianyao", "flower");
            at(y.XianchiIndex, "xianchi", "flower");
            at(yuejieIndex, "jieshen", "helper");
            at(santaiIndex, "santai", "adjective");
            at(bazuoIndex, "bazuo", "adjective");
            at(enguangIndex, "engguang", "adjective");
            at(tianguiIndex, "tiangui", "adjective");
            at(y.LongchiIndex, "longchi", "adjective");
            at(y.FenggeIndex, "fengge", "adjective");
            at(y.TiancaiIndex, "tiancai", "adjective");
            at(y.TianshouIndex, "tianshou", "adjective");
            at(taifuIndex, "taifu", "adjective");
            at(fenggaoIndex, "fenggao", "adjective");
            at(tianwuIndex, "tianwu", "adjective");
            at(y.HuagaiIndex, "huagai", "adjective");
            at(y.TianguanIndex, "tianguan", "adjective");
            at(y.TianfuIndex, "tianfu", "adjective");
            at(y.TianchuIndex, "tianchu", "adjective");
            at(tianyueIndex, "tianyue", "adjective");
            at(y.TiandeIndex, "tiande", "adjective");
            at(y.YuedeIndex, "yuede", "adjective");
            at(y.TiankongIndex, "tiankong", "adjective");
            at(y.XunkongIndex, "xunkong", "adjective");
            // 默认（非中州派）
            at(y.JieluIndex, "jielu", "adjective");
            at(y.KongwangIndex, "kongwang", "adjective");
            // 通用
            at(y.GuchenIndex, "guchen", "adjective");
            at(y.GuasuIndex, "guasu", "adjective");
            at(y.FeilianIndex, "feilian", "adjective");
            at(y.PosuiIndex, "posui", "adjective");
            at(tianxingIndex, "tianxing", "adjective");
            at(yinshaIndex, "yinsha", "adjective");
            at(y.TiankuIndex, "tianku", "adjective");
            at(y.TianxuIndex, "tianxu", "adjective");
            at(y.TianshiIndex, "tianshi", "adjective");
            at(y.TianshangIndex, "tianshang", "adjective");
            at(y.NianjieIndex, "nianjie", "helper");

            return ToArr(stars);
        }

        /// <summary>长生 12 神（中文）</summary>
        public static string[] GetChangsheng12(PanContext ctx)
        {
            var genderKey = ctx.GenderKey;
            var yearBranch = Util.Branches[ctx.Ganzhi.Yearly.Branch];
            var yinYang = EarthlyBranchInfo.Info[yearBranch].YinYang;
            var soul = PalaceUtil.GetSoulAndBody(ctx);
            var fiveElementClass = PalaceUtil.GetFiveElementsClass(soul.HeavenlyStemOfSoul, soul.EarthlyBranchOfSoul);
            var startIdx = GetChangsheng12StartIndex(fiveElementClass);

            string[] stars = ["changsheng", "muyu", "guandai", "linguan", "diwang", "shuai", "bing", "si", "mu", "jue", "tai", "yang"];
            var res = new string[12];
            for (var i = 0; i < 12; i++)
            {
                var idx = Heaven.Gender[genderKey] == yinYang ? Util.FixIndex(i + startIdx) : Util.FixIndex(startIdx - i);
                res[idx] = Util.T(stars[i]);
            }

            return res;
        }

        /// <summary>长生 12 神起始索引</summary>
        public static int GetChangsheng12StartIndex(string fiveElementClassKey)
        {
            return Heaven.FiveElementsClassValue[fiveElementClassKey] switch
            {
                2 => Util.FixEarthlyBranchIndex("shenEarthly"),
                3 => Util.FixEarthlyBranchIndex("haiEarthly"),
                4 => Util.FixEarthlyBranchIndex("siEarthly"),
                5 => Util.FixEarthlyBranchIndex("shenEarthly"),
                6 => Util.FixEarthlyBranchIndex("yinEarthly"),
                _ => 0
            };
        }

        /// <summary>博士 12 神（中文）</summary>
        public static string[] GetBoShi12(PanContext ctx)
        {
            var genderKey = ctx.GenderKey;
            var yearStem = Util.Stems[ctx.Ganzhi.Yearly.Stem];
            var yearBranch = Util.Branches[ctx.Ganzhi.Yearly.Branch];
            var yinYang = EarthlyBranchInfo.Info[yearBranch].YinYang;
            var (luIndex, _, _, _) = Location.GetLuYangTuoMaIndex(yearStem, yearBranch);

            string[] stars = ["boshi", "lishi", "qinglong", "xiaohao", "jiangjun", "zhoushu", "faylian", "xishen", "bingfu", "dahao", "fubing", "guanfu"];
            var res = new string[12];
            for (var i = 0; i < 12; i++)
            {
                var idx = Util.FixIndex(Heaven.Gender[genderKey] == yinYang ? luIndex + i : luIndex - i);
                res[idx] = Util.T(stars[i]);
            }

            return res;
        }

        /// <summary>流年 12 神（岁前 + 将前，中文）</summary>
        public static (string[] suiqian12, string[] jiangqian12) GetYearly12(PanContext ctx)
        {
            var yearBranch = Util.Branches[ctx.Ganzhi.Yearly.Branch];

            string[] ts12shen = ["suijian", "huiqi", "sangmen", "guansuo", "gwanfu", "xiaohao", "dahao", "longde", "baihu", "tiande", "diaoke", "bingfu"];
            var suiqian12 = new string[12];
            for (var i = 0; i < 12; i++)
            {
                var idx = Util.FixIndex(Util.FixEarthlyBranchIndex(yearBranch) + i);
                suiqian12[idx] = Util.T(ts12shen[i]);
            }

            string[] jq12shen = ["jiangxing", "panan", "suiyi", "xiishen", "huagai", "jiesha", "zhaisha", "tiansha", "zhibei", "xianchi", "yuesha", "wangshen"];
            var start = GetJiangqian12StartIndex(yearBranch);
            var jiangqian12 = new string[12];
            for (var i = 0; i < 12; i++)
            {
                var idx = Util.FixIndex(start + i);
                jiangqian12[idx] = Util.T(jq12shen[i]);
            }

            return (suiqian12, jiangqian12);
        }

        /// <summary>将前 12 神起始索引</summary>
        public static int GetJiangqian12StartIndex(string earthlyBranchKey)
        {
            return earthlyBranchKey switch
            {
                "yinEarthly" or "wuEarthly" or "xuEarthly" => Util.FixEarthlyBranchIndex("wuEarthly"),
                "shenEarthly" or "ziEarthly" or "chenEarthly" => Util.FixEarthlyBranchIndex("ziEarthly"),
                "siEarthly" or "youEarthly" or "chouEarthly" => Util.FixEarthlyBranchIndex("youEarthly"),
                "haiEarthly" or "maoEarthly" or "weiEarthly" => Util.FixEarthlyBranchIndex("maoEarthly"),
                _ => 0
            };
        }

        /// <summary>流耀：魁钺昌曲禄羊陀马鸾喜（大限/流年/流月/流日/流时）</summary>
        /// <param name="heavenlyStemKey">天干 key</param>
        /// <param name="earthlyBranchKey">地支 key</param>
        /// <param name="scope">origin/decadal/yearly/monthly/daily/hourly</param>
        public static Star[][] GetHoroscopeStar(string heavenlyStemKey, string earthlyBranchKey, string scope)
        {
            var (kuiIndex, yueIndex) = Location.GetKuiYueIndex(heavenlyStemKey);
            var (changIndex, quIndex) = Location.GetChangQuIndexByHeavenlyStem(heavenlyStemKey);
            var (luIndex, maIndex, yangIndex, tuoIndex) = Location.GetLuYangTuoMaIndex(heavenlyStemKey, earthlyBranchKey);
            var (hongluanIndex, tianxiIndex) = Location.GetLuanXiIndex(earthlyBranchKey);

            var stars = Empty();
            void at(int i, string key, string type) => (stars[i] ??= []).Add(NewStar(key, type));

            if (scope == "yearly")
            {
                at(Location.GetNianjieIndex(earthlyBranchKey), "nianjie", "helper");
            }

            at(kuiIndex, ScopeKey("kui", scope), "soft");
            at(yueIndex, ScopeKey("yue", scope), "soft");
            at(changIndex, ScopeKey("chang", scope), "soft");
            at(quIndex, ScopeKey("qu", scope), "soft");
            at(luIndex, ScopeKey("lu", scope), "lucun");
            at(yangIndex, ScopeKey("yang", scope), "tough");
            at(tuoIndex, ScopeKey("tuo", scope), "tough");
            at(maIndex, ScopeKey("ma", scope), "tianma");
            at(hongluanIndex, ScopeKey("luan", scope), "flower");
            at(tianxiIndex, ScopeKey("xi", scope), "flower");

            return ToArr(stars);
        }

        /// <summary>根据 scope 推导流耀中文名 key</summary>
        private static string ScopeKey(string suffix, string scope)
        {
            string[] byScope = scope switch
            {
                "decadal" => ["yun", "kui", "yue", "chang", "qu", "lu", "yang", "tuo", "ma", "luan", "xi"],
                "yearly" => ["liu", "kui", "yue", "chang", "qu", "lu", "yang", "tuo", "ma", "luan", "xi"],
                "monthly" => ["yue", "kui", "yue", "chang", "qu", "lu", "yang", "tuo", "ma", "luan", "xi"],
                "daily" => ["ri", "kui", "yue", "chang", "qu", "lu", "yang", "tuo", "ma", "luan", "xi"],
                "hourly" => ["shi", "kui", "yue", "chang", "qu", "lu", "yang", "tuo", "ma", "luan", "xi"],
                _ => ["t", "kuiMin", "yueMin", "changMin", "quMin", "luMin", "yangMin", "tuoMin", "maMin", "luanMin", "xi"]
            };
            var prefix = byScope[0];
            return prefix + suffix;
        }

        /// <summary>List[] -> Star[][]</summary>
        private static Star[][] ToArr(List<Star>[] stars)
        {
            var res = new Star[12][];
            for (var i = 0; i < 12; i++) res[i] = [.. (stars[i] ?? [])];
            return res;
        }
    }
}