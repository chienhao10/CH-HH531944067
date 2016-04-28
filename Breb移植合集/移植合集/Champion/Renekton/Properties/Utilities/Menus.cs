using EloBuddy.SDK.Menu.Values;
using ExorAIO.Utilities;

namespace ExorAIO.Champions.Renekton
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
            Variables.QMenu.Add("qspell.auto", new CheckBox("Logical"));
            Variables.QMenu.Add("qspell.farm", new CheckBox("Clear"));

            Variables.WMenu = Variables.Menu.AddSubMenu("Use W to:", "wmenu");
            Variables.WMenu.Add("wspell.combo", new CheckBox("Combo"));
            Variables.WMenu.Add("wspell.jgc", new CheckBox("JungleClear"));

            Variables.EMenu = Variables.Menu.AddSubMenu("Use E to:", "emenu");
            Variables.EMenu.AddLabel("It will only use the 1st E, you will have to manually use the 2nd E.");
            Variables.EMenu.Add("espell.combo", new CheckBox("Combo"));

            Variables.RMenu = Variables.Menu.AddSubMenu("Use R to:", "rmenu");
            Variables.RMenu.Add("rspell.lifesaver", new CheckBox("LifeSaver"));

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