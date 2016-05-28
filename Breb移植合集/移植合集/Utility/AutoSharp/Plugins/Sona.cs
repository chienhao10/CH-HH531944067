#region LICENSE

// Copyright 2014-2015 Support
// Sona.cs is part of Support.
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
// Filename: Support/Support/Sona.cs
// Created:  15/10/2014
// Date:     20/01/2015/11:20
// Author:   h3h3

#endregion

using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    #region

    

    #endregion

    public class Sona : PluginBase
    {
        public Sona()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 850);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 1000);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 350);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 1000);

            R.SetSkillshot(0.5f, 125, float.MaxValue, false, SkillshotType.SkillshotLine);
        }

        public override void OnUpdate(EventArgs args)
        {
            try
            {
                if (ComboMode)
                {
                    if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range)
                    {
                        Q.Cast();
                    }

                    //if (Target.LSIsValidTarget(AttackRange) &&
                    //    (Player.HasBuff("sonaqprocattacker") || Player.HasBuff("sonaqprocattacker")))
                    //{
                    //    Player.IssueOrder(GameObjectOrder.AttackUnit, Target);
                    //}

                    var allyW = Helpers.AllyBelowHp(ComboConfig["ComboHealthW"].Cast<Slider>().CurrentValue, W.Range);
                    if (W.CastCheck(allyW, "ComboW", true, false))
                    {
                        W.Cast();
                    }

                    if (E.IsReady() && Helpers.AllyInRange(E.Range).Count > 0)
                    {
                        E.Cast();
                    }

                    if (R.IsReady() && Heroes.Player.LSDistance(Target) < R.Range)
                    {
                        R.CastIfWillHit(Target, ComboConfig["ComboCountR"].Cast<Slider>().CurrentValue, true);
                    }
                }

                if (HarassMode)
                {
                    if (Q.CastCheck(Target, "HarassQ"))
                    {
                        Q.Cast();
                    }

                    var allyW = Helpers.AllyBelowHp(HarassConfig["HarassHealthW"].Cast<Slider>().CurrentValue, W.Range);
                    if (W.CastCheck(allyW, "HarassW", true, false))
                    {
                        W.Cast();
                    }

                    if (E.IsReady() && Helpers.AllyInRange(E.Range).Count > 0 && HarassConfig["HarassE"].Cast<CheckBox>().CurrentValue)
                    {
                        E.Cast();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly)
            {
                return;
            }

            if (R.CastCheck(gapcloser.Sender, "GapcloserR"))
            {
                R.Cast(Target, true);
            }
        }

        public override void OnPossibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (spell.DangerLevel < Interrupter2.DangerLevel.High || unit.IsAlly)
            {
                return;
            }

            if (R.CastCheck(unit, "InterruptR"))
            {
                R.Cast(Target, true);
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddBool("ComboR", "Use R", true);
            config.AddSlider("ComboCountR", "Targets hit to Ult", 3, 1, 5);
            config.AddSlider("ComboHealthW", "Health to Heal", 80, 1, 100);
        }

        public override void HarassMenu(Menu config)
        {
            config.AddBool("HarassQ", "Use Q", true);
            config.AddBool("HarassW", "Use W", true);
            config.AddBool("HarassE", "Use E", true);
            config.AddSlider("HarassHealthW", "Health to Heal", 60, 1, 100);
        }

        public override void InterruptMenu(Menu config)
        {
            config.AddBool("GapcloserR", "Use R to Interrupt Gapcloser", false);

            config.AddBool("InterruptR", "Use R to Interrupt Spells", true);
        }
    }
}