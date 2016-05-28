#region LICENSE

// Copyright 2014 - 2014 Support
// Soraka.cs is part of Support.
// Support is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// Support is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with Support. If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

#endregion

namespace AutoSharp.Plugins
{
    public class Soraka : PluginBase
    {
        public Soraka()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 950);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 450);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 925);
            R = new LeagueSharp.Common.Spell(SpellSlot.R);

            Q.SetSkillshot(0.5f, 300, 1750, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.5f, 70f, 1750, false, SkillshotType.SkillshotCircle);
        }

        public override void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var unit = gapcloser.Sender;

            if (MiscConfig["GapcloserQ"].Cast<CheckBox>().CurrentValue && unit.LSIsValidTarget(Q.Range) && Q.IsReady())
                Q.Cast(unit);

            if (MiscConfig["GapcloserE"].Cast<CheckBox>().CurrentValue && unit.LSIsValidTarget(E.Range) && E.IsReady())
                E.Cast(unit);
        }

        public override void OnPossibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (!MiscConfig["InterruptE"].Cast<CheckBox>().CurrentValue ||
                spell.DangerLevel < Interrupter2.DangerLevel.High ||
                unit.IsAlly)
                return;

            if (!unit.LSIsValidTarget(E.Range))
                return;

            if (!E.IsReady())
                return;

            E.Cast(unit);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range)
                {
                    Q.Cast(Target);
                }

                if (E.IsReady() && Heroes.Player.LSDistance(Target) < E.Range)
                {
                    E.Cast(Target);
                }
            }

            if (HarassMode)
            {
                if (Q.CastCheck(Target, "HarassQ"))
                {
                    Q.Cast(Target);
                }

                if (E.CastCheck(Target, "HarassE"))
                {
                    E.Cast(Target);
                }
            }

            AutoW();

            AutoR();
        }

       
        private void AutoW()
        {
            if (W.IsReady() && MiscConfig["AutoW"].Cast<CheckBox>().CurrentValue)
            {
                var ally = Helpers.AllyBelowHp(MiscConfig["AutoWPercent"].Cast<Slider>().CurrentValue, W.Range);
				if (ally==null) return;
                if (Player.LSIsRecalling() ||
                    ally.LSIsRecalling() ||
                    ObjectManager.Player.InFountain())
                    return;

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (ally != null && (Player.Health / Player.MaxHealth) * 100 > 70)
                {
                    W.Cast(ally);
                }
            }
        }

        private void AutoR()
        {
            if (R.IsReady() && MiscConfig["AutoR"].Cast<CheckBox>().CurrentValue)
            {
                var ally = Helpers.AllyBelowHp(MiscConfig["AutoRPercent"].Cast<Slider>().CurrentValue, R.Range);

                if (ally != null || (Player.Health / Player.MaxHealth) * 100 < MiscConfig["AutoRPercent"].Cast<Slider>().CurrentValue)
                {
                    R.Cast();
                }

            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboE", "Use E", true);
        }

        public override void HarassMenu(Menu config)
        {
            config.AddBool("HarassQ", "Use Q", true);
            config.AddBool("HarassE", "Use E", true);
        }

        public override void MiscMenu(Menu config)
        {
            config.AddBool("GapcloserQ", "Use Q to Interrupt Gapcloser", true);
            config.AddBool("GapcloserE", "Use E to Interrupt Gapcloser", true);

            config.AddBool("AutoW", "Auto use W", true);
            config.AddSlider("AutoWPercent", "W Percent", 50, 1, 100);

            config.AddBool("AutoR", "Auto use R", true);
            config.AddSlider("AutoRPercent", "R Percent", 15, 1, 100);

            config.AddBool("InterruptE", "Use E to Interrupt Spells", true);
        }
    }
}