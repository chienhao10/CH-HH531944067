using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp;
using LeagueSharp.Common;
using System.Linq;
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

            var pushDistance = MenuGenerator.miscMenu["dz191.vhr.misc.condemn.pushdistance"].Cast<Slider>().CurrentValue - 25;

            foreach (var target in HeroManager.Enemies.Where(en => en.LSIsValidTarget(Variables.spells[SpellSlot.E].Range)))
            {
                var Prediction = Variables.spells[SpellSlot.E].GetPrediction(target);

                if (Prediction.Hitchance >= HitChance.VeryHigh)
                {
                    var endPosition = Prediction.UnitPosition.LSExtend(ObjectManager.Player.ServerPosition, -pushDistance);
                    if (endPosition.LSIsWall())
                    {
                        Variables.spells[SpellSlot.E].CastOnUnit(target);
                    }
                    else
                    {
                        //It's not a wall.
                        var step = pushDistance / 5f;
                        for (float i = 0; i < pushDistance; i += step)
                        {
                            var endPositionEx = Prediction.UnitPosition.LSExtend(ObjectManager.Player.ServerPosition, -i);
                            if (endPositionEx.LSIsWall())
                            {
                                Variables.spells[SpellSlot.E].CastOnUnit(target);
                                return;
                            }
                        }
                    }
                }
            }

            /*
            var CondemnTarget = CondemnLogic.GetCondemnTarget(ObjectManager.Player.ServerPosition);
            if (CondemnTarget.LSIsValidTarget())
            {
                Variables.spells[SpellSlot.E].CastOnUnit(CondemnTarget);
            }
            */
        }
    }
}
