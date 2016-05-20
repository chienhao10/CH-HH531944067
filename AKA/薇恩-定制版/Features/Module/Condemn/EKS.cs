using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using EloBuddy;

namespace Auto_Carry_Vayne.Features.Module.Condemn
{
    class EKS : IModule
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
            return Manager.MenuManager.UseEKill && Manager.MenuManager.UseE && Manager.SpellManager.E.IsReady();
        }

        public void OnExecute()
        {
            var target = EntityManager.Heroes.Enemies.Find(en => en.IsValidTarget(Manager.SpellManager.E.Range) && en.GetBuffCount("vaynesilvereddebuff") == 2);
            if (target != null && !target.IsInvulnerable
                && target.Health + 60 <= (ObjectManager.Player.GetSpellDamage(target, SpellSlot.E) + ObjectManager.Player.GetSpellDamage(target, SpellSlot.W)))
            {
                Manager.SpellManager.E.Cast(target);
            }
        }
    }
}
