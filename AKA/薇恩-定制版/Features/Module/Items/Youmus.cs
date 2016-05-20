using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using EloBuddy;

namespace Auto_Carry_Vayne.Features.Module.Items
{
    class Youmus : IModule
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
            return Manager.MenuManager.Youmus && Item.HasItem(3142) && Item.CanUseItem(3142);
        }

        public void OnExecute()
        {
            var unit = TargetSelector.GetTarget(Manager.MenuManager.YoumusSlider, DamageType.Magical);

            if (unit != null)
            {
                Item.UseItem(3142);
            }
        }
    }
}
