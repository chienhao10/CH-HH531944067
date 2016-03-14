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

using Settings = MatrixLucian.Config.Modes.JungleClear;

namespace MatrixLucian.Modes
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
            if (Settings.UseQ && Q.IsReady())
            {
                var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Q.Range).Where(t => !t.IsDead && t.IsValid && !t.IsInvulnerable);
                foreach (var m in minions)
                {
                    if (Q1.GetPrediction(m).CollisionObjects.Where(t => t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count() >= minions.Count() - 1)
                    {
                        Q.Cast(m);
                        break;
                    }
                }
            }
            if (Settings.UseE && E.IsReady())
            {
                var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Q.Range).Where(t => !t.IsDead && t.IsValid && !t.IsInvulnerable); 
                if (minions.Count() > 1)
                {
                    E.Cast(Game.CursorPos);
                }
            }
            /*
            if (Settings.UseW && W.IsReady())
            {
                var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, W.Range).Where(t => !t.IsDead && t.IsValid && !t.IsInvulnerable); 
                if (minions.Count() > 0)
                {
                    W.Cast(minions.First());
                }
            }*/
        }
    }
}