using EloBuddy;
using EloBuddy.SDK;
using Settings = KA_Lux.Config.Modes.Harass;

namespace KA_Lux.Modes
{
    public sealed class Harass : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);
        }

        public override void Execute()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (target == null || target.IsZombie) return;

            if (Q.IsReady() && target.IsValidTarget(Q.Range) && Settings.UseQ)
            {
                var pred = Q.GetPrediction(target);
                if (pred.HitChancePercent >= 75)
                {
                    Q.Cast(pred.CastPosition);
                }
            }

            if (E.IsReady() && target.IsValidTarget(E.Range) && Settings.UseE)
            {
                E.Cast(E.GetPrediction(target).CastPosition);
                PermaActive.CastedE = true;
            }
        }
    }
}
