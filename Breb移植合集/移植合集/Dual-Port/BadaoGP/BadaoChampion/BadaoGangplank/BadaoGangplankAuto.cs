using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;
using EloBuddy.SDK;

namespace BadaoKingdom.BadaoChampion.BadaoGangplank
{
    public static class BadaoGangplankAuto
    {
        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Environment.TickCount - BadaoGangplankCombo.LastCondition >= 100 + Game.Ping)
            {
                foreach (var hero in HeroManager.Enemies.Where(x => x.LSIsValidTarget() && x.LSDistance(ObjectManager.Player) < 2000 && x.IsHPBarRendered))
                {
                    var pred = LeagueSharp.Common.Prediction.GetPrediction(hero, 0.5f).UnitPosition;
                    if (BadaoMainVariables.Q.IsReady())
                    {
                        foreach (var barrel in BadaoGangplankBarrels.QableBarrels())
                        {
                            var nbarrels = BadaoGangplankBarrels.ChainedBarrels(barrel);
                            if (nbarrels.Any(x => x.Bottle.LSDistance(pred) <= 330 /*+ hero.BoundingRadius*/))
                            {
                                Orbwalker.DisableMovement = true;
                                Orbwalker.DisableAttacking = true;
                                LeagueSharp.Common.Utility.DelayAction.Add(100 + Game.Ping, () =>
                                {
                                    Orbwalker.DisableMovement = false;
                                    Orbwalker.DisableAttacking = false;
                                });
                                BadaoMainVariables.Q.Cast(barrel.Bottle);
                                if (BadaoMainVariables.Q.Cast(barrel.Bottle) == LeagueSharp.Common.Spell.CastStates.SuccessfullyCasted)
                                {
                                    BadaoGangplankCombo.LastCondition = Environment.TickCount;
                                    return;
                                }
                            }
                        }
                    }
                }

                foreach (var hero in HeroManager.Enemies.Where(x => x.LSIsValidTarget() && x.LSDistance(ObjectManager.Player) < 2000 && x.IsHPBarRendered))
                {
                    var pred = LeagueSharp.Common.Prediction.GetPrediction(hero, 0.5f).UnitPosition;
                    foreach (var barrel in BadaoGangplankBarrels.AttackableBarrels())
                    {
                        var nbarrels = BadaoGangplankBarrels.ChainedBarrels(barrel);
                        if (nbarrels.Any(x => x.Bottle.LSDistance(pred) <= 330 /*+ hero.BoundingRadius*/))
                        {
                            Console.WriteLine("1");
                            Orbwalker.DisableMovement = true;
                            Orbwalker.DisableAttacking = true;
                            Console.WriteLine("2");
                            LeagueSharp.Common.Utility.DelayAction.Add(300 + Game.Ping, () =>
                            {
                                Orbwalker.DisableMovement = false;
                                Orbwalker.DisableAttacking = false;
                            });
                            Console.WriteLine("3");
                            Orbwalker.ForcedTarget = barrel.Bottle;
                            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, barrel.Bottle);
                            Console.WriteLine("4");
                            if (EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, barrel.Bottle))
                            {
                                BadaoGangplankCombo.LastCondition = Environment.TickCount;
                                return;
                            }
                        }
                    }
                }
            }
            if (BadaoMainVariables.W.IsReady() &&
                BadaoGangplankVariables.AutoWLowHealth &&
                BadaoGangplankVariables.AutoWLowHealthValue >= Player.Health * 100 / Player.MaxHealth)
            {
                BadaoMainVariables.W.Cast();
            }
            if (BadaoMainVariables.W.IsReady()
                && BadaoGangplankVariables.AutoWCC)
            {
                foreach (var bufftype in new BuffType[] {BuffType.Stun, BuffType.Snare, BuffType.Suppression,
                BuffType.Silence,BuffType.Taunt,BuffType.Charm,BuffType.Blind,BuffType.Fear,BuffType.Polymorph})
                {
                    if (Player.HasBuffOfType(bufftype))
                        BadaoMainVariables.W.Cast();
                }
            }
            if (BadaoMainVariables.Q.IsReady())
            {
                foreach (var hero in HeroManager.Enemies.Where(x => x.BadaoIsValidTarget(BadaoMainVariables.Q.Range)
                && BadaoMainVariables.Q.GetDamage(x) >= x.Health))
                {
                    BadaoMainVariables.Q.Cast(hero);
                }
            }
            Orbwalker.ForcedTarget = null;
        }
    }
}
