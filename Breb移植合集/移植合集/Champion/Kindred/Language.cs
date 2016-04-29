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
            comboMenu = config.AddSubMenu("连招", "Combo Settings");
            comboMenu.Add("q.combo.style", new ComboBox("(Q) 连招模式", 1, "鼠标", "100% 命中", "安全位置"));
            comboMenu.Add("q.combo", new CheckBox("使用 (Q)"));
            comboMenu.Add("w.combo", new CheckBox("使用 (W)"));
            comboMenu.Add("e.combo", new CheckBox("使用 (E)"));

            eMenu = config.AddSubMenu("(E) 设置", "(E) Settings");
            eMenu.AddGroupLabel("(E) 白名单");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
            {
                eMenu.Add("enemy." + enemy.CharData.BaseSkinName, new CheckBox(string.Format("E: {0}", enemy.CharData.BaseSkinName), Program.HighChamps.Contains(enemy.CharData.BaseSkinName)));
            }

            harassMenu = config.AddSubMenu("骚扰", "Harass Settings");
            harassMenu.Add("q.harass", new CheckBox("使用 Q"));
            harassMenu.Add("w.harass", new CheckBox("使用 W"));
            harassMenu.Add("e.harass", new CheckBox("使用 E"));
            harassMenu.Add("harass.mana", new Slider("蓝量管理", 20, 1, 99));

            laneClearMenu = config.AddSubMenu("清线", "Clear Settings");
            laneClearMenu.Add("q.clear", new CheckBox("使用 Q"));
            laneClearMenu.Add("q.minion.count", new Slider("Q 最低小兵数", 4, 1, 5));
            laneClearMenu.Add("clear.mana", new Slider("蓝量管理", 20, 1, 99));

            jungleClearMenu = config.AddSubMenu("清野", "Jungle Settings");
            jungleClearMenu.Add("q.jungle", new CheckBox("使用 Q"));
            jungleClearMenu.Add("w.jungle", new CheckBox("使用 W"));
            jungleClearMenu.Add("e.jungle", new CheckBox("使用 E"));
            jungleClearMenu.Add("jungle.mana", new Slider("蓝量管理", 20, 1, 99));

            ksMenu = config.AddSubMenu("抢头", "KillSteal Settings");
            ksMenu.Add("q.ks", new CheckBox("使用 Q"));
            ksMenu.Add("q.ks.count", new Slider("普攻次数", 2, 1, 5));

            miscMenu = config.AddSubMenu("杂项", "Miscellaneous");
            miscMenu.Add("q.antigapcloser", new CheckBox("防突进 Q!"));
            miscMenu.AddSeparator();
            miscMenu.AddGroupLabel("防 狮子狗");
            miscMenu.Add("anti.rengar", new CheckBox("防 狮子狗!"));
            miscMenu.Add("hp.percent.for.rengar", new Slider("最低血量%", 30, 1, 99));
            miscMenu.AddSeparator();
            miscMenu.AddGroupLabel("技能打断");
            miscMenu.Add("spell.broker", new CheckBox("技能打断!"));
            miscMenu.Add("katarina.r", new CheckBox("卡特 (R)"));
            miscMenu.Add("missfortune.r", new CheckBox("赏金 (R)"));
            miscMenu.Add("lucian.r", new CheckBox("卢锡安 (R)"));
            miscMenu.Add("hp.percent.for.broke", new Slider("最低血量%", 20, 1, 99));
            miscMenu.AddSeparator();
            miscMenu.AddGroupLabel("(R) 保护");
            miscMenu.Add("protector", new CheckBox("关闭保护?"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
            {
                foreach (var skillshot in SpellDatabase.Spells.Where(x => x.charName == enemy.ChampionName))
                {
                    miscMenu.Add("hero." + skillshot.spellName, new CheckBox("" + skillshot.charName + "(" + skillshot.spellKey + ")"));
                }
            }
            miscMenu.AddSeparator();
            miscMenu.AddGroupLabel("杂项");
            miscMenu.Add("e.method", new ComboBox("E 模式", 0, "当前位置"));
            miscMenu.Add("use.r", new CheckBox("使用 R"));
            miscMenu.AddSeparator();
            miscMenu.AddGroupLabel("R 白名单");
            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(o => o.IsAlly))
            {
                miscMenu.Add("respite." + ally.CharData.BaseSkinName, new CheckBox(string.Format("大招: {0}", ally.CharData.BaseSkinName), Program.HighChamps.Contains(ally.CharData.BaseSkinName)));
            }
            miscMenu.Add("min.hp.for.r", new Slider("R 最低血量%", 20, 1, 99));

            drawMenu = config.AddSubMenu("线圈", "Draw Settings");
            drawMenu.Add("aa.indicator", new CheckBox("普攻 指示器"));
            drawMenu.Add("q.drawx", new CheckBox("Q 范围"));
            drawMenu.Add("w.draw", new CheckBox("W 范围"));
            drawMenu.Add("e.draw", new CheckBox("E 范围"));
            drawMenu.Add("r.draw", new CheckBox("R 范围"));
        }
    }
}