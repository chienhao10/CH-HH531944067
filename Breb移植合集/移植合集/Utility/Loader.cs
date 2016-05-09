using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;

namespace PortAIO.Utility
{
    class Loader
    {

        public static bool useActivator { get { return Miscc["activator"].Cast<CheckBox>().CurrentValue; } }
        public static bool useTracker { get { return Miscc["tracker"].Cast<CheckBox>().CurrentValue; } }
        public static bool useRecall { get { return Miscc["recall"].Cast<CheckBox>().CurrentValue; } }
        public static bool useSkin { get { return Miscc["skin"].Cast<CheckBox>().CurrentValue; } }
        public static bool champOnly { get { return Miscc["champ"].Cast<CheckBox>().CurrentValue; } }
        public static bool utilOnly { get { return Miscc["util"].Cast<CheckBox>().CurrentValue; } }
        public static bool evade { get { return Miscc["evade"].Cast<CheckBox>().CurrentValue; } }
        public static bool godTracker { get { return Miscc["godTracker"].Cast<CheckBox>().CurrentValue; } }
        public static bool ping { get { return Miscc["ping"].Cast<CheckBox>().CurrentValue; } }
        public static bool human { get { return Miscc["human"].Cast<CheckBox>().CurrentValue; } }
        public static int soraka { get { return Miscc["soraka"].Cast<ComboBox>().CurrentValue; } }
        public static int kogmaw { get { return Miscc["kogmaw"].Cast<ComboBox>().CurrentValue; } }
        public static int leesin { get { return Miscc["leesin"].Cast<ComboBox>().CurrentValue; } }
        public static bool bubba { get { return Miscc["bubba"].Cast<CheckBox>().CurrentValue; } }
        public static int kalista { get { return Miscc["kalista"].Cast<ComboBox>().CurrentValue; } }
        public static bool gank { get { return Miscc["gank"].Cast<CheckBox>().CurrentValue; } }
        public static int diana { get { return Miscc["diana"].Cast<ComboBox>().CurrentValue; } }
        public static int cait { get { return Miscc["cait"].Cast<ComboBox>().CurrentValue; } }
        public static bool intro { get { return Miscc["intro"].Cast<CheckBox>().CurrentValue; } }
        public static int twitch { get { return Miscc["twitch"].Cast<ComboBox>().CurrentValue; } }
        public static int nidalee { get { return Miscc["nidalee"].Cast<ComboBox>().CurrentValue; } }
        public static int lucian { get { return Miscc["lucian"].Cast<ComboBox>().CurrentValue; } }
        public static int ashe { get { return Miscc["ashe"].Cast<ComboBox>().CurrentValue; } }
        public static int vayne { get { return Miscc["vayne"].Cast<ComboBox>().CurrentValue; } }
        public static int jayce { get { return Miscc["jayce"].Cast<ComboBox>().CurrentValue; } }
        public static int yasuo { get { return Miscc["yasuo"].Cast<ComboBox>().CurrentValue; } }
        public static int katarina { get { return Miscc["katarina"].Cast<ComboBox>().CurrentValue; } }
        public static int xerath { get { return Miscc["xerath"].Cast<ComboBox>().CurrentValue; } }
        public static int gragas { get { return Miscc["gragas"].Cast<ComboBox>().CurrentValue; } }


        public static Menu Miscc;

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static List<string> RandomUltChampsList = new List<string>(new[] { "Ezreal", "Jinx", "Ashe", "Draven", "Gangplank", "Ziggs", "Lux", "Xerath" });
        public static List<string> BaseUltList = new List<string>(new[] { "Jinx", "Ashe", "Draven", "Ezreal", "Karthus"});
        public static List<string> Champion = new List<string>(new[] { "Soraka", "KogMaw", "LeeSin", "Kalista", "Diana", "Caitlyn", "Twitch", "Nidalee", "Lucian", "Ashe", "Vayne", "Jayce", "Yasuo", "Katarina", "Xerath", "Gragas" });

