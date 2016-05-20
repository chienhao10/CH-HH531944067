using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using EloBuddy;

namespace Auto_Carry_Vayne.Features.Module.Summoners
{
    class Healme : IModule
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
            return Manager.SpellManager.Heal != null && Manager.SpellManager.Heal.IsReady() && Manager.MenuManager.Heal && Variables._Player.HealthPercent <= Manager.MenuManager.HealHp;
        }

        public void OnExecute()
        {
            if (Variables._Player.CountEnemiesInRange(800) >= 1)
            {
                Manager.SpellManager.Heal.Cast();
            }
            if (Variables._Player.HasBuff("summonerdot"))
            {
                Manager.SpellManager.Heal.Cast();
            }
        }
    }
}
