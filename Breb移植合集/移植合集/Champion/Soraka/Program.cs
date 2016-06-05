// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SophiesSoraka.cs" company="ChewyMoon">
//   Copyright (C) 2015 ChewyMoon
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   The sophies soraka.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using Spell = LeagueSharp.Common.Spell;

namespace Sophies_Soraka
{
    /// <summary>
    ///     The sophies soraka.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Reviewed. Suppression is OK here.")]
    internal class SophiesSoraka
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the e.
        /// </summary>
        /// <value>
        ///     The e.
        /// </value>
        public static Spell E { get; set; }

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        public static Menu Menu { get; set; }

        /// <summary>
        ///     Gets a value indicating whether to use packets.
        /// </summary>
        public static bool Packets
        {
            get { return false; }
        }

        /// <summary>
        ///     Gets or sets the q.
        /// </summary>
        /// <value>
        ///     The q.
        /// </value>
        public static Spell Q { get; set; }

        /// <summary>
        ///     Gets or sets the r.
        /// </summary>
        /// <value>
        ///     The r.
        /// </value>
        public static Spell R { get; set; }

        /// <summary>
        ///     Gets or sets the w.
        /// </summary>
        /// <value>
        ///     The w.
        /// </value>
        public static Spell W { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The on game load.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        public static void OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != "Soraka")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 750);
            W = new Spell(SpellSlot.W, 550);
            E = new Spell(SpellSlot.E, 900);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.3f, 125, 1750, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.4f, 70f, 1750, false, SkillshotType.SkillshotCircle);

            CreateMenu();

            PrintChat("loaded");

            Interrupter2.OnInterruptableTarget += InterrupterOnOnPossibleToInterrupt;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloserOnOnEnemyGapcloser;
            Game.OnUpdate += GameOnOnGameUpdate;
            Drawing.OnDraw += DrawingOnOnDraw;
            Orbwalker.OnPreAttack += OrbwalkingOnBeforeAttack;
        }

        /// <summary>
        ///     Prints to the chat.
        /// </summary>
        /// <param name="msg">The message.</param>
        public static void PrintChat(string msg)
        {
            Chat.Print("<font color='#3492EB'>Sophie's Soraka:</font> <font color='#FFFFFF'>" + msg + "</font>");
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The on enemy gapcloser event.
        /// </summary>
        /// <param name="gapcloser">
        ///     The gapcloser.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        private static void AntiGapcloserOnOnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var unit = gapcloser.Sender;

            if (getCheckBoxItem(miscMenu, "useQGapcloser") && unit.IsValidTarget(Q.Range) && Q.IsReady() && unit.IsEnemy)
            {
                Q.Cast(unit, Packets);
            }

            if (getCheckBoxItem(miscMenu, "useEGapcloser") && unit.IsValidTarget(E.Range) && E.IsReady() && unit.IsEnemy)
            {
                E.Cast(unit, Packets);
            }
        }

        /// <summary>
        ///     Automatics the ultimate.
        /// </summary>
        private static void AutoR()
        {
            if (!R.IsReady())
            {
                return;
            }

            if (ObjectManager.Get<AIHeroClient>()
                    .Any(
                        x =>
                        x.IsAlly && x.IsValidTarget(float.MaxValue, false)
                        && x.HealthPercent < getSliderItem(healMenu, "autoRPercent")))
            {
                R.Cast();
            }
        }

        /// <summary>
        ///     Automatics the W heal.
        /// </summary>
        private static void AutoW()
        {
            if (!W.IsReady())
            {
                return;
            }

            var autoWHealth = getSliderItem(healMenu, "autoWHealth");
            if (ObjectManager.Player.HealthPercent < autoWHealth)
            {
                return;
            }

            var dontWInFountain = getCheckBoxItem(healMenu, "DontWInFountain");
            if (dontWInFountain && ObjectManager.Player.InFountain())
            {
                return;
            }

            var healthPercent = getSliderItem(healMenu, "autoWPercent");

            var canidates =
                ObjectManager.Get<AIHeroClient>()
                    .Where(x => x.IsValidTarget(W.Range) && x.IsAlly && x.HealthPercent < healthPercent);
            var wMode = getBoxItem(healMenu, "HealingPriority");

            switch (wMode)
            {
                case 0:
                    canidates = canidates.OrderByDescending(x => x.TotalAttackDamage);
                    break;
                case 1:
                    canidates = canidates.OrderByDescending(x => x.TotalMagicalDamage);
                    break;
                case 2:
                    canidates = canidates.OrderBy(x => x.Health);
                    break;
                case 3:
                    canidates = canidates.OrderBy(x => x.Health).ThenBy(x => x.MaxHealth);
                    break;
            }

            var target = dontWInFountain ? canidates.FirstOrDefault(x => !x.InFountain()) : canidates.FirstOrDefault();

            if (target != null)
            {
                W.CastOnUnit(target);
            }
        }

        /// <summary>
        ///     The combo.
        /// </summary>
        private static void Combo()
        {
            var useQ = getCheckBoxItem(comboMenu, "useQ");
            var useE = getCheckBoxItem(comboMenu, "useE");
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (target == null)
            {
                return;
            }

            if (useQ && Q.IsReady())
            {
                Q.Cast(target, Packets);
            }

            if (useE && E.IsReady())
            {
                E.Cast(target, Packets);
            }
        }

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        public static Menu comboMenu, harassMenu, healMenu, drawingMenu, miscMenu;

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

        private static void CreateMenu()
        {
            Menu = MainMenu.AddMenu("Sophies's Soraka", "sSoraka");

            // Combo
            comboMenu = Menu.AddSubMenu("Combo", "ssCombo");
            comboMenu.Add("useQ", new CheckBox("Use Q"));
            comboMenu.Add("useE", new CheckBox("Use E"));

            // Harass
            harassMenu = Menu.AddSubMenu("Harass", "ssHarass");
            harassMenu.Add("useQHarass", new CheckBox("Use Q"));
            harassMenu.Add("useEHarass", new CheckBox("Use E"));
            harassMenu.Add("HarassMana", new Slider("Harass Mana Percent", 50, 1));
            harassMenu.Add("HarassToggle", new KeyBind("Harass! (toggle)", false, KeyBind.BindTypes.PressToggle, 'T'));

            // Healing
            healMenu = Menu.AddSubMenu("Heal Settings", "HSettings");
            healMenu.AddGroupLabel("W Settings");
            healMenu.Add("autoW", new CheckBox("Use W"));
            healMenu.Add("autoWPercent", new Slider("Ally Health Percent", 50, 1));
            healMenu.Add("autoWHealth", new Slider("My Health Percent", 30, 1));
            healMenu.Add("DontWInFountain", new CheckBox("Dont W in Fountain"));
            healMenu.Add("HealingPriority",
                new ComboBox("Healing Priority", 3, "Most AD", "Most AP", "Least Health",
                    "Least Health (Prioritize Squishies)"));
            healMenu.AddSeparator();
            healMenu.AddGroupLabel("R Settings");
            healMenu.Add("autoR", new CheckBox("Use R"));
            healMenu.Add("autoRPercent", new Slider("% Percent", 15, 1));

            // Drawing
            drawingMenu = Menu.AddSubMenu("Drawing", "ssDrawing");
            drawingMenu.Add("drawQ", new CheckBox("Draw Q"));
            drawingMenu.Add("drawW", new CheckBox("Draw W"));
            drawingMenu.Add("drawE", new CheckBox("Draw E"));

            // Misc
            miscMenu = Menu.AddSubMenu("Misc", "ssMisc");
            miscMenu.Add("useQGapcloser", new CheckBox("Q on Gapcloser"));
            miscMenu.Add("useEGapcloser", new CheckBox("E on Gapcloser"));
            miscMenu.Add("eInterrupt", new CheckBox("Use E to Interrupt"));
            miscMenu.Add("AttackMinions", new CheckBox("Attack Minions", false));
            miscMenu.Add("AttackChampions", new CheckBox("Attack Champions"));
        }

        /// <summary>
        ///     The on draw event.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void DrawingOnOnDraw(EventArgs args)
        {
            var drawQ = getCheckBoxItem(drawingMenu, "drawQ");
            var drawW = getCheckBoxItem(drawingMenu, "drawW");
            var drawE = getCheckBoxItem(drawingMenu, "drawE");

            var p = ObjectManager.Player.Position;

            if (drawQ)
            {
                Render.Circle.DrawCircle(p, Q.Range, Q.IsReady() ? Color.Aqua : Color.Red);
            }

            if (drawW)
            {
                Render.Circle.DrawCircle(p, W.Range, W.IsReady() ? Color.Aqua : Color.Red);
            }

            if (drawE)
            {
                Render.Circle.DrawCircle(p, E.Range, E.IsReady() ? Color.Aqua : Color.Red);
            }
        }

        /// <summary>
        ///     The  on game update event.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void GameOnOnGameUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

            if (getKeyBindItem(harassMenu, "HarassToggle")
            && ObjectManager.Player.Mana > getSliderItem(harassMenu, "HarassMana"))
            {
                Harass();
            }

            if (getCheckBoxItem(healMenu, "autoW"))
            {
                AutoW();
            }

            if (getCheckBoxItem(healMenu, "autoR"))
            {
                AutoR();
            }
        }

        /// <summary>
        ///     The harass.
        /// </summary>
        private static void Harass()
        {
            var useQ = getCheckBoxItem(harassMenu, "useQHarass");
            var useE = getCheckBoxItem(harassMenu, "useEHarass");
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (target == null)
            {
                return;
            }

            if (useQ && Q.IsReady())
            {
                Q.Cast(target, Packets);
            }

            if (useE && E.IsReady())
            {
                E.Cast(target, Packets);
            }
        }

        /// <summary>
        ///     The on possible to interrupt event.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void InterrupterOnOnPossibleToInterrupt(
            AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            var unit = sender;
            var spell = args;

            if (getCheckBoxItem(miscMenu, "eInterrupt") == false || spell.DangerLevel != Interrupter2.DangerLevel.High)
            {
                return;
            }

            if (!unit.IsValidTarget(E.Range))
            {
                return;
            }

            if (!E.IsReady())
            {
                return;
            }

            E.Cast(unit, Packets);
        }

        /// <summary>
        ///     Called before the orbwalker attacks a unit.
        /// </summary>
        /// <param name="args">The <see cref="Orbwalking.BeforeAttackEventArgs" /> instance containing the event data.</param>
        private static void OrbwalkingOnBeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (args.Target.IsValid<Obj_AI_Minion>() &&
                (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit)) 
                && !getCheckBoxItem(miscMenu, "AttackMinions") && ObjectManager.Player.CountAlliesInRange(1200) > 0)
            {
                args.Process = false;
            }

            if (args.Target.IsValid<AIHeroClient>() && !getCheckBoxItem(miscMenu, "AttackChampions") && ObjectManager.Player.CountAlliesInRange(1000) > 0)
            {
                args.Process = false;
            }

        }

        #endregion
    }
}