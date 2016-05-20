using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using EloBuddy;

namespace Auto_Carry_Vayne.Features.Module.Condemn
{
    class LowLifeE : IModule
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
            return Manager.MenuManager.LowLifeE && Manager.SpellManager.E.IsReady() && !Manager.SpellManager.Q.IsReady() && Variables._Player.HealthPercent <= Manager.MenuManager.LowLifeESlider;
        }

        public void OnExecute()
        {
            var meleeEnemies = EntityManager.Heroes.Enemies.FindAll(m => m.IsMelee && m.Position.Distance(Variables._Player) <= 400);

            if (meleeEnemies.Any())
            {
                var mostDangerous =
                    meleeEnemies.OrderByDescending(m => m.GetAutoAttackDamage(Variables._Player)).First();
                Manager.SpellManager.E.Cast(mostDangerous);
            }
        }
    }
}
