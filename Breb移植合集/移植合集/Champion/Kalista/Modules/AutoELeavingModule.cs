using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZLib.Modules;
using iKalistaReborn.Utils;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;

namespace iKalistaReborn.Modules
{
    class AutoELeavingModule : IModule
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
            Console.WriteLine("Leaving Module");
        }

        public string GetName()
        {
            return "AutoELeaving";
        }

        public bool ShouldGetExecuted()
        {
            return SpellManager.Spell[SpellSlot.E].IsReady() && getCheckBoxItem(comboMenu, "com.ikalista.combo.eLeaving");
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
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

        public void OnExecute()
        {
            var target =
                HeroManager.Enemies
                    .FirstOrDefault(x => x.HasRendBuff() && SpellManager.Spell[SpellSlot.E].IsInRange(x));
            if (target == null) return;
            var damage = Math.Ceiling(Helper.GetRendDamage(target)*100/target.Health);
            if (damage >= getSliderItem(comboMenu, "com.ikalista.combo.ePercent") && target.ServerPosition.LSDistance(ObjectManager.Player.ServerPosition, true) > Math.Pow(SpellManager.Spell[SpellSlot.E].Range * 0.8, 2))
            {
                SpellManager.Spell[SpellSlot.E].Cast();
            }
        }
    }
}
