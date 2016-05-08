using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

namespace SephLissandra
{
    internal class LissMenu
    {
        public static Menu Config,
            comboMenu,
            ksMenu,
            harassMenu,
            lastHitMenu,
            clearMenu,
            interruptMenu,
            blackListMenu,
            miscMenu,
            drawMenu;

        public static void CreateMenu()
        {
            Config = MainMenu.AddMenu("Seph丽桑卓", "Liss");

            comboMenu = Config.AddSubMenu("连招", "Combo");
            comboMenu.Add("Combo.UseQ", new CheckBox("使用 Q"));
            comboMenu.Add("Combo.UseW", new CheckBox("使用 W"));
            comboMenu.Add("Combo.UseE", new CheckBox("使用 E"));
            comboMenu.Add("Combo.UseE2", new CheckBox("使用 E2"));
            comboMenu.Add("Combo.UseR", new CheckBox("使用 R"));
            comboMenu.Add("Combo.ecountW", new Slider("2E + W 最少敌人数量", 2, 0, 5));
            comboMenu.Add("Combo.ecountR", new Slider("2E + R 最少敌人数量", 2, 0, 5));
            comboMenu.Add("Combo.Rcount", new Slider("最少敌人数量 自身R", 2, 0, 5));
            comboMenu.Add("Combo.MinRHealth", new Slider("敌人血量 X 以上使用R", 25, 1, 100));

            ksMenu = Config.AddSubMenu("抢头", "Killsteal");
            ksMenu.Add("Killsteal", new CheckBox("使用抢头"));
            ksMenu.Add("Killsteal.UseQ", new CheckBox("使用 Q"));
            ksMenu.Add("Killsteal.UseW", new CheckBox("使用 W"));
            ksMenu.Add("Killsteal.UseE", new CheckBox("使用 E"));
            ksMenu.Add("Killsteal.UseE2", new CheckBox("使用 E2"));
            ksMenu.Add("Killsteal.UseR", new CheckBox("使用 R"));
            ksMenu.Add("Killsteal.UseIgnite", new CheckBox("使用 点燃"));

            harassMenu = Config.AddSubMenu("骚扰", "Harass");
            harassMenu.Add("Keys.HarassT", new KeyBind("骚扰开关", false, KeyBind.BindTypes.PressToggle, 'H'));
            harassMenu.Add("Harass.UseQ", new CheckBox("使用 Q"));
            harassMenu.Add("Harass.UseW", new CheckBox("使用 W"));
            harassMenu.Add("Harass.UseE", new CheckBox("使用 E"));
            harassMenu.Add("Harass.Mana", new Slider("骚扰最低蓝量 (%)", 50));

            lastHitMenu = Config.AddSubMenu("尾兵", "Farm");
            lastHitMenu.Add("Farm.Mana", new Slider("最低蓝量 %", 50));
            lastHitMenu.Add("Farm.UseQ", new CheckBox("使用 Q"));
            lastHitMenu.Add("Farm.UseW", new CheckBox("使用 W"));
            lastHitMenu.Add("Farm.UseE", new CheckBox("使用 E"));

            clearMenu = Config.AddSubMenu("清线", "Waveclear");
            clearMenu.Add("Waveclear.UseQ", new CheckBox("使用 Q"));
            clearMenu.Add("Waveclear.UseW", new CheckBox("使用 W"));
            clearMenu.Add("Waveclear.Wcount", new Slider("最低小兵数量 W", 2, 0, 20));
            clearMenu.Add("Waveclear.UseE", new CheckBox("使用 E"));
            clearMenu.Add("Waveclear.UseE2", new CheckBox("使用 E2"));

            interruptMenu = Config.AddSubMenu("Interrupter", "技能打断 +");
            interruptMenu.Add("Interrupter", new CheckBox("打断危险技能"));
            interruptMenu.Add("Interrupter.UseW", new CheckBox("使用 W"));
            interruptMenu.Add("Interrupter.UseR", new CheckBox("使用 R"));
            interruptMenu.Add("Interrupter.AntiGapClose", new CheckBox("防突进"));
            interruptMenu.Add("Interrupter.AG.UseW", new CheckBox("防突进 W"));
            interruptMenu.Add("Interrupter.AG.UseR", new CheckBox("防突进 R"));

            blackListMenu = Config.AddSubMenu("Ultimate BlackList", "黑名单");
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(h => h.IsEnemy))
            {
                blackListMenu.Add("Blacklist." + hero.NetworkId, new CheckBox(hero.ChampionName, false));
            }

            miscMenu = Config.AddSubMenu("杂项", "Misc");
            miscMenu.Add("Misc.PrioritizeUnderTurret", new CheckBox("优先塔下目标"));
            miscMenu.Add("Misc.DontETurret", new CheckBox("不 2E 至塔下"));
            miscMenu.Add("Misc.EMouse", new KeyBind("E 至鼠标按键", false, KeyBind.BindTypes.HoldActive, 'G'));
            miscMenu.AddSeparator();
            miscMenu.Add("Hitchance.Q",
                new ComboBox("Q 命中率", 1, HitChance.Low.ToString(), HitChance.Medium.ToString(),
                    HitChance.High.ToString()));
            miscMenu.Add("Hitchance.E",
                new ComboBox("E 命中率", 1, HitChance.Low.ToString(), HitChance.Medium.ToString(),
                    HitChance.High.ToString()));
            miscMenu.Add("Misc.Debug", new CheckBox("调试", false));

            drawMenu = Config.AddSubMenu("线圈", "Drawing");
            drawMenu.Add("Drawing.DrawQ", new CheckBox("显示 Q"));
            drawMenu.Add("Drawing.DrawW", new CheckBox("显示 W"));
            drawMenu.Add("Drawing.DrawE", new CheckBox("显示 E"));
            drawMenu.Add("Drawing.DrawR", new CheckBox("显示 R"));
        }
    }
}