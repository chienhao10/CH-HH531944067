using System;
using System.Linq;
using DZLib.Modules;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using iKalistaReborn.Utils;
using LeagueSharp.Common;

namespace iKalistaReborn.Modules
{
    internal class AutoEModule : IModule
    {
        public static Menu
            comboMenu = Kalista.comboMenu,
            mixedMenu = Kalista.mixedMenu,
            laneclearMenu = Kalista.laneclearMenu,
            jungleStealMenu = Kalista.jungleStealMenu,
            miscMenu = Kalista.miscMenu,
            drawingMenu = Kalista.drawingMenu;

        public void OnLoad()
        {
            Console.WriteLine("Auto E Module Loaded");
        }

        public string GetName()
        {
            return "AutoEHarass";
        }

        public bool ShouldGetExecuted()
        {
            return SpellManager.Spell[SpellSlot.E].IsReady() && getCheckBoxItem(comboMenu, "com.ikalista.combo.autoE");
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                var enemy = HeroManager.Enemies.Where(hero => hero.HasRendBuff() && hero.LSIsValidTarget()).MinOrDefault(hero => hero.LSDistance(ObjectManager.Player, true));
                if (enemy == null)
                {
                    return;
                }
                if (enemy.LSDistance(ObjectManager.Player, true) < Math.Pow(SpellManager.Spell[SpellSlot.E].Range + 200, 2))
                {
                    if (ObjectManager.Get<Obj_AI_Minion>().Any(x => SpellManager.Spell[SpellSlot.E].IsInRange(x) && x.HasRendBuff() && x.IsRendKillable()))
                    {
                        SpellManager.Spell[SpellSlot.E].Cast();
                    }
                }
            }
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }
    }
}