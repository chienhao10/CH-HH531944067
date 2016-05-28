#region LICENSE

// Copyright 2014-2015 Support
// Braum.cs is part of Support.
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
// Filename: Support/Support/Braum.cs
// Created:  01/10/2014
// Date:     20/01/2015/11:20
// Author:   h3h3

#endregion

using System;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Braum : PluginBase
    {
        public Braum()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1000);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 650);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 0);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 1200);

            Q.SetSkillshot(0.25f, 60f, 1700f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.5f, 115f, 1400f, false, SkillshotType.SkillshotLine);
        }


        public override
            void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (Q.CastCheck(Target, "Combo.Q"))
                {
                    Q.Cast(Target);
                }

                if (R.CastCheck(Target, "Combo.R"))
                {
                    R.CastIfWillHit(Target, ComboConfig["Combo.R.Count"].Cast<Slider>().CurrentValue - 1);
                }
            }

            if (HarassMode)
            {
                if (Q.CastCheck(Target, "Harass.Q"))
                {
                    Q.Cast(Target);
                }
            }
        }

        public override void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Q.CastCheck(gapcloser.Sender, "Gapcloser.Q"))
            {
                Q.Cast(gapcloser.Sender);
            }
        }

        public override void OnPossibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (spell.DangerLevel < Interrupter2.DangerLevel.High || unit.IsAlly)
            {
                return;
            }

            if (R.CastCheck(unit, "Interrupt.R"))
            {
                R.Cast(unit);
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("Combo.Q", "Use Q", true);
            config.AddBool("Combo.R", "Use R", true);
            config.AddSlider("Combo.R.Count", "Targets hit by R", 2, 1, 5);
        }

        public override void HarassMenu(Menu config)
        {
            config.AddBool("Harass.Q", "Use Q", true);
        }

        public override void MiscMenu(Menu config)
        {
            config.AddBool("Misc.Shield.Skill", "Shield Skillshots", true);
            config.AddBool("Misc.Shield.Target", "Shield Targeted", true);
            config.AddSlider("Misc.Shield.Health", "Shield AA below HP", 30, 1, 100);
        }

        public override void InterruptMenu(Menu config)
        {
            config.AddBool("Gapcloser.Q", "Use Q to Interrupt Gapcloser", true);
            config.AddBool("Interrupt.R", "Use R to Interrupt Spells", true);
        }
    }
}