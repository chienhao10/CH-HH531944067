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

using Settings = MissFortune.Config.Modes.LaneClear;
namespace MissFortune.Modes
{
    public sealed class LaneClear : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            // Only execute this mode when the orbwalker is on laneclear mode
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear);
        }

        public override void Execute()
        {
            if (Combo.Rchanneling)
                return;
            if (Settings.mana >= Player.Instance.ManaPercent)
                return;

            castQ();
            if (Settings.UseQ && Q.IsReady())
            {
                var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable && t.IsInRange(Player.Instance.Position, Q.Range));
                if (minions != null)
                {
                    Q.Cast(minions.OrderBy(t => t.MaxHealth).First());
                }
            }
            if (Settings.UseW && W.IsReady())
            {
                var minion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsInRange(Player.Instance.Position, Q.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
                if (minion > 0)
                {
                    W.Cast();
                }
            }
        }
        public void castQ()
        {
            if (Settings.UseQHarass && Q.IsReady())
            {
                SpellManager.castQ(true, false);
            }
        }
    }
}
