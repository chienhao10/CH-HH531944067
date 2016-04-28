using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;
using Spell = LeagueSharp.Common.Spell;

namespace Elvarus
{
    internal enum Spells
    {
        Q,

        W,

        E,

        R
    }

    internal class Varus
    {
        #region Static Fields

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
        {
            {Spells.Q, new Spell(SpellSlot.Q, 925)},
            {Spells.W, new Spell(SpellSlot.W, 0)},
            {Spells.E, new Spell(SpellSlot.E, 925)},
            {Spells.R, new Spell(SpellSlot.R, 1100)}
        };

        #endregion

        #region Public Properties

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        #endregion

        #region Public Methods and Operators

        public static Menu
            Menu = ElVarusMenu.Menu,
            cMenu = ElVarusMenu.cMenu,
            hMenu = ElVarusMenu.hMenu,
            itemMenu = ElVarusMenu.itemMenu,
            lMenu = ElVarusMenu.lMenu,
            miscMenu = ElVarusMenu.miscMenu;

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

        public static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != "Varus")
            {
                return;
            }

            spells[Spells.Q].SetSkillshot(.25f, 70f, 1650f, false, SkillshotType.SkillshotLine);
            spells[Spells.E].SetSkillshot(0.35f, 120, 1500, false, SkillshotType.SkillshotCircle);
            spells[Spells.R].SetSkillshot(.25f, 120f, 1950f, false, SkillshotType.SkillshotLine);

            spells[Spells.Q].SetCharged("VarusQ", "VarusQ", 250, 1600, 1.2f);

