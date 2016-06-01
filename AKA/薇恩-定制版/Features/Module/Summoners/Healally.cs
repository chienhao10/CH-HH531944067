using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using EloBuddy;

namespace Auto_Carry_Vayne.Features.Module.Summoners
{
    class Healally : IModule
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
            return Manager.SpellManager.Heal != null && Manager.SpellManager.Heal.IsReady() && Manager.MenuManager.HealAlly;
        }

        public void OnExecute()
        {
            foreach (var ally in EntityManager.Heroes.Allies.Where(a => !a.IsDead && a.Position.Distance(Variables._Player) < Manager.SpellManager.Heal.Range && a.IsValid && a.HealthPercent <= Manager.MenuManager.HealAllyHp))
            {
                if (ally.CountEnemiesInRange(800) >= 1)
                {
                    Manager.SpellManager.Heal.Cast();
                }
                if (ally.HasBuff("summonerdot"))
                {
                    Manager.SpellManager.Heal.Cast();

                }
            }
        }
    }
}
