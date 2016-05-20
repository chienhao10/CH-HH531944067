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
using SharpDX;
using Color = System.Drawing.Color;

namespace ElVladimirReborn
{
    internal enum Spells
    {
        Q,

        W,

        E,

        R
    }


    internal static class Vladimir
    {
        #region Properties

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        #endregion

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

        #region Public Methods and Operators

        public static void OnLoad()
        {
            if (ObjectManager.Player.CharData.BaseSkinName != "Vladimir")
            {
                return;
            }

            spells[Spells.Q].SetTargetted(0.25f, spells[Spells.Q].Instance.SData.MissileSpeed);
            spells[Spells.R].SetSkillshot(0.25f, 175, 700, false, SkillshotType.SkillshotCircle);

            ignite = Player.GetSpellSlot("summonerdot");

            ElVladimirMenu.Initialize();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += Drawings.OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        #endregion

        #region Static Fields

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
        {
            {Spells.Q, new Spell(SpellSlot.Q, 600)},
            {Spells.W, new Spell(SpellSlot.W)},
            {Spells.E, new Spell(SpellSlot.E, 600)},
            {Spells.R, new Spell(SpellSlot.R, 700)}
        };

        private static SpellSlot ignite;

        #endregion

        #region Methods

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var gapCloserActive = getCheckBoxItem(ElVladimirMenu.settingsMenu,
                "ElVladimir.Settings.AntiGapCloser.Active");

            if (gapCloserActive && spells[Spells.W].IsReady()
                && gapcloser.Sender.LSDistance(Player) < spells[Spells.W].Range
                && Player.CountEnemiesInRange(spells[Spells.Q].Range) >= 1)
            {
                spells[Spells.W].Cast();
            }
        }

        private static void AreaOfEffectUltimate()
        {
            if (getCheckBoxItem(ElVladimirMenu.comboMenu, "ElVladimir.Combo.R") && spells[Spells.R].IsReady())
            {
                var target = TargetSelector.GetTarget(spells[Spells.R].Range, DamageType.Magical);
                if (target == null)
                {
                    return;
                }

                var hits = HeroManager.Enemies.Where(x => x.LSDistance(target) <= 400f).ToList();
                if (
                    hits.Any(
                        hit =>
                        hits.Count >= getSliderItem(ElVladimirMenu.comboMenu, "ElVladimir.Combo.Count.R")))
                {
                    var pred = spells[Spells.R].GetPrediction(target);
                    spells[Spells.R].Cast(pred.CastPosition);
                    Render.Circle.DrawCircle(pred.CastPosition, 400, Color.Red);
                }
            }
        }

        private static void OnCombo()
        {
            var target = TargetSelector.GetTarget(1500, DamageType.Magical);
            if (target == null)
            {
                return;
            }

            var countEnemy = getSliderItem(ElVladimirMenu.comboMenu, "ElVladimir.Combo.Count.R");

            if (getCheckBoxItem(ElVladimirMenu.comboMenu, "ElVladimir.Combo.Q") && spells[Spells.Q].IsReady()
                && target.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].CastOnUnit(target);
            }

            if (getCheckBoxItem(ElVladimirMenu.comboMenu, "ElVladimir.Combo.E") && spells[Spells.E].IsReady() && target.IsValidTarget(800))
            {
                Orbwalker.OrbwalkTo(Game.CursorPos);
                if (Player.LSDistance(target) < 800)
                {
                    spells[Spells.E].StartCharging();
                    if (spells[Spells.E].IsCharging)
                    {
                        if (Player.LSDistance(target) >= 550)
                        {
                            spells[Spells.E].Cast();
                        }
                    }
                }
            }

            if (getCheckBoxItem(ElVladimirMenu.comboMenu, "ElVladimir.Combo.W") && spells[Spells.W].IsReady()
                && target.IsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast();
            }

