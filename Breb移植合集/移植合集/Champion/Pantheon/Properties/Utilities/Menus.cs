using EloBuddy.SDK.Menu.Values;
using ExorAIO.Utilities;

namespace ExorAIO.Champions.Pantheon
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
            Variables.QMenu = Variables.Menu.AddSubMenu("使用Q:", "qmenu");
            Variables.QMenu.Add("qspell.combo", new CheckBox("连招"));
            Variables.QMenu.Add("qspell.ks", new CheckBox("抢头"));
            Variables.QMenu.Add("qspell.harass", new CheckBox("骚扰"));
            Variables.QMenu.Add("qspell.jgc", new CheckBox("清野"));
            Variables.QMenu.Add("qspell.mana", new Slider("骚扰/清野: 蓝量 >= x", 50, 10, 99));

            Variables.WMenu = Variables.Menu.AddSubMenu("使用W:", "wmenu");
            Variables.WMenu.Add("wspell.combo", new CheckBox("连招"));
            Variables.WMenu.Add("wspell.ks", new CheckBox("抢头"));
            Variables.WMenu.Add("wspell.ir", new CheckBox("技能打断"));

            Variables.EMenu = Variables.Menu.AddSubMenu("使用E:", "emenu");
            Variables.EMenu.Add("espell.combo", new CheckBox("连招"));
            Variables.EMenu.Add("espell.farm", new CheckBox("农兵"));
            Variables.EMenu.Add("espell.mana", new Slider("农兵: 蓝量 >= x", 50, 10, 99));

            Variables.DrawingsMenu = Variables.Menu.AddSubMenu("线圈", "drawingsmenu");
            Variables.DrawingsMenu.Add("drawings.q", new CheckBox("Q 范围"));
                //.SetValue(false).SetFontStyle(FontStyle.Regular, Color.Green);
            Variables.DrawingsMenu.Add("drawings.w", new CheckBox("W 范围"));
                //.SetValue(false).SetFontStyle(FontStyle.Regular, Color.Purple);
            Variables.DrawingsMenu.Add("drawings.e", new CheckBox("E 范围"));
                //.SetValue(false).SetFontStyle(FontStyle.Regular, Color.Cyan);
        }
    }
}