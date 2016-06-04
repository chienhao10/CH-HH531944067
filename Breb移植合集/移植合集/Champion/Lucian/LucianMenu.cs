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
            Config = MainMenu.AddMenu("LCS 合集: 卢锡安", "LCS Series: Lucian");

            comboMenu = Config.AddSubMenu(":: 连招", ":: Combo Settings");
            comboMenu.Add("lucian.q.combo", new CheckBox("使用 Q"));
            comboMenu.Add("lucian.e.combo", new CheckBox("使用 E"));
            comboMenu.Add("lucian.e.mode", new ComboBox("E 模式", 0, "安全位置", "鼠标位置"));
            comboMenu.Add("lucian.w.combo", new CheckBox("使用 W"));
            comboMenu.Add("lucian.disable.w.prediction", new CheckBox("关闭 W 预判"));
            comboMenu.Add("lucian.r.combo", new CheckBox("使用 R"));
            comboMenu.Add("lucian.combo.start.e", new CheckBox("E 全明星连招"));

            harassMenu = Config.AddSubMenu(":: 骚扰", ":: Harass Settings");
            harassMenu.Add("lucian.q.harass", new CheckBox("使用 Q"));
            harassMenu.Add("lucian.q.type", new ComboBox("骚扰模式", 0, "加长", "正常"));
            harassMenu.Add("lucian.w.harass", new CheckBox("使用 W"));
            harassMenu.Add("lucian.harass.mana", new Slider("最低蓝量", 50, 1, 99));

            clearMenu = Config.AddSubMenu(":: 推线", ":: Clear Settings");
            clearMenu.Add("lucian.q.clear", new CheckBox("使用 Q"));
            clearMenu.Add("lucian.w.clear", new CheckBox("使用 W"));
            clearMenu.Add("lucian.q.minion.hit.count", new Slider("(Q) 最少小兵数量", 3, 1, 5));
            clearMenu.Add("lucian.w.minion.hit.count", new Slider("(W) 最低小兵数量", 3, 1, 5));
            clearMenu.Add("lucian.clear.mana", new Slider("最低蓝量", 50, 1, 99));

            jungleMenu = Config.AddSubMenu(":: 清野", ":: Jungle Settings");
            jungleMenu.Add("lucian.q.jungle", new CheckBox("使用 Q"));
            jungleMenu.Add("lucian.w.jungle", new CheckBox("使用 W"));
            jungleMenu.Add("lucian.e.jungle", new CheckBox("使用 E"));
            jungleMenu.Add("lucian.jungle.mana", new Slider("最低蓝量", 50, 1, 99));

            killStealMenu = Config.AddSubMenu(":: 抢头", ":: KillSteal Settings");
            killStealMenu.Add("lucian.q.ks", new CheckBox("使用 Q"));
            killStealMenu.Add("lucian.w.ks", new CheckBox("使用 W"));

            miscMenu = Config.AddSubMenu(":: 杂项", ":: Miscellaneous");
            miscMenu.AddGroupLabel("自定义防突进");
            foreach (var gapclose in AntiGapcloseSpell.GapcloseableSpells.Where(x => ObjectManager.Get<AIHeroClient>().Any(y => y.ChampionName == x.ChampionName && y.IsEnemy)))
            {
                miscMenu.Add("gapclose." + gapclose.ChampionName + gapclose.Slot, new CheckBox("防突进: " + gapclose.ChampionName + " - 技能: " + gapclose.Slot));
                miscMenu.Add("gapclose.slider." + gapclose.SpellName + gapclose.Slot, new Slider("" + gapclose.ChampionName + " - 技能: " + gapclose.Slot + " 优先", gapclose.DangerLevel, 1, 5));
            }

            drawMenu = Config.AddSubMenu(":: 线圈", ":: Draw Settings");
            drawMenu.AddGroupLabel(":: 技能线圈");
            drawMenu.Add("lucian.q.draw", new CheckBox("Q 范围"));
            drawMenu.Add("lucian.q2.draw", new CheckBox("Q (加长) 范围"));
            drawMenu.Add("lucian.w.draw", new CheckBox("W 范围"));
            drawMenu.Add("lucian.e.draw", new CheckBox("E 范围"));
            drawMenu.Add("lucian.r.draw", new CheckBox("R 范围"));

            Config.Add("lucian.semi.manual.ult", new KeyBind("半自动 (R)!", false, KeyBind.BindTypes.HoldActive, 'A'));
        }
    }
}