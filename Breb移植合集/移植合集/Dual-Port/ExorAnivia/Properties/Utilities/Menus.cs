using EloBuddy.SDK.Menu.Values;
using ExorSDK.Utilities;
using LeagueSharp.SDK;

namespace ExorSDK.Champions.Anivia
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
            Vars.QMenu = Vars.Menu.AddSubMenu("Use Q to:");
            {
                Vars.QMenu.Add("combo", new CheckBox("Combo", true));
                Vars.QMenu.Add("logical", new CheckBox("Logical", true));
                Vars.QMenu.Add("killsteal", new CheckBox("KillSteal", true));
                Vars.QMenu.Add("clear", new Slider("Clear / if Mana >= x%", 50, 0, 101));
            }

            /// <summary>
            ///     Sets the menu for the W.
            /// </summary>
            Vars.WMenu = Vars.Menu.AddSubMenu("Use W to:");
            {
                Vars.WMenu.Add("combo", new CheckBox("Combo", true));
                Vars.WMenu.Add("logical", new CheckBox("Logical", true));
                Vars.WMenu.Add("interrupter", new CheckBox("Interrupt Enemy Channels", true));
                Vars.WMenu.Add("gapcloser", new CheckBox("Anti-Gapcloser", true));
                {
                    /// <summary>
                    ///     Sets the menu for the W Whitelist.
                    /// </summary>
                    Vars.WhiteListMenu = Vars.Menu.AddSubMenu("Wall: Whitelist Menu");
                    {
                        foreach (var target in GameObjects.EnemyHeroes)
                        {
                            Vars.WhiteListMenu.Add(target.ChampionName.ToLower(), new CheckBox($"Use against: {target.ChampionName}", true));
                        }
                    }
                }
            }

            /// <summary>
            ///     Sets the menu for the E.
            /// </summary>
            Vars.EMenu = Vars.Menu.AddSubMenu("Use E to:");
            {
                Vars.EMenu.Add("combo", new CheckBox("Combo", true));
                Vars.EMenu.Add("killsteal", new CheckBox("KillSteal", true));
            }

            /// <summary>
            ///     Sets the menu for the R.
            /// </summary>
            Vars.RMenu = Vars.Menu.AddSubMenu("Use R to:");
            {
                Vars.RMenu.Add("combo", new CheckBox("Combo", true));
                Vars.RMenu.Add("clear", new Slider("Clear / if Mana >= x%", 50, 0, 101));
            }

            /// <summary>
            ///     Sets the miscellaneous menu.
            /// </summary>
            Vars.MiscMenu = Vars.Menu.AddSubMenu("Miscellaneous");
            {
                Vars.MiscMenu.Add("tear", new CheckBox("Stack Tear", true));
            }

            /// <summary>
            ///     Sets the menu for the drawings.
            /// </summary>
            Vars.DrawingsMenu = Vars.Menu.AddSubMenu("Drawings");
            {
                Vars.DrawingsMenu.Add("q", new CheckBox("Q Range"));
                Vars.DrawingsMenu.Add("w", new CheckBox("W Range"));
                Vars.DrawingsMenu.Add("e", new CheckBox("E Range"));
                Vars.DrawingsMenu.Add("r", new CheckBox("R Range"));
            }
        }
    }
}