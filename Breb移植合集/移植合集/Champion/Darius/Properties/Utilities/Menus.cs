using EloBuddy.SDK.Menu.Values;
using ExorAIO.Utilities;

namespace ExorAIO.Champions.Darius
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
            /// Sets the prediction menu.
            /// </summary>

            /// <summary>
            /// Sets the spells menu.
            /// </summary>
            Variables.QMenu = Variables.Menu.AddSubMenu("Q Config");
            Variables.QMenu.Add("qspell.combo", new CheckBox("Combo"));
            Variables.QMenu.Add("qspell.harass", new CheckBox("Harass"));
            Variables.QMenu.Add("qspell.farm", new CheckBox("Clear"));
            Variables.QMenu.Add("qspell.mana", new Slider("Harass/Clear: Mana >= x%", 50, 0, 99));

            Variables.WMenu = Variables.Menu.AddSubMenu("W Config");
            Variables.WMenu.Add("wspell.combo", new CheckBox("Combo"));

            Variables.EMenu = Variables.Menu.AddSubMenu("E Config");
            Variables.EMenu.Add("espell.combo", new CheckBox("Combo"));
            Variables.EMenu.Add("espell.gp", new CheckBox("Anti-Gapcloser"));

            Variables.RMenu = Variables.Menu.AddSubMenu("R Config");
            Variables.RMenu.Add("rspell.ks", new CheckBox("KillSteal"));

            /// <summary>
            /// Sets the drawings menu.
            /// </summary>
            Variables.DrawingsMenu = Variables.Menu.AddSubMenu("Drawings");
            Variables.DrawingsMenu.Add("drawings.q", new CheckBox("Q Range"));
            Variables.DrawingsMenu.Add("drawings.w", new CheckBox("W Range"));
            Variables.DrawingsMenu.Add("drawings.e", new CheckBox("E Range"));
            Variables.DrawingsMenu.Add("drawings.r", new CheckBox("R Range"));
        }
    }
}