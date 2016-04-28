using EloBuddy.SDK.Menu.Values;
using ExorAIO.Utilities;

namespace ExorAIO.Champions.Ryze
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
            /// <summary>
            ///     Sets the prediction menu.
            /// </summary>

            Variables.QMenu = Variables.Menu.AddSubMenu("Use Q to:", "qmenu");
            Variables.QMenu.Add("qspell.combo", new CheckBox("Combo"));
            Variables.QMenu.Add("qspell.harass", new CheckBox("Harass"));
            Variables.QMenu.Add("qspell.ks", new CheckBox("KillSteal"));
            Variables.QMenu.Add("qspell.farm", new CheckBox("Clear"));
            Variables.QMenu.Add("qspell.mana", new Slider("Harass/Clear: Mana >= x%", 50, 0, 99));

            Variables.WMenu = Variables.Menu.AddSubMenu("Use W to:", "wmenu");
            Variables.WMenu.Add("wspell.combo", new CheckBox("Combo"));
            Variables.WMenu.Add("wspell.ks", new CheckBox("KillSteal"));
            Variables.WMenu.Add("wspell.gp", new CheckBox("Anti-Gapcloser"));
            Variables.WMenu.Add("wspell.farm", new CheckBox("Clear"));
            Variables.WMenu.Add("wspell.mana", new Slider("Clear: Mana >= x%", 50, 0, 99));

            Variables.EMenu = Variables.Menu.AddSubMenu("Use E to:", "emenu");
            Variables.EMenu.Add("espell.combo", new CheckBox("Combo"));
            Variables.EMenu.Add("espell.ks", new CheckBox("KillSteal"));
            Variables.EMenu.Add("espell.farm", new CheckBox("Clear"));
            Variables.EMenu.Add("espell.mana", new Slider("Clear: Mana >= x%", 50, 0, 99));

            Variables.RMenu = Variables.Menu.AddSubMenu("Use R to:", "rmenu");
            Variables.RMenu.Add("rspell.combo", new CheckBox("Combo"));
            Variables.RMenu.Add("rspell.farm", new CheckBox("Clear"));

            Variables.MiscMenu = Variables.Menu.AddSubMenu("Miscellaneous", "miscmenu");
            Variables.MiscMenu.Add("misc.manager", new CheckBox("Keep Perfect Passive (3)", false));
            Variables.MiscMenu.Add("misc.tear", new CheckBox("Stack Tear"));
            Variables.MiscMenu.Add("misc.tearmana", new Slider("KPP/Stack Tear: Mana > x%", 75, 1, 95));

            Variables.DrawingsMenu = Variables.Menu.AddSubMenu("Drawings", "drawingsmenu");
            Variables.DrawingsMenu.Add("drawings.q", new CheckBox("Q Range", false));
            Variables.DrawingsMenu.Add("drawings.w", new CheckBox("W Range", false));
            Variables.DrawingsMenu.Add("drawings.e", new CheckBox("E Range", false));
        }
    }
}