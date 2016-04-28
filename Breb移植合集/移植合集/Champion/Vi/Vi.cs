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

namespace ElVi
{
    internal enum Spells
    {
        Q,
        W,
        E,
        R
    }

    internal static class Vi
    {
        private static SpellSlot _ignite;
        private static SpellSlot _flash;
        private static AIHeroClient _qTargetLock;

        public static Menu
            _menu = ElViMenu._menu,
            cMenu = ElViMenu.cMenu,
            hMenu = ElViMenu.hMenu,
            rMenu = ElViMenu.rMenu,
            clearMenu = ElViMenu.clearMenu,
            miscMenu = ElViMenu.miscMenu;

        public static readonly Dictionary<Spells, Spell> Spells = new Dictionary<Spells, Spell>
        {
            {ElVi.Spells.Q, new Spell(SpellSlot.Q, 800)},
            {ElVi.Spells.W, new Spell(SpellSlot.W)},
            {ElVi.Spells.E, new Spell(SpellSlot.E, 600)},
            {ElVi.Spells.R, new Spell(SpellSlot.R, 800)}
        };

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
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

        public static void OnLoad()
        {
            if (ObjectManager.Player.CharData.BaseSkinName != "Vi")
            {
                return;
            }

            _ignite = Player.GetSpellSlot("summonerdot");
            _flash = Player.GetSpellSlot("SummonerFlash");

            Spells[ElVi.Spells.Q].SetSkillshot(Spells[ElVi.Spells.Q].Instance.SData.SpellCastTime,
                Spells[ElVi.Spells.Q].Instance.SData.LineWidth, Spells[ElVi.Spells.Q].Instance.SData.MissileSpeed, false,
                SkillshotType.SkillshotLine);
            Spells[ElVi.Spells.Q].SetCharged("ViQ", "ViQ", 100, 860, 1f);
            Spells[ElVi.Spells.E].SetSkillshot(Spells[ElVi.Spells.E].Instance.SData.SpellCastTime,
                Spells[ElVi.Spells.E].Instance.SData.LineWidth, Spells[ElVi.Spells.E].Instance.SData.MissileSpeed, false,
                SkillshotType.SkillshotLine);
            Spells[ElVi.Spells.R].SetTargetted(0.15f, 1500f);

            ElViMenu.Initialize();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += Drawings.Drawing_OnDraw;
            Orbwalker.OnPostAttack += OrbwalkingAfterAttack;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }

        private static void OrbwalkingAfterAttack(AttackableUnit target, EventArgs args)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                return;
            }

            var useE = getCheckBoxItem(cMenu, "ElVi.Combo.E");

            if (useE)
            {
                Spells[ElVi.Spells.E].Cast();
            }

