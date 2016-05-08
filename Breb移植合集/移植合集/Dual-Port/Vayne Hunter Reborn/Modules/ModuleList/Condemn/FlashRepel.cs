using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp;
using LeagueSharp.Common;
using VayneHunter_Reborn.Modules.ModuleHelpers;
using VayneHunter_Reborn.Utility;
using VayneHunter_Reborn.Utility.Helpers;
using VayneHunter_Reborn.Utility.MenuUtility;

namespace VayneHunter_Reborn.Modules.ModuleList.Condemn
{
    class FlashRepel : IModule
    {
        public void OnLoad()
        {
            Obj_AI_Base.OnProcessSpellCast += OnCast;
        }

        private void OnCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender is AIHeroClient && sender.LSIsValidTarget())
            {
                if (args.SData.Name.ToLower().Equals("summonerflash") && args.End.LSDistance(ObjectManager.Player.ServerPosition) <= 365f)
                {
                    Variables.spells[SpellSlot.E].CastOnUnit((AIHeroClient)sender);
                }
            }
        }

        public bool ShouldGetExecuted()
        {
            return MenuGenerator.miscMenu["dz191.vhr.misc.condemn.repelflash"].Cast<CheckBox>().CurrentValue &&
                   Variables.spells[SpellSlot.E].IsReady();
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
