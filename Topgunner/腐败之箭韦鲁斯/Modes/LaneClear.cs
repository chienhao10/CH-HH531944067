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

using Settings = CorruptedVarus.Config.Modes.LaneClear;
namespace CorruptedVarus.Modes
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
            //Console.WriteLine("fully " + Q.IsFullyCharged + " SpellMGR " + SpellManager.isCharging + " their charging " + Q.IsCharging);
            if (Settings.mana >= Player.Instance.ManaPercent)
                return;

            if (Settings.UseQ && Q.IsReady())
            {
                var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable && t.IsInRange(Player.Instance.Position, Q.Range));
                Obj_AI_Base m0 = null;
                foreach (var m in minions)
                {
                    if (!Q.IsCharging || !SpellManager.isCharging)
                    {
                        if (Q.GetPrediction(m).CollisionObjects.Where(t => t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count() >= 3)
                        {
                            Q.StartCharging();
                            SpellManager.isCharging = true;
                            break;
                        }
                    }
                    if (Q.IsFullyCharged && Q.IsCharging && SpellManager.isCharging)
                    {
                        if (m0 == null)
                        {
                            m0 = m;
                            continue;
                        }
                        if (Q.GetPrediction(m).CollisionObjects.Where(t => t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count() >= Q.GetPrediction(m0).CollisionObjects.Where(t => t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count())
                        {
                            m0 = m;
                        }
                    }
                }
                if (Q.IsFullyCharged && Q.IsCharging && SpellManager.isCharging && m0 != null)
                {
                    Q.Cast(m0);
                    SpellManager.isCharging = false;
                }
            }
            if (Settings.UseE && E.IsReady() && (!Q.IsCharging || !SpellManager.isCharging))
            {
                var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable && t.IsInRange(Player.Instance.Position, E.Range));
                foreach (var m in minions)
                {
                    if (m.GetBuffCount("varuswdebuff") == 2)
                    {
                        E.Cast(m);
                        break;
                    }
                }
            }
        }
    }
}
