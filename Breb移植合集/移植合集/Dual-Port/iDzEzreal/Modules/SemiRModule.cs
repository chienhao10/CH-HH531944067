using System;
using System.Linq;
using DZLib.Modules;
using iDZEzreal.MenuHelper;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SPrediction;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
namespace iDZEzreal.Modules
{
    class SemiRModule : IModule
    {
        public void OnLoad()
        {
            Console.WriteLine("Manual R Module Loaded.");
        }

        public bool ShouldGetExecuted()
        {
            return getKeyBindItem(Variables.miscMenu, "ezreal.misc.semimanualr");
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
            if (Variables.Spells[SpellSlot.R].IsReady())
            {
                var target = TargetSelector.GetTarget(2300f, DamageType.Physical);

                if (target.IsValidTarget(Variables.Spells[SpellSlot.R].Range)
                    && Ezreal.CanExecuteTarget(target)
                    && ObjectManager.Player.LSDistance(target) >= Orbwalking.GetRealAutoAttackRange(null) * 0.80f
                    &&
                    !(target.Health + 5 <
                      ObjectManager.Player.GetAutoAttackDamage(target) * 2 +
                      Variables.Spells[SpellSlot.Q].GetDamage(target))
                    && HeroManager.Enemies.Count(m => m.LSDistance(target.ServerPosition) < 200f) >= getSliderItem(Variables.comboMenu, "ezreal.combo.r.min"))
                {
                    Variables.Spells[SpellSlot.R].SPredictionCast(
                        target, target.IsMoving ? HitChance.VeryHigh : HitChance.High);
                }
            }
        }

        public string GetName()
        {
            return "SemiManualR";
        }
    }
}
