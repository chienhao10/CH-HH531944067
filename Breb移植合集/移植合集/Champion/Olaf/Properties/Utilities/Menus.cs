using EloBuddy.SDK.Menu.Values;
using ExorAIO.Utilities;

namespace ExorAIO.Champions.Olaf
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
            Variables.QMenu = Variables.Menu.AddSubMenu("使用 Q:", "qmenu");
            Variables.QMenu.Add("qspell.combo", new CheckBox("连招使用"));
            Variables.QMenu.Add("qspell.harass", new CheckBox("骚扰使用"));
            Variables.QMenu.Add("qspell.ks", new CheckBox("抢头使用"));
            Variables.QMenu.Add("qspell.farm", new CheckBox("清线使用"));
            Variables.QMenu.Add("qspell.mana", new Slider("骚扰/清线: 蓝量使用 >= x", 50, 10, 99));

            Variables.WMenu = Variables.Menu.AddSubMenu("使用 W:", "wmenu");
            Variables.WMenu.Add("wspell.combo", new CheckBox("连招使用"));

            Variables.EMenu = Variables.Menu.AddSubMenu("使用 E:", "esettingsmenu");
            Variables.EMenu.Add("espell.combo", new CheckBox("连招使用"));
            Variables.EMenu.Add("espell.jgc", new CheckBox("清野使用"));
            Variables.EMenu.Add("espell.mana", new Slider("清野: 蓝量使用 >= x", 50, 10, 99));

            Variables.RMenu = Variables.Menu.AddSubMenu("使用 R:", "rmenu");
            Variables.RMenu.Add("rspell.auto", new CheckBox("智能逻辑 (净化)"));

            Variables.DrawingsMenu = Variables.Menu.AddSubMenu("线圈", "drawingsmenu");
            Variables.DrawingsMenu.Add("drawings.q", new CheckBox("Q 范围", false));
                //.SetValue(false).SetFontStyle(FontStyle.Regular, Color.Green);
            Variables.DrawingsMenu.Add("drawings.e", new CheckBox("E 范围", false));
                //.SetValue(false).SetFontStyle(FontStyle.Regular, Color.Cyan);
        }
    }
}