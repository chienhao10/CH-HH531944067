using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;

namespace Auto_Carry_Vayne.Features.Module.Items
{
    class Potion : IModule
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
            return Manager.MenuManager.AutoPotion && Manager.SpellManager.HPPot.IsReady() && Manager.SpellManager.HPPot.IsOwned();
        }

        public void OnExecute()
        {
            if (!Variables._Player.IsInShopRange() && !(Player.HasBuff("RegenerationPotion")) && Variables._Player.HealthPercent <= Manager.MenuManager.AutoPotionHp)
            {
                Manager.SpellManager.HPPot.Cast();
            }
        }
    }
}
