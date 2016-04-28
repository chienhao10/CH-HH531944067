using System;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;

namespace Mordekaiser.Events
{
    internal class Harass
    {
        public Harass()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Utils.Player.Self.IsDead)
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) ||
                (Menu.getKeyBindItem(Menu.MenuE, "UseE.Toggle") && !Utils.Player.Self.IsRecalling()))
            {
                ExecuteE();
            }
        }

        private static void ExecuteE()
        {
            if (Utils.Player.Self.HealthPercent <= Menu.getSliderItem(Menu.MenuE, "UseE.Harass.MinHeal"))
            {
                return;
            }

            if (!Menu.getCheckBoxItem(Menu.MenuE, "UseE.Harass"))
                return;

            if (!Spells.E.IsReady())
            {
                return;
            }

            var t = TargetSelector.GetTarget(Spells.E.Range, DamageType.Magical);

            if (!t.IsValidTarget())
            {
                return;
            }

            Spells.E.Cast(t);
        }
    }
}