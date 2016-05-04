using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;

namespace ElRengarRevamped
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ActiveModes : Standards
    {
        #region Public Methods and Operators

        public static void Combo()
        {
            try
            {
                var target = TargetSelector.SelectedTarget ??
                             TargetSelector.GetTarget(spells[Spells.E].Range, DamageType.Physical);
                if (target.IsValidTarget() == false)
                {
                    return;
                }

                #region RengarR

                if (Ferocity <= 4)
                {
                    if (spells[Spells.Q].IsReady() && MenuInit.getCheckBoxItem(MenuInit.comboMenu, "Combo.Use.Q")
                        && Player.Distance(target) <= spells[Spells.Q].Range)
                    {
                        spells[Spells.Q].Cast();
                    }

                    if (!RengarR)
                    {
                        CastItems(target);

                        if (!HasPassive)
                        {
                            if (spells[Spells.E].IsReady() &&
                                MenuInit.getCheckBoxItem(MenuInit.comboMenu, "Combo.Use.E"))
                            {
                                CastE(target);
                            }
                        }
                        else
                        {
                            if (spells[Spells.E].IsReady() &&
                                MenuInit.getCheckBoxItem(MenuInit.comboMenu, "Combo.Use.E"))
                            {
                                if (!Player.IsDashing())
                                {
                                    return;
                                }

                                CastE(target);
                            }
                        }
                    }

                    if (spells[Spells.W].IsReady() && MenuInit.getCheckBoxItem(MenuInit.comboMenu, "Combo.Use.W"))
                    {
                        CastW();
                    }
                }

                if (Ferocity == 5)
                {
                    switch (MenuInit.getBoxItem(MenuInit.comboMenu, "Combo.Prio"))
                    {
                        case 0:
                            if (!RengarR)
                            {
                                if (spells[Spells.E].IsReady() && !HasPassive)
                                {
                                    CastE(target);

                                    if (MenuInit.getCheckBoxItem(MenuInit.comboMenu, "Combo.Switch.E") &&
                                        Environment.TickCount - Rengar.LastE >= 500
                                        && Utils.GameTimeTickCount - LastSwitch >= 350)
                                    {
                                        //MenuInit.Menu.Item("Combo.Prio").SetValue(new StringList(new[] { "E", "W", "Q" }, 2));
                                        MenuInit.comboMenu["Combo.Prio"].Cast<ComboBox>().CurrentValue = 2;
                                        LastSwitch = Utils.GameTimeTickCount;
                                    }
                                }
                            }
                            else
                            {
                                if (spells[Spells.E].IsReady() &&
                                    MenuInit.getCheckBoxItem(MenuInit.comboMenu, "Combo.Use.E"))
                                {
                                    if (!Player.IsDashing())
                                    {
                                        return;
                                    }

                                    CastE(target);
                                }
                            }
                            break;
                        case 1:
                            if (MenuInit.getCheckBoxItem(MenuInit.comboMenu, "Combo.Use.W") &&
                                spells[Spells.W].IsReady())
                            {
                                CastW();
                            }
                            break;
                        case 2:
                            if (spells[Spells.Q].IsReady() &&
                                MenuInit.getCheckBoxItem(MenuInit.comboMenu, "Combo.Use.Q") &&
                                Player.CountEnemiesInRange(Player.AttackRange + Player.BoundingRadius + 100) != 0)
                            {
                                spells[Spells.Q].Cast();
                            }
                            break;
                    }

                    if (!RengarR)
                    {
                        if (MenuInit.getCheckBoxItem(MenuInit.comboMenu, "Combo.Use.E.OutOfRange"))
                        {
                            CastE(target);
                        }
                    }
                }

                #region Summoner spells

                if (Youmuu.IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                {
                    Youmuu.Cast();
                }

                if (MenuInit.getCheckBoxItem(MenuInit.comboMenu, "Combo.Use.Smite") && !RengarR &&
                    Smite != SpellSlot.Unknown
                    && Player.Spellbook.CanUseSpell(Smite) == SpellState.Ready && !target.IsZombie)
                {
                    Player.Spellbook.CastSpell(Smite, target);
                }

                if (MenuInit.getCheckBoxItem(MenuInit.comboMenu, "Combo.Use.Ignite") && target.IsValidTarget(600f) &&
                    IgniteDamage(target) >= target.Health)
                {
                    Player.Spellbook.CastSpell(Ignite, target);
                }

                #endregion
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #endregion

        #region Methods

        private static void CastE(Obj_AI_Base target)
        {
            try
            {
                if (!spells[Spells.E].IsReady() || !target.IsValidTarget(spells[Spells.E].Range))
                {
                    return;
                }

                var prediction = spells[Spells.E].GetPrediction(target);
                if (prediction.Hitchance >= HitChance.High)
                {
                    spells[Spells.E].Cast(target);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void CastW()
        {
            try
            {
                if (!spells[Spells.W].IsReady() || Environment.TickCount - Rengar.LastE <= 200)
                {
                    return;
                }

                if (GetWHits().Item1 > 0)
                {
                    spells[Spells.W].Cast();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static Tuple<int, List<AIHeroClient>> GetWHits()
        {
            try
            {
                var hits =
                    HeroManager.Enemies.Where(
                        e =>
                            e.IsValidTarget() && e.Distance(Player) < 450f
                            || e.Distance(Player) < 450f && e.IsFacing(Player)).ToList();

                return new Tuple<int, List<AIHeroClient>>(hits.Count, hits);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return new Tuple<int, List<AIHeroClient>>(0, null);
        }

        #endregion

        public static void Harass()
        {
            // ReSharper disable once ConvertConditionalTernaryToNullCoalescing
            var target = TargetSelector.SelectedTarget != null
                ? TargetSelector.SelectedTarget
                : TargetSelector.GetTarget(spells[Spells.Q].Range, DamageType.Physical);

            if (target.IsValidTarget() == false)
            {
                return;
            }

            #region RengarR

            if (Ferocity == 5)
            {
                switch (MenuInit.getBoxItem(MenuInit.harassMenu, "Harass.Prio"))
                {
                    case 0:
                        if (!HasPassive && MenuInit.getCheckBoxItem(MenuInit.harassMenu, "Harass.Use.E") &&
                            spells[Spells.E].IsReady())
                        {
                            CastE(target);
                        }
                        break;

                    case 1:
                        if (MenuInit.getCheckBoxItem(MenuInit.harassMenu, "Harass.Use.Q") &&
                            target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast();
                        }
                        break;
                }
            }

            if (Ferocity <= 4)
            {
                if (MenuInit.getCheckBoxItem(MenuInit.harassMenu, "Harass.Use.Q") &&
                    target.IsValidTarget(spells[Spells.Q].Range))
                {
                    spells[Spells.Q].Cast();
                }

                if (RengarR)
                {
                    return;
                }

                CastItems(target);

                if (!HasPassive && MenuInit.getCheckBoxItem(MenuInit.harassMenu, "Harass.Use.E") &&
                    spells[Spells.E].IsReady())
                {
                    CastE(target);
                }

                if (MenuInit.getCheckBoxItem(MenuInit.harassMenu, "Harass.Use.W"))
                {
                    CastW();
                }
            }
        }

        public static void Jungleclear()
        {
            try
            {
                var minion =
                    MinionManager.GetMinions(
                        Player.ServerPosition,
                        spells[Spells.W].Range,
                        MinionTypes.All,
                        MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth).FirstOrDefault();

                if (minion == null)
                {
                    return;
                }

                CastItems(minion);

                if (Ferocity == 5 && MenuInit.getCheckBoxItem(MenuInit.jungleClear, "Jungle.Save.Ferocity"))
                {
                    if (minion.IsValidTarget(spells[Spells.W].Range) && !HasPassive)
                    {
                        CastItems(minion);
                    }
                    return;
                }

                if (MenuInit.getCheckBoxItem(MenuInit.jungleClear, "Jungle.Use.Q") && spells[Spells.Q].IsReady()
                    && minion.IsValidTarget(spells[Spells.Q].Range + 100))
                {
                    spells[Spells.Q].Cast();
                }

                if (MenuInit.getCheckBoxItem(MenuInit.jungleClear, "Jungle.Use.W") && spells[Spells.W].IsReady()
                    && minion.IsValidTarget(spells[Spells.W].Range) && !HasPassive)
                {
                    spells[Spells.W].Cast();
                }

                if (MenuInit.getCheckBoxItem(MenuInit.jungleClear, "Jungle.Use.E") && spells[Spells.E].IsReady()
                    && minion.IsValidTarget(spells[Spells.E].Range))
                {
                    spells[Spells.E].Cast(minion.Position);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void Laneclear()
        {
            try
            {
                var minion = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.W].Range).FirstOrDefault();
                if (minion == null)
                {
                    return;
                }

                if (Player.Spellbook.IsAutoAttacking || Orbwalker.IsAutoAttacking)
                {
                    return;
                }
                if (Ferocity == 5 && MenuInit.getCheckBoxItem(MenuInit.laneClear, "Clear.Save.Ferocity"))
                {
                    if (minion.IsValidTarget(spells[Spells.W].Range))
                    {
                        CastItems(minion);
                    }
                    return;
                }

                if (MenuInit.getCheckBoxItem(MenuInit.laneClear, "Clear.Use.Q") && spells[Spells.Q].IsReady()
                    && minion.IsValidTarget(spells[Spells.Q].Range))
                {
                    spells[Spells.Q].Cast();
                }

                if (MenuInit.getCheckBoxItem(MenuInit.laneClear, "Clear.Use.W") && spells[Spells.W].IsReady()
                    && minion.IsValidTarget(spells[Spells.W].Range))
                {
                    CastItems(minion);
                    spells[Spells.W].Cast();
                }

                if (MenuInit.getCheckBoxItem(MenuInit.laneClear, "Clear.Use.E") &&
                    spells[Spells.E].GetDamage(minion) > minion.Health
                    && spells[Spells.E].IsReady() && minion.IsValidTarget(spells[Spells.E].Range))
                {
                    spells[Spells.E].Cast(minion.Position);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        ///     Gets Youmuus Ghostblade
        /// </summary>
        /// <value>
        ///     Youmuus Ghostblade
        /// </value>
        private new static Items.Item Youmuu
        {
            get { return ItemData.Youmuus_Ghostblade.GetItem(); }
        }

        /// <summary>
        ///     Gets Ravenous Hydra
        /// </summary>
        /// <value>
        ///     Ravenous Hydra
        /// </value>
        private static Items.Item Hydra
        {
            get { return ItemData.Ravenous_Hydra_Melee_Only.GetItem(); }
        }

        /// <summary>
        ///     Gets Tiamat Item
        /// </summary>
        /// <value>
        ///     Tiamat Item
        /// </value>
        private static Items.Item Tiamat
        {
            get { return ItemData.Tiamat_Melee_Only.GetItem(); }
        }

        /// <summary>
        ///     Gets Titanic Hydra
        /// </summary>
        /// <value>
        ///     Titanic Hydra
        /// </value>
        private static Items.Item Titanic
        {
            get { return ItemData.Titanic_Hydra_Melee_Only.GetItem(); }
        }

        public static bool CastItems(Obj_AI_Base target)
        {
            if (Player.IsDashing() || Orbwalker.IsAutoAttacking || RengarR)
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
            if (Youmuu.IsReady() && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && youmuus.Cast())
            {
                return true;
            }

            var titanic = Titanic;
            return titanic.IsReady() && count > 0 && titanic.Cast();
        }

        #endregion
    }
}