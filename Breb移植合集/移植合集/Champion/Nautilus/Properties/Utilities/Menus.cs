using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using ExorSDK.Utilities;
using LeagueSharp.SDK;

namespace ExorSDK.Champions.Nautilus
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
            Vars.QMenu = Vars.Menu.AddSubMenu("q", "使用 Q:");
            {
                Vars.QMenu.Add("combo", new CheckBox("连招", true));
                Vars.QMenu.Add("killsteal", new CheckBox("抢头", true));
            }

            /// <summary>
            ///     Sets the menu for the W.
            /// </summary>
            Vars.WMenu = Vars.Menu.AddSubMenu("w", "使用 W:");
            {
                Vars.WMenu.Add("combo", new CheckBox("连招", true));
                Vars.WMenu.Add("buildings", new Slider("建筑物 / 如果蓝量 >= x%", 50, 0, 101));
                Vars.WMenu.Add("jungleclear", new Slider("清野 / 如果蓝量 >= x%", 50, 0, 101));
            }

            /// <summary>
            ///     Sets the menu for the E.
            /// </summary>
            Vars.EMenu = Vars.Menu.AddSubMenu("e", "使用 E:");
            {
                Vars.EMenu.Add("combo", new CheckBox("连招", true));
                Vars.EMenu.Add("harass", new Slider("骚扰 / 如果蓝量 >= x%", 50, 0, 101));
                Vars.EMenu.Add("clear", new Slider("清线 / 如果蓝量 >= x%", 50, 0, 101));
            }

            /// <summary>
            ///     Sets the menu for the R.
            /// </summary>
            Vars.RMenu = Vars.Menu.AddSubMenu("r", "使用 R:");
            {
                Vars.RMenu.Add("combo", new CheckBox("连招", true));
                Vars.RMenu.Add("killsteal", new CheckBox("抢头", true));
                {
                    /// <summary>
                    ///     Sets the whitelist menu for the R.
                    /// </summary>
                    Vars.WhiteListMenu = Vars.Menu.AddSubMenu("whitelist", "大招白名单");
                    {
                        foreach (var target in GameObjects.EnemyHeroes)
                        {
                            Vars.WhiteListMenu.Add(target.ChampionName.ToLower(), new CheckBox($"Use against: {target.ChampionName}", true));
                        }
                    }
                }
            }

            /// <summary>
            ///     Sets the drawings menu.
            /// </summary>
            Vars.DrawingsMenu = Vars.Menu.AddSubMenu("drawings", "线圈");
            {
                Vars.DrawingsMenu.Add("q", new CheckBox("Q 范围"));
                Vars.DrawingsMenu.Add("w", new CheckBox("W 范围"));
                Vars.DrawingsMenu.Add("e", new CheckBox("E 范围"));
                Vars.DrawingsMenu.Add("r", new CheckBox("R 范围"));
            }
        }
    }
}