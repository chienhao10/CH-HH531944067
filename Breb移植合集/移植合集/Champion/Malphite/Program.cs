using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using Damage = LeagueSharp.Common.Damage;
using Spell = LeagueSharp.Common.Spell;

namespace ElEasy.Plugins
{
    public class Malphite
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
            {Spells.Q, new Spell(SpellSlot.Q, 625)},
            {Spells.W, new Spell(SpellSlot.W, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100)},
            {Spells.E, new Spell(SpellSlot.E, 375)},
            {Spells.R, new Spell(SpellSlot.R, 1000)}
        };

        private static SpellSlot Ignite;

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
            get { return ObjectManager.Player; }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public static Menu comboMenu, rMenu, harassMenu, clearMenu, miscellaneousMenu;

        public void CreateMenu()
        {
            Menu = MainMenu.AddMenu("ElMalphite", "ElMalphite");

            comboMenu = Menu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("ElEasy.Malphite.Combo.Q", new CheckBox("Use Q"));
            comboMenu.Add("ElEasy.Malphite.Combo.W", new CheckBox("Use W"));
            comboMenu.Add("ElEasy.Malphite.Combo.E", new CheckBox("Use E"));
            comboMenu.Add("ElEasy.Malphite.Combo.Ignite", new CheckBox("Use Ignite"));

            rMenu = Menu.AddSubMenu("R Settings", "R");
            rMenu.Add("ElEasy.Malphite.Combo.R", new CheckBox("Use R"));
            rMenu.Add("ElEasy.Malphite.Combo.ForceR", new CheckBox("Force R when target can get killed", false));
            rMenu.Add("ElEasy.Malphite.Combo.R.Mode",
                new ComboBox("Mode ", 1, "Single target finisher", "Champions hit"));
            rMenu.Add("ElEasy.Malphite.Combo.Count.R", new Slider("Minimum champions hit by R", 2, 1, 5));

            harassMenu = Menu.AddSubMenu("Harass", "Harass");
            harassMenu.Add("ElEasy.Malphite.Harass.Q", new CheckBox("Use Q"));
            harassMenu.Add("ElEasy.Malphite.Harass.E", new CheckBox("Use E"));
            harassMenu.Add("ElEasy.Malphite.Harass.Player.Mana", new Slider("Minimum Mana", 55, 1));
            harassMenu.AddGroupLabel("Auto Harass Settings");
            harassMenu.Add("ElEasy.Malphite.AutoHarass.Activate",
                new KeyBind("Auto harass", false, KeyBind.BindTypes.PressToggle, 'L'));
            harassMenu.Add("ElEasy.Malphite.AutoHarass.Q", new CheckBox("Use Q"));
            harassMenu.Add("ElEasy.Malphite.AutoHarass.E", new CheckBox("Use E"));
            harassMenu.Add("ElEasy.Malphite.AutoHarass.PlayerMana", new Slider("Minimum mana", 55, 1));

            clearMenu = Menu.AddSubMenu("Clear", "Clear");
            clearMenu.AddGroupLabel("Wave Clear");
            clearMenu.Add("ElEasy.Malphite.LaneClear.Q", new CheckBox("Use Q"));
            clearMenu.Add("ElEasy.Malphite.LaneClear.W", new CheckBox("Use W"));
            clearMenu.Add("ElEasy.Malphite.LaneClear.E", new CheckBox("Use E"));
            clearMenu.AddGroupLabel("Jungle Clear");
            clearMenu.Add("ElEasy.Malphite.JungleClear.Q", new CheckBox("Use Q"));
            clearMenu.Add("ElEasy.Malphite.JungleClear.W", new CheckBox("Use W"));
            clearMenu.Add("ElEasy.Malphite.JungleClear.E", new CheckBox("Use E"));
            clearMenu.AddGroupLabel("Last Hit");
            clearMenu.Add("ElEasy.Malphite.Lasthit.Q", new CheckBox("Use Q"));
            clearMenu.AddSeparator();
            clearMenu.Add("ElEasy.Malphite.Clear.Player.Mana", new Slider("Minimum Mana for clear", 55, 1));

            miscellaneousMenu = Menu.AddSubMenu("Miscellaneous", "Miscellaneous");
            miscellaneousMenu.Add("ElEasy.Malphite.Interrupt.Activated", new CheckBox("Interrupt spells"));
            miscellaneousMenu.Add("ElEasy.Malphite.Draw.off", new CheckBox("Turn drawings off"));
            miscellaneousMenu.Add("ElEasy.Malphite.Castpos", new CheckBox("Draw R cast position"));
            miscellaneousMenu.Add("ElEasy.Malphite.Draw.Q", new CheckBox("Draw Q"));
            miscellaneousMenu.Add("ElEasy.Malphite.Draw.E", new CheckBox("Draw E"));
            miscellaneousMenu.Add("ElEasy.Malphite.Draw.R", new CheckBox("Draw R"));
        }

        public Malphite()
        {
            Console.WriteLine("Loaded Malphite");
            CreateMenu();
            Ignite = Player.GetSpellSlot("summonerdot");
            spells[Spells.R].SetSkillshot(0.00f, 270, 700, false, SkillshotType.SkillshotCircle);

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }

        #endregion

        #region Methods

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
            if (!getCheckBoxItem(miscellaneousMenu, "ElEasy.Malphite.Interrupt.Activated"))
            {
                return;
            }

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

        private void OnAutoHarass()
        {
            var qTarget = TargetSelector.GetTarget(spells[Spells.Q].Range, DamageType.Magical);
            var eTarget = TargetSelector.GetTarget(spells[Spells.E].Range, DamageType.Magical);

            if (qTarget == null || !qTarget.IsValid)
            {
                return;
            }

            var useQ = getCheckBoxItem(harassMenu, "ElEasy.Malphite.AutoHarass.Q");
            var useE = getCheckBoxItem(harassMenu, "ElEasy.Malphite.AutoHarass.E");
            var playerMana = getSliderItem(harassMenu, "ElEasy.Malphite.AutoHarass.PlayerMana");

            if (Player.ManaPercent < playerMana)
            {
                return;
            }

            if (useQ && spells[Spells.Q].IsReady() && qTarget.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast(qTarget);
            }

            if (useE && spells[Spells.E].IsReady() && eTarget.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast();
            }
        }

        private void OnCombo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, DamageType.Magical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            var useQ = getCheckBoxItem(comboMenu, "ElEasy.Malphite.Combo.Q");
            var useW = getCheckBoxItem(comboMenu, "ElEasy.Malphite.Combo.W");
            var useE = getCheckBoxItem(comboMenu, "ElEasy.Malphite.Combo.E");
            var useR = getCheckBoxItem(rMenu, "ElEasy.Malphite.Combo.R");
            var useI = getCheckBoxItem(comboMenu, "ElEasy.Malphite.Combo.Ignite");
            var ultType = getBoxItem(rMenu, "ElEasy.Malphite.Combo.R.Mode");

            var countEnemies = getSliderItem(rMenu, "ElEasy.Malphite.Combo.Count.R");

            if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast(target);
            }

            if (useW && spells[Spells.W].IsReady() && target.IsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast();
            }

            if (useE && spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast();
            }

            switch (ultType)
            {
                case 0:
                    if (useR && spells[Spells.R].IsReady() && target.IsValidTarget(spells[Spells.R].Range))
                    {
                        if (spells[Spells.R].GetDamage(target) > target.Health)
                        {
                            var pred = spells[Spells.R].GetPrediction(target);
                            if (pred.Hitchance >= HitChance.High)
                            {
                                spells[Spells.R].Cast(pred.CastPosition);
                            }
                        }
                    }
                    break;

                case 1:
                    if (useR && spells[Spells.R].IsReady() && target.IsValidTarget(spells[Spells.R].Range))
                    {
                        var pred = spells[Spells.R].GetPrediction(target);
                        if (pred.Hitchance >= HitChance.High)
                        {
                            var hits = HeroManager.Enemies.Where(x => x.Distance(target) <= 300f).ToList();
                            if (hits.Any(hit => hits.Count >= countEnemies))
                            {
                                spells[Spells.R].Cast(pred.CastPosition);
                            }
                        }
                    }
                    break;
            }
            //
            if (getCheckBoxItem(rMenu, "ElEasy.Malphite.Combo.ForceR"))
            {
                var getthabitch =
                    HeroManager.Enemies.FirstOrDefault(
                        x =>
                            x.Distance(target) <= 300f && spells[Spells.R].GetDamage(x) > x.Health
                            && Player.CountEnemiesInRange(1000) == 1);

                var pred = spells[Spells.R].GetPrediction(getthabitch);
                if (pred.Hitchance >= HitChance.High)
                {
                    spells[Spells.R].Cast(pred.CastPosition);
                }
            }


            if (target.IsValidTarget(600) && IgniteDamage(target) >= target.Health && useI)
            {
                Player.Spellbook.CastSpell(Ignite, target);
            }
        }

        private void OnDraw(EventArgs args)
        {
            var drawOff = getCheckBoxItem(miscellaneousMenu, "ElEasy.Malphite.Draw.off");
            var drawQ = getCheckBoxItem(miscellaneousMenu, "ElEasy.Malphite.Draw.Q");
            var drawE = getCheckBoxItem(miscellaneousMenu, "ElEasy.Malphite.Draw.E");
            var drawR = getCheckBoxItem(miscellaneousMenu, "ElEasy.Malphite.Draw.R");

            if (drawOff)
            {
                return;
            }
            //
            if (getCheckBoxItem(miscellaneousMenu, "ElEasy.Malphite.Castpos"))
            {
                var target = TargetSelector.GetTarget(spells[Spells.R].Range, DamageType.Magical);
                if (target == null) return;

                var hits = HeroManager.Enemies.Where(x => x.Distance(target) <= 300f).ToList();
                if (hits.Any(hit => hits.Count >= getSliderItem(rMenu, "ElEasy.Malphite.Combo.Count.R")))
                {
                    var pred = spells[Spells.R].GetPrediction(target);
                    Render.Circle.DrawCircle(pred.CastPosition, 300, Color.DeepSkyBlue);
                }
            }


            if (drawQ)
            {
                if (spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(Player.Position, spells[Spells.Q].Range, Color.White);
                }
            }


            if (drawE)
            {
                if (spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(Player.Position, spells[Spells.E].Range, Color.White);
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
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, DamageType.Magical);
            var eTarget = TargetSelector.GetTarget(spells[Spells.E].Range, DamageType.Magical);

            if (target == null || !target.IsValid)
            {
                return;
            }

            var useQ = getCheckBoxItem(harassMenu, "ElEasy.Malphite.Harass.Q");
            var useE = getCheckBoxItem(harassMenu, "ElEasy.Malphite.Harass.E");
            var playerMana = getSliderItem(harassMenu, "ElEasy.Malphite.Harass.Player.Mana");

            if (Player.ManaPercent < playerMana)
            {
                return;
            }

            if (useQ && spells[Spells.Q].IsReady() && spells[Spells.Q].IsInRange(target))
            {
                spells[Spells.Q].Cast(target);
            }

            if (useE && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target) && eTarget != null)
            {
                spells[Spells.E].Cast(eTarget);
            }
        }

        private void OnJungleclear()
        {
            var useQ = getCheckBoxItem(clearMenu, "ElEasy.Malphite.JungleClear.Q");
            var useW = getCheckBoxItem(clearMenu, "ElEasy.Malphite.JungleClear.W");
            var useE = getCheckBoxItem(clearMenu, "ElEasy.Malphite.JungleClear.E");
            var playerMana = getSliderItem(clearMenu, "ElEasy.Malphite.Clear.Player.Mana");

            if (Player.ManaPercent < playerMana)
            {
                return;
            }

            var minions =
                MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition,
                    spells[Spells.E].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (minions == null)
            {
                return;
            }

            if (spells[Spells.Q].IsReady() && useQ)
            {
                spells[Spells.Q].CastOnUnit(minions);
            }

            if (useW && spells[Spells.W].IsReady())
            {
                spells[Spells.W].Cast(Player);
            }

            if (useE && spells[Spells.E].IsReady() && minions.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast();
            }
        }

        private void OnLaneclear()
        {
            var useQ = getCheckBoxItem(clearMenu, "ElEasy.Malphite.LaneClear.Q");
            var useW = getCheckBoxItem(clearMenu, "ElEasy.Malphite.LaneClear.W");
            var useE = getCheckBoxItem(clearMenu, "ElEasy.Malphite.LaneClear.E");
            var playerMana = getSliderItem(clearMenu, "ElEasy.Malphite.Clear.Player.Mana");

            if (Player.ManaPercent < playerMana)
            {
                return;
            }

            var minions = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.E].Range).FirstOrDefault();
            if (minions == null)
            {
                return;
            }

            if (spells[Spells.Q].IsReady() && useQ)
            {
                var allMinions = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.E].Range);
                {
                    foreach (var minion in
                        allMinions.Where(minion => minion.Health <= Player.GetSpellDamage(minion, SpellSlot.Q)))
                    {
                        if (minion.IsValidTarget())
                        {
                            spells[Spells.Q].CastOnUnit(minion);
                            return;
                        }
                    }
                }
            }

            if (useW && spells[Spells.W].IsReady())
            {
                spells[Spells.W].Cast(Player);
            }

            if (useE && spells[Spells.E].IsReady() && minions.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast();
            }
        }

        private void OnLastHit()
        {
            var useQ = getCheckBoxItem(clearMenu, "ElEasy.Malphite.Lasthit.Q");
            var playerMana = getSliderItem(clearMenu, "ElEasy.Malphite.Clear.Player.Mana");

            if (Player.ManaPercent < playerMana || !useQ)
            {
                return;
            }

            var minions = MinionManager.GetMinions(
                Player.Position,
                spells[Spells.E].Range,
                MinionTypes.All,
                MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);

            foreach (var minion in minions)
            {
                if (spells[Spells.Q].GetDamage(minion) > minion.Health && spells[Spells.Q].IsReady())
                {
                    spells[Spells.Q].CastOnUnit(minion);
                }
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

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                OnLaneclear();
                OnJungleclear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                OnHarass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                OnLastHit();
            }

            if (getKeyBindItem(harassMenu, "ElEasy.Malphite.AutoHarass.Activate"))
            {
                OnAutoHarass();
            }
        }

        #endregion
    }
}