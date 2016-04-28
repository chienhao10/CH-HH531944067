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
            comboMenu = Config.AddSubMenu("Combo Settings", "Combo Settings");
            comboMenu.Add("q.combo", new CheckBox("Use Q"));
            comboMenu.Add("w.combo", new CheckBox("Use W"));
            comboMenu.Add("e.combo", new CheckBox("Use E"));

            qMenu = Config.AddSubMenu("Q Settings", "Q Settings");
            qMenu.AddGroupLabel("Q Whitelist");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
            {
                qMenu.Add("q.enemy." + enemy.ChampionName,
                    new CheckBox(string.Format("Q: {0}", enemy.CharData.BaseSkinName),
                        Piorty.HighChamps.Contains(enemy.CharData.BaseSkinName)));
            }
            qMenu.Add("auto.q.immobile", new CheckBox("Auto (Q) If Enemy Immobile"));
            qMenu.Add("auto.q.channeling", new CheckBox("Auto (Q) If Enemy Casting Channeling Spell"));

            wMenu = Config.AddSubMenu("W Settings", "W Settings");
            wMenu.AddGroupLabel("W Whitelist");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
            {
                wMenu.Add("w.enemy." + enemy.ChampionName,
                    new CheckBox(string.Format("W: {0}", enemy.CharData.BaseSkinName),
                        Piorty.HighChamps.Contains(enemy.CharData.BaseSkinName)));
            }

            eMenu = Config.AddSubMenu("E Settings", "E Settings");
            eMenu.AddGroupLabel("E Whitelist");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
            {
                eMenu.Add("e.enemy." + enemy.ChampionName,
                    new CheckBox(string.Format("E: {0}", enemy.CharData.BaseSkinName),
                        Piorty.HighChamps.Contains(enemy.CharData.BaseSkinName)));
            }
            eMenu.Add("e.enemy.count", new Slider("(E) Min. Enemy", 2, 1, 5));
            eMenu.Add("auto.e.enemy.immobile", new CheckBox("Auto (E) If Enemy Immobile"));
            eMenu.Add("auto.e.enemy.channeling", new CheckBox("Auto (E) If Enemy Casting Channeling Spell"));

            harassMenu = Config.AddSubMenu("Harass Settings", "Harass Settings");
            harassMenu.Add("q.harass", new CheckBox("Use Q"));
            harassMenu.Add("e.harass", new CheckBox("Use E"));
            harassMenu.Add("harass.mana", new Slider("Min. Mana", 50, 1, 99));

            clearMenu = Config.AddSubMenu("Clear Settings", "Clear Settings");
            clearMenu.Add("w.clear", new CheckBox("Use W"));
            clearMenu.Add("e.clear", new CheckBox("Use E"));
            clearMenu.Add("e.minion.hit.count", new Slider("(E) Min. Minion", 3, 1, 5));
            clearMenu.Add("clear.mana", new Slider("Min. Mana", 50, 1, 99));

            jungleMenu = Config.AddSubMenu("Jungle Settings", "Jungle Settings");
            jungleMenu.Add("q.jungle", new CheckBox("Use Q"));
            jungleMenu.Add("w.jungle", new CheckBox("Use W"));
            jungleMenu.Add("e.jungle", new CheckBox("Use E"));
            jungleMenu.Add("jungle.mana", new Slider("Min. Mana", 50, 1, 99));

            drawMenu = Config.AddSubMenu("Draw Settings", "Draw Settings");
            drawMenu.Add("q.draw", new CheckBox("Q Range"));
            drawMenu.Add("w.draw", new CheckBox("W Range"));
            drawMenu.Add("e.draw", new CheckBox("E Range"));
            drawMenu.Add("r.draw", new CheckBox("R Range"));
        }
    }
}