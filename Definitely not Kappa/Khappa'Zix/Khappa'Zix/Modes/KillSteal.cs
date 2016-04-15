namespace Khappa_Zix.Modes
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    using Load;

    using Misc;

    internal class KillSteal : Load
    {
        public static void Steal()
        {
            var target =
                EntityManager.Heroes.Enemies.FirstOrDefault(
                    x =>
                    x.IsValidTarget(W.Range) && !x.IsZombie && !x.IsInvulnerable
                    && !x.HasBuffOfType(BuffType.PhysicalImmunity));

            if (target == null)
            {
                return;
            }

            if (GetQDamage(target) >= target.TotalShieldHealth() && menu.KillSteal["Q"].Cast<CheckBox>().CurrentValue)
            {
                if (target.IsValidTarget(Q.Range) && Q.IsReady())
                {
                    Q.Cast(target);
                }
            }

            if (player.GetSpellDamage(target, SpellSlot.W) >= target.TotalShieldHealth() && W.IsReady()
                && menu.KillSteal["W"].Cast<CheckBox>().CurrentValue)
            {
                var pred = W.GetPrediction(target).HitChance >= Misc.hitchance;
                if (pred)
                {
                    W.Cast(W.GetPrediction(target).CastPosition);
                }
            }

            if (player.GetSpellDamage(target, SpellSlot.E) >= target.TotalShieldHealth() && E.IsReady()
                && menu.KillSteal["E"].Cast<CheckBox>().CurrentValue)
            {
                var pred = E.GetPrediction(target).HitChance >= Misc.hitchance;
                if (pred)
                {
                    E.Cast(E.GetPrediction(target).CastPosition);
                }
            }

            if (GetQDamage(target) + player.GetSpellDamage(target, SpellSlot.E) - 25 >= target.TotalShieldHealth()
                && menu.KillSteal["Q"].Cast<CheckBox>().CurrentValue
                && menu.KillSteal["E"].Cast<CheckBox>().CurrentValue)
            {
                if (Q.IsReady() && E.IsReady())
                {
                    var pred = E.GetPrediction(target).HitChance >= Misc.hitchance;
                    if (pred)
                    {
                        E.Cast(E.GetPrediction(target).CastPosition);
                    }
                }
            }
        }
    }
}