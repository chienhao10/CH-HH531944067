using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;

using Settings = RoamQueenQuinn.Config.Modes.Harass;

namespace RoamQueenQuinn.Modes
{
    public sealed class Harass : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);
        }

        public override void Execute()
        {
            if (Settings.Mana >= Player.Instance.ManaPercent)
                return;

            if (Settings.UseE && E.IsReady() && Orbwalker.ForcedTarget == null)
            {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                if (target != null)
                {
                    E.Cast(target);
                    Orbwalker.ForcedTarget = target;
                    return;
                }
            }
            if (Settings.UseQ && Q.IsReady() && Orbwalker.ForcedTarget == null)
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (target != null && Q.GetPrediction(target).HitChance >= HitChance.Medium)
                {
                    Q.Cast(target);
                    bool newtarget = false;
                    foreach (var e in EntityManager.Heroes.Enemies.Where(t => !t.IsDead && t.IsTargetable && !t.IsZombie && !t.IsInvulnerable && Player.Instance.IsInRange(t, 550)).OrderBy(t => t.MaxHealth))
                    {
                        if (e.HasBuff("quinnw"))
                        {
                            Orbwalker.ForcedTarget = e;
                            newtarget = true;
                            break;
                        }
                    }
                    if (!newtarget)
                    {
                        Orbwalker.ForcedTarget = null;
                        //Orbwalker.ResetAutoAttack();
                    }
                    return;
                }
            }
        }
    }
}
