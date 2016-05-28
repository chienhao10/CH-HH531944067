#region LICENSE

// Copyright 2014-2015 Support
// Alistar.cs is part of Support.
// 
// Support is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Support is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Support. If not, see <http://www.gnu.org/licenses/>.
// 
// Filename: Support/Support/Alistar.cs
// Created:  01/10/2014
// Date:     20/01/2015/11:20
// Author:   h3h3

#endregion

using System;
using EloBuddy;
using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
	#region

    #endregion

    public class Alistar : PluginBase
	{
		public Alistar()
		{
			Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 365);
			W = new LeagueSharp.Common.Spell(SpellSlot.W, 650);
			E = new LeagueSharp.Common.Spell(SpellSlot.E, 575);
			R = new LeagueSharp.Common.Spell(SpellSlot.R, 0);

			W.SetTargetted(0.5f, float.MaxValue);
		}

        public override void OnUpdate(EventArgs args)
        {
            if (Player.HasBuffOfType(BuffType.Taunt) || Player.HasBuffOfType(BuffType.Stun) ||
                Player.HasBuffOfType(BuffType.Snare) || Player.HasBuffOfType(BuffType.Polymorph) ||
                Player.HasBuffOfType(BuffType.Blind) || Player.HasBuffOfType(BuffType.Fear) ||
                Player.HasBuffOfType(BuffType.Silence) || Player.HealthPercent < 90)
            {
                if (R.IsReady())
                {
                    R.Cast();
                }
            }

            if (Q.CastCheck(Target, "Combo.Q"))
            {
                Q.Cast();
            }

            if (Q.IsReady() && W.CastCheck(Target, "Combo.W"))
            {
                W.Cast(Target);
                var jumpTime = Math.Max(0, Player.LSDistance(Target) - 500)*10/25 + 25;
                LeagueSharp.Common.Utility.DelayAction.Add((int) jumpTime, () => Q.Cast());
            }

            var ally = Helpers.AllyBelowHp(ComboConfig["Combo.E.Health"].Cast<Slider>().CurrentValue, E.Range);
            if (E.CastCheck(ally, "Combo.E", true, false))
            {
                E.Cast();
            }
        }

        public override void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly)
            {
                return;
            }

            if (W.CastCheck(gapcloser.Sender, "Gapcloser.W"))
            {
                W.Cast(gapcloser.Sender);
            }
        }

        public override void OnPossibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (spell.DangerLevel < Interrupter2.DangerLevel.High || unit.IsAlly)
            {
                return;
            }

            if (Q.CastCheck(unit, "Interrupt.Q"))
            {
                Q.Cast();
            }

            if (W.CastCheck(unit, "Interrupt.W"))
            {
                W.Cast(unit);
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("Combo.Q", "Use Q", true);
            config.AddBool("Combo.W", "Use WQ", true);
            config.AddBool("Combo.E", "Use E", true);
            config.AddSlider("Combo.E.Health", "Health to Heal", 20, 1, 100);
        }

        public override void HarassMenu(Menu config)
        {
            config.AddBool("Harass.Q", "Use Q", true);
            config.AddBool("Harass.E", "Use E", true);
            config.AddSlider("Harass.E.Health", "Health to Heal", 20, 1, 100);
        }

        public override void InterruptMenu(Menu config)
        {
            config.AddBool("Gapcloser.W", "Use W to Interrupt Gapcloser", true);

            config.AddBool("Interrupt.Q", "Use Q to Interrupt Spells", true);
            config.AddBool("Interrupt.W", "Use W to Interrupt Spells", true);
        }
    }
}