            ElVarusMenu.Initialize();
            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += Drawings.Drawing_OnDraw;
        }

        #endregion

        #region Methods

        private static void Combo()
        {
            var wTarget =
                HeroManager.Enemies.Find(
                    x => x.HasBuff("varuswdebuff") && x.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player)));
            var target = wTarget
                         ?? TargetSelector.GetTarget(
                             spells[Spells.Q].ChargedMaxRange,
                             DamageType.Physical);

            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            if (wTarget != null && getCheckBoxItem(cMenu, "ElVarus.Combo.W.Focus"))
            {
                Orbwalker.ForcedTarget = target;
            }

            var stackCount = getSliderItem(cMenu, "ElVarus.Combo.Stack.Count");
            var rCount = getSliderItem(cMenu, "ElVarus.Combo.R.Count");
            var comboQ = getCheckBoxItem(cMenu, "ElVarus.Combo.Q");
            var comboE = getCheckBoxItem(cMenu, "ElVarus.Combo.E");
            var comboR = getCheckBoxItem(cMenu, "ElVarus.Combo.R");
            var alwaysQ = getCheckBoxItem(cMenu, "ElVarus.combo.always.Q");

            if (comboE && spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast(target);
            }

            Items(target);

            if (spells[Spells.Q].IsReady() && comboQ && target.IsValidTarget(spells[Spells.Q].ChargedMaxRange))
            {
                if (alwaysQ)
                {
                    spells[Spells.Q].StartCharging();
                }
                else if (spells[Spells.W].Level == 0 || GetStacksOn(target) >= stackCount ||
                         spells[Spells.Q].GetDamage(target) > target.Health)
                {
                    spells[Spells.Q].StartCharging();
                }

                if (spells[Spells.Q].IsCharging)
                {
                    spells[Spells.Q].Cast(target);
                }
            }

            if (IsQKillable(target))
            {
                if (!spells[Spells.Q].IsCharging)
                {
                    spells[Spells.Q].StartCharging();
                }

                if (spells[Spells.Q].IsCharging)
                {
                    spells[Spells.Q].Cast(target);
                }
            }

            if (comboR && Player.CountEnemiesInRange(spells[Spells.R].Range) >= rCount && spells[Spells.R].IsReady()
                && target.IsValidTarget(spells[Spells.R].Range))
            {
                spells[Spells.R].CastOnBestTarget();
            }
        }

        private static double GetExecuteDamage(Obj_AI_Base target)
        {
            if (spells[Spells.Q].IsReady())
            {
                return spells[Spells.Q].GetDamage(target) + Player.TotalAttackDamage;
            }

            return 0;
        }

        private static float GetHealth(Obj_AI_Base target)
        {
            return target.Health;
        }

        private static int GetStacksOn(Obj_AI_Base target)
        {
            return
                target.Buffs.Where(
                    xBuff => xBuff.Name == "varuswdebuff" && target.IsValidTarget(spells[Spells.Q].ChargedMaxRange))
                    .Select(xBuff => xBuff.Count)
                    .FirstOrDefault();
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].ChargedMaxRange, DamageType.Physical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            var harassQ = getCheckBoxItem(hMenu, "ElVarus.Harass.Q");
            var harassE = getCheckBoxItem(hMenu, "ElVarus.Harass.E");
            var minmana = getSliderItem(hMenu, "minmanaharass");

            if (Player.ManaPercent > minmana)
            {
                if (harassE && spells[Spells.E].IsReady())
                {
                    spells[Spells.E].CastOnBestTarget();
                }

                if (harassQ)
                {
                    if (!spells[Spells.Q].IsCharging)
                    {
                        spells[Spells.Q].StartCharging();
                    }

                    if (spells[Spells.Q].IsReady())
                    {
                        var prediction = spells[Spells.Q].GetPrediction(target);
                        var distance =
                            Player.ServerPosition.Distance(
                                prediction.UnitPosition
                                + 200*(prediction.UnitPosition - Player.ServerPosition).Normalized(),
                                true);
                        if (distance < spells[Spells.Q].RangeSqr)
                        {
                            if (spells[Spells.Q].Cast(prediction.CastPosition))
                            {
                            }
                        }
                    }
                }
            }
        }

        private static bool IsQKillable(Obj_AI_Base target)
        {
            var hero = target as AIHeroClient;
            return GetExecuteDamage(target) > GetHealth(target) && (hero == null);
        }

        private static void Items(Obj_AI_Base target)
        {
            var botrk = ItemData.Blade_of_the_Ruined_King.GetItem();
            var ghost = ItemData.Youmuus_Ghostblade.GetItem();
            var cutlass = ItemData.Bilgewater_Cutlass.GetItem();

            var useYoumuu = getCheckBoxItem(itemMenu, "ElVarus.Items.Youmuu");
            var useCutlass = getCheckBoxItem(itemMenu, "ElVarus.Items.Cutlass");
            var useBlade = getCheckBoxItem(itemMenu, "ElVarus.Items.Blade");

            var useBladeEhp = getSliderItem(itemMenu, "ElVarus.Items.Blade.EnemyEHP");
            var useBladeMhp = getSliderItem(itemMenu, "ElVarus.Items.Blade.EnemyMHP");

            if (botrk.IsReady() && botrk.IsOwned(Player) && botrk.IsInRange(target)
                && target.HealthPercent <= useBladeEhp && useBlade)
            {
                botrk.Cast(target);
            }

            if (botrk.IsReady() && botrk.IsOwned(Player) && botrk.IsInRange(target)
                && Player.HealthPercent <= useBladeMhp && useBlade)
            {
                botrk.Cast(target);
            }

            if (cutlass.IsReady() && cutlass.IsOwned(Player) && cutlass.IsInRange(target)
                && target.HealthPercent <= useBladeEhp && useCutlass)
            {
                cutlass.Cast(target);
            }

            if (ghost.IsReady() && ghost.IsOwned(Player) && target.IsValidTarget(spells[Spells.Q].Range) && useYoumuu)
            {
                ghost.Cast();
            }
        }

        private static void JungleClear()
        {
            var useQ = getCheckBoxItem(lMenu, "useQFarmJungle");
            var useE = getCheckBoxItem(lMenu, "useEFarmJungle");
            var minmana = getSliderItem(lMenu, "minmanaclear");
            var minions = MinionManager.GetMinions(
                Player.ServerPosition,
                700,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            if (Player.ManaPercent >= minmana)
            {
                foreach (var minion in minions)
                {
                    if (spells[Spells.Q].IsReady() && useQ)
                    {
                        if (!spells[Spells.Q].IsCharging)
                        {
                            spells[Spells.Q].StartCharging();
                        }

                        if (spells[Spells.Q].IsCharging && spells[Spells.Q].Range >= spells[Spells.Q].ChargedMaxRange)
                            spells[Spells.Q].Cast(minion);
                    }

                    if (spells[Spells.E].IsReady() && useE)
                    {
                        spells[Spells.E].CastOnUnit(minion);
                    }
                }
            }
        }

        //Credits to God :cat_lazy:
        private static void Killsteal()
        {
            if (getCheckBoxItem(miscMenu, "ElVarus.KSSS") && spells[Spells.Q].IsReady())
            {
                foreach (
                    var target in
                        HeroManager.Enemies.Where(
                            enemy =>
                                enemy.IsValidTarget() && IsQKillable(enemy) &&
                                Player.Distance(enemy.Position) <= spells[Spells.Q].ChargedMaxRange))
                {
                    spells[Spells.Q].StartCharging();

                    if (spells[Spells.Q].IsCharging)
                    {
                        Orbwalker.DisableAttacking = true;
                        if (IsQKillable(target) && !target.IsInvulnerable)
                        {
                            spells[Spells.Q].Cast(target);
                        }
                    }
                    else
                    {
                        Orbwalker.DisableAttacking = false;
                    }
                }
            }
        }

        private static void LaneClear()
        {
            var useQ = getCheckBoxItem(lMenu, "useQFarm");
            var useE = getCheckBoxItem(lMenu, "useEFarm");
            var countMinions = getSliderItem(lMenu, "ElVarus.Count.Minions");
            var countMinionsE = getSliderItem(lMenu, "ElVarus.Count.Minions.E");
            var minmana = getSliderItem(lMenu, "minmanaclear");

            if (Player.ManaPercent < minmana)
            {
                return;
            }

            var minions = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.Q].Range);
            if (minions.Count <= 0)
            {
                return;
            }

            if (spells[Spells.Q].IsReady() && useQ)
            {
                var allMinions = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.Q].Range);
                {
                    foreach (var minion in
                        allMinions.Where(
                            minion => minion.Health <= Player.GetSpellDamage(minion, SpellSlot.Q)))
                    {
                        var killcount = 0;

                        foreach (var colminion in minions)
                        {
                            if (colminion.Health <= spells[Spells.Q].GetDamage(colminion))
                            {
                                killcount++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (killcount >= countMinions)
                        {
                            if (minion.IsValidTarget())
                            {
                                spells[Spells.Q].Cast(minion);
                                return;
                            }
                        }
                    }
                }
            }

            if (!useE || !spells[Spells.E].IsReady())
            {
                return;
            }

            var minionkillcount =
                minions.Count(x => spells[Spells.E].CanCast(x) && x.Health <= spells[Spells.E].GetDamage(x));

            if (minionkillcount >= countMinionsE)
            {
                foreach (var minion in minions.Where(x => x.Health <= spells[Spells.E].GetDamage(x)))
                {
                    spells[Spells.E].Cast(minion);
                }
            }
        }

        private static void OnGameUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
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

            Killsteal();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (Player.Buffs.Count(buf => buf.Name == "Muramana") == 0)
                {
                    var muramana = ItemData.Muramana.GetItem();
                    if (muramana.IsOwned(Player))
                    {
                        muramana.Cast();
                    }
                }
            }
            else
            {
                if (Player.Buffs.Count(buf => buf.Name == "Muramana") != 0)
                {
                    var muramana = ItemData.Muramana.GetItem();
                    if (muramana.IsOwned(Player))
                    {
                        muramana.Cast();
                    }
                }
            }

            var target = TargetSelector.GetTarget(spells[Spells.R].Range, DamageType.Physical);

            if (spells[Spells.R].IsReady() && target.IsValidTarget()
                && getKeyBindItem(cMenu, "ElVarus.SemiR"))
            {
                spells[Spells.R].CastOnUnit(target);
            }
        }

        #endregion
    }
}