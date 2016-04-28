using System;
using System.Collections.Generic;
using System.Drawing;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using Damage = LeagueSharp.Common.Damage;
using Spell = LeagueSharp.Common.Spell;

namespace ElEasy.Plugins
{
    public class Leona
    {
        #region Enums

        public enum Spells
        {
            Q,

            W,

            E,

            R
        }

        #endregion

        #region Static Fields

        private static readonly Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
        {
            {Spells.Q, new Spell(SpellSlot.Q, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100)},
            {Spells.W, new Spell(SpellSlot.W, 200)},
            {Spells.E, new Spell(SpellSlot.E, 700)},
            {Spells.R, new Spell(SpellSlot.R, 1200)}
        };

        private static SpellSlot Ignite;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public static Menu rootMenu, comboMenu, harassMenu, settingsMenu, miscellaneousMenu;

        public void CreateMenu()
        {
            rootMenu = MainMenu.AddMenu("ElLeona", "ElLeona");

            comboMenu = rootMenu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("ElEasy.Leona.Combo.Q", new CheckBox("Use Q"));
            comboMenu.Add("ElEasy.Leona.Combo.W", new CheckBox("Use W"));
            comboMenu.Add("ElEasy.Leona.Combo.E", new CheckBox("Use E"));
            comboMenu.Add("ElEasy.Leona.Combo.R", new CheckBox("Use R", false));
            comboMenu.Add("ElEasy.Leona.Combo.Count.Enemies", new Slider("Enemies in range for R", 2, 1, 5));
            comboMenu.Add("ElEasy.Leona.Hitchance", new ComboBox("Hitchance", 3, "Low", "Medium", "High", "Very High"));
            comboMenu.Add("ElEasy.Leona.Combo.Ignite", new CheckBox("Use Ignite"));

            harassMenu = rootMenu.AddSubMenu("Harass", "Harass");
            harassMenu.Add("ElEasy.Leona.Harass.Q", new CheckBox("Use Q"));
            harassMenu.Add("ElEasy.Leona.Harass.E", new CheckBox("Use E"));
            harassMenu.Add("ElEasy.Leona.Harass.Player.Mana", new Slider("Minimum Mana", 55));

            settingsMenu = rootMenu.AddSubMenu("Settings", "Settings");
            settingsMenu.Add("ElEasy.Leona.Interrupt.Activated", new CheckBox("Interrupt spells"));
            settingsMenu.Add("ElEasy.Leona.GapCloser.Activated", new CheckBox("Anti gapcloser"));

            miscellaneousMenu = rootMenu.AddSubMenu("Miscellaneous", "Miscellaneous");
            miscellaneousMenu.Add("ElEasy.Leona.Draw.off", new CheckBox("Turn drawings off"));
            miscellaneousMenu.Add("ElEasy.Leona.Draw.Q", new CheckBox("Draw Q"));
            miscellaneousMenu.Add("ElEasy.Leona.Draw.W", new CheckBox("Draw W"));
            miscellaneousMenu.Add("ElEasy.Leona.Draw.E", new CheckBox("Draw E"));
            miscellaneousMenu.Add("ElEasy.Leona.Draw.R", new CheckBox("Draw R"));
        }

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

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        public Leona()
        {
            Console.WriteLine("Loaded Leona");
            CreateMenu();
            Ignite = Player.GetSpellSlot("summonerdot");
            spells[Spells.E].SetSkillshot(0.25f, 120f, 2000f, false, SkillshotType.SkillshotLine);
            spells[Spells.R].SetSkillshot(1f, 300f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }

        #endregion

        #region Methods

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var gapCloserActive = getCheckBoxItem(settingsMenu, "ElEasy.Leona.Interrupt.Activated");

            if (gapCloserActive && spells[Spells.Q].IsReady()
                && gapcloser.Sender.Distance(Player) < spells[Spells.Q].Range)
            {
                spells[Spells.Q].Cast();
            }
        }

        private HitChance GetHitchance()
        {
            switch (getBoxItem(comboMenu, "ElEasy.Leona.Hitchance"))
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
                    return HitChance.Medium;
            }
        }

