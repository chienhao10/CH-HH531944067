using EloBuddy.SDK.Menu.Values;
using ExorSDK.Utilities;

namespace ExorSDK.Champions.Olaf
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
            Vars.QMenu = Vars.Menu.AddSubMenu("使用 Q:");
            {
                Vars.QMenu.Add("combo", new CheckBox("连招", true));
                Vars.QMenu.Add("killsteal", new CheckBox("抢头", true));
                Vars.QMenu.Add("harass", new Slider("如果蓝量 >= x%", 50, 0, 101));
                Vars.QMenu.Add("clear", new Slider("清线 / 如果蓝量 >= x%", 50, 0, 101));
            }

            /// <summary>
            ///     Sets the menu for the W.
            /// </summary>
            Vars.WMenu = Vars.Menu.AddSubMenu("使用 W:");
            {
                Vars.WMenu.Add("combo", new CheckBox("连招", true));
                Vars.WMenu.Add("clear", new Slider("清线 / 如果蓝量 >= x%", 50, 0, 101));
                Vars.WMenu.Add("buildings", new Slider("Buildings / 清线 >= x%", 50, 0, 101));
            }

            /// <summary>
            ///     Sets the menu for the E.
            /// </summary>
            Vars.EMenu = Vars.Menu.AddSubMenu("使用 E:");
            {
                Vars.EMenu.Add("combo", new CheckBox("连招", true));
                Vars.EMenu.Add("jungleclear", new Slider("清野 / 如果蓝量 >= x%", 50, 0, 101));
            }

            /// <summary>
            ///     Sets the menu for the R.
            /// </summary>
            Vars.RMenu = Vars.Menu.AddSubMenu("使用 R:");
            {
                Vars.RMenu.Add("logical", new CheckBox("逻辑", true));
            }

            /// <summary>
            ///     Sets the drawings menu.
            /// </summary>
            Vars.DrawingsMenu = Vars.Menu.AddSubMenu("线圈");
            {
                Vars.DrawingsMenu.Add("q", new CheckBox("Q 范围"));
                Vars.DrawingsMenu.Add("e", new CheckBox("E 范围"));
            }
        }
    }
}