using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using EloBuddy;

namespace Auto_Carry_Vayne.Features.Module.Condemn
{
    class AutoE : IModule
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
            return Manager.MenuManager.AutoE && Manager.MenuManager.UseE && Manager.SpellManager.E.IsReady();
        }

        public void OnExecute()
        {
            var ctarget = Logic.Condemn.GetTarget(Variables._Player.Position);
            if (ctarget != null)
            {
                Manager.SpellManager.E.Cast(ctarget);
            }
        }
    }
}
