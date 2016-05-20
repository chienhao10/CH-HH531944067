using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using EloBuddy;
using EloBuddy.SDK.Events;

namespace Auto_Carry_Vayne.Features.Module.Condemn
{
    class EInterrupt : IModule
    {
        public void OnLoad()
        {
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
        }

        public void Interrupter_OnInterruptableSpell(Obj_AI_Base sender,
    Interrupter.InterruptableSpellEventArgs e)
        {
            if (ShouldGetExecuted() && Extensions.IsValidTarget(e.Sender) && e.DangerLevel == EloBuddy.SDK.Enumerations.DangerLevel.High)
            {
                Manager.SpellManager.E.Cast(e.Sender);
            }
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public bool ShouldGetExecuted()
        {
            return Manager.MenuManager.InterruptE && Manager.MenuManager.UseE && Manager.SpellManager.E.IsReady();
        }

        public void OnExecute() { }
    }
}
