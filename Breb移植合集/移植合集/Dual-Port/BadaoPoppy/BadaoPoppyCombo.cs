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

namespace BadaoKingdom.BadaoChampion.BadaoPoppy
{
    public static class BadaoPoppyCombo
    {
        public static void BadaoActiavate()
        {
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.OnPostAttack += Orbwalking_AfterAttack;
        }

        private static void Orbwalking_AfterAttack(AttackableUnit target, EventArgs args)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                return;
            if (target.Position.LSDistance(ObjectManager.Player.Position) <= 200 + 125 + 140)
                BadaoChecker.BadaoUseTiamat();
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                return;
            if (BadaoPoppyHelper.UseRComboKillable())
            {
                if (!BadaoMainVariables.R.IsCharging)
                {
                    var killableTarget = HeroManager.Enemies.FirstOrDefault(x => x.BadaoIsValidTarget(500)
                        && BadaoMainVariables.R.GetDamage(x) >= x.Health);
                    if (killableTarget != null)
                        BadaoMainVariables.R.StartCharging();
                }
                else
                {
                    var killableTarget = HeroManager.Enemies.FirstOrDefault(x => x.BadaoIsValidTarget(500)
                        && BadaoMainVariables.R.GetDamage(x) >= x.Health);
                    if (killableTarget != null)
                        BadaoMainVariables.R.Cast(killableTarget.Position);
                    else
                    {
                        var target = TargetSelector.GetTarget(BadaoMainVariables.R.Range, DamageType.Physical);
                        if (target.BadaoIsValidTarget())
                            BadaoMainVariables.R.Cast(target);
                    }
                }
            }
            if (BadaoMainVariables.E.IsReady() && Environment.TickCount - BadaoPoppyVariables.QCastTick >= 1250)
            {
                foreach (var hero in HeroManager.Enemies
                    .Where(
                        x => x.BadaoIsValidTarget(BadaoMainVariables.E.Range)
                            && BadaoPoppyHelper.UseECombo(x)))
                {
                    var predPos = hero.Position.LSTo2D(); //BadaoMainVariables.E.GetPrediction(hero).UnitPosition.To2D();
                    if (BadaoMath.GetFirstWallPoint(predPos, predPos.Extend(ObjectManager.Player.Position.To2D(), -300 - hero.BoundingRadius)) != null)
                    {
                        BadaoMainVariables.E.Cast(hero);
                        goto nextStep;
                    }
                }
            }
            if (BadaoPoppyHelper.UseEComboGap() && Environment.TickCount - BadaoPoppyVariables.QCastTick >= 1250)
            {
                var target = TargetSelector.GetTarget(BadaoMainVariables.E.Range, DamageType.Physical);
                if (target.BadaoIsValidTarget() && !Orbwalking.InAutoAttackRange(target)
                    && LeagueSharp.Common.Prediction.GetPrediction(target,0.5f).UnitPosition.LSDistance(ObjectManager.Player.Position)
                    > target.Distance(ObjectManager.Player.Position) + 20)
                {
                    BadaoMainVariables.E.Cast(target);
                }
            }

            nextStep:
            if (BadaoPoppyHelper.UseQCombo())
            {
                var target = TargetSelector.GetTarget(BadaoMainVariables.Q.Range, DamageType.Physical);
                if (target.BadaoIsValidTarget())
                {
                    if (BadaoMainVariables.Q.Cast(target) == LeagueSharp.Common.Spell.CastStates.SuccessfullyCasted)
                        BadaoPoppyVariables.QCastTick = Environment.TickCount;
                }
            }
            if (BadaoPoppyHelper.UseWCombo())
            {
                var target = HeroManager.Enemies.OrderBy(i => i.LSDistance(ObjectManager.Player.Position))
                    .FirstOrDefault(x => x.BadaoIsValidTarget(600));
                if (target != null)
                    BadaoMainVariables.W.Cast();
            }

        }
    }
}
