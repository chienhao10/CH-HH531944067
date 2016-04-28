using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using PortAIO.Properties;
using Damage = LeagueSharp.Common.Damage;
using Spell = LeagueSharp.Common.Spell;

namespace ElDiana
{
    internal enum Spells
    {
        Q,

        W,

        E,

        R
    }

    internal static class Diana
    {
        #region Public Properties

        public static string ScriptVersion
        {
            get { return typeof (Diana).Assembly.GetName().Version.ToString(); }
        }

        #endregion

        #region Static Fields

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
        {
            {Spells.Q, new Spell(SpellSlot.Q, 895)},
            {Spells.W, new Spell(SpellSlot.W, 240)},
            {Spells.E, new Spell(SpellSlot.E, 450)},
            {Spells.R, new Spell(SpellSlot.R, 825)}
        };

        private static SpellSlot ignite;

        #endregion

        #region Properties

        private static HitChance CustomHitChance
        {
            get { return GetHitchance(); }
        }

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        #endregion

        #region Public Methods and Operators

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            float damage = 0;

            if (spells[Spells.Q].IsReady())
            {
                damage += spells[Spells.Q].GetDamage(enemy);
            }

            if (spells[Spells.W].IsReady())
            {
                damage += spells[Spells.W].GetDamage(enemy);
            }

            if (spells[Spells.E].IsReady())
            {
                damage += spells[Spells.E].GetDamage(enemy);
            }

            if (spells[Spells.R].IsReady())
            {
                damage += spells[Spells.R].GetDamage(enemy);
            }

            if (ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(ignite) != SpellState.Ready)
            {
                damage += (float) Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            }

            return damage;
        }

        #region Static Fields

        public static Menu _menu, comboMenu, harassMenu, laneclearMenu, jungleClearMenu, interruptMenu, miscomboMenu;

