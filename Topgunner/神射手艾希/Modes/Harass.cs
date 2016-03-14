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

using Settings = AshesToAshes.Config.Modes.Harass;

namespace AshesToAshes.Modes
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
                if (Player.Instance.CountEnemiesInRange(610) > 0)
                {
                    foreach (var b in Player.Instance.Buffs)
                        if (b.Name == "asheqcastready")
                        {
                            Q.Cast();
                        }
                }
            }
            if (Settings.UseW && W.IsReady())
            {
                var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                if (target != null && E.GetPrediction(target).HitChance >= HitChance.High)
                {
                    W.Cast(W.GetPrediction(target).CastPosition);
                }
            }
        }
    }
}
