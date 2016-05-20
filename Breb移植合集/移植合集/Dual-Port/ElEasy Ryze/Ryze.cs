namespace ElEasy.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    public class Ryze
    {
        #region Static Fields

        private static readonly Dictionary<Spells, LeagueSharp.Common.Spell> spells = new Dictionary<Spells, LeagueSharp.Common.Spell>
                                                                       {
                                                                           { Spells.Q, new LeagueSharp.Common.Spell(SpellSlot.Q, 900) },
                                                                           { Spells.W, new LeagueSharp.Common.Spell(SpellSlot.W, 600) },
                                                                           { Spells.E, new LeagueSharp.Common.Spell(SpellSlot.E, 600) },
                                                                           { Spells.R, new LeagueSharp.Common.Spell(SpellSlot.R) }
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

        public Menu comboMenu, harassMenu, clearMenu, miscellaneousMenu;

        public void CreateMenu()
        {
            this.Menu = MainMenu.AddMenu("ElRyze", "ElRyze");

            comboMenu = this.Menu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("ElEasy.Ryze.Combo.Q", new CheckBox("Use Q"));
            comboMenu.Add("ElEasy.Ryze.Combo.W", new CheckBox("Use W"));
            comboMenu.Add("ElEasy.Ryze.Combo.E", new CheckBox("Use E"));
            comboMenu.Add("ElEasy.Ryze.Combo.R", new CheckBox("Use R"));
            comboMenu.Add("ElEasy.Ryze.Combo.R.HP", new Slider("Use R when HP", 100));
            comboMenu.Add("ElEasy.Ryze.Combo.Ignite", new CheckBox("Use Ignite"));

            harassMenu = this.Menu.AddSubMenu("Harass", "Harass");
            harassMenu.Add("ElEasy.Ryze.Harass.Q", new CheckBox("Use Q"));
            harassMenu.Add("ElEasy.Ryze.Harass.W", new CheckBox("Use W"));
            harassMenu.Add("ElEasy.Ryze.Harass.E", new CheckBox("Use E"));
            harassMenu.Add("ElEasy.Ryze.Harass.Player.Mana", new Slider("Minimum Mana", 55));
            harassMenu.AddSeparator();
            harassMenu.AddGroupLabel("AutoHarass settings : ");
            harassMenu.Add("ElEasy.Ryze.AutoHarass.Activated", new KeyBind("Auto harass", false, KeyBind.BindTypes.PressToggle, 'L'));
            harassMenu.Add("ElEasy.Ryze.AutoHarass.Q", new CheckBox("Use Q"));
            harassMenu.Add("ElEasy.Ryze.AutoHarass.W", new CheckBox("Use W"));
            harassMenu.Add("ElEasy.Ryze.AutoHarass.E", new CheckBox("Use E"));
            harassMenu.Add("ElEasy.Ryze.AutoHarass.Mana", new Slider("Minimum mana", 55));

            clearMenu = this.Menu.AddSubMenu("Clear", "Clear");
            clearMenu.AddGroupLabel("Last Hit :");
            clearMenu.Add("ElEasy.Ryze.Lasthit.Q", new CheckBox("Use Q"));
            clearMenu.Add("ElEasy.Ryze.Lasthit.W", new CheckBox("Use E"));
            clearMenu.Add("ElEasy.Ryze.Lasthit.E", new CheckBox("Use W"));
            clearMenu.AddSeparator();
            clearMenu.AddGroupLabel("Lane Clear :");
            clearMenu.Add("ElEasy.Ryze.LaneClear.Q", new CheckBox("Use Q"));
            clearMenu.Add("ElEasy.Ryze.LaneClear.W", new CheckBox("Use W"));
            clearMenu.Add("ElEasy.Ryze.LaneClear.E", new CheckBox("Use E"));
            clearMenu.Add("ElEasy.Ryze.LaneClear.R", new CheckBox("Use R"));
            clearMenu.Add("ElEasy.Ryze.Clear.Player.Mana.Lane1", new Slider("Minimum Mana for clear", 1));
            clearMenu.AddSeparator();
            clearMenu.AddGroupLabel("Jungle Clear :");
            clearMenu.Add("ElEasy.Ryze.JungleClear.Q", new CheckBox("Use Q"));
            clearMenu.Add("ElEasy.Ryze.JungleClear.W", new CheckBox("Use W"));
            clearMenu.Add("ElEasy.Ryze.JungleClear.E", new CheckBox("Use E"));
            clearMenu.Add("ElEasy.Ryze.Clear.Player.Mana.Jungle", new Slider("Minimum Mana for clear", 55));

            miscellaneousMenu = this.Menu.AddSubMenu("Miscellaneous", "Miscellaneous");
            miscellaneousMenu.Add("ElEasy.Ryze.GapCloser.Activated", new CheckBox("Anti gapcloser"));
            miscellaneousMenu.Add("ElEasy.Ryze.AA", new CheckBox("Don't use AA in combo", false));
            miscellaneousMenu.Add("ElEasy.Ryze.Draw.off", new CheckBox("Turn drawings off"));
            miscellaneousMenu.Add("ElEasy.Ryze.Draw.Q", new CheckBox("Draw Q"));
            miscellaneousMenu.Add("ElEasy.Ryze.Draw.W", new CheckBox("Draw W"));
            miscellaneousMenu.Add("ElEasy.Ryze.Draw.E", new CheckBox("Draw E"));
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
            Console.WriteLine("Loaded Ryze");
            Ignite = this.Player.GetSpellSlot("summonerdot");
            spells[Spells.Q].SetSkillshot(
                0.25f,
                spells[Spells.Q].Instance.SData.LineWidth,
                spells[Spells.Q].Instance.SData.MissileSpeed,
                true,
                SkillshotType.SkillshotLine);

            Game.OnUpdate += this.OnUpdate;
            Drawing.OnDraw += this.OnDraw;
            Orbwalker.OnPreAttack += this.OrbwalkingBeforeAttack;
            AntiGapcloser.OnEnemyGapcloser += this.AntiGapcloser_OnEnemyGapcloser;
        }

        #endregion

        #region Methods

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (getCheckBoxItem(miscellaneousMenu, "ElEasy.Ryze.GapCloser.Activated") && spells[Spells.W].IsReady()
                && gapcloser.Sender.Distance(this.Player) < spells[Spells.W].Range)
            {
                spells[Spells.W].CastOnUnit(gapcloser.Sender);
            }
        }

        private float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            if (spells[Spells.Q].IsReady())
            {
                damage +=
                    this.Player.Buffs.Count(
                        buf => buf.Name.Equals("RyzePassiveStack", StringComparison.InvariantCultureIgnoreCase)) > 0
                        ? this.Player.GetSpellDamage(enemy, SpellSlot.Q) * 2.5f
                        : this.Player.GetSpellDamage(enemy, SpellSlot.Q);
            }

            if (spells[Spells.W].IsReady())
            {
                damage += this.Player.GetSpellDamage(enemy, SpellSlot.W);
            }

            if (spells[Spells.E].IsReady())
            {
                damage += this.Player.GetSpellDamage(enemy, SpellSlot.E);
            }

            if (spells[Spells.R].IsReady())
            {
                damage += this.Player.GetSpellDamage(enemy, SpellSlot.R);
            }

            return (float)damage;
        }

        private float IgniteDamage(AIHeroClient target)
        {
            if (Ignite == SpellSlot.Unknown || this.Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)this.Player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
        }

        private void OnAutoHarass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, DamageType.Magical);
            if (target == null || !target.IsValid)
            {
                return;
            }

            var useQ = getCheckBoxItem(harassMenu, "ElEasy.Ryze.AutoHarass.Q");
            var useW = getCheckBoxItem(harassMenu, "ElEasy.Ryze.AutoHarass.W");
            var useE = getCheckBoxItem(harassMenu, "ElEasy.Ryze.AutoHarass.E");
            var mana = getSliderItem(harassMenu, "ElEasy.Ryze.AutoHarass.Mana");

            if (this.Player.ManaPercent < mana)
            {
                return;
            }

            if (useQ && spells[Spells.Q].IsReady() && spells[Spells.Q].IsInRange(target))
            {
                var prediction = spells[Spells.Q].GetPrediction(target);
                if (prediction.Hitchance != HitChance.Impossible && prediction.Hitchance != HitChance.OutOfRange
                    && prediction.Hitchance != HitChance.Collision)
                {
                    spells[Spells.Q].Cast(target);
                }
            }

            if (useW && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(target))
            {
                spells[Spells.W].CastOnUnit(target);
            }

            if (useE && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target))
            {
                spells[Spells.E].CastOnUnit(target);
            }
        }

        /// <summary>
        /// WHAT A MESS
        /// </summary>
        private void OnCombo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.W].Range, DamageType.Magical);
            if (!target.IsValidTarget())
            {
                return;
            }

            var useQ = getCheckBoxItem(comboMenu, "ElEasy.Ryze.Combo.Q");
            var useW = getCheckBoxItem(comboMenu, "ElEasy.Ryze.Combo.W");
            var useE = getCheckBoxItem(comboMenu, "ElEasy.Ryze.Combo.E");
            var useR = getCheckBoxItem(comboMenu, "ElEasy.Ryze.Combo.R");
            var rHp = getSliderItem(comboMenu, "ElEasy.Ryze.Combo.R.HP");
            var useI = getCheckBoxItem(comboMenu, "ElEasy.Ryze.Combo.Ignite");

            if (this.Player.Level < 6)
            {
                switch (
                    this.Player.Buffs.Count(
                        buf => buf.Name.Equals("RyzePassiveStack", StringComparison.InvariantCultureIgnoreCase)))
                {
                    case 0:
                    case 1:
                        if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast(target);
                        }

                        if (useE && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target))
                        {
                            spells[Spells.E].CastOnUnit(target);
                        }
                        if (useW && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(target))
                        {
                            spells[Spells.W].CastOnUnit(target);
                        }

                        break;

                    case 2:
                        if (useE && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target))
                        {
                            spells[Spells.E].CastOnUnit(target);
                        }
                        if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast(target);
                        }
                        if (useW && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(target))
                        {
                            spells[Spells.W].CastOnUnit(target);
                        }
                        break;
                    case 3:
                        if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast(target);
                        }
                        if (useW && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(target))
                        {
                            spells[Spells.W].CastOnUnit(target);
                        }
                        if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast(target);
                        }
                        if (useE && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target))
                        {
                            spells[Spells.E].CastOnUnit(target);
                        }
                        break;

                    case 4:
                        if (useW && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(target))
                        {
                            spells[Spells.W].CastOnUnit(target);
                        }
                        if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast(target);
                        }
                        if (useE && spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
                        {
                            spells[Spells.E].CastOnUnit(target);
                        }
                        break;
                }
            }

            if (this.Player.Level >= 6)
            {
                switch (
                    this.Player.Buffs.Count(
                        buf => buf.Name.Equals("RyzePassiveStack", StringComparison.InvariantCultureIgnoreCase)))
                {
                    case 0:
                        if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast(target);
                        }

                        if (useE && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target))
                        {
                            spells[Spells.E].CastOnUnit(target);
                        }
                        if (useW && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(target))
                        {
                            spells[Spells.W].CastOnUnit(target);
                        }
                        break;

                    case 1:
                        if (useR && spells[Spells.R].IsReady())
                        {
                            spells[Spells.R].Cast();
                        }
                        if (useE && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target))
                        {
                            spells[Spells.E].CastOnUnit(target);
                        }
                        if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast(target);
                        }
                        if (useW && spells[Spells.W].IsReady() && target.IsValidTarget(spells[Spells.W].Range))
                        {
                            spells[Spells.W].CastOnUnit(target);
                        }
                        break;

                    case 2:
                        if (useR && spells[Spells.R].IsReady())
                        {
                            spells[Spells.R].Cast();
                        }
                        if (useW && spells[Spells.W].IsReady() && target.IsValidTarget(spells[Spells.W].Range))
                        {
                            spells[Spells.W].CastOnUnit(target);
                        }
                        if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast(target);
                        }
                        if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast(target);
                        }
                        if (useE && spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
                        {
                            spells[Spells.E].CastOnUnit(target);
                        }
                        break;

                    case 3:
                        if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast(target);
                        }
                        if (useW && spells[Spells.W].IsReady() && target.IsValidTarget(spells[Spells.W].Range))
                        {
                            spells[Spells.W].CastOnUnit(target);
                        }
                        if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast(target);
                        }
                        if (useE && spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
                        {
                            spells[Spells.E].CastOnUnit(target);
                        }
                        break;

                    case 4:
                        if (useW && spells[Spells.W].IsReady() && target.IsValidTarget(spells[Spells.W].Range))
                        {
                            spells[Spells.W].CastOnUnit(target);
                        }
                        if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast(target);
                        }
                        if (useE && spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
                        {
                            spells[Spells.E].CastOnUnit(target);
                        }
                        break;
                }
            }

            if (this.Player.Distance(target) <= 600 && this.IgniteDamage(target) >= target.Health && useI)
            {
                this.Player.Spellbook.CastSpell(Ignite, target);
            }
        }

        private void OnDraw(EventArgs args)
        {
            var drawOff = getCheckBoxItem(miscellaneousMenu, "ElEasy.Ryze.Draw.off");
            var drawQ = getCheckBoxItem(miscellaneousMenu, "ElEasy.Ryze.Draw.Q");
            var drawW = getCheckBoxItem(miscellaneousMenu, "ElEasy.Ryze.Draw.W");
            var drawE = getCheckBoxItem(miscellaneousMenu, "ElEasy.Ryze.Draw.E");

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
        }

        private void OnHarass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, DamageType.Magical);
            if (target == null || !target.IsValid)
            {
                return;
            }

            var useQ = getCheckBoxItem(harassMenu, "ElEasy.Ryze.Harass.Q");
            var useW = getCheckBoxItem(harassMenu, "ElEasy.Ryze.Harass.W");
            var useE = getCheckBoxItem(harassMenu, "ElEasy.Ryze.Harass.E");
            var mana = getSliderItem(harassMenu, "ElEasy.Ryze.Harass.Player.Mana");

            if (this.Player.ManaPercent < mana)
            {
                return;
            }

            if (useQ && spells[Spells.Q].IsReady() && spells[Spells.Q].IsInRange(target))
            {
                var pred = spells[Spells.Q].GetPrediction(target);
                if (pred.Hitchance >= HitChance.High && pred.CollisionObjects.Count == 0)
                {
                    spells[Spells.Q].Cast(target);
                }
            }

            if (useW && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(target))
            {
                spells[Spells.W].CastOnUnit(target);
            }

            if (useE && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target))
            {
                spells[Spells.E].CastOnUnit(target);
            }
        }

        private void OnJungleclear()
        {
            var useQ = getCheckBoxItem(clearMenu, "ElEasy.Ryze.JungleClear.Q");
            var useW = getCheckBoxItem(clearMenu, "ElEasy.Ryze.JungleClear.W");
            var useE = getCheckBoxItem(clearMenu, "ElEasy.Ryze.JungleClear.E");
            var mana = getSliderItem(clearMenu, "ElEasy.Ryze.Clear.Player.Mana.Jungle");

            if (this.Player.ManaPercent < mana)
            {
                return;
            }

            var minions =
                MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition,
                    spells[Spells.Q].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (minions == null)
            {
                return;
            }

            if (useQ && spells[Spells.Q].IsReady() && minions.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast(minions);
            }

            if (useW && spells[Spells.W].IsReady() && minions.IsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast(minions);
            }

            if (useE && spells[Spells.E].IsReady() && minions.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast(minions);
            }
        }

        private void OnLaneclear()
        {
            var useQ = getCheckBoxItem(clearMenu, "ElEasy.Ryze.LaneClear.Q");
            var useW = getCheckBoxItem(clearMenu, "ElEasy.Ryze.LaneClear.W");
            var useE = getCheckBoxItem(clearMenu, "ElEasy.Ryze.LaneClear.E");
            var useR = getCheckBoxItem(clearMenu, "ElEasy.Ryze.LaneClear.R");
            var mana = getSliderItem(clearMenu, "ElEasy.Ryze.Clear.Player.Mana.Lane1");

            if (this.Player.ManaPercent < mana)
            {
                return;
            }

            var target = MinionManager.GetMinions(this.Player.ServerPosition, spells[Spells.W].Range).FirstOrDefault();
            if (target == null)
            {
                return;
            }
            if (this.Player.Level < 6)
            {
                switch (
                    this.Player.Buffs.Count(
                        buf => buf.Name.Equals("RyzePassiveStack", StringComparison.InvariantCultureIgnoreCase)))
                {
                    case 0:
                    case 1:
                        if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast(target);
                        }

                        if (useE && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target))
                        {
                            spells[Spells.E].CastOnUnit(target);
                        }
                        if (useW && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(target))
                        {
                            spells[Spells.W].CastOnUnit(target);
                        }

                        break;

                    case 2:
                        if (useE && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target))
                        {
                            spells[Spells.E].CastOnUnit(target);
                        }
                        if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast(target);
                        }
                        if (useW && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(target))
                        {
                            spells[Spells.W].CastOnUnit(target);
                        }
                        break;
                    case 3:
                        if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast(target);
                        }
                        if (useW && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(target))
                        {
                            spells[Spells.W].CastOnUnit(target);
                        }
                        if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast(target);
                        }
                        if (useE && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target))
                        {
                            spells[Spells.E].CastOnUnit(target);
                        }
                        break;

                    case 4:
                        if (useW && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(target))
                        {
                            spells[Spells.W].CastOnUnit(target);
                        }
                        if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast(target);
                        }
                        if (useE && spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
                        {
                            spells[Spells.E].CastOnUnit(target);
                        }
                        break;
                }
            }

            if (this.Player.Level >= 6)
            {
                switch (
                    this.Player.Buffs.Count(
                        buf => buf.Name.Equals("RyzePassiveStack", StringComparison.InvariantCultureIgnoreCase)))
                {
                    case 0:
                        if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast(target);
                        }

                        if (useE && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target))
                        {
                            spells[Spells.E].CastOnUnit(target);
                        }
                        if (useW && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(target))
                        {
                            spells[Spells.W].CastOnUnit(target);
                        }
                        break;

                    case 1:
                        if (useR && spells[Spells.R].IsReady())
                        {
                            spells[Spells.R].Cast();
                        }
                        if (useE && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target))
                        {
                            spells[Spells.E].CastOnUnit(target);
                        }
                        if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast(target);
                        }
                        if (useW && spells[Spells.W].IsReady() && target.IsValidTarget(spells[Spells.W].Range))
                        {
                            spells[Spells.W].CastOnUnit(target);
                        }
                        break;

                    case 2:
                        if (useR && spells[Spells.R].IsReady())
                        {
                            spells[Spells.R].Cast();
                        }
                        if (useW && spells[Spells.W].IsReady() && target.IsValidTarget(spells[Spells.W].Range))
                        {
                            spells[Spells.W].CastOnUnit(target);
                        }
                        if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast(target);
                        }
                        if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast(target);
                        }
                        if (useE && spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
                        {
                            spells[Spells.E].CastOnUnit(target);
                        }
                        break;

                    case 3:
                        if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast(target);
                        }
                        if (useW && spells[Spells.W].IsReady() && target.IsValidTarget(spells[Spells.W].Range))
                        {
                            spells[Spells.W].CastOnUnit(target);
                        }
                        if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast(target);
                        }
                        if (useE && spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
                        {
                            spells[Spells.E].CastOnUnit(target);
                        }
                        break;

                    case 4:
                        if (useW && spells[Spells.W].IsReady() && target.IsValidTarget(spells[Spells.W].Range))
                        {
                            spells[Spells.W].CastOnUnit(target);
                        }
                        if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast(target);
                        }
                        if (useE && spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
                        {
                            spells[Spells.E].CastOnUnit(target);
                        }
                        break;
                }
            }
        }

        private void OnLasthit()
        {
            var useQ = getCheckBoxItem(clearMenu, "ElEasy.Ryze.Lasthit.Q");
            var useW = getCheckBoxItem(clearMenu, "ElEasy.Ryze.Lasthit.W");
            var useE = getCheckBoxItem(clearMenu, "ElEasy.Ryze.Lasthit.E");

            var minions = MinionManager.GetMinions(this.Player.ServerPosition, spells[Spells.W].Range).FirstOrDefault();
            if (minions == null)
            {
                return;
            }

            if (spells[Spells.Q].IsReady() && useQ)
            {
                if (HealthPrediction.GetHealthPrediction(minions, (int)0.25)
                    <= this.Player.GetSpellDamage(minions, SpellSlot.Q))
                {
                    spells[Spells.Q].Cast(minions);
                }
            }

            if (spells[Spells.W].IsReady() && useW)
            {
                var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, spells[Spells.W].Range);
                {
                    foreach (var minion in
                        allMinions.Where(
                            minion => minion.Health <= ObjectManager.Player.GetSpellDamage(minion, SpellSlot.W)))
                    {
                        if (minion.IsValidTarget(spells[Spells.W].Range))
                        {
                            spells[Spells.W].CastOnUnit(minion);
                            return;
                        }
                    }
                }
            }

            if (spells[Spells.E].IsReady() && useE)
            {
                var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, spells[Spells.E].Range);
                {
                    foreach (var minion in
                        allMinions.Where(
                            minion => minion.Health <= ObjectManager.Player.GetSpellDamage(minion, SpellSlot.E)))
                    {
                        if (minion.IsValidTarget())
                        {
                            spells[Spells.E].CastOnUnit(minion);
                            return;
                        }
                    }
                }
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

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                this.OnLasthit();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                this.OnHarass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                this.OnLaneclear();
                this.OnJungleclear();
            }

            if (getKeyBindItem(harassMenu, "ElEasy.Ryze.AutoHarass.Activated"))
            {
                this.OnAutoHarass();
            }
        }

        private void OrbwalkingBeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (getCheckBoxItem(miscellaneousMenu, "ElEasy.Ryze.AA") && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                args.Process = false;
            }
            else
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    args.Process =
                        !(spells[Spells.Q].IsReady() || spells[Spells.W].IsReady() || spells[Spells.E].IsReady()
                          || this.Player.Distance(args.Target) >= 1000);
                }
            }
        }

        #endregion
    }
}