using EloBuddy.SDK.Menu.Values;
using ExorAIO.Utilities;

namespace ExorAIO.Champions.Olaf
{
    /// <summary>
    ///     The menu class.
    /// </summary>
    internal class Menus
    {
        /// <summary>
        ///     Sets the menu.
        /// </summary>
        public static void Initialize()
        {
            Variables.QMenu = Variables.Menu.AddSubMenu("Use Q to:", "qmenu");
            Variables.QMenu.Add("qspell.combo", new CheckBox("Combo"));
            Variables.QMenu.Add("qspell.harass", new CheckBox("Harass"));
            Variables.QMenu.Add("qspell.ks", new CheckBox("KillSteal"));
            Variables.QMenu.Add("qspell.farm", new CheckBox("Clear"));
            Variables.QMenu.Add("qspell.mana", new Slider("Harass/Clear: Mana >= x", 50, 10, 99));

            Variables.WMenu = Variables.Menu.AddSubMenu("Use W to:", "wmenu");
            Variables.WMenu.Add("wspell.combo", new CheckBox("Combo"));

            Variables.EMenu = Variables.Menu.AddSubMenu("Use E to:", "esettingsmenu");
            Variables.EMenu.Add("espell.combo", new CheckBox("Combo"));
            Variables.EMenu.Add("espell.jgc", new CheckBox("JungleClear"));
            Variables.EMenu.Add("espell.mana", new Slider("JungleClear: Mana >= x", 50, 10, 99));

            Variables.RMenu = Variables.Menu.AddSubMenu("Use R to:", "rmenu");
            Variables.RMenu.Add("rspell.auto", new CheckBox("Logical (Cleanse)"));

            Variables.DrawingsMenu = Variables.Menu.AddSubMenu("Drawings", "drawingsmenu");
            Variables.DrawingsMenu.Add("drawings.q", new CheckBox("Q Range", false));
                //.SetValue(false).SetFontStyle(FontStyle.Regular, Color.Green);
            Variables.DrawingsMenu.Add("drawings.e", new CheckBox("E Range", false));
                //.SetValue(false).SetFontStyle(FontStyle.Regular, Color.Cyan);
        }
    }
}