        private float IgniteDamage(AIHeroClient target)
        {
            if (Ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float) Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private void Interrupter2_OnInterruptableTarget(
            AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (args.DangerLevel != Interrupter2.DangerLevel.High
                || sender.Distance(Player) > spells[Spells.R].Range)
            {
                return;
            }

            if (sender.IsValidTarget(spells[Spells.R].Range) && args.DangerLevel == Interrupter2.DangerLevel.High
                && spells[Spells.R].IsReady())
            {
                spells[Spells.R].Cast(sender);
            }
        }

        private void OnCombo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.E].Range, DamageType.Magical);
            if (target == null || !target.IsValid)
            {
                return;
            }

            var useQ = getCheckBoxItem(comboMenu, "ElEasy.Leona.Combo.Q");
            var useW = getCheckBoxItem(comboMenu, "ElEasy.Leona.Combo.W");
            var useE = getCheckBoxItem(comboMenu, "ElEasy.Leona.Combo.E");
            var useR = getCheckBoxItem(comboMenu, "ElEasy.Leona.Combo.R");
            var useI = getCheckBoxItem(comboMenu, "ElEasy.Leona.Combo.Ignite");
            var countEnemies = getSliderItem(comboMenu, "ElEasy.Leona.Combo.Count.Enemies");

            if (useQ)
            {
                //check aa range
                if (spells[Spells.Q].IsReady() && !target.HasBuff("BlackShield") || !target.HasBuff("SivirShield")
                    || !target.HasBuff("BansheesVeil") || !target.HasBuff("ShroudofDarkness"))
                {
                    spells[Spells.Q].Cast();
                }
            }

            if (useW && spells[Spells.W].IsReady() && target.IsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast();
            }

            if (useE && spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
            {
                var pred = spells[Spells.E].GetPrediction(target).Hitchance;
                if (pred >= HitChance.VeryHigh)
                {
                    spells[Spells.E].Cast(target);
                }
            }

            if (useR && spells[Spells.R].IsReady() && target.IsValidTarget(spells[Spells.R].Range)
                && Player.CountEnemiesInRange(spells[Spells.R].Range) >= countEnemies)
            {
                var pred = spells[Spells.R].GetPrediction(target);
                if (pred.Hitchance >= HitChance.VeryHigh)
                {
                    spells[Spells.R].CastIfHitchanceEquals(target, HitChance.Immobile);
                }
            }

            if (target.IsValidTarget(600) && IgniteDamage(target) >= target.Health && useI)
            {
                Player.Spellbook.CastSpell(Ignite, target);
            }
        }

        private void OnDraw(EventArgs args)
        {
            var drawOff = getCheckBoxItem(miscellaneousMenu, "ElEasy.Leona.Draw.off");
            var drawQ = getCheckBoxItem(miscellaneousMenu, "ElEasy.Leona.Draw.Q");
            var drawE = getCheckBoxItem(miscellaneousMenu, "ElEasy.Leona.Draw.E");
            var drawW = getCheckBoxItem(miscellaneousMenu, "ElEasy.Leona.Draw.W");
            var drawR = getCheckBoxItem(miscellaneousMenu, "ElEasy.Leona.Draw.R");

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
                    Render.Circle.DrawCircle(Player.Position, spells[Spells.E].Range, Color.White);
                }
            }

            if (drawW)
            {
                if (spells[Spells.W].Level > 0)
                {
                    Render.Circle.DrawCircle(Player.Position, spells[Spells.W].Range, Color.White);
                }
            }

            if (drawR)
            {
                if (spells[Spells.R].Level > 0)
                {
                    Render.Circle.DrawCircle(Player.Position, spells[Spells.R].Range, Color.White);
                }
            }
        }

        private void OnHarass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.E].Range, DamageType.Magical);
            if (target == null || !target.IsValid)
            {
                return;
            }

            var useQ = getCheckBoxItem(harassMenu, "ElEasy.Leona.Harass.Q");
            var useE = getCheckBoxItem(harassMenu, "ElEasy.Leona.Harass.E");
            var playerMana = getSliderItem(harassMenu, "ElEasy.Leona.Harass.Player.Mana");

            if (Player.ManaPercent < playerMana)
            {
                return;
            }

            if (useQ && spells[Spells.Q].IsReady())
            {
                spells[Spells.Q].Cast();
            }

            if (useE && spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast(target);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                OnCombo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                OnHarass();
            }
        }

        #endregion
    }
}