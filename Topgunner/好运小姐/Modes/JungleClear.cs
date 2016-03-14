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

using Settings = MissFortune.Config.Modes.JungleClear;

namespace MissFortune.Modes
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
            if (Combo.Rchanneling)
                return;
            if (Settings.mana >= Player.Instance.ManaPercent)
                return;

            if (Settings.UseQ && Q.IsReady())
            {
                var minion = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Q.Range).Where(t => !t.IsDead && t.IsValid && !t.IsInvulnerable).OrderBy(t => t.MaxHealth).FirstOrDefault();
                if(minion != null)
                {
                    Q.Cast(minion);
                }
            }
            if (Settings.UseW && W.IsReady())
            {
                var minion = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Q.Range).Where(t => !t.IsDead && t.IsValid && !t.IsInvulnerable).OrderBy(t => t.MaxHealth).FirstOrDefault();
                if (minion != null)
                {
                    W.Cast();
                }
            }
        }
    }
}