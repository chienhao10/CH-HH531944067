using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using EloBuddy;

namespace Auto_Carry_Vayne.Features.Module.Misc
{
    class FocusW : IModule
    {
        public void OnLoad()
        {

        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public bool ShouldGetExecuted()
        {
            return Manager.MenuManager.FocusW;
        }

        public void OnExecute()
        {
            if (FocusWTarget == null &&
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                return;
            }
            if (FocusWTarget.IsValidTarget(Variables._Player.GetAutoAttackRange()) &&
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                TargetSelector.GetPriority(FocusWTarget);
            }
            else
            {
                TargetSelector.GetPriority(
                    TargetSelector.GetTarget(Variables._Player.AttackRange, DamageType.Physical));
            }
        }

        private static AIHeroClient FocusWTarget
        {
            get
            {
                return ObjectManager.Get<AIHeroClient>()
                    .Where(
                        enemy =>
                            !enemy.IsDead &&
                            enemy.IsValidTarget((Manager.SpellManager.Q.IsReady() ? Manager.SpellManager.Q.Range : 0) + Variables._Player.AttackRange))
                    .FirstOrDefault(
                        enemy => enemy.Buffs.Any(buff => buff.Name == "vaynesilvereddebuff" && buff.Count > 0));
            }
        }
    }
}

