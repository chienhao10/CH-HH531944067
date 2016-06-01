using System.Linq;
using ExorSDK.Utilities;
using LeagueSharp.SDK;
using EloBuddy.SDK.Menu.Values;

namespace ExorSDK.Champions.Nunu
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
            ///     Sets the menu for the Q.
            /// </summary>
            Vars.QMenu = Vars.Menu.AddSubMenu("Use Q to:");
            {
                Vars.QMenu.Add("jungleclear", new CheckBox("JungleSteal", true));
                Vars.QMenu.Add("logical", new CheckBox("Logical", true));
                Vars.QMenu.Add("laneclear", new Slider("LaneClear / if Mana >= x%", 50, 0, 101));
            }

            /// <summary>
            ///     Sets the menu for the W.
            /// </summary>
            Vars.WMenu = Vars.Menu.AddSubMenu("Use W to:");
            {
                Vars.WMenu.Add("logical", new Slider("Logical / if Mana >= x%", 50, 0, 101));
                {
                    /// <summary>
                    ///     Sets the whitelist menu for the W.
                    /// </summary>
                    Vars.WhiteListMenu = Vars.Menu.AddSubMenu("W: Whitelist Menu");
                    {
                        foreach (var target in GameObjects.AllyHeroes.Where(h => !h.IsMe))
                        {
                            Vars.WhiteListMenu.Add(target.ChampionName.ToLower(), new CheckBox($"Use on: {target.ChampionName}", true));
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
                Vars.EMenu.Add("harass", new Slider("Harass / if Mana >= x%", 50, 0, 101));
                Vars.EMenu.Add("clear", new Slider("Clear / if Mana >= x%", 50, 0, 101));
            }

            /// <summary>
            ///     Sets the menu for the R.
            /// </summary>
            Vars.RMenu = Vars.Menu.AddSubMenu("Use R to:");
            {
                Vars.RMenu.Add("bool", new CheckBox("Semi-Automatic R", true));
                Vars.RMenu.Add("key", new KeyBind("Key:", false, KeyBind.BindTypes.HoldActive, 'T'));
            }

            /// <summary>
            ///     Sets the drawings menu.
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