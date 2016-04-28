using EloBuddy.SDK.Menu.Values;
using ExorAIO.Utilities;

namespace ExorAIO.Champions.Pantheon
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
            Variables.QMenu.Add("qspell.ks", new CheckBox("KillSteal"));
            Variables.QMenu.Add("qspell.harass", new CheckBox("Harass"));
            Variables.QMenu.Add("qspell.jgc", new CheckBox("JungleClear"));
            Variables.QMenu.Add("qspell.mana", new Slider("Harass/JungleClear: Mana >= x", 50, 10, 99));

            Variables.WMenu = Variables.Menu.AddSubMenu("Use W to:", "wmenu");
            Variables.WMenu.Add("wspell.combo", new CheckBox("Combo"));
            Variables.WMenu.Add("wspell.ks", new CheckBox("KillSteal"));
            Variables.WMenu.Add("wspell.ir", new CheckBox("Interrupt Enemy Channels"));

            Variables.EMenu = Variables.Menu.AddSubMenu("Use E to:", "emenu");
            Variables.EMenu.Add("espell.combo", new CheckBox("Combo"));
            Variables.EMenu.Add("espell.farm", new CheckBox("Clear"));
            Variables.EMenu.Add("espell.mana", new Slider("Clear: Mana >= x", 50, 10, 99));

            Variables.DrawingsMenu = Variables.Menu.AddSubMenu("Drawings", "drawingsmenu");
            Variables.DrawingsMenu.Add("drawings.q", new CheckBox("Q Range"));
                //.SetValue(false).SetFontStyle(FontStyle.Regular, Color.Green);
            Variables.DrawingsMenu.Add("drawings.w", new CheckBox("W Range"));
                //.SetValue(false).SetFontStyle(FontStyle.Regular, Color.Purple);
            Variables.DrawingsMenu.Add("drawings.e", new CheckBox("E Range"));
                //.SetValue(false).SetFontStyle(FontStyle.Regular, Color.Cyan);
        }
    }
}