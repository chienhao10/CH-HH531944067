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

using Settings = PurifierVayne.Config.Modes.Harass;

namespace PurifierVayne.Modes
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

            if (Settings.UseE && E.IsReady())
            {
                SpellManager.CastE();
            }
        }

        internal static void Spellblade(AttackableUnit target, EventArgs args)
        {
            if (Settings.Mana >= Player.Instance.ManaPercent || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) || !Settings.UseQ || Player.Instance.CountEnemiesInRange(850) == 0)
                return;
            SpellManager.CastQ(true);
        }
    }
}
