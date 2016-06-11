using System;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using VayneHunter_Reborn.External.Evade;
using VayneHunter_Reborn.Skills.Condemn.Methods;
using VayneHunter_Reborn.Utility;
using VayneHunter_Reborn.Utility.MenuUtility;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using System.Linq;

namespace VayneHunter_Reborn.Skills.Condemn
{
    class CondemnLogic
    {
        private static LeagueSharp.Common.Spell E
        {
            get { return Variables.spells[SpellSlot.E]; }
        }

        private static readonly LeagueSharp.Common.Spell TrinketSpell = new LeagueSharp.Common.Spell(SpellSlot.Trinket);

        public static void OnLoad()
        {
            Variables.spells[SpellSlot.E].SetTargetted(0.375f, float.MaxValue);
            InterrupterGapcloser.OnLoad();
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Obj_AI_Base.OnProcessSpellCast += WindWall.OnProcessSpellCast;
        }

        public static void Execute(EventArgs args)
        {

            if (!E.IsEnabledAndReady(Orbwalker.ActiveModesFlags.ToString()))
            {
                return;
            }

            /*
            var CondemnTarget = GetCondemnTarget(ObjectManager.Player.ServerPosition);
            if (CondemnTarget.LSIsValidTarget())
            {
               // var AAForE = MenuExtensions.GetItemValue<Slider>("dz191.vhr.misc.condemn.noeaa").Value;

               // if (CondemnTarget.Health / ObjectManager.Player.GetAutoAttackDamage(CondemnTarget, true) < AAForE)
               // {
               //     return;
               // }

                E.CastOnUnit(CondemnTarget);
                TrinketBush(CondemnTarget.ServerPosition.LSExtend(ObjectManager.Player.ServerPosition, -450f));
            }
            */

            var pushDistance = MenuGenerator.miscMenu["dz191.vhr.misc.condemn.pushdistance"].Cast<Slider>().CurrentValue - 25;

            foreach (var target in HeroManager.Enemies.Where(en => en.LSIsValidTarget(E.Range) && !en.LSIsDashing()))
            {
                var Prediction = Variables.spells[SpellSlot.E].GetPrediction(target);

                if (Prediction.Hitchance >= HitChance.VeryHigh)
                {
                    var endPosition = Prediction.UnitPosition.LSExtend(ObjectManager.Player.ServerPosition, -pushDistance);
                    if (endPosition.LSIsWall())
                    {
                        E.CastOnUnit(target);
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
                                E.CastOnUnit(target);
                                return;
                            }
                        }
                    }
                }
            }
        }

        private static void TrinketBush(Vector3 endPosition)
        {
            if (TrinketSpell.IsReady())
            {
                var extended = ObjectManager.Player.ServerPosition.LSExtend(endPosition, 400f);
                if (NavMesh.IsWallOfGrass(extended, 130f) && !NavMesh.IsWallOfGrass(ObjectManager.Player.ServerPosition, 65f))
                {
                    LeagueSharp.Common.Utility.DelayAction.Add((int)(Game.Ping / 2f + 250), () =>
                    {
                        TrinketSpell.Cast(extended);
                    });
                }
            }
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            return;
        }

        public static Obj_AI_Base GetCondemnTarget(Vector3 fromPosition)
        {
            switch (MenuGenerator.miscMenu["dz191.vhr.misc.condemn.condemnmethod"].Cast<ComboBox>().CurrentValue)
            {
                case 0:
                    //VH Revolution
                    return VHRevolution.GetTarget(fromPosition);
                case 1:
                    //VH Reborn
                    return VHReborn.GetTarget(fromPosition);
                case 2:
                    //Marksman / Gosu
                    return VayneHunter_Reborn.Skills.Condemn.Methods.Marksman.GetTarget(fromPosition);
                case 3:
                    //Shine#
                    return Shine.GetTarget(fromPosition);
            }
            return null;
        }
    }
}
