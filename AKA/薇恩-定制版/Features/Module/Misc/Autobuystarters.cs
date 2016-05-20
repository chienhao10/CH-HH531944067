using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using EloBuddy;

namespace Auto_Carry_Vayne.Features.Module.Misc
{
    class AutoBuyStarters : IModule
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
            return Manager.MenuManager.AutobuyStarters && Variables._Player.Level == 1;
        }

        public void OnExecute()
        {
            if (Variables.bought || Variables.ticks / Game.TicksPerSecond < 3)
            {
                Variables.ticks++;
                return;
            }

            Variables.bought = true;
            if (Game.MapId == GameMapId.SummonersRift)
            {
                Shop.BuyItem(ItemId.Dorans_Blade);
                Shop.BuyItem(ItemId.Health_Potion);
                Shop.BuyItem(ItemId.Warding_Totem_Trinket);

            }
        }
    }
}
