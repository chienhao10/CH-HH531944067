using System;
using System.Linq;
using DZLib.Modules;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using iKalistaReborn.Utils;
using LeagueSharp.Common;
using EloBuddy.SDK;

namespace iKalistaReborn.Modules
{
    internal class AutoRendModule : IModule
    {
        public static Menu
            comboMenu = Kalista.comboMenu,
            mixedMenu = Kalista.mixedMenu,
            laneclearMenu = Kalista.laneclearMenu,
            jungleStealMenu = Kalista.jungleStealMenu,
            miscMenu = Kalista.miscMenu,
            drawingMenu = Kalista.drawingMenu;

        public string GetName()
        {
            return "AutoRend";
        }

        public bool ShouldGetExecuted()
        {
            return SpellManager.Spell[SpellSlot.E].IsReady() &&
                   getCheckBoxItem(comboMenu, "com.ikalista.combo.useE");
        }

        public void OnExecute()
        {
            foreach (var source in EntityManager.Heroes.Enemies.Where(x => x.LSIsValidTarget() && x.HasRendBuff() && SpellManager.Spell[SpellSlot.E].IsInRange(x)))
            {
                if (source.IsRendKillable() || source.GetRendBuffCount() >= getSliderItem(comboMenu, "com.ikalista.combo.stacks"))
                {
                    SpellManager.Spell[SpellSlot.E].Cast();
                }
            }
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public void OnLoad()
        {
            Console.WriteLine("Auto Rend Module Loaded");
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