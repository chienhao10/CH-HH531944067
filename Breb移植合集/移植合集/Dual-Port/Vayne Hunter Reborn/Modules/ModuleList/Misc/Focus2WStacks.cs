using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp;
using LeagueSharp.Common;
using System.Linq;
using VayneHunter_Reborn.Modules.ModuleHelpers;
using VayneHunter_Reborn.Utility.Helpers;
using VayneHunter_Reborn.Utility.MenuUtility;

namespace VayneHunter_Reborn.Modules.ModuleList.Misc
{
    class Focus2WStacks : IModule
    {
        public void OnLoad()
        {

        }

        public bool ShouldGetExecuted()
        {
            return MenuGenerator.miscMenu["dz191.vhr.misc.general.specialfocus"].Cast<CheckBox>().CurrentValue && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            var target = HeroManager.Enemies.Find(en => en.IsValidTarget(ObjectManager.Player.AttackRange + 65f + 65f) && en.Has2WStacks());
            if (target != null)
            {
                Orbwalker.ForcedTarget = target;
            }

            if (Game.Time < 25 * 60 * 1000)
            {
                var ADC =
                    HeroManager.Enemies.Where(m => TargetSelector.GetPriority(m) > 4 && m.IsValidTarget()).OrderBy(m => m.TotalAttackDamage).FirstOrDefault();

                if (ADC != null && Orbwalking.InAutoAttackRange(ADC))
                {
                    Orbwalker.ForcedTarget = target;
                }
                else
                {
                    Orbwalker.ForcedTarget = Orbwalker.LastTarget as Obj_AI_Base;
                }
            }
        }
    }
}
