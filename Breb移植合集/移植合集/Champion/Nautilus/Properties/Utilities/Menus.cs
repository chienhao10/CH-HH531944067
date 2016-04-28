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
            Variables.QMenu = Variables.Menu.AddSubMenu("Use Q to:", "qmenu");
            Variables.QMenu.Add("qspell.combo", new CheckBox("Combo"));
            Variables.QMenu.Add("qspell.ks", new CheckBox("KillSteal"));

            Variables.WMenu = Variables.Menu.AddSubMenu("Use W to:", "wmenu");
            Variables.WMenu.Add("wspell.combo", new CheckBox("Combo"));

            Variables.EMenu = Variables.Menu.AddSubMenu("Use E to:", "emenu");
            Variables.EMenu.Add("espell.combo", new CheckBox("Combo"));
            Variables.EMenu.Add("espell.harass", new CheckBox("Harass"));
            Variables.EMenu.Add("espell.farm", new CheckBox("Clear"));
            Variables.EMenu.Add("espell.mana", new Slider("Clear: Mana >= x%", 50, 0, 99));

            Variables.RMenu = Variables.Menu.AddSubMenu("Use R to:", "rmenu");
            Variables.RMenu.Add("rspell.combo", new CheckBox("Combo"));
            Variables.RMenu.Add("rspell.ks", new CheckBox("KillSteal"));

            Variables.WhiteListMenu = Variables.Menu.AddSubMenu("Ultimate: Whitelist Menu", "rmenu.whitelistmenu");
            foreach (var champ in HeroManager.Enemies)
            {
                Variables.WhiteListMenu.Add("rspell.whitelist." + champ.ChampionName.ToLower(),
                    new CheckBox("Use against: " + champ.ChampionName));
            }

            /// <summary>
            /// Sets the drawings menu.
            /// </summary>
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