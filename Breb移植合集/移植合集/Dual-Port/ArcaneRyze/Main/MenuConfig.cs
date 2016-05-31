#region

using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

#endregion

namespace Arcane_Ryze.Main
{
    internal class MenuConfig
    {
        private static Menu Menu { get; set; }
        public static Menu comboMenu, laneMenu, jungleMenu, miscMenu, drawMenu;
        public static void Load()
        {

            Menu = MainMenu.AddMenu("Arcane Ryze", "arcaneryze");

            comboMenu = Menu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("KillStealSummoner", new CheckBox("KillSteal Ignite", true));
            //  comboMenu.Add("QCollision", new CheckBox("Ignore Q Collision", true));


            laneMenu = Menu.AddSubMenu("Lane", "LaneMenu");
            laneMenu.Add("LaneR", new CheckBox("Use R", true));
            laneMenu.Add("LaneQ", new CheckBox("Last Hit Q AA", true));
            laneMenu.Add("LaneMana", new Slider("Mana Slider", 50, 0, 100));

            jungleMenu = Menu.AddSubMenu("Jungle", "JungleMenu");
            jungleMenu.Add("JungleR", new CheckBox("Use R", false));

            miscMenu = Menu.AddSubMenu("Misc", "MiscMenu");
            miscMenu.Add("UseItems", new CheckBox("Use Items", true));

            drawMenu = Menu.AddSubMenu("Draw", "Draw");
            drawMenu.Add("dind", new CheckBox("Damage Indicator", true));

        }

        public static bool JungleR { get { return jungleMenu["JungleR"].Cast<CheckBox>().CurrentValue; } }
        public static int LaneMana { get { return laneMenu["LaneMana"].Cast<Slider>().CurrentValue; } }
        public static bool LaneR { get { return laneMenu["LaneR"].Cast<CheckBox>().CurrentValue; } }
        public static bool LaneQ { get { return laneMenu["LaneQ"].Cast<CheckBox>().CurrentValue; } }
        public static bool UseItems { get { return miscMenu["UseItems"].Cast<CheckBox>().CurrentValue; } }
        public static bool KillStealSummoner { get { return comboMenu["KillStealSummoner"].Cast<CheckBox>().CurrentValue; } }
        public static bool dind { get { return drawMenu["dind"].Cast<CheckBox>().CurrentValue; } }

    }
}