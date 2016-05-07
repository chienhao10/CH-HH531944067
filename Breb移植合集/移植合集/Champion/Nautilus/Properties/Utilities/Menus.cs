using EloBuddy.SDK.Menu.Values;
using ExorAIO.Utilities;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Nautilus
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
            /// Sets the spells menu.
            /// </summary>
            Variables.QMenu = Variables.Menu.AddSubMenu("使用Q:", "qmenu");
            Variables.QMenu.Add("qspell.combo", new CheckBox("连招"));
            Variables.QMenu.Add("qspell.ks", new CheckBox("抢头"));

            Variables.WMenu = Variables.Menu.AddSubMenu("使用W:", "wmenu");
            Variables.WMenu.Add("wspell.combo", new CheckBox("连招"));

            Variables.EMenu = Variables.Menu.AddSubMenu("使用E:", "emenu");
            Variables.EMenu.Add("espell.combo", new CheckBox("连招"));
            Variables.EMenu.Add("espell.harass", new CheckBox("骚扰"));
            Variables.EMenu.Add("espell.farm", new CheckBox("农兵"));
            Variables.EMenu.Add("espell.mana", new Slider("农兵: 蓝量 >= x%", 50, 0, 99));

            Variables.RMenu = Variables.Menu.AddSubMenu("使用R:", "rmenu");
            Variables.RMenu.Add("rspell.combo", new CheckBox("连招"));
            Variables.RMenu.Add("rspell.ks", new CheckBox("抢头"));

            Variables.WhiteListMenu = Variables.Menu.AddSubMenu("大招: 白名单", "rmenu.whitelistmenu");
            foreach (var champ in HeroManager.Enemies)
            {
                Variables.WhiteListMenu.Add("rspell.whitelist." + champ.NetworkId,
                    new CheckBox("使用于: " + champ.ChampionName));
            }

            /// <summary>
            /// Sets the drawings menu.
            /// </summary>
            Variables.DrawingsMenu = Variables.Menu.AddSubMenu("线圈", "drawingsmenu");
            Variables.DrawingsMenu.Add("drawings.q", new CheckBox("Q 范围"));
                //.SetValue(false).SetFontStyle(FontStyle.Regular, Color.Green);
            Variables.DrawingsMenu.Add("drawings.w", new CheckBox("W 范围"));
                //.SetValue(false).SetFontStyle(FontStyle.Regular, Color.Purple);
            Variables.DrawingsMenu.Add("drawings.e", new CheckBox("E 范围"));
                //.SetValue(false).SetFontStyle(FontStyle.Regular, Color.Cyan);
            Variables.DrawingsMenu.Add("drawings.r", new CheckBox("R 范围"));
                //.SetValue(false).SetFontStyle(FontStyle.Regular, Color.Red);
        }
    }
}