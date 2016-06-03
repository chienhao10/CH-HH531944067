#region

using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.SDK.Core.UI;

#endregion

namespace Swiftly_Teemo.Main
{
    internal class MenuConfig
    {
        public static Menu Menu, comboMenu, laneMenu, drawMenu;

        public static bool KillStealSummoner;
        public static bool LaneQ;
        public static bool dind;
        public static bool EngageDraw;
        public static bool Flee;

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }


        public static void Load()
        {

            Menu = MainMenu.AddMenu("Swiftly Teemo", "Teemo");

            comboMenu = Menu.AddSubMenu("Combo", "ComboMenu");
            comboMenu.Add("KillStealSummoner", new CheckBox("KillSteal Summoner", true));

            laneMenu = Menu.AddSubMenu("Lane", "LaneMenu");
            laneMenu.Add("LaneQ", new CheckBox("Last Hit Q AA", true));

            drawMenu = Menu.AddSubMenu("Draw", "Draw");
            drawMenu.Add("dind", new CheckBox("Damage Indicator", true));
            drawMenu.Add("EngageDraw", new CheckBox("Draw Engage", true));

            Menu.Add("Flee", new KeyBind("Flee", false, KeyBind.BindTypes.HoldActive, 'Z'));

            KillStealSummoner = getCheckBoxItem(comboMenu, "KillStealSummoner");
            LaneQ = getCheckBoxItem(laneMenu, "LaneQ");
            dind = getCheckBoxItem(drawMenu, "dind");
            EngageDraw = getCheckBoxItem(drawMenu, "EngageDraw");
            Flee = getKeyBindItem(Menu, "Flee");

        }

    }
}
