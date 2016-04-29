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
            Variables.QMenu = Variables.Menu.AddSubMenu("Q 设置");
            Variables.QMenu.Add("qspell.combo", new CheckBox("连招"));
            Variables.QMenu.Add("qspell.harass", new CheckBox("骚扰"));
            Variables.QMenu.Add("qspell.farm", new CheckBox("清线"));
            Variables.QMenu.Add("qspell.mana", new Slider("骚扰/清线: 蓝量 >= x%", 50, 0, 99));

            Variables.WMenu = Variables.Menu.AddSubMenu("W 设置");
            Variables.WMenu.Add("wspell.combo", new CheckBox("连招使用"));

            Variables.EMenu = Variables.Menu.AddSubMenu("E 设置");
            Variables.EMenu.Add("espell.combo", new CheckBox("连招使用"));
            Variables.EMenu.Add("espell.gp", new CheckBox("防突进"));

            Variables.RMenu = Variables.Menu.AddSubMenu("R 设置");
            Variables.RMenu.Add("rspell.ks", new CheckBox("抢头"));

            /// <summary>
            /// Sets the drawings menu.
            /// </summary>
            Variables.DrawingsMenu = Variables.Menu.AddSubMenu("线圈");
            Variables.DrawingsMenu.Add("drawings.q", new CheckBox("Q 范围"));
            Variables.DrawingsMenu.Add("drawings.w", new CheckBox("W 范围"));
            Variables.DrawingsMenu.Add("drawings.e", new CheckBox("E 范围"));
            Variables.DrawingsMenu.Add("drawings.r", new CheckBox("R 范围"));
        }
    }
}