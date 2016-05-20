using System;
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
    class AutoHarassModule : IModule
    {
        public void OnLoad()
        {
            Console.WriteLine("Auto Harass Module Loaded.");
        }

        public bool ShouldGetExecuted()
        {
            return getCheckBoxItem(Variables.moduleMenu, "ezreal.modules." + GetName().ToLowerInvariant());
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
            if (Variables.Spells[SpellSlot.Q].IsReady() && getCheckBoxItem(Variables.mixedMenu, "ezreal.mixed.q"))
            {
                var target = TargetSelector.GetTarget(Variables.Spells[SpellSlot.Q].Range, DamageType.Physical);
                if (target.IsValidTarget(Variables.Spells[SpellSlot.Q].Range))
                {
                    var prediction = Variables.Spells[SpellSlot.Q].GetSPrediction(target);
                    var castPosition = prediction.CastPosition.Extend((Vector2)ObjectManager.Player.Position, -140);
                    if (prediction.HitChance >= MenuGenerator.GetHitchance())
                    {
                        Variables.Spells[SpellSlot.Q].Cast(castPosition);
                    }
                }
            }

            if (Variables.Spells[SpellSlot.W].IsReady() && getCheckBoxItem(Variables.mixedMenu, "ezreal.mixed.w") && ObjectManager.Player.ManaPercent > 35)
            {
                var wTarget = TargetSelector.GetTarget(Variables.Spells[SpellSlot.W].Range, DamageType.Physical);
                if (wTarget.IsValidTarget(Variables.Spells[SpellSlot.W].Range)
                    && Variables.Spells[SpellSlot.W].GetSPrediction(wTarget).HitChance >= MenuGenerator.GetHitchance())
                {
                    Variables.Spells[SpellSlot.W].Cast(Variables.Spells[SpellSlot.W].GetSPrediction(wTarget).CastPosition);
                }
            }
        }

        public string GetName()
        {
            return "AutoHarass";
        }
    }
}
