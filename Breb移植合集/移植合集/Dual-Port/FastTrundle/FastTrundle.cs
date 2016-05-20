using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using ItemData = LeagueSharp.Common.Data.ItemData;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;

namespace FastTrundle
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
        #region Data

        public static Dictionary<Spells, LeagueSharp.Common.Spell> spells = new Dictionary<Spells, LeagueSharp.Common.Spell>
                                                             {
                                                                 { Spells.Q, new  LeagueSharp.Common.Spell(SpellSlot.Q, 550f) },
                                                                 { Spells.W, new  LeagueSharp.Common.Spell(SpellSlot.W, 900f) },
                                                                 { Spells.E, new  LeagueSharp.Common.Spell(SpellSlot.E, 1000f) },
                                                                 { Spells.R, new  LeagueSharp.Common.Spell(SpellSlot.R, 700f) }
                                                             };

        private static SpellSlot ignite;

        public static Menu
            comboMenu,
            harassMenu,
            lastHitMenu,
            laneClearMenu,
            jungleClearMenu,
            itemMenu,
            miscMenu;

        private static Vector3 pillarPosition;

        private static bool allowQAfterAA, allowItemsAfterAA;

        public static string ScriptVersion => typeof(Trundle).Assembly.GetName().Version.ToString();

        private static AIHeroClient Player => ObjectManager.Player;

        #endregion

        #region Methods

        #region Event handlers

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Trundle") return;

            spells[Spells.E].SetSkillshot(0.5f, 188f, 1600f, false, SkillshotType.SkillshotCircle);
            ignite = Player.GetSpellSlot("summonerdot");

            FastTrundleMenu.Initialize();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Obj_AI_Base.OnSpellCast += ObjAIBase_OnDoCast;

            comboMenu = FastTrundleMenu.comboMenu;
            harassMenu = FastTrundleMenu.harassMenu;
            lastHitMenu = FastTrundleMenu.lastHitMenu;
            laneClearMenu = FastTrundleMenu.laneClearMenu;
            jungleClearMenu = FastTrundleMenu.jungleClearMenu;
            itemMenu = FastTrundleMenu.itemMenu;
            miscMenu = FastTrundleMenu.miscMenu;
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

        private static void ObjAIBase_OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || !Orbwalking.IsAutoAttack(args.SData.Name)) return;
            if (args.Target == null || !args.Target.IsValid) return;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                if (Orbwalker.LastTarget == null) return;

                if (allowQAfterAA && !(args.Target is Obj_AI_Turret || args.Target is Obj_Barracks || args.Target is Obj_BarracksDampener || args.Target is Obj_Building) && spells[Spells.Q].IsReady())
                {
                    spells[Spells.Q].Cast();
                    Orbwalker.ResetAutoAttack();
                    return;
                }
                if (allowItemsAfterAA && getCheckBoxItem(itemMenu, "FastTrundle.Items.Titanic") && Items.HasItem(3748) && Items.CanUseItem(3748)) // Titanic
                {
                    Items.UseItem(3748);
                    Orbwalker.ResetAutoAttack();
                    return;
                }
                if (allowItemsAfterAA && getCheckBoxItem(itemMenu, "FastTrundle.Items.Hydra") && Items.HasItem(3077) && Items.CanUseItem(3077))
                {
                    Items.UseItem(3077);
                    Orbwalker.ResetAutoAttack();
                    return;
                }
                if (allowItemsAfterAA && getCheckBoxItem(itemMenu, "FastTrundle.Items.Hydra") && Items.HasItem(3074) && Items.CanUseItem(3074))
                {
                    Items.UseItem(3074);
                    Orbwalker.ResetAutoAttack();
                    return;
                }
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.LSDistance(Player) > spells[Spells.E].Range || !getCheckBoxItem(miscMenu, "FastTrundle.Antigapcloser"))
            {
                return;
            }

            if (gapcloser.Sender.LSIsValidTarget(spells[Spells.E].Range))
            {
                if (spells[Spells.E].IsReady())
                {
                    spells[Spells.E].Cast(gapcloser.Sender);
                }
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!getCheckBoxItem(miscMenu, "FastTrundle.Interrupter"))
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

        private static void OnDraw(EventArgs args)
        {
            var newTarget = TargetSelector.GetTarget(spells[Spells.E].Range + 200, DamageType.Physical);
            var drawOff = getCheckBoxItem(miscMenu, "FastTrundle.Draw.off");
            var drawQ = getCheckBoxItem(miscMenu, "FastTrundle.Draw.Q");
            var drawW = getCheckBoxItem(miscMenu, "FastTrundle.Draw.W");
            var drawE = getCheckBoxItem(miscMenu, "FastTrundle.Draw.E");
            var drawR = getCheckBoxItem(miscMenu, "FastTrundle.Draw.R");
            var drawPillar = getCheckBoxItem(miscMenu, "FastTrundle.Draw.Pillar");

            if (drawOff)
            {
                return;
            }

            if (drawQ)
            {
                if (spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.Q].Range, System.Drawing.Color.White);
                }
            }

            if (drawW)
            {
                if (spells[Spells.W].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.W].Range, System.Drawing.Color.White);
                }
            }

            if (drawE)
            {
                if (spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.E].Range, System.Drawing.Color.White);
                }
            }

            if (drawR)
            {
                if (spells[Spells.R].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.R].Range, System.Drawing.Color.White);
                }
            }

            if (drawPillar)
            {
                if (spells[Spells.E].Level > 0
                    && newTarget != null
                    && newTarget.IsVisible
                    && newTarget.LSIsValidTarget()
                    && !newTarget.IsDead
                    && Player.LSDistance(newTarget) < 3000)
                {
                    Drawing.DrawCircle(GetPillarPosition(newTarget), 188, System.Drawing.Color.White);
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            allowQAfterAA = allowItemsAfterAA = false;
            if (Player.IsDead || Player.LSIsRecalling() || MenuGUI.IsChatOpen || Shop.IsOpen) return;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LastHit();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                LaneClear();
                JungleClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
        }

        #endregion

        #region Orbwalking Modes

        private static void JungleClear()
        {
            allowItemsAfterAA = true;

            var minion =
                MinionManager.GetMinions(
                    Player.ServerPosition,
                    spells[Spells.W].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (minion == null) return;

            if (Player.ManaPercent < getSliderItem(jungleClearMenu, "FastTrundle.JungleClear.Mana"))
                return;

            if (getCheckBoxItem(jungleClearMenu, "FastTrundle.JungleClear.Q") && spells[Spells.Q].IsReady()
                && minion.LSIsValidTarget(spells[Spells.Q].Range))
            {
                allowQAfterAA = true;
            }

            if (getCheckBoxItem(jungleClearMenu, "FastTrundle.JungleClear.W") && spells[Spells.W].IsReady()
                && minion.LSIsValidTarget(700))
            {
                spells[Spells.W].Cast(minion.Position);
            }
        }

        private static void LaneClear()
        {
            allowItemsAfterAA = true;

            var minion = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.W].Range).FirstOrDefault();
            if (minion == null) return;

            if (Player.ManaPercent < getSliderItem(laneClearMenu, "FastTrundle.LaneClear.Mana"))
                return;

            if (getCheckBoxItem(laneClearMenu, "FastTrundle.LaneClear.Q")
                && spells[Spells.Q].IsReady()
                && minion.LSIsValidTarget(spells[Spells.Q].Range))
            {
                if (getCheckBoxItem(laneClearMenu, "FastTrundle.LaneClear.Q.Lasthit"))
                {
                    if (minion.Health <= QDamage(minion)
                        && (minion.Health > Player.LSGetAutoAttackDamage(minion) ||
                            (!Player.Spellbook.IsAutoAttacking && !Orbwalker.CanAutoAttack))) // don't overkill with Q unless we need AA reset to get it
                    {
                        spells[Spells.Q].Cast();
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                    }
                }
                else
                    allowQAfterAA = true;
            }

            if (getCheckBoxItem(laneClearMenu, "FastTrundle.LaneClear.W") && spells[Spells.W].IsReady()
                && minion.LSIsValidTarget(700))
            {
                spells[Spells.W].Cast(minion.Position);
            }
        }

        private static void LastHit()
        {
            var minion = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.W].Range).FirstOrDefault();
            if (minion == null) return;

            if (Player.ManaPercent < getSliderItem(lastHitMenu, "FastTrundle.LastHit.Mana")) return;

            if (getCheckBoxItem(lastHitMenu, "FastTrundle.LastHit.Q")
                && spells[Spells.Q].IsReady()
                && minion.LSIsValidTarget(spells[Spells.Q].Range)
                && minion.Health <= QDamage(minion)
                && (minion.Health > Player.LSGetAutoAttackDamage(minion) ||
                    (!Player.Spellbook.IsAutoAttacking && !Orbwalker.CanAutoAttack))) // don't overkill with Q unless we need AA reset to get it
            {
                spells[Spells.Q].Cast();
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
            }
        }

        private static void Combo()
        {
            allowItemsAfterAA = true;

            var target = TargetSelector.GetTarget(spells[Spells.E].Range, DamageType.Physical);
            if (target == null || !target.LSIsValidTarget()) return;

            if (getCheckBoxItem(comboMenu, "FastTrundle.Combo.E") && spells[Spells.E].IsReady()
                && target.LSIsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast(GetPillarPosition(target));
            }

            UseItems(target);

            if (getCheckBoxItem(comboMenu, "FastTrundle.Combo.W") && spells[Spells.W].IsReady()
                && target.LSIsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast(target.Position);
            }

            if (getCheckBoxItem(comboMenu, "FastTrundle.Combo.Q") && target.LSIsValidTarget(spells[Spells.Q].Range))
            {
                allowQAfterAA = true;
            }

            if (spells[Spells.R].IsReady() && getCheckBoxItem(comboMenu, "FastTrundle.Combo.R"))
            {
                foreach (var hero in ObjectManager.Get<AIHeroClient>())
                {
                    if (hero.IsEnemy)
                    {
                        var getEnemies = getCheckBoxItem(comboMenu, "FastTrundle.R.On" + hero.ChampionName);
                        if (comboMenu["FastTrundle.R.On" + hero.ChampionName] != null && getEnemies)
                        {
                            spells[Spells.R].Cast(hero);
                        }

                        if (comboMenu["FastTrundle.R.On" + hero.ChampionName] != null && !getEnemies && Player.LSCountEnemiesInRange(1500) == 1)
                        {
                            spells[Spells.R].Cast(hero);
                        }
                    }
                }
            }

            if (Player.LSDistance(target) <= 600 && IgniteDamage(target) >= target.Health
                && getCheckBoxItem(comboMenu, "FastTrundle.Combo.Ignite"))
            {
                Player.Spellbook.CastSpell(ignite, target);
            }
        }

        private static void Harass()
        {
            allowItemsAfterAA = true;

            var target = TargetSelector.GetTarget(spells[Spells.E].Range, DamageType.Physical);
            if (target == null || !target.LSIsValidTarget()) return;

            if (Player.ManaPercent < getSliderItem(harassMenu, "FastTrundle.Harass.Mana")) return;

            if (getCheckBoxItem(harassMenu, "FastTrundle.Harass.Q") && target.LSIsValidTarget(spells[Spells.Q].Range))
            {
                allowQAfterAA = true;
            }

            if (getCheckBoxItem(harassMenu, "FastTrundle.Harass.E") && spells[Spells.E].IsReady()
                && target.LSIsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast(GetPillarPosition(target));
            }

            if (getCheckBoxItem(harassMenu, "FastTrundle.Harass.W") && spells[Spells.W].IsReady()
                && target.LSIsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast(target.Position);
            }
        }

        #endregion

        #region Helpers

        private static void UseItems(Obj_AI_Base target)
        {
            if (getCheckBoxItem(itemMenu, "FastTrundle.Items.Blade")
                && Player.HealthPercent <= getSliderItem(itemMenu, "FastTrundle.Items.Blade.MyHP"))
            {
                if (ItemData.Blade_of_the_Ruined_King.GetItem().IsReady()
                    && ItemData.Blade_of_the_Ruined_King.Range < Player.LSDistance(target))
                {
                    ItemData.Blade_of_the_Ruined_King.GetItem().Cast(target);
                }

                if (ItemData.Bilgewater_Cutlass.GetItem().IsReady()
                    && ItemData.Bilgewater_Cutlass.Range < Player.LSDistance(target))
                {
                    ItemData.Bilgewater_Cutlass.GetItem().Cast(target);
                }
            }

            if (getCheckBoxItem(itemMenu, "FastTrundle.Items.Youmuu"))
            {
                if (ItemData.Youmuus_Ghostblade.GetItem().IsReady()
                    && Orbwalking.GetRealAutoAttackRange(Player) < Player.LSDistance(target))
                {
                    ItemData.Youmuus_Ghostblade.GetItem().Cast();
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

        private static double QDamage(Obj_AI_Base target)
        {
            return Player.LSGetAutoAttackDamage(target) + spells[Spells.Q].GetDamage(target);
        }

        private static Vector3 GetPillarPosition(AIHeroClient target)
        {
            pillarPosition = Player.Position;

            return V2E(pillarPosition, target.Position, target.LSDistance(pillarPosition) + 230).To3D();
        }

        private static Vector2 V2E(Vector3 from, Vector3 direction, float distance)
        {
            return from.LSTo2D() + distance * Vector3.Normalize(direction - from).LSTo2D();
        }

        #endregion

        #endregion
    }
}
