#region LICENSE

// Copyright 2014-2015 Support
// Blitzcrank.cs is part of Support.
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
// Filename: Support/Support/Blitzcrank.cs
// Created:  05/10/2014
// Date:     20/01/2015/11:20
// Author:   h3h3

#endregion

using System;
using EloBuddy;
using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using System.Linq;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{

    public class Blitzcrank : PluginBase
	{
		public Blitzcrank()
		{
			Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 900);
			W = new LeagueSharp.Common.Spell(SpellSlot.W, 0);
			E = new LeagueSharp.Common.Spell(SpellSlot.E, AttackRange);
			R = new LeagueSharp.Common.Spell(SpellSlot.R, 600);

			Q.SetSkillshot(0.25f, 70f, 1800f, true, SkillshotType.SkillshotLine);
		}

		private bool BlockQ
		{
			get
			{
				if (!Q.IsReady())
				{
					return true;
				}

				if (!MiscConfig["Misc.Q.Block"].Cast<CheckBox>().CurrentValue)
				{
					return false;
				}

				if (!Target.LSIsValidTarget())
				{
					return true;
				}

				if (Target.HasBuff("BlackShield"))
				{
					return true;
				}

				if (Helpers.AllyInRange(1200)
					.Any(ally => ally.LSDistance(Target) < ally.AttackRange + ally.BoundingRadius))
				{
					return true;
				}

				return Player.LSDistance(Target) < MiscConfig["Misc.Q.Block.Distance"].Cast<Slider>().CurrentValue;
			}
		}

        public override void OnUpdate(EventArgs args)
        {
            var target = TargetSelector.GetTarget(900, DamageType.Physical);

            try
            {
                if (ComboMode)
                {
                    if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range && !BlockQ)
                    {
                        Q.Cast(target);
                    }

                    if (E.CastCheck(target))
                    {
                        if (E.Cast())
                        {
                            Orbwalker.ResetAutoAttack();
                            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                        }
                    }

                    if (E.IsReady() && target.LSIsValidTarget() && Target.HasBuff("RocketGrab"))
                    {
                        if (E.Cast())
                        {
                            Orbwalker.ResetAutoAttack();
                            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                        }
                    }

                    if (W.IsReady() && Player.LSCountEnemiesInRange(1500) > 0)
                    {
                        W.Cast();
                    }

                    if (R.IsReady() && Heroes.Player.LSDistance(Target) < R.Range)
                    {
                        if (Helpers.EnemyInRange(ComboConfig["ComboCountR"].Cast<Slider>().CurrentValue, R.Range))
                        {
                            R.Cast();
                        }
                    }
                }

                if (HarassMode)
                {
                    if (Q.CastCheck(Target, "HarassQ") && !BlockQ)
                    {
                        Q.Cast(target);
                    }

                    if (E.CastCheck(target))
                    {
                        if (E.Cast())
                        {
                            Orbwalker.ResetAutoAttack();
                            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                        }
                    }

                    if (E.IsReady() && target.LSIsValidTarget() && target.HasBuff("RocketGrab"))
                    {
                        if (E.Cast())
                        {
                            Orbwalker.ResetAutoAttack();
                            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnAfterAttack(AttackableUnit target, EventArgs args)
        {
            if (!target.IsValid<AIHeroClient>() && !target.Name.ToLower().Contains("ward"))
            {
                return;
            }

            if (!E.IsReady())
            {
                return;
            }

            if (E.Cast())
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

            if (E.CastCheck(gapcloser.Sender, "GapcloserE"))
            {
                if (E.Cast())
                {
                    Orbwalker.ResetAutoAttack();
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, gapcloser.Sender);
                }
            }

            if (R.CastCheck(gapcloser.Sender, "GapcloserR"))
            {
                R.Cast();
            }
        }

        public override void OnPossibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (spell.DangerLevel < Interrupter2.DangerLevel.High || unit.IsAlly)
            {
                return;
            }

            if (E.CastCheck(unit, "InterruptE"))
            {
                if (E.Cast())
                {
                    Orbwalker.ResetAutoAttack();
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, unit);
                }
            }

            if (Q.CastCheck(Target, "InterruptQ"))
            {
                Q.Cast(unit);
            }

            if (R.CastCheck(unit, "InterruptR"))
            {
                R.Cast();
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboR", "Use R", true);
            config.AddSlider("ComboCountR", "Targets in range to Ult", 2, 1, 5);
        }

        public override void MiscMenu(Menu config)
        {
            config.AddBool("Misc.Q.Block", "Block Q on close Targets", true);
            config.AddSlider("Misc.Q.Block.Distance", "Q Block Distance", 400, 0, 800);
        }

        public override void HarassMenu(Menu config)
        {
            config.AddBool("HarassQ", "Use Q", true);
        }

        public override void InterruptMenu(Menu config)
        {
            config.AddBool("GapcloserE", "Use E to Interrupt Gapcloser", true);
            config.AddBool("GapcloserR", "Use R to Interrupt Gapcloser", true);
            config.AddBool("InterruptQ", "Use Q to Interrupt Spells", true);
            config.AddBool("InterruptE", "Use E to Interrupt Spells", true);
            config.AddBool("InterruptR", "Use R to Interrupt Spells", true);
        }
    }
}