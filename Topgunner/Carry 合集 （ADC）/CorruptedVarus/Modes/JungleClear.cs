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

using Settings = CorruptedVarus.Config.Modes.JungleClear;

namespace CorruptedVarus.Modes
{
    public sealed class JungleClear : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            if (Game.MapId == GameMapId.SummonersRift)
                return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear);
            else
                return false;
        }

        public override void Execute()
        {
            if (Settings.mana >= Player.Instance.ManaPercent)
                return;
            if (Settings.UseQ && Q.IsReady())
            {
                var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Q.Range).Where(t => !t.IsDead && t.IsValid && !t.IsInvulnerable);
                Obj_AI_Base m0 = null;
                if (!Q.IsCharging || !SpellManager.isCharging)
                {
                    foreach (var m in minions)
                    {
                        if (Q.GetPrediction(m).CollisionObjects.Where(t => t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count() >= minions.Count() - 1)
                        {
                            Q.StartCharging();
                            SpellManager.isCharging = true;
                            break;
                        }
                    }
                }
                if (Q.IsFullyCharged && Q.IsCharging && SpellManager.isCharging)
                {
                    foreach (var m in minions)
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
                var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Q.Range).Where(t => !t.IsDead && t.IsValid && !t.IsInvulnerable);
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