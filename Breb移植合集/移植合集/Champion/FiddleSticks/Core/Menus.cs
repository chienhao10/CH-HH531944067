using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Feedlesticks.Core
{
    internal class Menus
    {
        /// <summary>
        ///     Menu
        /// </summary>
        public static Menu Config, comboMenu, qMenu, wMenu, eMenu, harassMenu, clearMenu, jungleMenu, drawMenu;

        /// <summary>
        ///     General Menu
        /// </summary>
        public static void Init()
        {
            comboMenu = Config.AddSubMenu("连招设置", "Combo Settings");
            comboMenu.Add("q.combo", new CheckBox("使用 Q"));
            comboMenu.Add("w.combo", new CheckBox("使用 W"));
            comboMenu.Add("e.combo", new CheckBox("使用 E"));

            qMenu = Config.AddSubMenu("Q 设置", "Q Settings");
            qMenu.AddGroupLabel("Q 白名单");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
            {
                qMenu.Add("q.enemy." + enemy.NetworkId, new CheckBox(string.Format("Q: {0}", enemy.CharData.BaseSkinName), Piorty.HighChamps.Contains(enemy.CharData.BaseSkinName)));
            }
            qMenu.Add("auto.q.immobile", new CheckBox("自动 (Q) 无法移动目标"));
            qMenu.Add("auto.q.channeling", new CheckBox("自动 (Q) 吟唱技能的目标"));

            wMenu = Config.AddSubMenu("W 设置", "W Settings");
            wMenu.AddGroupLabel("W 白名单");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
            {
                wMenu.Add("w.enemy." + enemy.NetworkId, new CheckBox(string.Format("W: {0}", enemy.CharData.BaseSkinName), Piorty.HighChamps.Contains(enemy.CharData.BaseSkinName)));
            }

            eMenu = Config.AddSubMenu("E 设置", "E Settings");
            eMenu.AddGroupLabel("E 白名单");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
            {
                eMenu.Add("e.enemy." + enemy.NetworkId,
                    new CheckBox(string.Format("E: {0}", enemy.CharData.BaseSkinName),
                        Piorty.HighChamps.Contains(enemy.CharData.BaseSkinName)));
            }
            eMenu.Add("e.enemy.count", new Slider("(E) 最低敌人数量", 2, 1, 5));
            eMenu.Add("auto.e.enemy.immobile", new CheckBox("自动 (E) 无法移动目标"));
            eMenu.Add("auto.e.enemy.channeling", new CheckBox("自动 (E) 吟唱技能的目标"));

            harassMenu = Config.AddSubMenu("骚扰设置", "Harass Settings");
            harassMenu.Add("q.harass", new CheckBox("使用 Q"));
            harassMenu.Add("e.harass", new CheckBox("使用 E"));
            harassMenu.Add("harass.mana", new Slider("最低蓝量", 50, 1, 99));

            clearMenu = Config.AddSubMenu("清线设置", "Clear Settings");
            clearMenu.Add("w.clear", new CheckBox("使用 W"));
            clearMenu.Add("e.clear", new CheckBox("使用 E"));
            clearMenu.Add("e.minion.hit.count", new Slider("(E) 最低小兵数量", 3, 1, 5));
            clearMenu.Add("clear.mana", new Slider("最低蓝量", 50, 1, 99));

            jungleMenu = Config.AddSubMenu("清野设置", "Jungle Settings");
            jungleMenu.Add("q.jungle", new CheckBox("使用 Q"));
            jungleMenu.Add("w.jungle", new CheckBox("使用 W"));
            jungleMenu.Add("e.jungle", new CheckBox("使用 E"));
            jungleMenu.Add("jungle.mana", new Slider("最低蓝量", 50, 1, 99));

            drawMenu = Config.AddSubMenu("线圈设置", "Draw Settings");
            drawMenu.Add("q.draw", new CheckBox("Q 范围"));
            drawMenu.Add("w.draw", new CheckBox("W 范围"));
            drawMenu.Add("e.draw", new CheckBox("E 范围"));
            drawMenu.Add("r.draw", new CheckBox("R 范围"));
        }
    }
}