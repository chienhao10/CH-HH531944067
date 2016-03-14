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

using Settings = MatrixLucian.Config.Modes.Harass;

namespace MatrixLucian.Modes
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
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (target != null)
                {
                    Q.Cast(target);
                }
            } 
            if (Settings.UseQ && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(Q1.Range, DamageType.Physical);
                if (target != null && Q1.GetPrediction(target).HitChance >= HitChance.Medium)
                {
                    SpellManager.castQ1(target);
                }
            }
            if (Settings.UseE && Settings.UseQ && Q.IsReady() && E.IsReady())
            {
                var target = TargetSelector.GetTarget(Q.Range + E.Range, DamageType.Physical);
                if (target != null)
                {
                    SpellManager.dashToQ1(target);
                }
            }
        }
    }
}
