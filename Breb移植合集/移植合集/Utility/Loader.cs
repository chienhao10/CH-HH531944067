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
            Miscc = MainMenu.AddMenu("PortAIO Misc", "berbsicmisc");

            Miscc.AddGroupLabel("Champion Changes");
            if (Champion.Contains(ObjectManager.Player.ChampionName))
            {
                if (Player.ChampionName.Equals(Champion[0]))
                {
                    Miscc.Add("soraka", new ComboBox("Use addon for Soraka : ", 0, "Sophie Soraka", "ChallengerSeries"));
                }
                if (Player.ChampionName.Equals(Champion[1]))
                {
                    Miscc.Add("kogmaw", new ComboBox("Use addon for Kog'Maw : ", 0, "Sharpshooter", "ChallengerSeries"));
                }
                if (Player.ChampionName.Equals(Champion[2]))
                {
                    //Miscc.Add("bubba", new CheckBox("Enable Bubba Kush (WreckingBall)?"));
                }
                if (Player.ChampionName.Equals(Champion[3]))
                {
                    Miscc.Add("kalista", new ComboBox("Use addon for Kalista : ", 0, "iKalista", "iKalista - Reborn"));
                }
            }
            else
            {
                Miscc.AddLabel("This champion is not supported for these feature.");
            }
            Miscc.AddSeparator();
            Miscc.AddGroupLabel("Util Changes");
            Miscc.AddLabel("Please F5 after making any changes below >>");
            Miscc.Add("champ", new CheckBox("Champ only mode? (No utils will load)", false));
            Miscc.Add("util", new CheckBox("Util only mode? (No champs will load)", false));
            Miscc.AddSeparator();
            Miscc.Add("activator", new CheckBox("Enable ElUtilitySuite?"));
            Miscc.Add("tracker", new CheckBox("Enable NabbTracker?"));
            Miscc.Add("recall", new CheckBox("Enable Recall Tracker?"));
            Miscc.AddSeparator();
            Miscc.Add("skin", new CheckBox("Enable Skin Hack?"));
            //Miscc.Add("evade", new CheckBox("Enable Evade?", false));
            Miscc.Add("godTracker", new CheckBox("Enable God Jungle Tracker?", false));
            Miscc.AddSeparator();
            Miscc.Add("ping", new CheckBox("Enable Ping Block?", false));
            Miscc.Add("human", new CheckBox("Enable Humanizer?", false));
            Miscc.Add("gank", new CheckBox("Enable GankAlerter?", false));

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
