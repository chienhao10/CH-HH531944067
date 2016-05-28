using EloBuddy.SDK.Menu.Values;
using ExorSDK.Utilities;

namespace ExorSDK.Champions.Caitlyn
{
    /// <summary>
    ///     The menu class.
    /// </summary>
    internal class Menus
    {
        /// <summary>
        ///     Initializes the menus.
        /// </summary>
        public static void Initialize()
        {
            /// <summary>
            ///     Sets the menu for the Q.
            /// </summary>
            Vars.QMenu = Vars.Menu.AddSubMenu("Use Q to:", "Q");
            {
                Vars.QMenu.Add("logical", new CheckBox("Logical", true));
                Vars.QMenu.Add("killsteal", new CheckBox("KillSteal", true));
                Vars.QMenu.Add("clear", new Slider("Clear / if Mana >= x%", 50, 0, 101));
            }

            /// <summary>
            ///     Sets the menu for the W.
            /// </summary>
            Vars.WMenu = Vars.Menu.AddSubMenu("Use W to:", "W");
            {
                Vars.WMenu.Add("logical", new CheckBox("Logical", true));
                Vars.WMenu.Add("gapcloser", new CheckBox("Anti-Gapcloser", true));
            }

            /// <summary>
            ///     Sets the menu for the E.
            /// </summary>
            Vars.EMenu = Vars.Menu.AddSubMenu("Use E to:", "e");
            {
                Vars.EMenu.Add("combo", new CheckBox("Combo", true));
                Vars.EMenu.Add("gapcloser", new CheckBox("Anti-Gapcloser", true));
            }

            /// <summary>
            ///     Sets the menu for the R.
            /// </summary>
            Vars.RMenu = Vars.Menu.AddSubMenu("Use R to:", "r");
            {
                Vars.RMenu.Add("killsteal", new CheckBox("KillSteal", true));
                Vars.RMenu.Add("bool", new CheckBox("Semi-Automatic R", true));
                Vars.RMenu.Add("key", new KeyBind("Key (Semi-Auto) : ", false, KeyBind.BindTypes.HoldActive, 'T'));
            }

            /// <summary>
            ///     Sets the menu for the drawings.
            /// </summary>
            Vars.DrawingsMenu = Vars.Menu.AddSubMenu("Drawings", "Ddrawings");
            {
                Vars.DrawingsMenu.Add("q", new CheckBox("Q Range"));
                Vars.DrawingsMenu.Add("w", new CheckBox("W Range"));
                Vars.DrawingsMenu.Add("e", new CheckBox("E Range"));
                Vars.DrawingsMenu.Add("r", new CheckBox("R Range"));
            }
        }
    }
}