            Orbwalker.ResetAutoAttack();
        }

        #region OnGameUpdate

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                var target = TargetSelector.GetTarget(Spells[ElVi.Spells.Q].Range, DamageType.Physical);

                if (_qTargetLock == null)
                {
                    _qTargetLock = target;
                    OnCombo(_qTargetLock);
                }
                else
                {
                    _qTargetLock = null;
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                OnLaneClear();
                OnJungleClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                OnHarass();
            }

            if (getKeyBindItem(cMenu, "ElVi.Combo.Flash"))
            {
                FlashQ();
            }
        }

        #endregion

        #region OnJungleClear

        private static void OnJungleClear()
        {
            var useQ = getCheckBoxItem(clearMenu, "ElVi.JungleClear.Q");
            var useE = getCheckBoxItem(clearMenu, "ElVi.JungleClear.E");
            var playerMana = getSliderItem(clearMenu, "ElVi.Clear.Player.Mana");

            if (Player.ManaPercent < playerMana)
                return;

            var minions = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition, Spells[ElVi.Spells.E].Range, MinionTypes.All, MinionTeam.Neutral,
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
                    if (minions.Count == minions.Count(x => Player.Distance(x) < Spells[ElVi.Spells.Q].Range))
                    {
                        Spells[ElVi.Spells.Q].Cast(minions[0]);
                    }
                }
            }

            if (useE && Spells[ElVi.Spells.E].IsReady())
            {
                var bestFarmPos = Spells[ElVi.Spells.E].GetLineFarmLocation(minions);
                if (minions.Count == minions.Count(x => Player.Distance(x) < Spells[ElVi.Spells.E].Range) &&
                    bestFarmPos.Position.IsValid() && bestFarmPos.MinionsHit > 1)
                {
                    Spells[ElVi.Spells.E].Cast();
                }
            }
        }

        #endregion

        #region OnLaneClear

        private static void OnLaneClear()
        {
            var useQ = getCheckBoxItem(clearMenu, "ElVi.LaneClear.Q");
            var useE = getCheckBoxItem(clearMenu, "ElVi.LaneClear.E");
            var playerMana = getSliderItem(clearMenu, "ElVi.Clear.Player.Mana");

            if (Player.ManaPercent < playerMana)
                return;

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
                    if (minions.Count == minions.Count(x => Player.Distance(x) < Spells[ElVi.Spells.Q].Range) &&
                        bestFarmPos.Position.IsValid() && bestFarmPos.MinionsHit > 2)
                    {
                        Spells[ElVi.Spells.Q].Cast(bestFarmPos.Position);
                    }
                }
            }

            if (useE && Spells[ElVi.Spells.E].IsReady())
            {
                var bestFarmPos = Spells[ElVi.Spells.E].GetLineFarmLocation(minions);
                if (minions.Count == minions.Count(x => Player.Distance(x) < Spells[ElVi.Spells.E].Range) &&
                    bestFarmPos.Position.IsValid() && bestFarmPos.MinionsHit > 1)
                {
                    Spells[ElVi.Spells.E].Cast();
                }
            }
        }

        #endregion

        #region Harass

        private static void OnHarass()
        {
            var target = TargetSelector.GetTarget(
                Spells[ElVi.Spells.Q].Range, DamageType.Physical);

            _qTargetLock = _qTargetLock == null ? target : null;

            if (target == null || !target.IsValid)
            {
                return;
            }

            var useQ = getCheckBoxItem(hMenu, "ElVi.Harass.Q");
            var useE = getCheckBoxItem(hMenu, "ElVi.Harass.E");


            if (useQ && Spells[ElVi.Spells.Q].IsReady())
            {
                if (Spells[ElVi.Spells.Q].IsCharging)
                {
                    Spells[ElVi.Spells.Q].Cast(target);
                    return;
                }

                if (!Spells[ElVi.Spells.Q].IsCharging)
                {
                    Spells[ElVi.Spells.Q].StartCharging();
                    return;
                }
            }

            if (useE && Spells[ElVi.Spells.E].IsReady())
            {
                Spells[ElVi.Spells.E].Cast();
            }
        }

        #endregion

        #region Combo

        private static void OnCombo(AIHeroClient target)
        {
            /* var target = TargetSelector.GetTarget(Spells[ElVi.Spells.Q].Range, TargetSelector.DamageType.Physical);
            if (target == null || !target.IsValid)
                return;*/

            if (_qTargetLock != null)
            {
                target = _qTargetLock;
            }
            else
            {
                _qTargetLock = target;
            }

            if (target == null || !target.IsValid)
            {
                return;
            }

            var useQ = getCheckBoxItem(cMenu, "ElVi.Combo.Q");
            var useR = getCheckBoxItem(cMenu, "ElVi.Combo.R");
            var useI = getCheckBoxItem(cMenu, "ElVi.Combo.I");

            if (useQ && Spells[ElVi.Spells.Q].IsReady())
            {
                if (Spells[ElVi.Spells.Q].IsCharging)
                {
                    Spells[ElVi.Spells.Q].Cast(target);
                    return;
                }

                if (!Spells[ElVi.Spells.Q].IsCharging)
                {
                    Spells[ElVi.Spells.Q].StartCharging();
                    return;
                }
            }

            if (useR && Spells[ElVi.Spells.R].IsReady() && Spells[ElVi.Spells.R].IsInRange(target))
            {
                var selectedEnemy =
                    HeroManager.Enemies.Where(
                        hero =>
                            hero.IsEnemy && !hero.HasBuff("BlackShield") || !hero.HasBuff("SivirShield") ||
                            !hero.HasBuff("BansheesVeil") ||
                            !hero.HasBuff("ShroudofDarkness") &&
                            getCheckBoxItem(rMenu, "ElVi.Settings.R" + hero.BaseSkinName))
                        .OrderByDescending(x => x.MaxHealth)
                        .FirstOrDefault();

                if (selectedEnemy == null || !selectedEnemy.IsValid)
                {
                    return;
                }

                var rTarget = TargetSelector.GetTarget(Spells[ElVi.Spells.R].Range, DamageType.Physical);

                if (Spells[ElVi.Spells.R].CanCast(rTarget) &&
                    rTarget.Health <= Spells[ElVi.Spells.Q].GetDamage(rTarget)*2 + GetComboDamage(rTarget))
                {
                    Spells[ElVi.Spells.R].CastOnUnit(rTarget);
                }

                // Console.WriteLine(selectedEnemy);
                //Console.WriteLine("R Damage 1: {0}", rDamage);
                //Console.WriteLine("R Damage: {0}", Spells[ElVi.Spells.R].GetDamage(selectedEnemy));
            }


            if (Player.Distance(target) <= 600 && IgniteDamage(target) >= target.Health && useI)
            {
                Player.Spellbook.CastSpell(_ignite, target);
            }
        }

        #endregion

        #region FlashQ

        private static void FlashQ()
        {
            var target = TargetSelector.GetTarget(Spells[ElVi.Spells.Q].ChargedMaxRange, DamageType.Physical);
            if (target == null || !target.IsValid)
                return;

            var position = Spells[ElVi.Spells.Q].GetPrediction(target, true).CastPosition;

            if (Spells[ElVi.Spells.Q].IsReady())
            {
                if (!Spells[ElVi.Spells.Q].IsCharging)
                {
                    Spells[ElVi.Spells.Q].StartCharging();
                }
                else
                {
                    ObjectManager.Player.Spellbook.CastSpell(_flash, position);
                    Spells[ElVi.Spells.Q].Cast(target.ServerPosition);
                }
            }
        }

        #endregion

        #region IgniteDamage

        private static float IgniteDamage(AIHeroClient target)
        {
            if (_ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(_ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float) Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        #endregion

        #region Interrupters

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (getCheckBoxItem(miscMenu, "ElVi.misc.AntiGapCloser"))
            {
                if (Spells[ElVi.Spells.Q].IsReady() &&
                    gapcloser.Sender.Distance(Player) < Spells[ElVi.Spells.Q].Range)
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

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            var useInterrupter = getCheckBoxItem(miscMenu, "ElVi.misc.Interrupter");
            if (!useInterrupter)
                return;

            if (args.DangerLevel != Interrupter2.DangerLevel.High ||
                sender.Distance(Player) > Spells[ElVi.Spells.Q].Range)
                return;

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

        #endregion

        #region GetComboDamage   

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            if (Spells[ElVi.Spells.Q].IsReady())
            {
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q);
            }
            if (Spells[ElVi.Spells.E].IsReady())
            {
                damage += Player.GetSpellDamage(enemy, SpellSlot.E)*Spells[ElVi.Spells.E].Instance.Ammo +
                          Player.GetAutoAttackDamage(enemy);
            }

            if (Spells[ElVi.Spells.R].IsReady())
            {
                damage += Player.GetSpellDamage(enemy, SpellSlot.R);
            }

            return (float) damage;
        }

        #endregion
    }
}