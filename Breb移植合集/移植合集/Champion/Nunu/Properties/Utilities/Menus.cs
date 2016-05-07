using System.Linq;
using EloBuddy.SDK.Menu.Values;
using ExorAIO.Utilities;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Nunu
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
            Variables.QMenu.Add("qspell.jgc", new CheckBox("偷野"));
            Variables.QMenu.Add("qspell.auto", new CheckBox("自动逻辑"));
            Variables.QMenu.Add("qspell.lc", new CheckBox("清线"));
            Variables.QMenu.Add("qspell.mana", new Slider("清线蓝量 >= x%", 50, 0, 99));

            Variables.WMenu = Variables.Menu.AddSubMenu("使用W:", "wmenu");
            Variables.WMenu.Add("wspell.auto", new CheckBox("自动逻辑"));
            Variables.WMenu.Add("wspell.mana", new Slider("逻辑蓝量 >= x%", 50, 0, 99));

            Variables.WhiteListMenu = Variables.Menu.AddSubMenu("W: 白名单", "wmenu.whitelistmenu");
            {
                foreach (var champ in HeroManager.Allies.Where(h => !h.IsMe))
                {
                    Variables.WhiteListMenu.Add("wspell.whitelist." + champ.NetworkId, new CheckBox("使用在: " + champ.ChampionName));
                }
            }

            Variables.EMenu = Variables.Menu.AddSubMenu("使用E:", "emenu");
            Variables.EMenu.Add("espell.combo", new CheckBox("连招"));
            Variables.EMenu.Add("espell.ks", new CheckBox("抢头"));
            Variables.EMenu.Add("espell.harass", new CheckBox("骚扰"));
            Variables.EMenu.Add("espell.farm", new CheckBox("农兵"));
            Variables.EMenu.Add("espell.mana", new Slider("骚扰/推选: 蓝量 >= x%", 50, 0, 99));

            Variables.RMenu = Variables.Menu.AddSubMenu("使用R:", "rmenu");
            Variables.RMenu.Add("rspell.boolrsa", new CheckBox("开启半自动按键 R"));
            Variables.RMenu.Add("rspell.keyrsa", new KeyBind("按键:", false, KeyBind.BindTypes.HoldActive, 'T'));

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