        #endregion

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static void Initialize()
        {
            _menu = MainMenu.AddMenu("ElDiana", "menu");

            comboMenu = _menu.AddSubMenu("Combo", "Combo");
            comboMenu.AddGroupLabel("R Settings");
            comboMenu.Add("ElDiana.Combo.R.Mode", new Slider("Mode (0 : Normal | 1 : Misaya (R > Q)) : ", 0, 0, 1));
            comboMenu.Add("ElDiana.Combo.R", new CheckBox("Use R"));
            comboMenu.Add("ElDiana.Combo.R.MisayaMinRange",
                new Slider("R Minimum Range for Misaya ", Convert.ToInt32(spells[Spells.R].Range*0.8), 0,
                    Convert.ToInt32(spells[Spells.R].Range)));
            comboMenu.Add("ElDiana.Combo.R.PreventUnderTower", new Slider("Don't use ult if HP% <  ", 20));
            comboMenu.AddSeparator();
            comboMenu.Add("ElDiana.Combo.Q", new CheckBox("Use Q"));
            comboMenu.Add("ElDiana.Combo.W", new CheckBox("Use W"));
            comboMenu.Add("ElDiana.Combo.E", new CheckBox("Use E"));
            comboMenu.Add("ElDiana.Combo.Secure", new CheckBox("Use R to secure kill"));
            comboMenu.Add("ElDiana.Combo.UseSecondRLimitation",
                new Slider("Max close enemies for secure kill with R", 5, 1, 5));
            comboMenu.Add("ElDiana.Combo.Ignite", new CheckBox("Use Ignite"));
            comboMenu.Add("ElDiana.hitChance", new Slider("Hitchance Q (Lowest to Highest)", 3, 0, 3));

            harassMenu = _menu.AddSubMenu("Harass", "Harass");
            harassMenu.Add("ElDiana.Harass.Q", new CheckBox("Use Q"));
            harassMenu.Add("ElDiana.Harass.W", new CheckBox("Use W"));
            harassMenu.Add("ElDiana.Harass.E", new CheckBox("Use E"));
            harassMenu.Add("ElDiana.Harass.Mana", new Slider("Minimum mana for harass", 55));

            laneclearMenu = _menu.AddSubMenu("Laneclear", "Laneclear");
            laneclearMenu.Add("ElDiana.LaneClear.Q", new CheckBox("Use Q"));
            laneclearMenu.Add("ElDiana.LaneClear.W", new CheckBox("Use W"));
            laneclearMenu.Add("ElDiana.LaneClear.E", new CheckBox("Use E"));
            laneclearMenu.Add("ElDiana.LaneClear.R", new CheckBox("Use R"));
            laneclearMenu.Add("ElDiana.LaneClear.Count.Minions.Q", new Slider("Minions in range for Q", 2, 1, 5));
            laneclearMenu.Add("ElDiana.LaneClear.Count.Minions.W", new Slider("Minions in range for W", 2, 1, 5));
            laneclearMenu.Add("ElDiana.LaneClear.Count.Minions.E", new Slider("Minions in range for E", 2, 1, 5));


            jungleClearMenu = _menu.AddSubMenu("Jungleclear", "Jungleclear");
            jungleClearMenu.Add("ElDiana.JungleClear.Q", new CheckBox("Use Q"));
            jungleClearMenu.Add("ElDiana.JungleClear.W", new CheckBox("Use W"));
            jungleClearMenu.Add("ElDiana.JungleClear.E", new CheckBox("Use E"));
            jungleClearMenu.Add("ElDiana.JungleClear.R", new CheckBox("Use R"));

            interruptMenu = _menu.AddSubMenu("Interrupt", "Interrupt");
            interruptMenu.Add("ElDiana.Interrupt.UseEInterrupt", new CheckBox("Use E to interrupt"));
            interruptMenu.Add("ElDiana.Interrupt.UseEDashes", new CheckBox("Use E to interrupt dashes"));

            miscomboMenu = _menu.AddSubMenu("Misc", "Misc");
            miscomboMenu.Add("ElDiana.Draw.off", new CheckBox("Turn drawings off", false));
            miscomboMenu.Add("ElDiana.Draw.Q", new CheckBox("Draw Q"));
            miscomboMenu.Add("ElDiana.Draw.W", new CheckBox("Draw W"));
            miscomboMenu.Add("ElDiana.Draw.E", new CheckBox("Draw E"));
            miscomboMenu.Add("ElDiana.Draw.R", new CheckBox("Draw R"));
            miscomboMenu.Add("ElDiana.Draw.RMisaya", new CheckBox("Draw Misaya Combo Range"));
            miscomboMenu.Add("ElDiana.Draw.Text", new CheckBox("Draw Text"));
            miscomboMenu.Add("ElDiana.DrawComboDamage", new CheckBox("Draw combo damage"));

            DrawDamage.DamageToUnit = GetComboDamage;
            DrawDamage.Enabled = getCheckBoxItem(miscomboMenu, "ElDiana.DrawComboDamage");
            DrawDamage.FillColor = Color.Red;

            Console.WriteLine(Resources.Diana_Initialize_Menu_Loaded);
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            var drawOff = getCheckBoxItem(miscomboMenu, "ElDiana.Draw.off");
            var drawQ = getCheckBoxItem(miscomboMenu, "ElDiana.Draw.Q");
            var drawW = getCheckBoxItem(miscomboMenu, "ElDiana.Draw.W");
            var drawE = getCheckBoxItem(miscomboMenu, "ElDiana.Draw.E");
            var drawR = getCheckBoxItem(miscomboMenu, "ElDiana.Draw.R");
            var drawRMisaya = getCheckBoxItem(miscomboMenu, "ElDiana.Draw.RMisaya");
            var misayaRange = getSliderItem(comboMenu, "ElDiana.Combo.R.MisayaMinRange");

            if (drawOff)
            {
                return;
            }

            if (drawQ)
            {
                if (spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.Q].Range, Color.White);
                }
            }

            if (drawE)
            {
                if (spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.E].Range, Color.White);
                }
            }

            if (drawW)
            {
                if (spells[Spells.W].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.W].Range, Color.White);
                }
            }

            if (drawR)
            {
                if (spells[Spells.R].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.R].Range, Color.White);
                }
            }

            if (drawRMisaya)
            {
                if (spells[Spells.R].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, misayaRange, Color.White);
                }
            }
        }

        public static void OnLoad()
        {
            if (ObjectManager.Player.CharData.BaseSkinName != "Diana")
            {
                return;
            }

            spells[Spells.Q].SetSkillshot(0.25f, 150f, 1400f, false, SkillshotType.SkillshotCircle);
            ignite = Player.GetSpellSlot("summonerdot");

            Initialize();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;

            Interrupter2.OnInterruptableTarget += (source, eventArgs) =>
            {
                var eSlot = spells[Spells.E];
                if (getCheckBoxItem(interruptMenu, "ElDiana.Interrupt.UseEInterrupt") && eSlot.IsReady() &&
                    eSlot.Range >= Player.Distance(source))
                {
                    eSlot.Cast();
                }
            };

            CustomEvents.Unit.OnDash += (source, eventArgs) =>
            {
                if (!source.IsEnemy)
                {
                    return;
                }
                var eSlot = spells[Spells.E];
                var dis = Player.Distance(source);
                Console.WriteLine(source.Name + @" > " + eSlot.Range + @" : " + dis);
                if (!eventArgs.IsBlink && getCheckBoxItem(interruptMenu, "ElDiana.Interrupt.UseEDashes") &&
                    eSlot.IsReady() && eSlot.Range >= dis)
                {
                    eSlot.Cast();
                }
            };
        }

        #endregion

        #region Methods

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, DamageType.Magical);
            if (target == null || !target.IsValid)
            {
                return;
            }

            var useQ = getCheckBoxItem(comboMenu, "ElDiana.Combo.Q");
            var useW = getCheckBoxItem(comboMenu, "ElDiana.Combo.W");
            var useE = getCheckBoxItem(comboMenu, "ElDiana.Combo.E");
            var useR = getCheckBoxItem(comboMenu, "ElDiana.Combo.R");
            var useIgnite = getCheckBoxItem(comboMenu, "ElDiana.Combo.Ignite");
            var secondR = getCheckBoxItem(comboMenu, "ElDiana.Combo.Secure");
            var useSecondRLimitation = getSliderItem(comboMenu, "ElDiana.Combo.UseSecondRLimitation");
            var minHpToDive = getSliderItem(comboMenu, "ElDiana.Combo.R.PreventUnderTower");

            if (useQ && spells[Spells.Q].IsReady() && Player.Distance(target) <= spells[Spells.Q].Range)
            {
                var pred = spells[Spells.Q].GetPrediction(target);
                if (pred.Hitchance >= HitChance.VeryHigh)
                {
                    spells[Spells.Q].Cast(pred.CastPosition);
                }
            }

            if (useR && spells[Spells.R].IsReady() && target.IsValidTarget(spells[Spells.R].Range)
                && target.HasBuff("dianamoonlight")
                && (!target.UnderTurret(true) || (minHpToDive <= Player.HealthPercent)))
            {
                spells[Spells.R].Cast(target);
            }

            if (useW && spells[Spells.W].IsReady() && target.IsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast();
            }

            if (useE && spells[Spells.E].IsReady() && target.IsValidTarget(400f))
            {
                spells[Spells.E].Cast();
            }

            if (secondR && (!target.UnderTurret(true) || (minHpToDive <= Player.HealthPercent)))
            {
                var closeEnemies = Player.GetEnemiesInRange(spells[Spells.R].Range*2).Count;

                if (closeEnemies <= useSecondRLimitation && useR && !spells[Spells.Q].IsReady()
                    && spells[Spells.R].IsReady())
                {
                    if (target.Health < spells[Spells.R].GetDamage(target)
                        && (!target.UnderTurret(true) || (minHpToDive <= Player.HealthPercent)))
                    {
                        spells[Spells.R].Cast(target);
                    }
                }

                if (closeEnemies <= useSecondRLimitation && spells[Spells.R].IsReady())
                {
                    if (target.Health < spells[Spells.R].GetDamage(target))
                    {
                        spells[Spells.R].Cast(target);
                    }
                }
            }

            if (Player.Distance(target) <= 600 && IgniteDamage(target) >= target.Health && useIgnite)
            {
                Player.Spellbook.CastSpell(ignite, target);
            }
        }

        private static HitChance GetHitchance()
        {
            switch (getSliderItem(comboMenu, "ElDiana.hitChance"))
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.VeryHigh;
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, DamageType.Physical);
            if (target == null || !target.IsValid)
            {
                return;
            }

            var useQ = getCheckBoxItem(harassMenu, "ElDiana.Harass.Q");
            var useW = getCheckBoxItem(harassMenu, "ElDiana.Harass.W");
            var useE = getCheckBoxItem(harassMenu, "ElDiana.Harass.E");
            var checkMana = getSliderItem(harassMenu, "ElDiana.Harass.Mana");

            if (Player.ManaPercent < checkMana)
            {
                return;
            }

            if (useQ && spells[Spells.Q].IsReady() && spells[Spells.Q].IsInRange(target))
            {
                var pred = spells[Spells.Q].GetPrediction(target);
                if (pred.Hitchance >= CustomHitChance)
                {
                    spells[Spells.Q].Cast(target);
                }
            }

            if (useW && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(target))
            {
                spells[Spells.W].Cast();
            }

            if (useE && spells[Spells.E].IsReady() && Player.Distance(target) <= spells[Spells.E].Range)
            {
                spells[Spells.E].Cast();
            }
        }

        private static float IgniteDamage(AIHeroClient target)
        {
            if (ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float) Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private static void JungleClear()
        {
            var minions = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                spells[Spells.Q].Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            var useQ = getCheckBoxItem(jungleClearMenu, "ElDiana.JungleClear.Q");
            var useW = getCheckBoxItem(jungleClearMenu, "ElDiana.JungleClear.W");
            var useE = getCheckBoxItem(jungleClearMenu, "ElDiana.JungleClear.E");
            var useR = getCheckBoxItem(jungleClearMenu, "ElDiana.JungleClear.R");

            var qMinions = minions.FindAll(minion => minion.IsValidTarget(spells[Spells.Q].Range));
            var qMinion = qMinions.FirstOrDefault();

            if (qMinion == null)
            {
                return;
            }

            if (useQ && spells[Spells.Q].IsReady())
            {
                if (qMinion.IsValidTarget())
                {
                    spells[Spells.Q].Cast(qMinion);
                }
            }

            if (useW && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(qMinion))
            {
                spells[Spells.W].Cast();
            }

            if (useE && spells[Spells.E].IsReady()
                && qMinions.Count(m => Player.Distance(m) < spells[Spells.W].Range) < 1 &&
                spells[Spells.E].IsInRange(qMinion))
            {
                spells[Spells.E].Cast();
            }

            if (useR && spells[Spells.R].IsReady())
            {
                //find Mob with moonlight buff
                var moonlightMob =
                    minions.FindAll(minion => minion.HasBuff("dianamoonlight")).OrderBy(minion => minion.HealthPercent);
                if (moonlightMob.Any())
                {
                    //only cast when killable
                    var canBeKilled = moonlightMob.Find(minion => minion.Health < spells[Spells.R].GetDamage(minion));

                    //cast R on mob that can be killed
                    if (canBeKilled.IsValidTarget())
                    {
                        spells[Spells.R].Cast(canBeKilled);
                    }
                }
            }
        }

        private static void LaneClear()
        {
            var minion =
                MinionManager.GetMinions(ObjectManager.Player.ServerPosition, spells[Spells.Q].Range).FirstOrDefault();
            if (minion == null || minion.Name.ToLower().Contains("ward"))
            {
                return;
            }

            var useQ = getCheckBoxItem(laneclearMenu, "ElDiana.LaneClear.Q");
            var useW = getCheckBoxItem(laneclearMenu, "ElDiana.LaneClear.W");
            var useE = getCheckBoxItem(laneclearMenu, "ElDiana.LaneClear.E");
            var useR = getCheckBoxItem(laneclearMenu, "ElDiana.LaneClear.R");

            var countQ = getSliderItem(laneclearMenu, "ElDiana.LaneClear.Count.Minions.Q");
            var countW = getSliderItem(laneclearMenu, "ElDiana.LaneClear.Count.Minions.W");
            var countE = getSliderItem(laneclearMenu, "ElDiana.LaneClear.Count.Minions.E");

            var minions = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                spells[Spells.Q].Range,
                MinionTypes.All,
                MinionTeam.NotAlly);

            var qMinions = minions.FindAll(minionQ => minion.IsValidTarget(spells[Spells.Q].Range));
            var qMinion = qMinions.Find(minionQ => minionQ.IsValidTarget());

            if (useQ && spells[Spells.Q].IsReady()
                && spells[Spells.Q].GetCircularFarmLocation(minions).MinionsHit >= countQ)
            {
                spells[Spells.Q].Cast(qMinion);
            }

            if (useW && spells[Spells.W].IsReady()
                && spells[Spells.W].GetCircularFarmLocation(minions).MinionsHit >= countW)
            {
                spells[Spells.W].Cast();
            }

            if (useE && spells[Spells.E].IsReady() && Player.Distance(qMinion) < 200
                && spells[Spells.E].GetCircularFarmLocation(minions).MinionsHit >= countE)
            {
                spells[Spells.E].Cast();
            }

            var minionsR = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                spells[Spells.Q].Range,
                MinionTypes.All,
                MinionTeam.NotAlly,
                MinionOrderTypes.MaxHealth);

            if (useR && spells[Spells.R].IsReady())
            {
                //find Mob with moonlight buff
                var moonlightMob = minionsR.FindAll(x => x.HasBuff("dianamoonlight")).OrderBy(x => minion.HealthPercent);
                if (moonlightMob.Any())
                {
                    //only cast when killable
                    var canBeKilled = moonlightMob.Find(x => minion.Health < spells[Spells.R].GetDamage(minion));

                    //cast R on mob that can be killed
                    if (canBeKilled.IsValidTarget())
                    {
                        spells[Spells.R].Cast(canBeKilled);
                    }
                }
            }
        }

        private static void MisayaCombo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, DamageType.Magical);
            if (target == null || !target.IsValid)
            {
                return;
            }

            var minHpToDive = getSliderItem(comboMenu, "ElDiana.Combo.R.PreventUnderTower");

            var useQ = getCheckBoxItem(comboMenu, "ElDiana.Combo.Q");
            var useW = getCheckBoxItem(comboMenu, "ElDiana.Combo.W");
            var useE = getCheckBoxItem(comboMenu, "ElDiana.Combo.E");
            var useR = getCheckBoxItem(comboMenu, "ElDiana.Combo.R") &&
                       (!target.UnderTurret(true) || (minHpToDive <= Player.HealthPercent));

            var useIgnite = getCheckBoxItem(comboMenu, "ElDiana.Combo.Ignite");

            var secondR = getCheckBoxItem(comboMenu, "ElDiana.Combo.Secure") &&
                          (!target.UnderTurret(true) || (minHpToDive <= Player.HealthPercent));

            var distToTarget = Player.Distance(target);

            var misayaMinRange = getSliderItem(comboMenu, "ElDiana.Combo.R.MisayaMinRange");
            var useSecondRLimitation = getSliderItem(comboMenu, "ElDiana.Combo.UseSecondRLimitation");

            // Can use R, R is ready but player too far from the target => do nothing
            if (useR && spells[Spells.R].IsReady() && distToTarget > spells[Spells.R].Range)
            {
                return;
            }

            // Prerequisites for Misaya Combo : If target is too close, won't work
            if (useQ && useR && spells[Spells.Q].IsReady() && spells[Spells.R].IsReady()
                && distToTarget >= misayaMinRange)
            {
                spells[Spells.R].Cast(target);
                // No need to check the hitchance since R is a targeted dash.
                spells[Spells.Q].Cast(target);
            }

            // Misaya Combo is not possible, classic mode then

            if (useQ && spells[Spells.Q].IsReady() && spells[Spells.Q].IsInRange(target))
            {
                var pred = spells[Spells.Q].GetPrediction(target);
                if (pred.Hitchance >= CustomHitChance)
                {
                    spells[Spells.Q].Cast(target);
                }
            }

            if (useR && spells[Spells.R].IsReady() && spells[Spells.R].IsInRange(target)
                && target.HasBuff("dianamoonlight"))
            {
                spells[Spells.R].Cast(target);
            }

            if (useW && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(target))
            {
                spells[Spells.W].Cast();
            }

            if (useE && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target))
            {
                var pred = spells[Spells.E].GetPrediction(target);
                if (pred.Hitchance >= CustomHitChance)
                {
                    spells[Spells.E].Cast();
                }
            }

            if (secondR)
            {
                var closeEnemies = Player.GetEnemiesInRange(spells[Spells.R].Range*2).Count;

                if (closeEnemies <= useSecondRLimitation && useR && !spells[Spells.Q].IsReady()
                    && spells[Spells.R].IsReady())
                {
                    if (target.Health < spells[Spells.R].GetDamage(target))
                    {
                        spells[Spells.R].Cast(target);
                    }
                }

                if (closeEnemies <= useSecondRLimitation && spells[Spells.R].IsReady())
                {
                    if (target.Health < spells[Spells.R].GetDamage(target))
                    {
                        spells[Spells.R].Cast(target);
                    }
                }
            }

            if (Player.Distance(target) <= 600 && IgniteDamage(target) >= target.Health && useIgnite)
            {
                Player.Spellbook.CastSpell(ignite, target);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                var ultType = getSliderItem(comboMenu, "ElDiana.Combo.R.Mode");
                switch (ultType)
                {
                    case 0:
                        Combo();
                        break;

                    case 1:
                        MisayaCombo();
                        break;
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                LaneClear();
                JungleClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
        }

        #endregion
    }
}