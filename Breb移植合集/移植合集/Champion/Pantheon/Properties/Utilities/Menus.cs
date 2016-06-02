using EloBuddy.SDK.Menu.Values;
using ExorSDK.Utilities;

namespace ExorSDK.Champions.Pantheon
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
            Vars.QMenu = Vars.Menu.AddSubMenu("使用Q:");
            {
                Vars.QMenu.Add("combo", new CheckBox("连招"));
                Vars.QMenu.Add("killsteal", new CheckBox("抢头"));
                Vars.QMenu.Add("harass", new Slider("骚扰 / 如果蓝量 >= x%", 50, 10, 101));
                Vars.QMenu.Add("jungleclear", new Slider("清野 / 如果蓝量 >= x%", 50, 10, 101));
            }

            /// <summary>
            ///     Sets the menu for the W.
            /// </summary>
            Vars.WMenu = Vars.Menu.AddSubMenu("使用W:");
            {
                Vars.WMenu.Add("combo", new CheckBox("连招"));
                Vars.WMenu.Add("killsteal", new CheckBox("抢头"));
                Vars.WMenu.Add("interrupter", new CheckBox("技能打断"));
            }

            /// <summary>
            ///     Sets the menu for the E.
            /// </summary>
            Vars.EMenu = Vars.Menu.AddSubMenu("使用E:");
            {
                Vars.EMenu.Add("combo", new CheckBox("连招"));
                Vars.EMenu.Add("clear", new Slider("清线 /如果蓝量 >= x%", 50, 10, 101));
            }

            /// <summary>
            ///     Sets the drawings menu.
            /// </summary>
            Vars.DrawingsMenu = Vars.Menu.AddSubMenu("线圈");
            {
                Vars.DrawingsMenu.Add("q", new CheckBox("Q 范围"));
                Vars.DrawingsMenu.Add("w", new CheckBox("W 范围"));
                Vars.DrawingsMenu.Add("e", new CheckBox("E 范围"));
            }
        }
    }
}