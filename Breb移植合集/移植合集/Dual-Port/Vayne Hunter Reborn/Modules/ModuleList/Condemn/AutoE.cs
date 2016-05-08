using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp;
using LeagueSharp.Common;
using VayneHunter_Reborn.Modules.ModuleHelpers;
using VayneHunter_Reborn.Skills.Condemn;
using VayneHunter_Reborn.Utility;
using VayneHunter_Reborn.Utility.MenuUtility;

namespace VayneHunter_Reborn.Modules.ModuleList.Condemn
{
    class AutoE : IModule
    {
        public void OnLoad()
        {
            
        }

        public bool ShouldGetExecuted()
        {
            return MenuGenerator.miscMenu["dz191.vhr.misc.condemn.autoe"].Cast<CheckBox>().CurrentValue && Variables.spells[SpellSlot.E].IsReady();
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            var CondemnTarget = CondemnLogic.GetCondemnTarget(ObjectManager.Player.ServerPosition);
            if (CondemnTarget.LSIsValidTarget())
            {
                Variables.spells[SpellSlot.E].CastOnUnit(CondemnTarget);
            }
        }
    }
}
