namespace Khappa_Zix.Modes
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    using Load;

    internal class Clear : Load
    {
        public static void LaneClear()
        {
            var minion =
                EntityManager.MinionsAndMonsters.EnemyMinions.Where(x => x.IsValidTarget(W.Range))
                    .OrderByDescending(x => x.Health)
                    .LastOrDefault(x => x != null);
            if (minion == null)
            {
                return;
            }

            if (Q.IsReady() && minion.IsValidTarget(Q.Range) && menu.Clear["Qc"].Cast<CheckBox>().CurrentValue)
            {
                if (player.GetSpellDamage(minion, SpellSlot.Q) > minion.Health && minion.IsValidTarget(Q.Range))
                {
                    Q.Cast(minion);
                }
                Q.Cast(minion);
            }

            if (W.IsReady() && menu.Clear["Wc"].Cast<CheckBox>().CurrentValue)
            {
                if (player.GetSpellDamage(minion, SpellSlot.W) > minion.Health)
                {
                    W.Cast(W.GetPrediction(minion).CastPosition);
                }
                W.Cast(W.GetPrediction(minion).CastPosition);
            }

            if (E.IsReady() && minion.IsValidTarget(E.Range) && menu.Clear["Ec"].Cast<CheckBox>().CurrentValue)
            {
                if (player.GetSpellDamage(minion, SpellSlot.E) > minion.Health && minion.IsValidTarget(E.Range))
                {
                    E.Cast(E.GetPrediction(minion).CastPosition);
                }

                E.Cast(E.GetPrediction(minion).CastPosition);
            }
        }

        public static void LastHit()
        {
            var minion =
                EntityManager.MinionsAndMonsters.EnemyMinions.Where(x => x.IsValidTarget(W.Range))
                    .OrderByDescending(x => x.Health)
                    .LastOrDefault(x => x != null);
            if (minion == null)
            {
                return;
            }

            if (Q.IsReady() && player.GetSpellDamage(minion, SpellSlot.Q) > minion.Health
                && menu.Clear["Qh"].Cast<CheckBox>().CurrentValue && minion.IsValidTarget(Q.Range))
            {
                Q.Cast(minion);
            }

            if (W.IsReady() && player.GetSpellDamage(minion, SpellSlot.W) > minion.Health
                && menu.Clear["Wh"].Cast<CheckBox>().CurrentValue)
            {
                W.Cast(W.GetPrediction(minion).CastPosition);
            }

            if (E.IsReady() && player.GetSpellDamage(minion, SpellSlot.E) > minion.Health
                && menu.Clear["Eh"].Cast<CheckBox>().CurrentValue && minion.IsValidTarget(E.Range))
            {
                E.Cast(E.GetPrediction(minion).CastPosition);
            }
        }

        public static void JungleClear()
        {
            var minion =
                EntityManager.MinionsAndMonsters.GetJungleMonsters()
                    .Where(x => x.IsValidTarget(W.Range))
                    .OrderByDescending(x => x.MaxHealth)
                    .FirstOrDefault(x => x != null);
            if (minion == null)
            {
                return;
            }

            if (Q.IsReady() && minion.IsValidTarget(Q.Range) && menu.Clear["Qj"].Cast<CheckBox>().CurrentValue)
            {
                Q.Cast(minion);
            }

            if (W.IsReady() && menu.Clear["Wj"].Cast<CheckBox>().CurrentValue)
            {
                W.Cast(W.GetPrediction(minion).CastPosition);
            }

            if (E.IsReady() && minion.IsValidTarget(E.Range) && menu.Clear["Ej"].Cast<CheckBox>().CurrentValue)
            {
                E.Cast(E.GetPrediction(minion).CastPosition);
            }
        }
    }
}