using System;
using System.Linq;
using DZLib.Modules;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using iKalistaReborn.Utils;
using LeagueSharp.Common;
using LeagueSharp.SDK;

namespace iKalistaReborn.Modules
{
    internal class JungleStealModule : IModule
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
            Console.WriteLine("Jungle Steal Module Loaded");
        }

        public string GetName()
        {
            return "JungleSteal";
        }

        public bool ShouldGetExecuted()
        {
            return SpellManager.Spell[SpellSlot.E].IsReady() &&
                   getCheckBoxItem(jungleStealMenu, "com.ikalista.jungleSteal.enabled");
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public void OnExecute()
        {

            var small =
                GameObjects.JungleSmall.Any(
                    x => SpellManager.Spell[SpellSlot.E].CanCast(x) && x.IsMobKillable() && x.IsValid);
            var large =
                GameObjects.JungleLarge.Any(
                    x => SpellManager.Spell[SpellSlot.E].CanCast(x) && x.IsMobKillable() && x.IsValid);
            var legendary =
                GameObjects.JungleLegendary.Any(
                    x => SpellManager.Spell[SpellSlot.E].CanCast(x) && x.IsMobKillable() && x.IsValid);

            if ((small && getCheckBoxItem(jungleStealMenu, "com.ikalista.jungleSteal.small"))
                || (large && getCheckBoxItem(jungleStealMenu, "com.ikalista.jungleSteal.large"))
                || (legendary && getCheckBoxItem(jungleStealMenu, "com.ikalista.jungleSteal.legendary")))
            {
                SpellManager.Spell[SpellSlot.E].Cast();
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