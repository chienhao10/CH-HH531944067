using System.Linq;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using Settings = KA_Lux.Config.Modes.JungleClear;

namespace KA_Lux.Modes
{
    public sealed class JungleClear : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear);
        }

        public override void Execute()
        {
            var jgminion =
                EntityManager.MinionsAndMonsters.GetJungleMonsters()
                    .OrderByDescending(j => j.Health)
                    .FirstOrDefault(j => j.IsValidTarget(Q.Range));

            if (jgminion == null)return;

            if (E.IsReady() && jgminion.IsValidTarget(E.Range) && Settings.UseE)
            {
                E.Cast(jgminion);
                PermaActive.CastedE = true;
            }

            if (Q.IsReady() && jgminion.IsValidTarget(Q.Range) && Settings.UseQ)
            {
                var pred = Q.GetPrediction(jgminion);
                if (pred.HitChance >= HitChance.High)
                {
                    Q.Cast(pred.CastPosition);
                }
            }
        }
    }
}
