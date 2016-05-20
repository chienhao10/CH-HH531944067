using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using EloBuddy;

namespace Auto_Carry_Vayne.Features.Module.Summoners
{
    class Barrier : IModule
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
            return Manager.SpellManager.Barrier != null && Manager.SpellManager.Barrier.IsReady() && Manager.MenuManager.Barrier && Variables._Player.HealthPercent <= Manager.MenuManager.BarrierHp;
        }

        public void OnExecute()
        {
            if (Variables._Player.CountEnemiesInRange(800) >= 1)
            {
                Manager.SpellManager.Barrier.Cast();
            }
            if (Variables._Player.HasBuff("summonerdot"))
            {
                Manager.SpellManager.Barrier.Cast();
            }
        }
    }
}
