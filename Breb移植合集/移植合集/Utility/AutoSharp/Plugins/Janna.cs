#region LICENSE

// Copyright 2014-2015 Support
// Janna.cs is part of Support.
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
// Filename: Support/Support/Janna.cs
// Created:  01/10/2014
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

    public class Janna : PluginBase
    {
        public Janna()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1100);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 600);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 800);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 550);

            Q.SetSkillshot(0.25f, 120f, 900f, false, SkillshotType.SkillshotLine);
            GameObject.OnCreate += TowerAttackOnCreate;
            GameObject.OnCreate += RangeAttackOnCreate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        private int LastQInterrupt { get; set; }
        private bool IsUltChanneling { get; set; }

        public override void OnUpdate(EventArgs args)
        {
            try
            {
                if (Player.IsChannelingImportantSpell())
                {
                    return;
                }

                if (IsUltChanneling)
                {
                    Orbwalker.DisableMovement = false;
                    Orbwalker.DisableAttacking = false;
                    IsUltChanneling = false;
                }

                if (ComboMode)
                {
                    if (Q.CastCheck(Target, "Combo.Q"))
                    {
                        var pred = Q.GetPrediction(Target);
                        if (pred.Hitchance >= HitChance.High)
                        {
                            Q.Cast(pred.CastPosition);
                            Q.Cast();
                        }
                    }

                    if (W.CastCheck(Target, "Combo.W"))
                    {
                        W.Cast(Target);
                    }

                    var ally = Helpers.AllyBelowHp(ComboConfig["Combo.R.Health"].Cast<Slider>().CurrentValue, R.Range);
                    if (R.CastCheck(ally, "Combo.R", true, false) && Player.LSCountEnemiesInRange(1000) > 0)
                    {
                        R.Cast();
                    }
                }

                if (HarassMode)
                {
                    if (W.CastCheck(Target, "Harass.W"))
                    {
                        W.Cast(Target);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "ReapTheWhirlwind")
            {

                Orbwalker.DisableMovement = true;
                Orbwalker.DisableAttacking = true;
                IsUltChanneling = true;
            }
        }

        private void RangeAttackOnCreate(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid<MissileClient>() || IsUltChanneling)
            {
                return;
            }

            var missile = (MissileClient)sender;

            // Caster ally hero / not me
            if (!missile.SpellCaster.IsValid<AIHeroClient>() || !missile.SpellCaster.IsAlly || missile.SpellCaster.IsMe ||
                missile.SpellCaster.IsMelee)
            {
                return;
            }

            // Target enemy hero
            if (!missile.Target.IsValid<AIHeroClient>() || !missile.Target.IsEnemy)
            {
                return;
            }

            var caster = (AIHeroClient)missile.SpellCaster;

            // only in SBTW mode
            if (E.IsReady() && E.IsInRange(caster) && (ComboMode || HarassMode) &&
                MiscConfig["Misc.E.AA." + caster.ChampionName].Cast<CheckBox>().CurrentValue)
            {
                E.Cast(caster);
            }
        }

        private void TowerAttackOnCreate(GameObject sender, EventArgs args)
        {
            if (!E.IsReady() || !MiscConfig["Misc.E.Tower"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            if (sender.IsValid<MissileClient>() && !IsUltChanneling)
            {
                var missile = (MissileClient)sender;

                // Ally Turret -> Enemy Hero
                if (missile.SpellCaster.IsValid<Obj_AI_Turret>() && missile.SpellCaster.IsAlly &&
                    missile.Target.IsValid<AIHeroClient>() && missile.Target.IsEnemy)
                {
                    var turret = (Obj_AI_Turret)missile.SpellCaster;

                    if (E.IsInRange(turret))
                    {
                        E.Cast(turret);
                    }
                }
            }
        }

        public override void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly)
            {
                return;
            }

            if (Q.CastCheck(gapcloser.Sender, "Gapcloser.Q"))
            {
                var pred = Q.GetPrediction(gapcloser.Sender);
                if (pred.Hitchance >= HitChance.Medium)
                {
                    Q.Cast(pred.CastPosition);
                    Q.Cast();
                }
            }

            if (W.CastCheck(gapcloser.Sender, "Gapcloser.W"))
            {
                W.Cast(gapcloser.Sender);
            }
        }

        public override void OnPossibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if ((spell.DangerLevel < Interrupter2.DangerLevel.High && unit.IsAlly))
            {
                return;
            }

            if (Q.CastCheck(unit, "Interrupt.Q"))
            {
                var pred = Q.GetPrediction(unit);
                if (pred.Hitchance >= HitChance.Medium)
                {
                    Q.Cast(pred.CastPosition);
                    Q.Cast();
                    LastQInterrupt = Environment.TickCount;
                    return;
                }
            }

            if (!Q.IsReady() && Environment.TickCount - LastQInterrupt > 500 && R.CastCheck(unit, "Interrupt.R"))
            {
                R.Cast();
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("Combo.Q", "Use Q", true);
            config.AddBool("Combo.W", "Use W", true);
            config.AddBool("Combo.R", "Use R", true);
            config.AddSlider("Combo.R.Health", "Health to Ult", 15, 1, 100);
        }

        public override void HarassMenu(Menu config)
        {
            config.AddBool("Harass.W", "Use W", true);
        }

        public override void MiscMenu(Menu config)
        {
            config.AddBool("Misc.E.Tower", "Use E on Towers", true);

            // build aa menu
            //var aa = config.AddSubMenu(new Menu("Use E on Attacks", "Misc.E.AA.Menu"));
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(h => h.IsAlly && !h.IsMe))
            {
                config.AddBool("Misc.E.AA." + hero.ChampionName, "E on AA : " + hero.ChampionName, true);
            }
        }

        public override void ManaMenu(Menu config)
        {
            config.AddSlider("Mana.E.Priority.1", "E Priority 1", 65, 0, 100);
            config.AddSlider("Mana.E.Priority.2", "E Priority 2", 35, 0, 100);
            config.AddSlider("Mana.E.Priority.3", "E Priority 3", 10, 0, 100);
        }

        public override void InterruptMenu(Menu config)
        {
            config.AddBool("Gapcloser.Q", "Use Q to Interrupt Gapcloser", true);
            config.AddBool("Gapcloser.W", "Use W to Interrupt Gapcloser", true);

            config.AddBool("Interrupt.Q", "Use Q to Interrupt Spells", true);
            config.AddBool("Interrupt.R", "Use R to Interrupt Spells", true);
        }
    }
}