        public static void Menu()
        {
            Miscc = MainMenu.AddMenu("CH汉化控制台", "berbsicmisc");
            Miscc.Add("intro", new CheckBox("加载 引导界面?", true));
            Miscc.AddSeparator();
            Miscc.AddGroupLabel("英雄脚本切换");
            if (Champion.Contains(ObjectManager.Player.ChampionName))
            {
                if (Player.ChampionName.Equals(Champion[0]))
                {
                    Miscc.Add("soraka", new ComboBox("切换 索拉卡脚本 : ", 0, "Sophie 索拉卡", "挑战者系列"));
                }
                if (Player.ChampionName.Equals(Champion[1]))
                {
                    Miscc.Add("kogmaw", new ComboBox("切换 大嘴脚本 : ", 0, "神射手", "挑战者系列"));
                }
                if (Player.ChampionName.Equals(Champion[2]))
                {
                    Miscc.Add("leesin", new ComboBox("切换 李星脚本 : ", 0, "ValvraveSharp", "El李星 : 重生"));
                }
                if (Player.ChampionName.Equals(Champion[3]))
                {
                    Miscc.Add("kalista", new ComboBox("切换 滑板鞋脚本 : ", 0, "i滑板鞋", "i滑板鞋 - 重生", "挑战者系列"));
                }
                if (Player.ChampionName.Equals(Champion[4]))
                {
                    Miscc.Add("diana", new ComboBox("切换 皎月脚本 : ", 0, "El皎月", "Nechrito 皎月"));
                }
                if (Player.ChampionName.Equals(Champion[5]))
                {
                    Miscc.Add("cait", new ComboBox("切换 女警脚本 : ", 0, "OKTW", "Exor系列 : AIO"));
                }
                if (Player.ChampionName.Equals(Champion[6]))
                {
                    Miscc.Add("twitch", new ComboBox("切换 老鼠脚本 : ", 0, "OKTW", "Nechrito 老鼠"));
                }
                if (Player.ChampionName.Equals(Champion[7]))
                {
                    Miscc.Add("nidalee", new ComboBox("切换 豹女脚本 : ", 0, "Kurisu", "Nechrito"));
                }
                if (Player.ChampionName.Equals(Champion[8]))
                {
                    Miscc.Add("lucian", new ComboBox("切换 卢锡安脚本 : ", 0, "LCS 卢锡安", "挑战者系列", "i卢锡安"));
                }
                if (Player.ChampionName.Equals(Champion[9]))
                {
                    Miscc.Add("ashe", new ComboBox("Use addon for Ashe : ", 0, "OKTW", "ChallengerSeries"));
                }
                if (Player.ChampionName.Equals(Champion[10]))
                {
                    Miscc.Add("vayne", new ComboBox("Use addon for Vayne : ", 0, "ChallengerVayne", "VayneHunterReborn"));
                }
                if (Player.ChampionName.Equals(Champion[11]))
                {
                    Miscc.Add("jayce", new ComboBox("Use addon for Jayce : ", 0, "OKTW", "Hoe's Jayce"));
                }
                if (Player.ChampionName.Equals(Champion[12]))
                {
                    Miscc.Add("yasuo", new ComboBox("Use addon for Yasuo : ", 0, "ValvraveSharp", "YasuoPro"));
                }
                if (Player.ChampionName.Equals(Champion[13]))
                {
                    Miscc.Add("katarina", new ComboBox("Use addon for Katarina : ", 0, "Staberina", "e.Motion Katarina"));
                }
                if (Player.ChampionName.Equals(Champion[14]))
                {
                    Miscc.Add("xerath", new ComboBox("Use addon for Xerath : ", 0, "OKTW", "ElXerath"));
                }
                if (Player.ChampionName.Equals(Champion[15]))
                {
                    Miscc.Add("gragas", new ComboBox("Use addon for Gragas : ", 0, "Drunk Carry", "Nechrito"));
                }
            }
            else
            {
                Miscc.AddLabel("此英雄暂不支持此功能.");
            }
            Miscc.AddSeparator();
            Miscc.AddGroupLabel("修改加载内容");
            Miscc.AddLabel("更改已下内容，请按 F5 重新载入 >>");
            Miscc.Add("champ", new CheckBox("英雄模式? (只载入英雄脚本)", false));
            Miscc.Add("util", new CheckBox("功能模式? (只载入功能脚本)", false));
            Miscc.AddSeparator();
            Miscc.Add("activator", new CheckBox("载入 El活化剂?"));
            Miscc.Add("tracker", new CheckBox("载入 Nabb计时器?"));
            Miscc.Add("recall", new CheckBox("载入 回城计时?"));
            Miscc.AddSeparator();
            Miscc.Add("skin", new CheckBox("开启换肤?"));
            //Miscc.Add("evade", new CheckBox("Enable Evade?", false));
            Miscc.Add("godTracker", new CheckBox("载入 野区计时（El活化剂中有?", false));
            Miscc.AddSeparator();
            Miscc.Add("ping", new CheckBox("载入 信号管理器（玩家信号）?", false));
            Miscc.Add("human", new CheckBox("载入 人性化?", false));
            Miscc.Add("gank", new CheckBox("载入 Gank提示?", false));

            /*
            Miscc.Add("stream", new CheckBox("Enable StreamBuddy?", false));
            public static bool stream { get { return Miscc["stream"].Cast<CheckBox>().CurrentValue; } }
            public static bool randomUlt { get { return Miscc["randomUlt"].Cast<CheckBox>().CurrentValue; } }
            public static bool baseUlt { get { return Miscc["baseUlt"].Cast<CheckBox>().CurrentValue; } }

            if (RandomUltChampsList.Contains(ObjectManager.Player.ChampionName))
            {
                Miscc.Add("randomUlt", new CheckBox("Enable Random Ult?", false));
            }

            if (BaseUltList.Contains(ObjectManager.Player.ChampionName))
            {
                Miscc.Add("baseUlt", new CheckBox("Enable Base Ult?", false));
            }
            */
        }
    }
}
