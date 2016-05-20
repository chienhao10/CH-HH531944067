using LeagueSharp.Common;

namespace ExorAIO.Champions.Anivia
{
    using System.Drawing;
    using ExorAIO.Utilities;
    using Color = SharpDX.Color;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Menu;
    using EloBuddy;

    /// <summary>
    ///     The menu class.
    /// </summary>
    class Menus
    {

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }
        /// <summary>
        ///     Initializes the menus.
        /// </summary>
        /// 

        public static void Initialize()
        {
            Variables.QMenu = Variables.Menu.AddSubMenu("Use Q to:", "qmenu");
            Variables.QMenu.Add("qspell.combo", new CheckBox("Combo"));
            Variables.QMenu.Add("qspell.auto", new CheckBox("Logical"));
            Variables.QMenu.Add("qspell.ks", new CheckBox("KillSteal"));
            Variables.QMenu.Add("qspell.farm", new CheckBox("Clear"));

            Variables.WMenu = Variables.Menu.AddSubMenu("Use W to:", "wmenu");
            Variables.WMenu.Add("wspell.combo", new CheckBox("Combo"));
            Variables.WMenu.Add("wspell.gp", new CheckBox("Anti-Gapcloser"));
            Variables.WMenu.Add("wspell.ir", new CheckBox("Interrupt Enemy Channels"));

            Variables.WhiteListMenu = Variables.Menu.AddSubMenu("Wall: Whitelist Menu", "wmenu.whitelistmenu");
            foreach (var champ in HeroManager.Enemies)
            {
                Variables.WhiteListMenu.Add("wspell.whitelist." + champ.NetworkId,
                    new CheckBox("Use against: " + champ.ChampionName));
            }

            Variables.EMenu = Variables.Menu.AddSubMenu("Use E to:", "emenu");
            Variables.EMenu.Add("espell.combo", new CheckBox("Combo"));
            Variables.EMenu.Add("espell.ks", new CheckBox("KillSteal"));


            Variables.RMenu = Variables.Menu.AddSubMenu("Use R to:", "rmenu");
            Variables.RMenu.Add("rspell.combo", new CheckBox("Combo"));
            Variables.RMenu.Add("rspell.farm", new CheckBox("LaneClear"));

            Variables.MiscMenu = Variables.Menu.AddSubMenu("Miscellaneous", "miscmenu");
            Variables.MiscMenu.Add("misc.tear", new CheckBox("Stack Tear"));

            Variables.DrawingsMenu = Variables.Menu.AddSubMenu("Drawings", "drawingsmenu");
            Variables.DrawingsMenu.Add("drawings.q", new CheckBox("Q Range", false));
            Variables.DrawingsMenu.Add("drawings.w", new CheckBox("W Range", false));
            Variables.DrawingsMenu.Add("drawings.e", new CheckBox("E Range", false));
            Variables.DrawingsMenu.Add("drawings.r", new CheckBox("R Range", false));
        }
    }
}
