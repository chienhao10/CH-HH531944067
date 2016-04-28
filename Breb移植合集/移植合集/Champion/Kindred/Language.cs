using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using Kindred___YinYang.Spell_Database;

namespace Kindred___YinYang
{
    internal class Language
    {
        public static Menu config = Program.Config;
        public static Menu comboMenu, eMenu, harassMenu, laneClearMenu, jungleClearMenu, ksMenu, miscMenu, drawMenu;

        public static void MenuInit()
        {
            EnglishMenu();
        }

        public static void EnglishMenu()
        {
            comboMenu = config.AddSubMenu("Combo Settings", "Combo Settings");
            comboMenu.Add("q.combo.style", new ComboBox("(Q) Combo Style", 1, "Cursor", "100% Hit", "Safe Position"));
            comboMenu.Add("q.combo", new CheckBox("Use (Q)"));
            comboMenu.Add("w.combo", new CheckBox("Use (W)"));
            comboMenu.Add("e.combo", new CheckBox("Use (E)"));

            eMenu = config.AddSubMenu("(E) Settings", "(E) Settings");
            eMenu.AddGroupLabel("(E) Whitelist");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
            {
                eMenu.Add("enemy." + enemy.CharData.BaseSkinName, new CheckBox(string.Format("E: {0}", enemy.CharData.BaseSkinName), Program.HighChamps.Contains(enemy.CharData.BaseSkinName)));
            }

            harassMenu = config.AddSubMenu("Harass Settings", "Harass Settings");
            harassMenu.Add("q.harass", new CheckBox("Use Q"));
            harassMenu.Add("w.harass", new CheckBox("Use W"));
            harassMenu.Add("e.harass", new CheckBox("Use E"));
            harassMenu.Add("harass.mana", new Slider("Mana Manager", 20, 1, 99));

            laneClearMenu = config.AddSubMenu("Clear Settings", "Clear Settings");
            laneClearMenu.Add("q.clear", new CheckBox("Use Q"));
            laneClearMenu.Add("q.minion.count", new Slider("Q Minion Count", 4, 1, 5));
            laneClearMenu.Add("clear.mana", new Slider("Mana Manager", 20, 1, 99));

            jungleClearMenu = config.AddSubMenu("Jungle Settings", "Jungle Settings");
            jungleClearMenu.Add("q.jungle", new CheckBox("Use Q"));
            jungleClearMenu.Add("w.jungle", new CheckBox("Use W"));
            jungleClearMenu.Add("e.jungle", new CheckBox("Use E"));
            jungleClearMenu.Add("jungle.mana", new Slider("Mana Manager", 20, 1, 99));

            ksMenu = config.AddSubMenu("KillSteal Settings", "KillSteal Settings");
            ksMenu.Add("q.ks", new CheckBox("Use Q"));
            ksMenu.Add("q.ks.count", new Slider("Basic Attack Count", 2, 1, 5));

            miscMenu = config.AddSubMenu("Miscellaneous", "Miscellaneous");
            miscMenu.Add("q.antigapcloser", new CheckBox("Anti-Gapcloser Q!"));
            miscMenu.AddSeparator();
            miscMenu.AddGroupLabel("Anti Rengar");
            miscMenu.Add("anti.rengar", new CheckBox("Anti Rengar!"));
            miscMenu.Add("hp.percent.for.rengar", new Slider("Min. HP Percent", 30, 1, 99));
            miscMenu.AddSeparator();
            miscMenu.AddGroupLabel("Spell Breaker");
            miscMenu.Add("spell.broker", new CheckBox("Spell Breaker!"));
            miscMenu.Add("katarina.r", new CheckBox("Katarina (R)"));
            miscMenu.Add("missfortune.r", new CheckBox("Miss Fortune (R)"));
            miscMenu.Add("lucian.r", new CheckBox("Lucian (R)"));
            miscMenu.Add("hp.percent.for.broke", new Slider("Min. HP Percent", 20, 1, 99));
            miscMenu.AddSeparator();
            miscMenu.AddGroupLabel("(R) Protector");
            miscMenu.Add("protector", new CheckBox("Disable Protector?"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
            {
                foreach (var skillshot in SpellDatabase.Spells.Where(x => x.charName == enemy.ChampionName))
                {
                    miscMenu.Add("hero." + skillshot.spellName, new CheckBox("" + skillshot.charName + "(" + skillshot.spellKey + ")"));
                }
            }
            miscMenu.AddSeparator();
            miscMenu.AddGroupLabel("Misc");
            miscMenu.Add("e.method", new ComboBox("E Method", 0, "Cursor Position"));
            miscMenu.Add("use.r", new CheckBox("Use R"));
            miscMenu.AddSeparator();
            miscMenu.AddGroupLabel("R Whitelist");
            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(o => o.IsAlly))
            {
                miscMenu.Add("respite." + ally.CharData.BaseSkinName, new CheckBox(string.Format("Ult: {0}", ally.CharData.BaseSkinName), Program.HighChamps.Contains(ally.CharData.BaseSkinName)));
            }
            miscMenu.Add("min.hp.for.r", new Slider("Min. HP Percent for R", 20, 1, 99));

            drawMenu = config.AddSubMenu("Draw Settings", "Draw Settings");
            drawMenu.Add("aa.indicator", new CheckBox("AA Indicator"));
            drawMenu.Add("q.drawx", new CheckBox("Q Range"));
            drawMenu.Add("w.draw", new CheckBox("W Range"));
            drawMenu.Add("e.draw", new CheckBox("E Range"));
            drawMenu.Add("r.draw", new CheckBox("R Range"));
        }
    }
}