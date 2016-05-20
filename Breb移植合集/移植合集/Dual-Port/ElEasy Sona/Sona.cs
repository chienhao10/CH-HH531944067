namespace ElEasy.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    internal class Sona
    {
        #region Static Fields

        private static readonly Dictionary<Spells, LeagueSharp.Common.Spell> spells = new Dictionary<Spells, LeagueSharp.Common.Spell>
                                                                       {
                                                                           { Spells.Q, new LeagueSharp.Common.Spell(SpellSlot.Q, 850) },
                                                                           { Spells.W, new LeagueSharp.Common.Spell(SpellSlot.W, 1000) },
                                                                           { Spells.E, new LeagueSharp.Common.Spell(SpellSlot.E, 350) },
                                                                           { Spells.R, new LeagueSharp.Common.Spell(SpellSlot.R, 1000) }
                                                                       };

        private static SpellSlot Ignite;

        #endregion

        #region Enums

        public enum Spells
        {
            Q,

            W,

            E,

            R
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        private Menu Menu { get; set; }

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        /// 


        public static Menu comboMenu, harassMenu, healMenu, miscellaneousMenu;
        public void CreateMenu()
        {
            this.Menu = MainMenu.AddMenu("ElSona", "ElSona");

            comboMenu = this.Menu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("ElEasy.Sona.Combo.Q", new CheckBox("Use Q"));
            comboMenu.Add("ElEasy.Sona.Combo.W", new CheckBox("Use W"));
            comboMenu.Add("ElEasy.Sona.Combo.E", new CheckBox("Use E"));
            comboMenu.Add("ElEasy.Sona.Combo.R", new CheckBox("Use R"));
            comboMenu.Add("ElEasy.Sona.Combo.Count.R", new Slider("Minimum hit by R", 2, 1, 5));
            comboMenu.Add("ElEasy.Sona.Combo.Ignite", new CheckBox("Use Ignite"));

            harassMenu = this.Menu.AddSubMenu("Harass", "Harass");
            harassMenu.Add("ElEasy.Sona.Harass.Q", new CheckBox("Use Q"));
            harassMenu.Add("ElEasy.Sona.Harass.Player.Mana", new Slider("Minimum Mana", 55));
            harassMenu.Add("ElEasy.Sona.Autoharass.Activated", new KeyBind("Autoharass", false, KeyBind.BindTypes.PressToggle, 'L'));

            healMenu = this.Menu.AddSubMenu("Heal", "Heal");
            healMenu.Add("ElEasy.Sona.Heal.Activated", new CheckBox("Heal"));
            healMenu.Add("ElEasy.Sona.Heal.Player.HP", new Slider("Player HP", 55));
            healMenu.Add("ElEasy.Sona.Heal.Ally.HP", new Slider("Ally HP", 55));
            healMenu.Add("ElEasy.Sona.Heal.Player.Mana", new Slider("Minimum Mana", 55));

            miscellaneousMenu = this.Menu.AddSubMenu("Miscellaneous", "Miscellaneous");
            miscellaneousMenu.Add("ElEasy.Sona.Interrupt.Activated", new CheckBox("Interrupt spells"));
            miscellaneousMenu.Add("ElEasy.Sona.GapCloser.Activated", new CheckBox("Anti gapcloser"));
            miscellaneousMenu.Add("ElEasy.Sona.Draw.off", new CheckBox("Turn drawings off"));
            miscellaneousMenu.Add("ElEasy.Sona.Draw.Q", new CheckBox("Draw Q"));
            miscellaneousMenu.Add("ElEasy.Sona.Draw.W", new CheckBox("Draw W"));
            miscellaneousMenu.Add("ElEasy.Sona.Draw.E", new CheckBox("Draw E"));
            miscellaneousMenu.Add("ElEasy.Sona.Draw.R", new CheckBox("Draw R"));
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

        public void Load()
        {
            CreateMenu();
            Console.WriteLine("Loaded Sona");
            Ignite = this.Player.GetSpellSlot("summonerdot");

            spells[Spells.R].SetSkillshot(0.5f, 125, 3000f, false, SkillshotType.SkillshotLine);

            Game.OnUpdate += this.OnUpdate;
            Drawing.OnDraw += this.OnDraw;
            AntiGapcloser.OnEnemyGapcloser += this.AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += this.Interrupter2_OnInterruptableTarget;
        }

        #endregion

        #region Methods

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (getCheckBoxItem(miscellaneousMenu, "ElEasy.Sona.GapCloser.Activated") && spells[Spells.R].IsReady()
                && gapcloser.Sender.IsValidTarget(spells[Spells.R].Range))
            {
                spells[Spells.R].Cast(gapcloser.Sender);
            }
        }

        private void AutoHarass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, DamageType.Magical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            if (getKeyBindItem(harassMenu, "ElEasy.Sona.Autoharass.Activated")
                && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast(target);
            }
        }

        private void HealManager()
        {
            var useHeal = getCheckBoxItem(healMenu, "ElEasy.Sona.Heal.Activated");
            var playerMana = getSliderItem(healMenu, "ElEasy.Sona.Heal.Player.Mana");
            var playerHp = getSliderItem(healMenu, "ElEasy.Sona.Heal.Player.HP");
            var allyHp = getSliderItem(healMenu, "ElEasy.Sona.Heal.Ally.HP");

            if (this.Player.IsRecalling() || this.Player.InFountain() || !useHeal
                || this.Player.ManaPercent < playerMana || !spells[Spells.W].IsReady())
            {
                return;
            }

            if ((this.Player.Health / this.Player.MaxHealth) * 100 <= playerHp)
            {
                spells[Spells.W].Cast();
            }

            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(h => h.IsAlly && !h.IsMe))
            {
                if ((hero.Health / hero.MaxHealth) * 100 <= allyHp && spells[Spells.W].IsInRange(hero))
                {
                    spells[Spells.W].Cast();
                }
            }
        }

        private float IgniteDamage(AIHeroClient target)
        {
            if (Ignite == SpellSlot.Unknown || this.Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)this.Player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
        }

        private void Interrupter2_OnInterruptableTarget(
            AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (args.DangerLevel != Interrupter2.DangerLevel.High
                || sender.Distance(this.Player) > spells[Spells.R].Range)
            {
                return;
            }

            if (sender.IsValidTarget(spells[Spells.R].Range) && args.DangerLevel == Interrupter2.DangerLevel.High
                && spells[Spells.R].IsReady())
            {
                spells[Spells.R].Cast(sender.Position);
            }
        }

        private void OnCombo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, DamageType.Magical);
            var rTarget = TargetSelector.GetTarget(spells[Spells.R].Range, DamageType.Magical);

            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            var useQ = getCheckBoxItem(comboMenu, "ElEasy.Sona.Combo.Q");
            var useW = getCheckBoxItem(comboMenu, "ElEasy.Sona.Combo.W");
            var useE = getCheckBoxItem(comboMenu, "ElEasy.Sona.Combo.E");
            var useR = getCheckBoxItem(comboMenu, "ElEasy.Sona.Combo.R");
            var useI = getCheckBoxItem(comboMenu, "ElEasy.Sona.Combo.Ignite");
            var hitByR = getSliderItem(comboMenu, "ElEasy.Sona.Combo.Count.R");

            if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast(target);
            }

            if (useE && spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast();
            }

            if (useW && spells[Spells.W].IsReady() && target.IsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast();
            }

            if (useR && spells[Spells.R].IsReady() && rTarget.IsValidTarget(spells[Spells.R].Range))
            {
                var pred = spells[Spells.R].GetPrediction(target);
                if (pred.Hitchance >= HitChance.High)
                {
                    var hits = HeroManager.Enemies.Where(x => x.Distance(target) <= spells[Spells.R].Width).ToList();
                    Console.WriteLine(hits.Count);
                    if (hits.Any(hit => hits.Count >= hitByR))
                    {
                        spells[Spells.R].Cast(pred.CastPosition);
                    }
                }
            }

            if (this.Player.Distance(target) <= 600 && this.IgniteDamage(target) >= target.Health && useI)
            {
                this.Player.Spellbook.CastSpell(Ignite, target);
            }
        }

        private void OnDraw(EventArgs args)
        {
            var drawOff = getCheckBoxItem(miscellaneousMenu, "ElEasy.Sona.Draw.off");
            var drawQ = getCheckBoxItem(miscellaneousMenu, "ElEasy.Sona.Draw.Q");
            var drawW = getCheckBoxItem(miscellaneousMenu, "ElEasy.Sona.Draw.W");
            var drawE = getCheckBoxItem(miscellaneousMenu, "ElEasy.Sona.Draw.E");
            var drawR = getCheckBoxItem(miscellaneousMenu, "ElEasy.Sona.Draw.R");

            if (drawOff)
            {
                return;
            }

            if (drawQ)
            {
                if (spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(this.Player.Position, spells[Spells.Q].Range, Color.White);
                }
            }

            if (drawE)
            {
                if (spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(this.Player.Position, spells[Spells.E].Range, Color.White);
                }
            }

            if (drawW)
            {
                if (spells[Spells.W].Level > 0)
                {
                    Render.Circle.DrawCircle(this.Player.Position, spells[Spells.W].Range, Color.White);
                }
            }

            if (drawR)
            {
                if (spells[Spells.W].Level > 0)
                {
                    Render.Circle.DrawCircle(this.Player.Position, spells[Spells.R].Range, Color.White);
                }
            }
        }

        private void OnHarass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, DamageType.Magical);
            if (target == null || !target.IsValid)
            {
                return;
            }

            if (this.Player.ManaPercent < getSliderItem(harassMenu, "ElEasy.Sona.Harass.Player.Mana"))
            {
                return;
            }

            if (getCheckBoxItem(harassMenu, "ElEasy.Sona.Harass.Q") && spells[Spells.Q].IsReady()
                && target.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast(target);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (this.Player.IsDead)
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                this.OnCombo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                this.OnHarass();
            }

            this.HealManager();
            this.AutoHarass();
        }

        #endregion
    }
}