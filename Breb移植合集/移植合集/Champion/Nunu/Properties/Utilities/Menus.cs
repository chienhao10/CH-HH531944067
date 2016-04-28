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
            Variables.QMenu = Variables.Menu.AddSubMenu("Use Q to:", "qmenu");
            Variables.QMenu.Add("qspell.jgc", new CheckBox("JungleSteal"));
            Variables.QMenu.Add("qspell.auto", new CheckBox("Logical"));
            Variables.QMenu.Add("qspell.lc", new CheckBox("LaneClear"));
            Variables.QMenu.Add("qspell.mana", new Slider("LaneClear: Mana >= x%", 50, 0, 99));

            Variables.WMenu = Variables.Menu.AddSubMenu("Use W to:", "wmenu");
            Variables.WMenu.Add("wspell.auto", new CheckBox("Logical"));
            Variables.WMenu.Add("wspell.mana", new Slider("Logical: Mana >= x%", 50, 0, 99));

            Variables.WhiteListMenu = Variables.Menu.AddSubMenu("W: Whitelist Menu", "wmenu.whitelistmenu");
            {
                foreach (var champ in HeroManager.Allies.Where(h => !h.IsMe))
                {
                    Variables.WhiteListMenu.Add("wspell.whitelist." + champ.ChampionName.ToLower(),
                        new CheckBox("Use on: " + champ.ChampionName));
                }
            }

            Variables.EMenu = Variables.Menu.AddSubMenu("Use E to:", "emenu");
            Variables.EMenu.Add("espell.combo", new CheckBox("Combo"));
            Variables.EMenu.Add("espell.ks", new CheckBox("KillSteal"));
            Variables.EMenu.Add("espell.harass", new CheckBox("Harass"));
            Variables.EMenu.Add("espell.farm", new CheckBox("Clear"));
            Variables.EMenu.Add("espell.mana", new Slider("Harass/Clear: Mana >= x%", 50, 0, 99));

            Variables.RMenu = Variables.Menu.AddSubMenu("Use R to:", "rmenu");
            Variables.RMenu.Add("rspell.boolrsa", new CheckBox("Semi-Automatic R"));
            Variables.RMenu.Add("rspell.keyrsa", new KeyBind("Key:", false, KeyBind.BindTypes.HoldActive, 'T'));

            Variables.DrawingsMenu = Variables.Menu.AddSubMenu("Drawings", "drawingsmenu");
            Variables.DrawingsMenu.Add("drawings.q", new CheckBox("Q Range"));
                //.SetValue(false).SetFontStyle(FontStyle.Regular, Color.Green);
            Variables.DrawingsMenu.Add("drawings.w", new CheckBox("W Range"));
                //.SetValue(false).SetFontStyle(FontStyle.Regular, Color.Purple);
            Variables.DrawingsMenu.Add("drawings.e", new CheckBox("E Range"));
                //.SetValue(false).SetFontStyle(FontStyle.Regular, Color.Cyan);
            Variables.DrawingsMenu.Add("drawings.r", new CheckBox("R Range"));
                //.SetValue(false).SetFontStyle(FontStyle.Regular, Color.Red);
        }
    }
}