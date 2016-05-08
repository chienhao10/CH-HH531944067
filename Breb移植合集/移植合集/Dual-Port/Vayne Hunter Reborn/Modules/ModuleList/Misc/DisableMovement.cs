using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using VayneHunter_Reborn.Modules.ModuleHelpers;
using VayneHunter_Reborn.Utility;
using VayneHunter_Reborn.Utility.MenuUtility;

namespace VayneHunter_Reborn.Modules.ModuleList.Misc
{
    class DisableMovement : IModule
    {
        public void OnLoad()
        {

        }

        public bool ShouldGetExecuted()
        {
            return true;
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            Orbwalker.DisableMovement = MenuGenerator.miscMenu["dz191.vhr.misc.general.disablemovement"].Cast<CheckBox>().CurrentValue;
            Orbwalker.DisableAttacking = MenuGenerator.miscMenu["dz191.vhr.misc.general.disableattk"].Cast<CheckBox>().CurrentValue;
        }
    }
}
