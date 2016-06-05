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
    class EKS : IModule
    {
        public void OnLoad()
        {

        }

        public bool ShouldGetExecuted()
        {
            return MenuGenerator.miscMenu["dz191.vhr.misc.condemn.eks"].Cast<CheckBox>().CurrentValue &&
                   Variables.spells[SpellSlot.E].IsReady();
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            var target = HeroManager.Enemies.Find(en => en.LSIsValidTarget(Variables.spells[SpellSlot.E].Range) && en.Has2WStacks());
            if (target.LSIsValidTarget(Variables.spells[SpellSlot.E].Range) && target.Health + 45 <= (ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.E) + ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.W)))
            {
                Variables.spells[SpellSlot.E].CastOnUnit(target);
            }
        }
    }
}
