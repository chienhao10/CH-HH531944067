namespace Khappa_Zix.Modes
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    using Load;

    using Misc;

    internal class Harass : Load
    {
        public static void Start()
        {
            var target =
                EntityManager.Heroes.Enemies.FirstOrDefault(
                    x => x.IsValidTarget(W.Range) && !x.IsZombie && !x.IsInvulnerable && !x.HasBuffOfType(BuffType.PhysicalImmunity));

            if (target == null)
            {
                return;
            }

            if (W.IsReady() && target.IsValidTarget(W.Range) && menu.Harass["W"].Cast<CheckBox>().CurrentValue)
            {
                var pred = W.GetPrediction(target).HitChance >= Misc.hitchance;
                if (pred)
                {
                    W.Cast(W.GetPrediction(target).CastPosition);
                }
            }

            if (Q.IsReady() && target.IsValidTarget(Q.Range) && menu.Harass["Q"].Cast<CheckBox>().CurrentValue)
            {
                Q.Cast(target);
            }

            if (E.IsReady() && target.IsValidTarget(W.Range) && Q.IsReady() && !target.IsValidTarget(Q.Range - 50)
                && menu.Harass["E"].Cast<CheckBox>().CurrentValue)
            {
                var pred = E.GetPrediction(target).HitChance >= Misc.hitchance;

                if (E.GetPrediction(target).CastPosition.IsUnderTurret() && target.IsUnderEnemyturret()
                    && !menu.Harass["Edive"].Cast<CheckBox>().CurrentValue)
                {
                    return;
                }

                if (pred)
                {
                    E.Cast(E.GetPrediction(target).CastPosition);
                }
            }
        }
    }
}