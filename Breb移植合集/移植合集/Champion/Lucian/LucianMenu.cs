using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

namespace LCS_Lucian
{
    internal class LucianMenu
    {
        public static Menu Config, comboMenu, harassMenu, clearMenu, jungleMenu, killStealMenu, miscMenu, drawMenu;

        public static void MenuInit()
        {
            Config = MainMenu.AddMenu("LCS Series: Lucian", "LCS Series: Lucian");

            comboMenu = Config.AddSubMenu(":: Combo Settings", ":: Combo Settings");
            comboMenu.Add("lucian.q.combo", new CheckBox("Use Q"));
            comboMenu.Add("lucian.e.combo", new CheckBox("Use E"));
            comboMenu.Add("lucian.e.mode", new ComboBox("E Type", 0, "Safe", "Cursor Position"));
            comboMenu.Add("lucian.w.combo", new CheckBox("Use W"));
            comboMenu.Add("lucian.disable.w.prediction", new CheckBox("Disable W Prediction"));
            comboMenu.Add("lucian.r.combo", new CheckBox("Use R"));
            comboMenu.Add("lucian.combo.start.e", new CheckBox("Start Combo With E"));

            harassMenu = Config.AddSubMenu(":: Harass Settings", ":: Harass Settings");
            harassMenu.Add("lucian.q.harass", new CheckBox("Use Q"));
            harassMenu.Add("lucian.q.type", new ComboBox("Harass Type", 0, "Extended", "Normal"));
            harassMenu.Add("lucian.w.harass", new CheckBox("Use W"));
            harassMenu.Add("lucian.harass.mana", new Slider("Min. Mana", 50, 1, 99));

            clearMenu = Config.AddSubMenu(":: Clear Settings", ":: Clear Settings");
            clearMenu.Add("lucian.q.clear", new CheckBox("Use Q"));
            clearMenu.Add("lucian.w.clear", new CheckBox("Use W"));
            clearMenu.Add("lucian.q.minion.hit.count", new Slider("(Q) Min. Minion Hit", 3, 1, 5));
            clearMenu.Add("lucian.w.minion.hit.count", new Slider("(W) Min. Minion Hit", 3, 1, 5));
            clearMenu.Add("lucian.clear.mana", new Slider("Min. Mana", 50, 1, 99));

            jungleMenu = Config.AddSubMenu(":: Jungle Settings", ":: Jungle Settings");
            jungleMenu.Add("lucian.q.jungle", new CheckBox("Use Q"));
            jungleMenu.Add("lucian.w.jungle", new CheckBox("Use W"));
            jungleMenu.Add("lucian.e.jungle", new CheckBox("Use E"));
            jungleMenu.Add("lucian.jungle.mana", new Slider("Min. Mana", 50, 1, 99));

            killStealMenu = Config.AddSubMenu(":: KillSteal Settings", ":: KillSteal Settings");
            killStealMenu.Add("lucian.q.ks", new CheckBox("Use Q"));
            killStealMenu.Add("lucian.w.ks", new CheckBox("Use W"));

            miscMenu = Config.AddSubMenu(":: Miscellaneous", ":: Miscellaneous");
            miscMenu.AddGroupLabel("Custom Anti-Gapcloser");
            foreach (
                var gapclose in
                    AntiGapcloseSpell.GapcloseableSpells.Where(
                        x => ObjectManager.Get<AIHeroClient>().Any(y => y.ChampionName == x.ChampionName && y.IsEnemy)))
            {
                miscMenu.Add("gapclose." + gapclose.ChampionName, new CheckBox("Anti-Gapclose: " + gapclose.ChampionName + " - Spell: " + gapclose.Slot));
                miscMenu.Add("gapclose.slider." + gapclose.SpellName, new Slider("" + gapclose.ChampionName + " - Spell: " + gapclose.Slot + " Priorty", gapclose.DangerLevel, 1, 5));
            }

            drawMenu = Config.AddSubMenu(":: Draw Settings", ":: Draw Settings");
            drawMenu.AddGroupLabel(":: Skill Draws");
            drawMenu.Add("lucian.q.draw", new CheckBox("Q Range"));
            drawMenu.Add("lucian.q2.draw", new CheckBox("Q (Extended) Range"));
            drawMenu.Add("lucian.w.draw", new CheckBox("W Range"));
            drawMenu.Add("lucian.e.draw", new CheckBox("E Range"));
            drawMenu.Add("lucian.r.draw", new CheckBox("R Range"));

            Config.Add("lucian.semi.manual.ult", new KeyBind("Semi-Manual (R)!", false, KeyBind.BindTypes.HoldActive, 'A'));
        }
    }
}