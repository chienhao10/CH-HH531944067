namespace ElVi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    internal enum Spells
    {
        Q,

        W,

        E,

        R
    }

    internal static class Vi
    {
        #region Static Fields

        public static readonly Dictionary<Spells, LeagueSharp.Common.Spell> Spells = new Dictionary<Spells, LeagueSharp.Common.Spell>
                                                                      {
                                                                          { ElVi.Spells.Q, new LeagueSharp.Common.Spell(SpellSlot.Q, 800) },
                                                                          { ElVi.Spells.W, new LeagueSharp.Common.Spell(SpellSlot.W) },
                                                                          { ElVi.Spells.E, new LeagueSharp.Common.Spell(SpellSlot.E, 600) },
                                                                          { ElVi.Spells.R, new LeagueSharp.Common.Spell(SpellSlot.R, 800) }
                                                                      };

        private static SpellSlot flash;

        private static SpellSlot ignite;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets Ravenous Hydra
        /// </summary>
        /// <value>
        ///     Ravenous Hydra
        /// </value>
        private static Items.Item Hydra => ItemData.Ravenous_Hydra_Melee_Only.GetItem();

        private static AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        /// <summary>
        ///     Gets Tiamat Item
        /// </summary>
        /// <value>
        ///     Tiamat Item
        /// </value>
        private static Items.Item Tiamat => ItemData.Tiamat_Melee_Only.GetItem();

        /// <summary>
        ///     Gets Titanic Hydra
        /// </summary>
        /// <value>
        ///     Titanic Hydra
        /// </value>
        private static Items.Item Titanic => ItemData.Titanic_Hydra_Melee_Only.GetItem();

        /// <summary>
        ///     Gets Youmuus Ghostblade
        /// </summary>
        /// <value>
        ///     Youmuus Ghostblade
        /// </value>
        private static Items.Item Youmuu => ItemData.Youmuus_Ghostblade.GetItem();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Cast items
        /// </summary>
        /// <param name="target"></param>
        /// <returns>true or false</returns>
        public static bool CastItems(Obj_AI_Base target)
        {
            if (Player.LSIsDashing() || Player.Spellbook.IsAutoAttacking)
            {
                return false;
            }

            var units =
                MinionManager.GetMinions(385, MinionTypes.All, MinionTeam.NotAlly).Count(o => !(o is Obj_AI_Turret));
            var heroes = Player.GetEnemiesInRange(385).Count;
            var count = units + heroes;

            var tiamat = Tiamat;
            if (tiamat.IsReady() && count > 0 && tiamat.Cast())
            {
                return true;
            }

            var hydra = Hydra;
            if (Hydra.IsReady() && count > 0 && hydra.Cast())
            {
                return true;
            }

            var youmuus = Youmuu;
            if (Youmuu.IsReady() && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && youmuus.Cast())
            {
                return true;
            }

            var titanic = Titanic;
            return titanic.IsReady() && count > 0 && titanic.Cast();
        }

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            if (Spells[ElVi.Spells.Q].IsReady())
            {
                damage += Player.LSGetSpellDamage(enemy, SpellSlot.Q);
            }
            if (Spells[ElVi.Spells.E].IsReady())
            {
                damage += Player.LSGetSpellDamage(enemy, SpellSlot.E) * Spells[ElVi.Spells.E].Instance.Ammo
                          + (float)Player.GetAutoAttackDamage(enemy);
            }

            if (Spells[ElVi.Spells.R].IsReady())
            {
                damage += Player.LSGetSpellDamage(enemy, SpellSlot.R);
            }

            return (float)damage;
        }

        public static void OnLoad()
        {
            if (ObjectManager.Player.CharData.BaseSkinName != "Vi")
            {
                return;
            }

            ignite = Player.GetSpellSlot("summonerdot");
            flash = Player.GetSpellSlot("SummonerFlash");

            Spells[ElVi.Spells.Q].SetSkillshot(0.5f, 75f, float.MaxValue, false, SkillshotType.SkillshotLine);
            Spells[ElVi.Spells.Q].SetCharged("ViQ", "ViQ", 100, 860, 1f);

            Spells[ElVi.Spells.E].SetSkillshot(0.15f, 150f, float.MaxValue, false, SkillshotType.SkillshotLine);
            Spells[ElVi.Spells.R].SetTargetted(0.15f, 1500f);

            ElViMenu.Initialize();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += Drawings.Drawing_OnDraw;
            Orbwalker.OnPostAttack += OrbwalkingAfterAttack;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
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

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (getCheckBoxItem(ElViMenu.miscMenu, "ElVi.misc.AntiGapCloser"))
            {
                if (Spells[ElVi.Spells.Q].IsReady() && gapcloser.Sender.LSDistance(Player) < Spells[ElVi.Spells.Q].Range)
                {
                    if (!Spells[ElVi.Spells.Q].IsCharging)
                    {
                        Spells[ElVi.Spells.Q].StartCharging();
                    }
                    else
                    {
                        Spells[ElVi.Spells.Q].Cast(gapcloser.Sender);
                    }
                }
            }
        }

        private static void FlashQ()
        {
            var target = TargetSelector.GetTarget(
                Spells[ElVi.Spells.Q].ChargedMaxRange,
                DamageType.Physical);
            if (!target.LSIsValidTarget())
            {
                return;
            }

            var position = Spells[ElVi.Spells.Q].GetPrediction(target, true).CastPosition;

            if (Spells[ElVi.Spells.Q].IsReady())
            {
                if (!Spells[ElVi.Spells.Q].IsCharging)
                {
                    Spells[ElVi.Spells.Q].StartCharging();
                }
                else
                {
                    ObjectManager.Player.Spellbook.CastSpell(flash, position);
                    Spells[ElVi.Spells.Q].Cast(target.ServerPosition);
                }
            }
        }

        private static float IgniteDamage(AIHeroClient target)
        {
            if (ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)Player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
        }

        private static void Interrupter2_OnInterruptableTarget(
            AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            var useInterrupter = getCheckBoxItem(ElViMenu.miscMenu, "ElVi.misc.Interrupter");
            if (!useInterrupter)
            {
                return;
            }

            if (args.DangerLevel != Interrupter2.DangerLevel.High
                || sender.LSDistance(Player) > Spells[ElVi.Spells.Q].Range)
            {
                return;
            }

            if (Spells[ElVi.Spells.Q].IsReady())
            {
                if (!Spells[ElVi.Spells.Q].IsCharging)
                {
                    Spells[ElVi.Spells.Q].StartCharging();
                }
                else
                {
                    Spells[ElVi.Spells.Q].Cast(sender);
                }
            }

            if (Spells[ElVi.Spells.R].CanCast(sender) && args.DangerLevel >= Interrupter2.DangerLevel.High)
            {
                Spells[ElVi.Spells.R].Cast(sender);
            }
        }

        private static void OnCombo()
        {
            var target = TargetSelector.GetTarget(
                Spells[ElVi.Spells.Q].ChargedMaxRange,
                DamageType.Physical);
            if (!target.LSIsValidTarget())
            {
                return;
            }

            if (getCheckBoxItem(ElViMenu.cMenu, "ElVi.Combo.Q") && Spells[ElVi.Spells.Q].IsReady())
            {
                if (Spells[ElVi.Spells.Q].IsCharging)
                {
                    if (Spells[ElVi.Spells.Q].IsInRange(target))
                    {
                        var prediction = Spells[ElVi.Spells.Q].GetPrediction(target);
                        if (prediction.Hitchance >= HitChance.High)
                        {
                            if (Spells[ElVi.Spells.Q].Range == Spells[ElVi.Spells.Q].ChargedMaxRange)
                            {
                                if (Spells[ElVi.Spells.Q].Cast(prediction.CastPosition))
                                {
                                    return;
                                }
                            }
                            else if (Spells[ElVi.Spells.Q].GetDamage(target) > target.Health)
                            {
                                var distance =
                                    Player.ServerPosition.LSDistance(
                                        prediction.UnitPosition
                                        * (prediction.UnitPosition - Player.ServerPosition).LSNormalized(),
                                        true);
                                if (distance < Spells[ElVi.Spells.Q].RangeSqr)
                                {
                                    if (Spells[ElVi.Spells.Q].Cast(prediction.CastPosition))
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Spells[ElVi.Spells.Q].StartCharging();
                }
            }

            CastItems(target);

            if (getCheckBoxItem(ElViMenu.cMenu, "ElVi.Combo.R") && Spells[ElVi.Spells.R].IsReady()
                && target.LSIsValidTarget(Spells[ElVi.Spells.R].Range))
            {
                var enemy =
                    HeroManager.Enemies.Where(
                        hero => getCheckBoxItem(ElViMenu.rMenu, "ElVi.Settings.R" + hero.CharData.BaseSkinName))
                        .OrderByDescending(x => x.MaxHealth)
                        .FirstOrDefault();

                if (enemy.LSIsValidTarget() == false)
                {
                    return;
                }

                Spells[ElVi.Spells.R].CastOnUnit(enemy);
            }

            if (Player.LSDistance(target) <= 600 && IgniteDamage(target) >= target.Health
                && getCheckBoxItem(ElViMenu.cMenu, "ElVi.Combo.I"))
            {
                Player.Spellbook.CastSpell(ignite, target);
            }
        }

        private static void OnHarass()
        {
            var target = TargetSelector.GetTarget(
                Spells[ElVi.Spells.Q].ChargedMaxRange,
                DamageType.Physical);
            if (!target.LSIsValidTarget())
            {
                return;
            }

            if (getCheckBoxItem(ElViMenu.hMenu, "ElVi.Harass.Q") && Spells[ElVi.Spells.Q].IsReady())
            {
                if (Spells[ElVi.Spells.Q].IsCharging)
                {
                    Spells[ElVi.Spells.Q].Cast(target);
                }
                else
                {
                    Spells[ElVi.Spells.Q].StartCharging();
                }
            }

            if (getCheckBoxItem(ElViMenu.hMenu, "ElVi.Harass.E") && Spells[ElVi.Spells.E].IsReady())
            {
                Spells[ElVi.Spells.E].Cast();
            }
        }

        private static void OnJungleClear()
        {
            var useQ = getCheckBoxItem(ElViMenu.clearMenu, "ElVi.JungleClear.Q");
            var useE = getCheckBoxItem(ElViMenu.clearMenu, "ElVi.JungleClear.E");
            var playerMana = getSliderItem(ElViMenu.clearMenu, "ElVi.Clear.Player.Mana");

            if (Player.ManaPercent < playerMana)
            {
                return;
            }

            var minions = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                Spells[ElVi.Spells.E].Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            if (minions.Count <= 0)
            {
                return;
            }

            if (useQ && Spells[ElVi.Spells.Q].IsReady())
            {
                if (!Spells[ElVi.Spells.Q].IsCharging)
                {
                    Spells[ElVi.Spells.Q].StartCharging();
                }
                else
                {
                    if (minions.Count == minions.Count(x => Player.LSDistance(x) < Spells[ElVi.Spells.Q].Range))
                    {
                        Spells[ElVi.Spells.Q].Cast(minions[0]);
                    }
                }
            }

            if (useE && Spells[ElVi.Spells.E].IsReady())
            {
                var bestFarmPos = Spells[ElVi.Spells.E].GetLineFarmLocation(minions);
                if (minions.Count == minions.Count(x => Player.LSDistance(x) < Spells[ElVi.Spells.E].Range)
                    && bestFarmPos.Position.IsValid() && bestFarmPos.MinionsHit > 1)
                {
                    Spells[ElVi.Spells.E].Cast();
                }
            }
        }

        private static void OnLaneClear()
        {
            var useQ = getCheckBoxItem(ElViMenu.clearMenu, "ElVi.LaneClear.Q");
            var useE = getCheckBoxItem(ElViMenu.clearMenu, "ElVi.LaneClear.E");
            var playerMana = getSliderItem(ElViMenu.clearMenu, "ElVi.Clear.Player.Mana");

            if (Player.ManaPercent < playerMana)
            {
                return;
            }

            var minions = MinionManager.GetMinions(Player.ServerPosition, Spells[ElVi.Spells.Q].Range);
            if (minions.Count <= 1)
            {
                return;
            }

            if (useQ && Spells[ElVi.Spells.Q].IsReady())
            {
                if (!Spells[ElVi.Spells.Q].IsCharging)
                {
                    Spells[ElVi.Spells.Q].StartCharging();
                }
                else
                {
                    var bestFarmPos = Spells[ElVi.Spells.Q].GetLineFarmLocation(minions);
                    if (minions.Count == minions.Count(x => Player.LSDistance(x) < Spells[ElVi.Spells.Q].Range)
                        && bestFarmPos.Position.IsValid() && bestFarmPos.MinionsHit > 2)
                    {
                        Spells[ElVi.Spells.Q].Cast(bestFarmPos.Position);
                    }
                }
            }

            if (useE && Spells[ElVi.Spells.E].IsReady())
            {
                if (minions.Count == minions.Count(x => Player.LSDistance(x) < Spells[ElVi.Spells.E].Range))
                {
                    Spells[ElVi.Spells.E].Cast();
                }
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
                OnCombo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                OnHarass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                OnLaneClear();
                OnJungleClear();
            }

            if (getKeyBindItem(ElViMenu.cMenu, "ElVi.Combo.Flash"))
            {
                FlashQ();
            }
        }

        private static void OrbwalkingAfterAttack(AttackableUnit target, EventArgs args)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                return;
            }

            if (getCheckBoxItem(ElViMenu.cMenu, "ElVi.Combo.E"))
            {
                Spells[ElVi.Spells.E].Cast();
            }
        }

        #endregion
    }
}