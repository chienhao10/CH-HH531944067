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

using Settings = MissFortune.Config.Modes.Harass;

namespace MissFortune.Modes
{
    public sealed class Harass : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);
        }

        public override void Execute()
        {
            if (Combo.Rchanneling)
                return;
            if (Settings.Mana >= Player.Instance.ManaPercent)
                return;

            if (Settings.UseE && E.IsReady())
            {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                if (target != null && E.GetPrediction(target).HitChance >= HitChance.High)
                {
                    E.Cast(target);
                }
            }
            castQ();
        }

        public void castQ()
        {
            if (Settings.UseQ && Q.IsReady())
            {
                if (Settings.useQMinionKillOnly)
                {
                    SpellManager.castQ(true, true);
                }
                else
                {
                    SpellManager.castQ(true, false);
                }
            }
        }
    }
}
