using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Damage = LeagueSharp.Common.Damage;
using Spell = LeagueSharp.Common.Spell;

namespace ElTrundle
{
    internal enum Spells
    {
        Q,

        W,

        E,

        R
    }

    internal static class Trundle
    {
        public static Menu
            Menu,
            comboMenu,
            harassMenu,
            laneClearMenu,
            jungleClearMenu,
            miscMenu;

        #region Public Properties

        public static string ScriptVersion
        {
            get { return typeof (Trundle).Assembly.GetName().Version.ToString(); }
        }

        #endregion

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
            if (ObjectManager.Player.CharData.BaseSkinName != "Trundle")
            {
                return;
            }

            spells[Spells.E].SetSkillshot(0.5f, 188f, 1600f, false, SkillshotType.SkillshotCircle);
            ignite = Player.GetSpellSlot("summonerdot");

            ElTrundleMenu.Initialize();
            Menu = ElTrundleMenu.Menu;
            comboMenu = ElTrundleMenu.comboMenu;
            harassMenu = ElTrundleMenu.harassMenu;
            laneClearMenu = ElTrundleMenu.laneClearMenu;
            jungleClearMenu = ElTrundleMenu.jungleClearMenu;
            miscMenu = ElTrundleMenu.miscMenu;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;

        }

        #endregion

        #region Static Fields

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
        {
            {Spells.Q, new Spell(SpellSlot.Q, 550f)},
            {Spells.W, new Spell(SpellSlot.W, 900f)},
            {Spells.E, new Spell(SpellSlot.E, 1000f)},
            {Spells.R, new Spell(SpellSlot.R, 700f)}
        };

        private static SpellSlot ignite;

        private static Vector3 pillarPosition;

        #endregion

