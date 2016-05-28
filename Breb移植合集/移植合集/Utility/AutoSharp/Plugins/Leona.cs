#region LICENSE

// Copyright 2014-2015 Support
// Leona.cs is part of Support.
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
// Filename: Support/Support/Leona.cs
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
    public class Leona : PluginBase
    {
        public Leona()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, AttackRange);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, AttackRange);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 700);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 1200);

            E.SetSkillshot(0.25f, 100f, 2000f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(1f, 300f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (Q.CastCheck(Target))
                {
                    Orbwalker.ResetAutoAttack();
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, Target);
                }

                if (W.CastCheck(Target, "ComboQWE"))
                {
                    W.Cast();
                }

                if (E.CastCheck(Target, "ComboQWE") && Q.IsReady())
                {
                    // Max Range with VeryHigh Hitchance / Immobile
                    if (E.GetPrediction(Target).Hitchance >= HitChance.VeryHigh)
                    {
                        if (E.Cast(Target) == LeagueSharp.Common.Spell.CastStates.SuccessfullyCasted)
                        {
                            W.Cast();
                        }
                    }

                    // Lower Range
                    if (E.GetPrediction(Target, false, 775).Hitchance >= HitChance.High)
                    {
                        if (E.Cast(Target) == LeagueSharp.Common.Spell.CastStates.SuccessfullyCasted)
                        {
                            W.Cast();
                        }
                    }
                }

                if (E.IsReady() && Heroes.Player.LSDistance(Target) < E.Range)
                {
                    E.Cast(Target);
                }

                if (R.IsReady() && Heroes.Player.LSDistance(Target) < R.Range)
                {
                    R.CastIfHitchanceEquals(Target, HitChance.Immobile);
                }
            }
        }

        public override void OnAfterAttack(AttackableUnit target, EventArgs args)
        {
            if (!(target is AIHeroClient) && !target.Name.ToLower().Contains("ward"))
            {
                return;
            }

            if (!Q.IsReady())
            {
                return;
            }

            if (Q.Cast())
            {
                Orbwalker.ResetAutoAttack();
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
            }
        }

        public override void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly)
            {
                return;
            }

            if (Q.CastCheck(gapcloser.Sender, "GapcloserQ"))
            {
                if (Q.Cast())
                {
                    Orbwalker.ResetAutoAttack();
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, gapcloser.Sender);
                }
            }
        }

        public override void OnPossibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (spell.DangerLevel < Interrupter2.DangerLevel.High || unit.IsAlly)
            {
                return;
            }

            if (Q.CastCheck(unit, "InterruptQ"))
            {
                if (Q.Cast())
                {
                    Orbwalker.ResetAutoAttack();
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, unit);
                }

                return;
            }

            if (R.CastCheck(unit, "InterruptR"))
            {
                R.Cast(unit);
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboE", "Use E without Q", false);
            config.AddBool("ComboQWE", "Use Q/W/E", true);
            config.AddBool("ComboR", "Use R", true);
        }

        public override void InterruptMenu(Menu config)
        {
            config.AddBool("GapcloserQ", "Use Q to Interrupt Gapcloser", true);

            config.AddBool("InterruptQ", "Use Q to Interrupt Spells", true);
            config.AddBool("InterruptR", "Use R to Interrupt Spells", true);
        }
    }
}