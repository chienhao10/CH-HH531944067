using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Settings = KA_Ezreal.Config.Modes.LastHit;

namespace KA_Ezreal.Modes
{
    public sealed class LastHit : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit);
        }

        public override void Execute()
        {
            var laneMinion =
                EntityManager.MinionsAndMonsters.GetLaneMinions()
                    .OrderByDescending(m => m.Health)
                    .FirstOrDefault(
                        m => m.IsValidTarget(Q.Range) && Prediction.Health.GetPrediction(m, Q.CastDelay) <= SpellDamage.GetRealDamage(SpellSlot.Q, m) &&
                        Prediction.Health.GetPrediction(m, Q.CastDelay) > Player.Instance.GetAutoAttackDamage(m));

            if (laneMinion == null) return;

            if (Settings.UseQ && Q.IsReady() && Settings.LastMana <= Player.Instance.ManaPercent && !Player.Instance.IsInAutoAttackRange(laneMinion))
            {
                Q.Cast(laneMinion);
            }
        }
    }
}