        #region Methods

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.LSDistance(Player) > spells[Spells.E].Range ||
                !getCheckBoxItem(miscMenu, "ElTrundle.Antigapcloser"))
            {
                return;
            }

            if (gapcloser.Sender.IsValidTarget(spells[Spells.E].Range))
            {
                if (spells[Spells.E].IsReady())
                {
                    spells[Spells.E].Cast(gapcloser.Sender);
                }
            }
        }

        private static Vector3 GetPillarPosition(AIHeroClient target)
        {
            pillarPosition = Player.Position;

            return V2E(pillarPosition, target.Position, target.LSDistance(pillarPosition) + 208).To3D();
        }

        private static float IgniteDamage(AIHeroClient target)
        {
            if (ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float) Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private static void Interrupter2_OnInterruptableTarget(
            AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!getCheckBoxItem(miscMenu, "ElTrundle.Interrupter"))
            {
                return;
            }

            if (args.DangerLevel != Interrupter2.DangerLevel.High || sender.LSDistance(Player) > spells[Spells.E].Range)
            {
                return;
            }

            if (spells[Spells.E].CanCast(sender) && args.DangerLevel >= Interrupter2.DangerLevel.High)
            {
                spells[Spells.E].Cast(sender.Position);
            }
        }

        private static void Jungleclear()
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

            if (Player.ManaPercent < getSliderItem(jungleClearMenu, "ElTrundle.JungleClear.Mana"))
            {
                return;
            }

            if (getCheckBoxItem(jungleClearMenu, "ElTrundle.JungleClear.Q") && spells[Spells.Q].IsReady()
                && minion.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast();
                return;
            }

            if (getCheckBoxItem(jungleClearMenu, "ElTrundle.JungleClear.W") && spells[Spells.W].IsReady()
                && minion.IsValidTarget(700))
            {
                spells[Spells.W].Cast(minion.Position);
            }
        }

        private static void Laneclear()
        {
            var minion = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.W].Range).FirstOrDefault();
            if (minion == null)
            {
                return;
            }

            if (Player.ManaPercent < getSliderItem(laneClearMenu, "ElTrundle.LaneClear.Mana"))
            {
                return;
            }

            if (getCheckBoxItem(laneClearMenu, "ElTrundle.LaneClear.Q") && spells[Spells.Q].IsReady()
                && minion.IsValidTarget(spells[Spells.Q].Range))
            {
                if (getCheckBoxItem(laneClearMenu, "ElTrundle.LaneClear.Q.Lasthit") &&
                    minion.Health < spells[Spells.Q].GetDamage(minion))
                {
                    spells[Spells.Q].Cast();
                    return;
                }

                spells[Spells.Q].Cast();
                return;
            }

            if (getCheckBoxItem(laneClearMenu, "ElTrundle.LaneClear.W") && spells[Spells.W].IsReady() &&
                minion.IsValidTarget(700))
            {
                spells[Spells.W].Cast(minion.Position);
            }
        }

        private static void OnCombo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.E].Range, DamageType.Physical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            if (getCheckBoxItem(comboMenu, "ElTrundle.Combo.E") && spells[Spells.E].IsReady()
                && target.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast(GetPillarPosition(target));
            }

            if (getCheckBoxItem(comboMenu, "ElTrundle.Combo.W") && spells[Spells.W].IsReady()
                && target.IsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast(target.Position);
            }

            if (getCheckBoxItem(comboMenu, "ElTrundle.Combo.Q") && target.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast();
            }

            if (spells[Spells.R].IsReady() && getCheckBoxItem(comboMenu, "ElTrundle.Combo.R"))
            {
                foreach (var hero in ObjectManager.Get<AIHeroClient>())
                {
                    if (hero.IsEnemy)
                    {
                        var getEnemies = getCheckBoxItem(comboMenu, "ElTrundle.R.On" + hero.CharData.BaseSkinName);
                        if (comboMenu["ElTrundle.R.On" + hero.CharData.BaseSkinName] != null && getEnemies)
                        {
                            spells[Spells.R].Cast(hero);
                        }

                        if (comboMenu["ElTrundle.R.On" + hero.CharData.BaseSkinName] != null && !getEnemies && Player.CountEnemiesInRange(1500) == 1)
                        {
                            spells[Spells.R].Cast(hero);
                        }
                    }
                }
            }

            if (Player.LSDistance(target) <= 600 && IgniteDamage(target) >= target.Health
                && getCheckBoxItem(comboMenu, "ElTrundle.Combo.Ignite"))
            {
                Player.Spellbook.CastSpell(ignite, target);
            }
        }

        private static void OnDraw(EventArgs args)
        {
            var newTarget = TargetSelector.GetTarget(spells[Spells.E].Range + 200, DamageType.Physical);
            var drawOff = getCheckBoxItem(miscMenu, "ElTrundle.Draw.off");
            var drawQ = getCheckBoxItem(miscMenu, "ElTrundle.Draw.Q");
            var drawW = getCheckBoxItem(miscMenu, "ElTrundle.Draw.W");
            var drawE = getCheckBoxItem(miscMenu, "ElTrundle.Draw.E");
            var drawR = getCheckBoxItem(miscMenu, "ElTrundle.Draw.R");

            if (newTarget != null && newTarget.IsVisible && newTarget.IsValidTarget() && !newTarget.IsDead
                && Player.LSDistance(newTarget) < 3000)
            {
                Drawing.DrawCircle(GetPillarPosition(newTarget), 188, Color.DeepPink);
            }

            if (drawOff)
            {
                return;
            }

            if (drawQ)
            {
                if (spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.Q].Range, Color.White);
                }
            }

            if (drawW)
            {
                if (spells[Spells.W].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.W].Range, Color.White);
                }
            }

            if (drawE)
            {
                if (spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.E].Range, Color.White);
                }
            }

            if (drawR)
            {
                if (spells[Spells.R].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.R].Range, Color.White);
                }
            }
        }

        private static void OnHarass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.E].Range, DamageType.Physical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            if (Player.ManaPercent < getSliderItem(harassMenu, "ElTrundle.Harass.Mana"))
            {
                return;
            }

            if (getCheckBoxItem(harassMenu, "ElTrundle.Harass.Q") && target.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast();
            }

            if (getCheckBoxItem(harassMenu, "ElTrundle.Harass.E") && spells[Spells.E].IsReady()
                && target.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast(GetPillarPosition(target));
            }

            if (getCheckBoxItem(harassMenu, "ElTrundle.Harass.W") && spells[Spells.W].IsReady() &&
                target.IsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast(target.Position);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling() || MenuGUI.IsChatOpen)
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                OnCombo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Laneclear();
                Jungleclear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                OnHarass();
            }
        }

        private static Vector2 V2E(Vector3 from, Vector3 direction, float distance)
        {
            return from.To2D() + distance*Vector3.Normalize(direction - from).To2D();
        }

        #endregion
    }
}