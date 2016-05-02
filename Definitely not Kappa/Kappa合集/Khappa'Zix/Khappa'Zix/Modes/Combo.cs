namespace Khappa_Zix.Modes
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    using Load;

    using Misc;

    internal class Combo : Load
    {
        public static bool Jumping;

        public static readonly bool doubleJump = menu.Jump["double"].Cast<CheckBox>().CurrentValue;

        public static readonly int Dis = menu.Combo["dis"].Cast<Slider>().CurrentValue;

        internal static void Start()
        {
            var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);

            if (target != null && !target.IsZombie && !target.IsInvulnerable && !target.HasBuffOfType(BuffType.PhysicalImmunity))
            {
                var Distance = player.Distance(target);

                if (doubleJump && target.IsValidTarget(Q.Range)
                    && (GetQDamage(target) >= target.Health || player.GetSpellDamage(target, SpellSlot.W) >= target.Health))
                {
                    return;
                }

                if (W.IsReady() && target.IsValidTarget(W.Range) && menu.Combo["W"].Cast<CheckBox>().CurrentValue)
                {
                    var pred = W.GetPrediction(target).HitChance >= Misc.hitchance;
                    if (pred)
                    {
                        W.Cast(W.GetPrediction(target).CastPosition);
                    }
                }

                if (Q.IsReady() && target.IsValidTarget(Q.Range) && menu.Combo["Q"].Cast<CheckBox>().CurrentValue)
                {
                    Q.Cast(target);
                }

                if (E.IsReady() && target.IsValidTarget(E.Range) && Q.IsReady() && !target.IsValidTarget(Q.Range - 50)
                    && menu.Combo["E"].Cast<CheckBox>().CurrentValue)
                {
                    var pred = E.GetPrediction(target).HitChance >= Misc.hitchance;

                    if ((E.GetPrediction(target).CastPosition.IsUnderTurret() && target.IsUnderEnemyturret()
                         && !menu.Combo["Edive"].Cast<CheckBox>().CurrentValue)
                        || target.CountEnemiesInRange(600) >= menu.Combo["safe"].Cast<Slider>().CurrentValue)
                    {
                        return;
                    }

                    if (pred && player.Distance(target) > Dis)
                    {
                        E.Cast(E.GetPrediction(target).CastPosition);
                    }
                }

                if (!menu.Combo["useR"].Cast<CheckBox>().CurrentValue
                    || (menu.Combo["R"].Cast<CheckBox>().CurrentValue || !R.IsReady() || Q.IsReady() || W.IsReady() || E.IsReady()))
                {
                    return;
                }

                if (menu.Combo["Rmode"].Cast<ComboBox>().CurrentValue == 0 && R.IsReady())
                {
                    if ((Q.IsReady() || W.IsReady() || E.IsReady()) && player.ManaPercent >= 15)
                    {
                        if (Distance <= E.Range + Q.Range - 25 + (player.MoveSpeed * 0.7) && Distance > Q.Range && E.IsReady())
                        {
                            R.Cast();
                        }
                    }
                }

                if (menu.Combo["Rmode"].Cast<ComboBox>().CurrentValue == 1 && R.IsReady() && target.IsValidTarget(Q.Range + 25))
                {
                    R.Cast();
                }

                if (menu.Combo["danger"].Cast<Slider>().CurrentValue >= player.CountEnemiesInRange(550) && R.IsReady())
                {
                    R.Cast();
                }
            }
        }
    }
}