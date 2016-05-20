using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;

namespace Auto_Carry_Vayne.Features.Module.Items
{
    class Biscuit : IModule
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
            return Manager.MenuManager.AutoBiscuit && Manager.SpellManager.Biscuit.IsReady() && Manager.SpellManager.Biscuit.IsOwned();
        }

        public void OnExecute()
        {
            if (!Variables._Player.IsInShopRange() && !(Player.HasBuff("RegenerationPotion")) && Variables._Player.HealthPercent <= Manager.MenuManager.AutoBiscuitHp)
            {
                Manager.SpellManager.Biscuit.Cast();
            }
        }
    }
}
