using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZLib.Modules;
using iDZEzreal.MenuHelper;
using LeagueSharp;
using LeagueSharp.Common;
using SPrediction;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace iDZEzreal.Modules
{
    public class QKSModule : IModule
    {
        public void OnLoad()
        {
            Console.WriteLine("QKS Module Loaded");
        }

        public bool ShouldGetExecuted()
        {
            return Variables.Spells[SpellSlot.Q].IsReady() && getCheckBoxItem(Variables.moduleMenu, "ezreal.modules." + GetName().ToLowerInvariant());
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

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            foreach (
                var enemy in HeroManager.Enemies.Where(m => m.Health + 5 <= Variables.Spells[SpellSlot.Q].GetDamage(m) && m.LSIsValidTarget(Variables.Spells[SpellSlot.Q].Range)))
            {
                var sPrediction = Variables.Spells[SpellSlot.Q].GetSPrediction(enemy);
                if (sPrediction.HitChance >= HitChance.High)
                {
                    Variables.Spells[SpellSlot.Q].Cast(sPrediction.CastPosition);
                }
            }
        }

        public string GetName()
        {
            return "QKS";
        }
    }
}