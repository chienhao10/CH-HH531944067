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
        public static bool bubba { get { return Miscc["bubba"].Cast<CheckBox>().CurrentValue; } }
        public static int kalista { get { return Miscc["kalista"].Cast<ComboBox>().CurrentValue; } }
        public static bool gank { get { return Miscc["gank"].Cast<CheckBox>().CurrentValue; } }

        public static Menu Miscc;

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static List<string> RandomUltChampsList = new List<string>(new[] { "Ezreal", "Jinx", "Ashe", "Draven", "Gangplank", "Ziggs", "Lux", "Xerath" });
        public static List<string> BaseUltList = new List<string>(new[] { "Jinx", "Ashe", "Draven", "Ezreal", "Karthus"});
        public static List<string> Champion = new List<string>(new[] { "Soraka", "KogMaw", "LeeSin", "Kalista" });

        public static void Menu()
        {
            Miscc = MainMenu.AddMenu("移植合集 杂项", "berbsicmisc");

            Miscc.AddGroupLabel("英雄更改");
            if (Champion.Contains(ObjectManager.Player.ChampionName))
            {
                if (Player.ChampionName.Equals(Champion[0]))
                {
                    Miscc.Add("soraka", new ComboBox("切换 索拉卡脚本 : ", 0, "Sophie Soraka", "ChallengerSeries"));
                }
                if (Player.ChampionName.Equals(Champion[1]))
                {
                    Miscc.Add("kogmaw", new ComboBox("切换 大嘴脚本 : ", 0, "Sharpshooter", "ChallengerSeries"));
                }
                if (Player.ChampionName.Equals(Champion[2]))
                {
                    //Miscc.Add("bubba", new CheckBox("Enable Bubba Kush (WreckingBall)?"));
                }
                if (Player.ChampionName.Equals(Champion[3]))
                {
                    Miscc.Add("kalista", new ComboBox("切换 滑板鞋脚本 : ", 0, "iKalista", "iKalista - Reborn"));
                }
            }
            else
            {
                Miscc.AddLabel("此英雄暂不支持已下功能.");
            }
            Miscc.AddSeparator();
            Miscc.AddGroupLabel("活化剂更改");
            Miscc.AddLabel("更改已下内容，请按 F5 重新载入 >>");
            Miscc.Add("champ", new CheckBox("英雄模式? (只会载入英雄脚本)", false));
            Miscc.Add("util", new CheckBox("功能模式? (只载入功能脚本)", false));
            Miscc.AddSeparator();
            Miscc.Add("activator", new CheckBox("载入 ElUtilitySuite（活化剂）?"));
            Miscc.Add("tracker", new CheckBox("载入 NabbTracker（计时器）?"));
            Miscc.Add("recall", new CheckBox("载入 回城计时?"));
            Miscc.AddSeparator();
            Miscc.Add("skin", new CheckBox("载入 换肤?"));
            //Miscc.Add("evade", new CheckBox("Enable Evade?", false));
            Miscc.Add("godTracker", new CheckBox("载入 野区计时?", false));
            Miscc.AddSeparator();
            Miscc.Add("ping", new CheckBox("载入 信号管理器（游戏内玩家发的信号）?", false));
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
