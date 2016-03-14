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

using Settings = CorruptedVarus.Config.Modes.Harass;

namespace CorruptedVarus.Modes
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
            if (Settings.UseQ && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(1400, DamageType.Physical);
                if (target != null)
                {
                    if (!Q.IsCharging || !SpellManager.isCharging)
                    {
                        Q.StartCharging();
                        SpellManager.isCharging = true;
                    }
                    if (Q.IsFullyCharged && Q.IsCharging && SpellManager.isCharging)
                    {
                        Q.Cast(Q.GetPrediction(target).CastPosition);
                        SpellManager.isCharging = false;
                    }
                }
            }
            if (Settings.UseE && E.IsReady() && (!Q.IsCharging || !SpellManager.isCharging))
            {
                var target = TargetSelector.GetTarget(SpellManager.E.Range, DamageType.Physical);
                if (target != null)
                {
                    E.Cast(E.GetPrediction(target).CastPosition);
                }
            }
        }
    }
}
