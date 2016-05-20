using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using EloBuddy;

namespace Auto_Carry_Vayne.Features.Module.Misc
{
    class AutoBuyTrinkets : IModule
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
            return Manager.MenuManager.AutobuyTrinkets && Variables._Player.Level >= 9;
        }

        public void OnExecute()
        {
            if (Game.MapId == GameMapId.SummonersRift)
            {
                if (Variables._Player.IsInShopRange() && Item.HasItem((int)ItemId.Warding_Totem_Trinket))
                {
                    Shop.BuyItem(ItemId.Farsight_Alteration);
                }
            }
        }
    }
}
