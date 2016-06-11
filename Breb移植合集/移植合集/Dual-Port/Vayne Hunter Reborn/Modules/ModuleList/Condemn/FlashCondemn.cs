using System;
using System.Linq;
using EloBuddy;
using LeagueSharp.Common;
using VayneHunter_Reborn.Modules.ModuleHelpers;
using VayneHunter_Reborn.Skills.Tumble.VHRQ;
using VayneHunter_Reborn.Utility;
using VayneHunter_Reborn.Utility.MenuUtility;
using EloBuddy.SDK.Menu.Values;

namespace VayneHunter_Reborn.Modules.ModuleList.Condemn
{
    class FlashCondemn : IModule
    {
        private static Spell E
        {
            get { return Variables.spells[SpellSlot.E]; }
        }

        private static Spell Flash
        {
            get { return new Spell(ObjectManager.Player.GetSpellSlot("SummonerFlash"), 425f); }
        }

        public void OnLoad()
        {
        }

        public bool ShouldGetExecuted()
        {
            return MenuGenerator.miscMenu["dz191.vhr.misc.condemn.flashcondemn"].Cast<KeyBind>().CurrentValue &&
                   Variables.spells[SpellSlot.E].IsReady() && Flash.Slot != SpellSlot.Unknown && Flash.IsReady();
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnAfterAA;
        }

        public void OnExecute()
        {
            var pushDistance = MenuGenerator.miscMenu["dz191.vhr.misc.condemn.pushdistance"].Cast<Slider>().CurrentValue - 25;


            foreach (var target in HeroManager.Enemies.Where(en => en.LSIsValidTarget(E.Range) && !en.LSIsDashing()))
            {
                var canFlashBehind = ObjectManager.Player.LSDistance(target) <
                                     Flash.Range - ObjectManager.Player.BoundingRadius;
                var flashPosition = ObjectManager.Player.ServerPosition.LSExtend(target.ServerPosition, Flash.Range);

                if (!canFlashBehind || !flashPosition.IsSafe())
                {
                    return;
                }

                var Prediction = Variables.spells[SpellSlot.E].GetPrediction(target);

                if (Prediction.Hitchance >= HitChance.VeryHigh)
                {
                    var endPosition = Prediction.UnitPosition.LSExtend(flashPosition, -pushDistance);
                    if (endPosition.LSIsWall())
                    {
                        Variables.LastCondemnFlashTime = Environment.TickCount;
                        E.CastOnUnit(target);
                        Flash.Cast(flashPosition);
                    }
                    else
                    {
                        //It's not a wall.
                        var step = pushDistance / 5f;
                        for (float i = 0; i < pushDistance; i += step)
                        {
                            var endPositionEx = Prediction.UnitPosition.LSExtend(flashPosition, -i);
                            if (endPositionEx.LSIsWall())
                            {
                                Variables.LastCondemnFlashTime = Environment.TickCount;
                                E.CastOnUnit(target);
                                Flash.Cast(flashPosition);
                                return;
                            }
                        }
                    }
                }

            }
        }
    }
}
