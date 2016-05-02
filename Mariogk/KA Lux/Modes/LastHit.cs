using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

using Settings = KA_Lux.Config.Modes.LastHit;

namespace KA_Lux.Modes
{
    public sealed class LastHit : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit);
        }

        public override void Execute()
        {
            var minion =
                EntityManager.MinionsAndMonsters.GetLaneMinions()
                    .OrderByDescending(m => m.Health)
                    .FirstOrDefault(m => m.IsValidTarget(Q.Range));

            if (minion == null) return;

            if (Q.IsReady() && minion.IsValidTarget(Q.Range) && Settings.UseQ)
            {
                Q.Cast(minion);
            }

            var minions =
                EntityManager.MinionsAndMonsters.GetLaneMinions()
                    .Where(
                        m =>
                            m.Health <= SpellDamage.GetRealDamage(SpellSlot.E, m) && m.IsEnemy &&
                            m.IsValidTarget(E.Range))
                    .ToArray();
            if (minions.Length == 0) return;

            var farmLocation = Prediction.Position.PredictCircularMissileAoe(minions, E.Range, E.Width,
                E.CastDelay, E.Speed).OrderByDescending(r => r.GetCollisionObjects<Obj_AI_Minion>().Length).FirstOrDefault();

            if (farmLocation != null && Settings.UseE && Settings.LastMana <= Player.Instance.ManaPercent)
            {
                var predictedMinion = farmLocation.GetCollisionObjects<Obj_AI_Minion>();
                if (predictedMinion.Length >= Settings.ECount)
                {
                    E.Cast(farmLocation.CastPosition);
                    PermaActive.CastedE = true;
                }
            }
        }
    }
}
