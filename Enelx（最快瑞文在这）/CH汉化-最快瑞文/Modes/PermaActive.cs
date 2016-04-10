using System;
using EloBuddy;
using EloBuddy.SDK;
using Ex1L_Riven.Base;

namespace Ex1L_Riven.Modes
{
    internal class PermaActive
    {
        public static bool ShouldBeExecuted()
        {
            return true;
        }

        public static void Execute()
        {
            if (Variables.UseAutoW)
            {
                if (Player.Instance.CountEnemiesInRange(Spells.W.Range) >= Variables.MinEnemiesW && Spells.W.IsReady())
                {
                    Spells.W.Cast();
                }
            }

            // Killsteal
            var rtarget = TargetSelector.GetTarget(Spells.R2.Range, DamageType.Physical);
            var qtarget = TargetSelector.GetTarget(400, DamageType.Physical);
            var wtarget = TargetSelector.GetTarget(Spells.W.Range, DamageType.Physical);

            if (rtarget != null && !rtarget.IsZombie && !rtarget.IsInvulnerable)
            {
                if (Riven.R1Activated && rtarget.Health <= Spells.R2Damage(rtarget, rtarget.Health))
                {
                    if (Spells.R2.IsReady())
                    {
                        var prediction = Spells.R2.GetPrediction(rtarget);
                        Spells.R2.Cast(prediction.CastPosition);
                    }
                }
            }

            if (qtarget != null && !qtarget.IsZombie && !qtarget.IsInvulnerable && ModeController.Activemode(Orbwalker.ActiveModes.Combo))
            {
                if (Spells.Q.IsReady() && qtarget.Health <= Spells.QDamage(qtarget)*1.5)
                {
                    Spells.Q.Cast(qtarget.ServerPosition);
                }
            }

            if (wtarget != null && !wtarget.IsZombie && !wtarget.IsInvulnerable)
            {
                if (Spells.W.IsReady() && wtarget.Health <= Spells.WDamage(wtarget))
                {
                    Spells.W.Cast();
                }
            }
        }
    }
}