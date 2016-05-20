using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;

namespace Auto_Carry_Vayne.Features.Module.Condemn
{
    class AntiFlash : IModule
    {
        public void OnLoad()
        {
            Obj_AI_Base.OnProcessSpellCast += OnCast;
        }

        private void OnCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender is AIHeroClient && sender.IsValidTarget())
            {
                if (ShouldGetExecuted() && args.SData.Name.ToLower().Equals("summonerflash") && args.End.Distance(ObjectManager.Player.ServerPosition) <= 365f)
                {
                    Manager.SpellManager.E.Cast(sender);
                }
            }
        }

        public bool ShouldGetExecuted()
        {
            return Manager.SpellManager.E.IsReady();
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.Other;
        }

        public void OnExecute()
        {

        }
    }
}
