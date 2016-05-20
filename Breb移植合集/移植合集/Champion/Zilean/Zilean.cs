using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using Damage = LeagueSharp.Common.Damage;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace ElZilean
{
    internal enum Spells
    {
        Q,

        W,

        E,

        R
    }

    internal class Zilean
    {
        #region Public Properties

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        #endregion

        #region Static Fields

        public static Menu
            comboMenu = ZileanMenu.comboMenu,
            harassMenu = ZileanMenu.harassMenu,
            clearMenu = ZileanMenu.clearMenu,
            castUltMenu = ZileanMenu.castUltMenu,
            miscMenu = ZileanMenu.miscMenu;

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

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
        {
            {Spells.Q, new Spell(SpellSlot.Q, 900)},
            {Spells.W, new Spell(SpellSlot.W, 0)},
            {Spells.E, new Spell(SpellSlot.E, 700)},
            {Spells.R, new Spell(SpellSlot.R, 900)}
        };

        private static SpellSlot ignite;

        #endregion

        #region Public Methods and Operators

        public static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.CharData.BaseSkinName != "Zilean")
            {
                return;
            }

            spells[Spells.Q].SetSkillshot(0.3f, 210f, 2000f, false, SkillshotType.SkillshotCircle);
            ignite = Player.GetSpellSlot("summonerdot");

            ZileanMenu.Initialize();
            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += Drawings.Drawing_OnDraw;
            Orbwalker.OnPreAttack += OrbwalkingBeforeAttack;

            comboMenu = ZileanMenu.comboMenu;
            harassMenu = ZileanMenu.harassMenu;
            clearMenu = ZileanMenu.clearMenu;
            castUltMenu = ZileanMenu.castUltMenu;
            miscMenu = ZileanMenu.miscMenu;
        }

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            try
            {
                float damage = 0;

                if (!Orbwalker.IsAutoAttacking)
                {
                    damage += ObjectManager.Player.GetAutoAttackDamage(enemy, true);
                }

                if (spells[Spells.Q].IsReady())
                {
                    damage += spells[Spells.Q].GetDamage(enemy);
                }

                return damage;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return 0;
        }

        #endregion

        #region Methods

        private static void Combo()
        {
            var qTarget =
                HeroManager.Enemies.Find(x => x.HasBuff("ZileanQEnemyBomb") && x.IsValidTarget(spells[Spells.Q].Range));
            var target = qTarget ?? TargetSelector.GetTarget(spells[Spells.Q].Range, DamageType.Magical);

            if (!target.IsValidTarget())
            {
                return;
            }

            Orbwalker.ForcedTarget = target;

            if (getCheckBoxItem(comboMenu, "ElZilean.Combo.E") && spells[Spells.E].IsReady()
                && target.IsValidTarget(spells[Spells.E].Range))
            {
                if (!spells[Spells.Q].IsReady())
                {
                    return;
                }
                spells[Spells.E].Cast(target);
            }

            var zileanQEnemyBomb =
                HeroManager.Enemies.Find(x => x.HasBuff("ZileanQEnemyBomb") && x.IsValidTarget(spells[Spells.Q].Range));
            if (getCheckBoxItem(comboMenu, "ElZilean.Combo.Q") && spells[Spells.Q].IsReady()
            && target.IsValidTarget(spells[Spells.Q].Range) && !target.CanMove)
            {
                var pred = spells[Spells.Q].GetPrediction(target);
                if (pred.Hitchance >= HitChance.High)
                {
                    spells[Spells.Q].Cast(target);
                }
            }
            else if (getCheckBoxItem(comboMenu, "ElZilean.Combo.Q") && spells[Spells.Q].IsReady()
                && target.IsValidTarget(spells[Spells.Q].Range))
            {
                var pred = spells[Spells.Q].GetPrediction(target);
                if (pred.Hitchance >= HitChance.High)
                {
                    spells[Spells.Q].Cast(pred.CastPosition);
                }
            }

            if (getCheckBoxItem(comboMenu, "ElZilean.Combo.W") && zileanQEnemyBomb != null)
            {
                Utility.DelayAction.Add(100, () => { spells[Spells.W].Cast(); });
            }

            if (getCheckBoxItem(comboMenu, "ElZilean.Combo.Ignite") && target.IsValidTarget(600f) &&
                IgniteDamage(target) >= target.Health)
            {
                Player.Spellbook.CastSpell(ignite, target);
            }
        }

        private static void Flee()
        {
            Orbwalker.OrbwalkTo(Game.CursorPos);

            if (spells[Spells.E].IsReady())
            {
                spells[Spells.E].Cast();
            }

            if (spells[Spells.W].IsReady())
            {
                spells[Spells.W].Cast();
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, DamageType.Magical);
            if (target == null)
            {
                return;
            }
            if (getCheckBoxItem(harassMenu, "ElZilean.Harass.Q") && spells[Spells.Q].IsReady()
                && target.IsValidTarget(spells[Spells.Q].Range) && !target.CanMove)
            {
                var pred = spells[Spells.Q].GetPrediction(target);
                if (pred.Hitchance >= HitChance.High)
                {
                    spells[Spells.Q].Cast(target);
                }
            }
            else if (getCheckBoxItem(harassMenu, "ElZilean.Harass.Q") && spells[Spells.Q].IsReady()
                && target.IsValidTarget(spells[Spells.Q].Range))
            {
                var pred = spells[Spells.Q].GetPrediction(target);
                if (pred.Hitchance >= HitChance.High)
                {
                    spells[Spells.Q].Cast(pred.CastPosition);
                }
            }

            if (getCheckBoxItem(harassMenu, "ElZilean.Harass.E") && spells[Spells.E].IsReady()
                && target.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast(target);
            }
        }

        private static float IgniteDamage(AIHeroClient target)
        {
            if (ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private static void LaneClear()
        {
            var minion = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.Q].Range).FirstOrDefault();
            if (minion == null)
            {
                return;
            }

            var bestFarmLocation =
                MinionManager.GetBestCircularFarmLocation(
                    MinionManager.GetMinions(spells[Spells.Q].Range).Select(m => m.ServerPosition.To2D()).ToList(),
                    spells[Spells.Q].Width,
                    spells[Spells.Q].Range);

            if (getCheckBoxItem(clearMenu, "ElZilean.Clear.Q") && minion.IsValidTarget() && spells[Spells.Q].IsReady())
            {
                spells[Spells.Q].Cast(bestFarmLocation.Position);
            }

            if (getCheckBoxItem(clearMenu, "ElZilean.Clear.W") && !spells[Spells.Q].IsReady())
            {
                spells[Spells.W].Cast();
            }
        }

        private static void OnGameUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

            UltAlly();
            SelfUlt();

            if (getKeyBindItem(miscMenu, "FleeActive") || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();
            }

            if (getKeyBindItem(harassMenu, "ElZilean.AutoHarass"))
            {
                var target = TargetSelector.GetTarget(spells[Spells.Q].Range, DamageType.Magical);
                if (!target.IsValidTarget())
                {
                    return;
                }

                if (Player.ManaPercent <= getSliderItem(harassMenu, "ElZilean.harass.mana"))
                {
                    return;
                }

                if (getCheckBoxItem(harassMenu, "ElZilean.UseQAutoHarass") && spells[Spells.Q].IsReady()
                    && target.IsValidTarget(spells[Spells.Q].Range))
                {
                    var prediction = spells[Spells.Q].GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.VeryHigh)
                    {
                        spells[Spells.Q].Cast(target);
                    }
                }

                if (getCheckBoxItem(harassMenu, "ElZilean.UseEAutoHarass") && spells[Spells.E].IsReady()
                    && target.IsValidTarget(spells[Spells.E].Range))
                {
                    spells[Spells.E].Cast(target);
                }
            }
        }

        private static void OrbwalkingBeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (getCheckBoxItem(miscMenu, "AA.Block") && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                args.Process = false;
            }
            else
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    args.Process = !(spells[Spells.Q].IsReady() || Player.LSDistance(args.Target) >= 1000);
                }
            }

            if (getCheckBoxItem(miscMenu, "ElZilean.SupportMode"))
            {
                if (args.Target is Obj_AI_Minion)
                {
                    args.Process = false;
                }
            }
        }

        private static void SelfUlt()
        {
            if (Player.IsRecalling() || Player.InFountain() || Player.IsInvulnerable ||
                Player.HasBuffOfType(BuffType.SpellImmunity)
                || Player.HasBuffOfType(BuffType.Invulnerability))
            {
                return;
            }

            var useSelftHp = getSliderItem(castUltMenu, "ElZilean.HP");
            if (getCheckBoxItem(castUltMenu, "ElZilean.R") && Player.Health / Player.MaxHealth * 100 <= useSelftHp
                && spells[Spells.R].IsReady() && Player.CountEnemiesInRange(650) > 0)
            {
                spells[Spells.R].Cast(Player);
            }
        }

        private static void UltAlly()
        {
            if (Player.IsRecalling() || Player.InFountain())
            {
                return;
            }

            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly && !x.IsMe))
            {
                if (getCheckBoxItem(castUltMenu, "ElZilean.useult")
                    && (hero.Health / hero.MaxHealth * 100
                        <= getSliderItem(castUltMenu, "ElZilean.Ally.HP"))
                    && spells[Spells.R].IsReady() && Player.CountEnemiesInRange(1000) > 0
                    && (hero.LSDistance(Player.ServerPosition) <= spells[Spells.R].Range))
                {
                    if (castUltMenu["ElZilean.Cast.Ult.Ally" + hero.CharData.BaseSkinName] != null &&
                        getCheckBoxItem(castUltMenu, "ElZilean.Cast.Ult.Ally" + hero.CharData.BaseSkinName))
                    {
                        if (hero.IsInvulnerable || hero.HasBuffOfType(BuffType.SpellImmunity) ||
                            hero.HasBuffOfType(BuffType.Invulnerability))
                        {
                            return;
                        }
                        spells[Spells.R].Cast(hero);
                    }
                }
            }
        }

        #endregion
    }
}
