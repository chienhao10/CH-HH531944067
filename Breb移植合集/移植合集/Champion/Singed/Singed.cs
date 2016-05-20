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

namespace ElSinged
{
    internal enum Spells
    {
        Q,

        W,

        E,

        R
    }

    internal class Singed
    {
        #region Public Properties

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        #endregion

        #region Public Methods and Operators

        public static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.CharData.BaseSkinName != "Singed")
            {
                return;
            }

            ignite = Player.GetSpellSlot("summonerdot");

            spells[Spells.W].SetSkillshot(0.5f, 350, 700, false, SkillshotType.SkillshotCircle);

            ElSingedMenu.Initialize();
            Game.OnUpdate += OnGameUpdate;
            Orbwalker.OnPreAttack += OrbwalkingBeforeAttack;
            Drawing.OnDraw += Drawings.Drawing_OnDraw;

            Menu = ElSingedMenu.Menu;
            cMenu = ElSingedMenu.cMenu;
            hMenu = ElSingedMenu.hMenu;
            lcMenu = ElSingedMenu.lcMenu;
            miscMenu = ElSingedMenu.miscMenu;
        }

        #endregion

        #region Static Fields

        public static Menu
            Menu = ElSingedMenu.Menu,
            cMenu = ElSingedMenu.cMenu,
            hMenu = ElSingedMenu.hMenu,
            lcMenu = ElSingedMenu.lcMenu,
            miscMenu = ElSingedMenu.miscMenu;

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

        public static float poisonTime;

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
        {
            {Spells.Q, new Spell(SpellSlot.Q, 0)},
            {Spells.W, new Spell(SpellSlot.W, 1000)},
            {Spells.E, new Spell(SpellSlot.E, 125)},
            {Spells.R, new Spell(SpellSlot.R, 0)}
        };

        private static SpellSlot ignite;

        #endregion

        #region Methods

        private static void CastQ()
        {
            if (spells[Spells.Q].Instance.ToggleState == 1 && Environment.TickCount - poisonTime > 1200)
            {
                poisonTime = Environment.TickCount + 1200;
                spells[Spells.Q].Cast();
            }
            if (spells[Spells.Q].Instance.ToggleState == 2)
            {
                poisonTime = Environment.TickCount + 1200;
            }
        }

        private static void Combo()
        {
            if (Player.CountEnemiesInRange(getSliderItem(miscMenu, "ElSinged.Misc.QRange")) < 1 && spells[Spells.Q].Instance.ToggleState == 2 && !getCheckBoxItem(miscMenu, "DontOffQ"))
            {
                spells[Spells.Q].Cast();
            }
            var target = TargetSelector.GetTarget(spells[Spells.W].Range, DamageType.Magical);
            if (target == null)
            {
                return;
            }

            var comboCount = getSliderItem(cMenu, "ElSinged.Combo.R.Count");

            var qTarget = HeroManager.Enemies.FirstOrDefault( enemy => enemy.IsValidTarget() && enemy.LSDistance(Player) < 200 && Player.IsMoving && enemy.IsMoving);

            if (getCheckBoxItem(cMenu, "ElSinged.Combo.Q") && spells[Spells.Q].IsReady() && (qTarget != null || target.HasBuff("poisontrailtarget") || Player.LSDistance(target) <= 500))
            {
                CastQ();
            }

            if (getCheckBoxItem(cMenu, "ElSinged.Combo.W") && target.IsValidTarget(spells[Spells.W].Range)
                && spells[Spells.W].IsReady())
            {
                var pred = spells[Spells.W].GetPrediction(target);
                if (spells[Spells.W].Range - 80 > pred.CastPosition.LSDistance(Player.Position)
                    && pred.Hitchance >= HitChance.High)
                {
                    spells[Spells.W].Cast(pred.CastPosition);
                }
            }

            if (getCheckBoxItem(cMenu, "ElSinged.Combo.E") && spells[Spells.E].IsReady())
            {
                spells[Spells.E].CastOnUnit(target);
            }

            if (getCheckBoxItem(cMenu, "ElSinged.Combo.R") &&
                Player.CountEnemiesInRange(spells[Spells.W].Range) >= comboCount)
            {
                spells[Spells.R].Cast();
            }

            if (getCheckBoxItem(cMenu, "ElSinged.Combo.Ignite") && Player.LSDistance(target) <= 600 &&
                IgniteDamage(target) >= target.Health)
            {
                Player.Spellbook.CastSpell(ignite, target);
            }
        }

        private static void Harass()
        {
            if (Player.CountEnemiesInRange(getSliderItem(miscMenu, "ElSinged.Misc.QRange")) < 1 && spells[Spells.Q].Instance.ToggleState == 2 && !getCheckBoxItem(miscMenu, "DontOffQ"))
            {
                spells[Spells.Q].Cast();
            }
            var target = TargetSelector.GetTarget(spells[Spells.W].Range, DamageType.Magical);
            if (target == null)
            {
                return;
            }

            var qTarget =
                HeroManager.Enemies.FirstOrDefault(
                    enemy => enemy.IsValidTarget() && enemy.LSDistance(Player) < 200 && Player.IsMoving && enemy.IsMoving);

            if (getCheckBoxItem(hMenu, "ElSinged.Harass.Q") && spells[Spells.Q].IsReady()
                && (qTarget != null || target.HasBuff("poisontrailtarget") || Player.LSDistance(target) <= 500))
            {
                CastQ();
            }


            if (getCheckBoxItem(hMenu, "ElSinged.Harass.W") && target.IsValidTarget(spells[Spells.W].Range)
                && spells[Spells.W].IsReady())
            {
                var pred = spells[Spells.W].GetPrediction(target);
                if (spells[Spells.W].Range - 80 > pred.CastPosition.LSDistance(Player.Position)
                    && pred.Hitchance >= HitChance.High)
                {
                    spells[Spells.W].Cast(pred.CastPosition);
                }
            }

            if (getCheckBoxItem(hMenu, "ElSinged.Harass.E") && spells[Spells.E].IsReady())
            {
                spells[Spells.E].CastOnUnit(target);
            }

            if (getCheckBoxItem(hMenu, "ElSinged.Harass.W"))
            {
                var pred = spells[Spells.W].GetPrediction(target);
                if (spells[Spells.W].Range - 80 > pred.CastPosition.LSDistance(Player.Position)
                    && pred.Hitchance >= HitChance.High)
                {
                    spells[Spells.W].Cast(pred.CastPosition);
                }
            }
        }

        private static float IgniteDamage(AIHeroClient target)
        {
            if (ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float) Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private static void LaneClear()
        {
            var minion = MinionManager.GetMinions(ObjectManager.Player.Position, 400).FirstOrDefault();
            if (minion == null)
            {
                return;
            }

            if (MinionManager.GetMinions(ObjectManager.Player.Position, 400).Count < 1 && spells[Spells.Q].Instance.ToggleState == 2 && !getCheckBoxItem(miscMenu, "DontOffQ"))
            {
                spells[Spells.Q].Cast();
            }

            if (getCheckBoxItem(lcMenu, "ElSinged.Laneclear.E") && spells[Spells.E].GetDamage(minion) > minion.Health &&
                minion.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].CastOnUnit(minion);
            }

            if (getCheckBoxItem(lcMenu, "ElSinged.Laneclear.Q") && spells[Spells.Q].IsReady())
            {
                spells[Spells.Q].Cast();
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

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) && !!getCheckBoxItem(miscMenu, "DontOffQ"))
            {
                TurnOffQ();
            }
        }

        private static void OrbwalkingBeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (spells[Spells.E].IsReady() && args.Target.IsValidTarget(spells[Spells.E].Range))
                {
                    args.Process = false;
                    spells[Spells.E].Cast();
                }
            }
        }

        private static void TurnOffQ()
        {
            if (spells[Spells.Q].Instance.ToggleState == 2 && Environment.TickCount - poisonTime > 1200)
            {
                spells[Spells.Q].Cast();
            }
        }

        #endregion
    }
}