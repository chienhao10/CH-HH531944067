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

using Settings = SivirDamage.Config.Modes.Harass;

namespace SivirDamage.Modes
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
                if (target != null && Q.GetPrediction(target).HitChance >= HitChance.Medium)
                {
                    int count = 0;
                    foreach (var e in Q.GetPrediction(target).CollisionObjects)
                    {
                        if (e.NetworkId != target.NetworkId)
                        {
                            count++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    int dmgMod = 100 - count * 15;
                    if(dmgMod < 40)
                        dmgMod = 40;
                    if (dmgMod >= Settings.damageMin)
                    {
                        Q.Cast(Q.GetPrediction(target).CastPosition);
                    }
                }
            }
            if (Settings.UseW && W.IsReady())
            {
                var target = TargetSelector.GetTarget(500, DamageType.Physical);
                if (target != null)
                {
                    W.Cast();
                }
            }
        }
    }
}
