using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp;
using LeagueSharp.Common;
using VayneHunter_Reborn.Modules.ModuleHelpers;
using VayneHunter_Reborn.Utility;
using VayneHunter_Reborn.Utility.MenuUtility;

namespace VayneHunter_Reborn.Modules.ModuleList.Condemn
{
    class EAfterAA : IModule
    {
        public void OnLoad()
        {
        }

        public bool ShouldGetExecuted()
        {
            return MenuGenerator.miscMenu["dz191.vhr.misc.condemn.enextauto"].Cast<KeyBind>().CurrentValue && Variables.spells[SpellSlot.E].IsReady();
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnAfterAA;
        }

        public void OnExecute()
        {
            var target = Orbwalker.LastTarget;
            if (target.IsValidTarget(Variables.spells[SpellSlot.E].Range) && (target is AIHeroClient))
            {
                var menuKey = MenuGenerator.miscMenu["dz191.vhr.misc.condemn.enextauto"].Cast<KeyBind>().CurrentValue;
                //Variables.spells[SpellSlot.E].CastOnUnit(target as AIHeroClient);
                MenuGenerator.miscMenu["dz191.vhr.misc.condemn.enextauto"].Cast<KeyBind>().CurrentValue = false;
            }
        }
    }
}