            if (getCheckBoxItem(ElVladimirMenu.comboMenu, "ElVladimir.Combo.R.Killable"))
            {
                if (getCheckBoxItem(ElVladimirMenu.comboMenu, "ElVladimir.Combo.SmartUlt"))
                {
                    if (spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range)
                        && spells[Spells.Q].GetDamage(target) >= target.Health)
                    {
                        spells[Spells.Q].Cast();
                    }

                    if (spells[Spells.R].IsReady() && spells[Spells.R].GetDamage(target) >= target.Health && !target.IsDead)
                    {
                        var pred = spells[Spells.R].GetPrediction(target);
                        if (pred.Hitchance >= HitChance.VeryHigh)
                        {
                            spells[Spells.R].Cast(pred.CastPosition);
                        }
                    }
                }
            }

            if (getCheckBoxItem(ElVladimirMenu.comboMenu, "ElVladimir.Combo.Ignite") && Player.LSDistance(target) <= 600 && Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) >= target.Health)
            {
                Player.Spellbook.CastSpell(ignite, target);
            }
        }

        private static void OnHarass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, DamageType.Magical);
            if (target == null)
            {
                return;
            }

            if (getCheckBoxItem(ElVladimirMenu.harassMenu, "ElVladimir.Harass.Q") && spells[Spells.Q].IsReady()
                && target.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].CastOnUnit(target);
            }

            if (getCheckBoxItem(ElVladimirMenu.harassMenu, "ElVladimir.Harass.E") && spells[Spells.E].IsReady() && target.IsValidTarget(800))
            {
                if (Player.LSDistance(target) < 800)
                {
                    spells[Spells.E].StartCharging();
                }
            }
        }

        private static void OnJungleClear()
        {
            var useQ = getCheckBoxItem(ElVladimirMenu.clearMenu, "ElVladimir.JungleClear.Q");
            var useE = getCheckBoxItem(ElVladimirMenu.clearMenu, "ElVladimir.JungleClear.E");
            var playerHp = getSliderItem(ElVladimirMenu.clearMenu, "ElVladimir.WaveClear.Health.E");

            if (spells[Spells.Q].IsReady() && useQ)
            {
                var allMinions = MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition,
                    spells[Spells.W].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);
                {
                    foreach (var minion in
                        allMinions.Where(minion => minion.IsValidTarget()))
                    {
                        spells[Spells.Q].CastOnUnit(minion);
                        return;
                    }
                }
            }

            if (spells[Spells.E].IsReady() && Player.Health / Player.MaxHealth * 100 >= playerHp && useE)
            {
                var minions = MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition,
                    spells[Spells.W].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);
                if (minions.Count <= 0)
                {
                    return;
                }

                if (minions.Count > 1)
                {
                    spells[Spells.E].StartCharging();
                }
            }
        }

        private static void OnLaneClear()
        {
            var useQ = getCheckBoxItem(ElVladimirMenu.clearMenu, "ElVladimir.WaveClear.Q");
            var useE = getCheckBoxItem(ElVladimirMenu.clearMenu, "ElVladimir.WaveClear.E");
            var playerHp = getSliderItem(ElVladimirMenu.clearMenu, "ElVladimir.WaveClear.Health.E");

            if (spells[Spells.Q].IsReady() && useQ)
            {
                var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, spells[Spells.Q].Range);
                {
                    foreach (var minion in
                        allMinions.Where(
                            minion => minion.Health <= ObjectManager.Player.GetSpellDamage(minion, SpellSlot.Q)))
                    {
                        if (minion.IsValidTarget())
                        {
                            spells[Spells.Q].CastOnUnit(minion);
                            return;
                        }
                    }
                }
            }

            if (spells[Spells.E].IsReady() && Player.Health / Player.MaxHealth * 100 >= playerHp && useE)
            {
                var minions = MinionManager.GetMinions(Player.ServerPosition, 800);
                if (minions.Count <= 0)
                {
                    return;
                }

                if (minions.Count > 1)
                {
                    spells[Spells.E].StartCharging();
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                OnCombo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                OnHarass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                OnLaneClear();
                OnJungleClear();
            }

            AreaOfEffectUltimate();
        }

        #endregion
    }
}