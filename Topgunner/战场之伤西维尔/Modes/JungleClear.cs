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

using Settings = SivirDamage.Config.Modes.JungleClear;

namespace SivirDamage.Modes
{
    public sealed class JungleClear : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear);
        }

        public override void Execute()
        {
            if (Settings.mana >= Player.Instance.ManaPercent)
                return;

            var minions = EntityManager.MinionsAndMonsters.CombinedAttackable.Where(t => !t.IsDead && t.IsValid && !t.IsInvulnerable && t.IsInRange(Player.Instance.Position, Q.Range));  
            if (Settings.UseQ && Q.IsReady())
            {
                foreach (var m in minions)
                {
                    if (Q.GetPrediction(m).CollisionObjects.Where(t => !t.IsDead && t.IsValid && !t.IsInvulnerable).Count() >= minions.Count() - 1)
                    {
                        Q.Cast(m);
                    }
                }
            }
            if (Settings.UseW && W.IsReady())
            {
                if (minions != null && minions.Count() > 0)
                {
                    W.Cast();
                }
            }
        }
    }
}