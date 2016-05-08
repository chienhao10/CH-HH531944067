using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using VayneHunter_Reborn.Modules.ModuleHelpers;
using VayneHunter_Reborn.Skills.Condemn;
using VayneHunter_Reborn.Utility;
using VayneHunter_Reborn.Utility.MenuUtility;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

namespace VayneHunter_Reborn.Modules.ModuleList.Condemn
{
    class AutoKindred : IModule
    {
        public void OnLoad()
        {

        }

        public bool ShouldGetExecuted()
        {
            return false && MenuGenerator.miscMenu["dz191.vhr.misc.condemn.repelkindred"].Cast<CheckBox>().CurrentValue &&
                   Variables.spells[SpellSlot.E].IsReady() && ObjectManager.Player.LSCountEnemiesInRange(1500f) == 1 
                   && ObjectManager.Player.GetAlliesInRange(1500f).Count(m => !m.IsMe) == 0;
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            var CondemnTarget =
                HeroManager.Enemies.FirstOrDefault(h => h.LSIsValidTarget(Variables.spells[SpellSlot.E].Range) && h.HasBuff("KindredRNoDeathBuff") &&
                        h.HealthPercent <= 10);
            if (CondemnTarget.LSIsValidTarget())
            {
                Variables.spells[SpellSlot.E].CastOnUnit(CondemnTarget);
            }
        }
    }
}
