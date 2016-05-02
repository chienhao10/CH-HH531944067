using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using Settings = KA_Lux.Config.Modes.Combo;

namespace KA_Lux.Modes
{
    public sealed class Combo : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
        }

        public override void Execute()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target == null || target.IsZombie) return;

            if (Q.IsReady() && target.IsValidTarget(Q.Range) && Settings.UseQ)
            {
                var pred = Q.GetPrediction(target);
                if (pred.HitChancePercent >= 80)
                {
                    Q.Cast(pred.CastPosition);
                }
            }

            if (E.IsReady() && target.IsValidTarget(E.Range) && Settings.UseE && Settings.UseESnared
                ? target.HasBuffOfType(BuffType.Snare)
                : target.IsValidTarget(E.Range))
            {
                var pred = E.GetPrediction(target);
                if (pred.HitChancePercent >= 70)
                {
                    E.Cast(pred.CastPosition);
                }
                PermaActive.CastedE = true;
            }

            if (R.IsReady() && Settings.UseR)
            {
                var targetR = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                if(targetR == null)return;

                if (target.IsValidTarget(R.Range) && Settings.UseRSnared
                    ? target.HasBuffOfType(BuffType.Snare)
                    : target.IsValidTarget(R.Range) && Prediction.Health.GetPrediction(targetR, R.CastDelay) > 10)
                {
                    if (targetR.HasBuffOfType(BuffType.Snare) || targetR.HasBuffOfType(BuffType.Stun))
                    {
                        R.Cast(targetR.Position);
                    }
                    else
                    {
                        var pred = R.GetPrediction(target);
                        if (pred.HitChancePercent >= 95)
                        {
                            R.Cast(pred.CastPosition);
                        }
                    }
                }
            }
        }
    }
}
