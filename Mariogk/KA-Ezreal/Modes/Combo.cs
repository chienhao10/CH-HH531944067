using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using Settings = KA_Ezreal.Config.Modes.Combo;
using Misc = KA_Ezreal.Config.Modes.Misc;

namespace KA_Ezreal.Modes
{
    public sealed class Combo : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
        }

        public override void Execute()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (target != null && !target.IsZombie && !target.HasUndyingBuff())
            {
                //Q To AA Cancel
                if (Settings.UseQ && Q.IsReady() && target.IsValidTarget(Q.Range) &&
                    !target.IsInRange(Player.Instance, Player.Instance.GetAutoAttackRange()))
                {
                    var pred = Q.GetPrediction(target);
                    if (pred.HitChancePercent >= 75)
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }

                //W To AA Cancel
                if (Settings.UseW && W.IsReady() && target.IsValidTarget(W.Range) &&
                    !target.IsInRange(Player.Instance, Player.Instance.GetAutoAttackRange()))
                {
                    var pred = W.GetPrediction(target);
                    if (pred.HitChancePercent >= 90)
                    {
                        W.Cast(pred.CastPosition);
                    }
                }

                //Normal Q and W
                if (Settings.UseQ && EventsManager.CanQCancel && target.IsValidTarget(Q.Range) &&
                    target.IsInRange(Player.Instance, Player.Instance.GetAutoAttackRange()))
                {
                    var pred = Q.GetPrediction(target);
                    if (pred.HitChancePercent >= 75)
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }

                if (Settings.UseW && EventsManager.CanWCancel && target.IsValidTarget(W.Range) &&
                    target.IsInRange(Player.Instance, Player.Instance.GetAutoAttackRange()))
                {
                    var pred = W.GetPrediction(target);
                    if (pred.HitChancePercent >= 90)
                    {
                        W.Cast(pred.CastPosition);
                    }
                }
            }

            var targetR = TargetSelector.GetTarget(Misc.maxR, DamageType.Physical);
            if (targetR == null || targetR.IsZombie || targetR.HasUndyingBuff()) return;

            //Test R
            if (Settings.UseR && R.IsReady() && target.IsValidTarget(R.Range))
            {
                var pred = R.GetPrediction(target);
                if (pred.CastPosition.CountEnemiesInRange(R.Width) > Settings.MinR && pred.HitChancePercent >= 90)
                {
                    R.Cast(pred.CastPosition);
                }
            }
        }
    }
}
