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
using Spell = LeagueSharp.Common.Spell;

namespace ElTristana
{

    #region

    internal enum Spells
    {
        Q,

        W,

        E,

        R
    }

    #endregion

    internal static class Tristana
    {
        #region Static Fields

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
        {
            {Spells.Q, new Spell(SpellSlot.Q, 550)},
            {Spells.W, new Spell(SpellSlot.W, 900)},
            {Spells.E, new Spell(SpellSlot.E, 625)},
            {Spells.R, new Spell(SpellSlot.R, 700)}
        };

        #endregion

        #region Public Properties

        public static Menu
            Menu = MenuInit.Menu_,
            comboMenu = MenuInit.comboMenu,
            suicideMenu = MenuInit.suicideMenu,
            harassMenu = MenuInit.harassMenu,
            laneClearMenu = MenuInit.laneClearMenu,
            jungleClearMenu = MenuInit.jungleClearMenu,
            miscMenu = MenuInit.miscMenu,
            killstealMenu = MenuInit.killstealMenu;

        public static string ScriptVersion
        {
            get { return typeof(Tristana).Assembly.GetName().Version.ToString(); }
        }

        #endregion

        #region Properties

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }
        #endregion

        #region Public Methods and Operators

        // <summary>
        /// Gets or sets the last harass position
        /// </summary>
        /// <value>
        ///     The last harass position
        /// </value>
        private static Vector3? LastHarassPos { get; set; }

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
            if (ObjectManager.Player.ChampionName != "Tristana")
            {
                return;
            }

            Console.WriteLine("Injected ElTristana AMK");

            spells[Spells.W].SetSkillshot(0.35f, 250f, 1400f, false, SkillshotType.SkillshotCircle);

            MenuInit.Initialize();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;

            Menu = MenuInit.Menu_;
            comboMenu = MenuInit.comboMenu;
            suicideMenu = MenuInit.suicideMenu;
            harassMenu = MenuInit.harassMenu;
            laneClearMenu = MenuInit.laneClearMenu;
            jungleClearMenu = MenuInit.jungleClearMenu;
            miscMenu = MenuInit.miscMenu;
            killstealMenu = MenuInit.killstealMenu;

        }

        private static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            try
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    if (!(args.Target is AIHeroClient))
                    {
                        return;
                    }

                    var targeta =
                        HeroManager.Enemies.Find(
                            x => x.HasBuff("TristanaECharge") && x.IsValidTarget(spells[Spells.E].Range));
                    if (targeta == null)
                    {
                        return;
                    }
                    if (Orbwalking.InAutoAttackRange(targeta))
                    {
                        Orbwalker.ForcedTarget = targeta;
                    }
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) ||
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                {
                    var minion = args.Target as Obj_AI_Minion;
                    if (minion != null && minion.HasBuff("TristanaECharge"))
                    {
                        Orbwalker.ForcedTarget = minion;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Methods

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (getCheckBoxItem(miscMenu, "ElTristana.Antigapcloser"))
            {
                if (gapcloser.Sender.IsValidTarget(250f) && spells[Spells.R].IsReady())
                {
                    spells[Spells.R].Cast(gapcloser.Sender);
                }
            }
        }

        private static BuffInstance GetECharge(this Obj_AI_Base target)
        {
            return target.Buffs.Find(x => x.DisplayName == "TristanaECharge");
        }


        private static void Interrupter2_OnInterruptableTarget(
            AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!getCheckBoxItem(miscMenu, "ElTristana.Interrupter"))
            {
                return;
            }

            if (sender.Distance(Player) <= spells[Spells.R].Range)
            {
                spells[Spells.R].Cast(sender);
            }
        }

        private static bool IsECharged(this Obj_AI_Base target)
        {
            return target.GetECharge() != null;
        }

        private static void OnCombo()
        {
            var eTarget =
                HeroManager.Enemies.Find(
                    x => x.HasBuff("TristanaECharge") && x.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player)));
            var target = eTarget ?? TargetSelector.GetTarget(spells[Spells.E].Range, DamageType.Physical);

            if (!target.IsValidTarget())
            {
                return;
            }

            if (getCheckBoxItem(comboMenu, "ElTristana.Combo.Focus.E"))
            {
                var passiveTarget =
                    HeroManager.Enemies.Find(
                        x => x.HasBuff("TristanaECharge") && x.IsValidTarget(spells[Spells.E].Range));
                Orbwalker.ForcedTarget = passiveTarget ?? null;
            }


            if (spells[Spells.E].IsReady() && getCheckBoxItem(comboMenu, "ElTristana.Combo.E")
                && Player.ManaPercent > getSliderItem(comboMenu, "ElTristana.Combo.E.Mana"))
            {
                foreach (var hero in HeroManager.Enemies.OrderBy(x => x.Health))
                {
                    if (hero.IsEnemy)
                    {
                        var getEnemies = getCheckBoxItem(comboMenu, "ElTristana.E.On" + hero.ChampionName);
                        if (comboMenu["ElTristana.E.On" + hero.ChampionName] != null && getEnemies)
                        {
                            spells[Spells.E].Cast(hero);
                            Orbwalker.ForcedTarget = hero;
                        }

                        if (comboMenu["ElTristana.E.On" + hero.ChampionName] != null && !getEnemies && Player.CountEnemiesInRange(1500) == 1)
                        {
                            spells[Spells.E].Cast(hero);
                            Orbwalker.ForcedTarget = hero;
                        }
                    }
                }
            }

            if (spells[Spells.R].IsReady() && getCheckBoxItem(comboMenu, "ElTristana.Combo.R"))
            {
                if (spells[Spells.R].GetDamage(target) > target.Health)
                {
                    spells[Spells.R].Cast(target);
                }
            }

            if (IsECharged(target) && getCheckBoxItem(comboMenu, "ElTristana.Combo.Always.RE"))
            {
                if (spells[Spells.R].GetDamage(target)
                    + spells[Spells.E].GetDamage(target) * (0.3 * target.GetBuffCount("TristanaECharge") + 1)
                    > target.Health)
                {
                    spells[Spells.R].Cast(target);
                }
            }

            if (spells[Spells.Q].IsReady() && getCheckBoxItem(comboMenu, "ElTristana.Combo.Q")
                && target.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.Q].Cast();
            }
        }

        private static bool HasEBuff(this Obj_AI_Base target)
        {
            return target.HasBuff("TristanaECharge");
        }

        private static void OnDraw(EventArgs args)
        {
            var target =
                HeroManager.Enemies.Find(
                    x => x.HasBuff("TristanaECharge") && x.IsValidTarget(2000));
            if (!target.IsValidTarget())
            {
                return;
            }

            if (getCheckBoxItem(miscMenu, "ElTristana.DrawStacks"))
            {
                if (LastHarassPos == null)
                {
                    LastHarassPos = Player.ServerPosition;
                }

                var x = target.HPBarPosition.X + 45;
                var y = target.HPBarPosition.Y - 25;

                if (spells[Spells.E].Level > 0)
                {
                    if (HasEBuff(target)) //Credits to lizzaran 
                    {
                        var stacks = target.GetBuffCount("TristanaECharge");
                        if (stacks > -1)
                        {
                            for (var i = 0; 4 > i; i++)
                            {
                                Drawing.DrawLine(x + i * 20, y, x + i * 20 + 10, y, 10,
                                    i > stacks ? Color.DarkGray : Color.DeepSkyBlue);
                            }
                        }

                        if (stacks == 3)
                        {
                            if (getCheckBoxItem(suicideMenu, "ElTristana.W"))
                            {
                                if (getCheckBoxItem(suicideMenu, "ElTristana.W.Jump"))
                                {
                                    if (getCheckBoxItem(suicideMenu, "ElTristana.W.Jump.kill"))
                                    {
                                        if (target.IsValidTarget(spells[Spells.W].Range) &&
                                            Player.CountEnemiesInRange(getSliderItem(suicideMenu,
                                                "ElTristana.W.Enemies.Range")) <=
                                            getSliderItem(suicideMenu, "ElTristana.W.Enemies"))
                                        {
                                            if (getCheckBoxItem(suicideMenu, "ElTristana.W.Jump.tower"))
                                            {
                                                var underTower = target.UnderTurret();
                                                if (underTower)
                                                {
                                                    return;
                                                }
                                            }

                                            if (spells[Spells.W].GetDamage(target) > target.Health + 15)
                                            {
                                                spells[Spells.W].Cast(target.ServerPosition);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (target.IsValidTarget(spells[Spells.W].Range) &&
                                            Player.CountEnemiesInRange(getSliderItem(suicideMenu,
                                                "ElTristana.W.Enemies.Range")) <=
                                            getSliderItem(suicideMenu, "ElTristana.W.Enemies"))
                                        {
                                            if (getCheckBoxItem(suicideMenu, "ElTristana.W.Jump.tower"))
                                            {
                                                var underTower = target.UnderTurret();
                                                if (underTower)
                                                {
                                                    return;
                                                }
                                            }
                                            spells[Spells.W].Cast(target.ServerPosition);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (getCheckBoxItem(miscMenu, "ElTristana.Draw.off"))
            {
                return;
            }

            if (getCheckBoxItem(miscMenu, "ElTristana.Draw.Q"))
            {
                if (spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.Q].Range, Color.White);
                }
            }

            if (getCheckBoxItem(miscMenu, "ElTristana.Draw.E"))
            {
                if (spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.E].Range, Color.White);
                }
            }

            if (getCheckBoxItem(miscMenu, "ElTristana.Draw.R"))
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

            if (spells[Spells.E].IsReady() && getCheckBoxItem(harassMenu, "ElTristana.Harass.E")
                && Player.ManaPercent > getSliderItem(harassMenu, "ElTristana.Harass.E.Mana"))
            {
                foreach (var hero in HeroManager.Enemies.OrderByDescending(x => x.Health))
                {
                    if (hero.IsEnemy)
                    {
                        var getEnemies = getCheckBoxItem(harassMenu, "ElTristana.E.On.Harass" + hero.CharData.BaseSkinName);
                        if (harassMenu["ElTristana.E.On.Harass" + hero.CharData.BaseSkinName] != null && getEnemies)
                        {
                            spells[Spells.E].Cast(hero);
                            Orbwalker.ForcedTarget = hero;
                        }
                    }
                }
            }

            if (spells[Spells.Q].IsReady() && getCheckBoxItem(harassMenu, "ElTristana.Harass.Q")
                && target.IsValidTarget(spells[Spells.E].Range))
            {
                if (IsECharged(target) && getCheckBoxItem(harassMenu, "ElTristana.Harass.QE"))
                {
                    spells[Spells.Q].Cast();
                }
                else if (!getCheckBoxItem(harassMenu, "ElTristana.Harass.QE"))
                {
                    spells[Spells.Q].Cast();
                }
            }
        }

        private static void OnJungleClear()
        {
            var minions =
                MinionManager.GetMinions(
                    spells[Spells.Q].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (!minions.IsValidTarget() || minions == null)
            {
                return;
            }

            if (spells[Spells.E].IsReady() && getCheckBoxItem(jungleClearMenu, "ElTristana.JungleClear.E")
                && Player.ManaPercent > getSliderItem(jungleClearMenu, "ElTristana.JungleClear.E.Mana"))
            {
                spells[Spells.E].CastOnUnit(minions);
            }

            if (spells[Spells.Q].IsReady() && getCheckBoxItem(jungleClearMenu, "ElTristana.JungleClear.Q"))
            {
                spells[Spells.Q].Cast();
            }
        }

        private static void OnLaneClear()
        {
            if (getCheckBoxItem(laneClearMenu, "ElTristana.LaneClear.Tower"))
            {
                foreach (var tower in ObjectManager.Get<Obj_AI_Turret>())
                {
                    if (!tower.IsDead && tower.Health > 100 && tower.IsEnemy && tower.IsValidTarget() && Player.ServerPosition.Distance(tower.ServerPosition) < Orbwalking.GetRealAutoAttackRange(Player))
                    {
                        spells[Spells.E].Cast(tower);
                    }
                }
            }

            var eminion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(x => x.HasBuff("TristanaECharge") && x.LSIsValidTarget(1000)).FirstOrDefault();

            if (eminion != null)
            {
                Orbwalker.ForcedTarget = eminion;
            }

            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, spells[Spells.E].Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth);
            if (minions.Count <= 0)
            {
                return;
            }

            if (spells[Spells.Q].IsReady() && minions.Count > 0 && getCheckBoxItem(laneClearMenu, "ElTristana.LaneClear.Q"))
            {
                spells[Spells.Q].Cast();
            }

            if (spells[Spells.E].IsReady() && getCheckBoxItem(laneClearMenu, "ElTristana.LaneClear.E") && minions.Count > 2 && Player.ManaPercent > getSliderItem(laneClearMenu, "ElTristana.LaneClear.E.Mana"))
            {
                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().OrderByDescending(m => m.Health))
                {
                    spells[Spells.E].Cast(minion);
                    Orbwalker.ForcedTarget = minion;
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            try
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    OnCombo();
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) ||
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                {
                    OnLaneClear();
                    OnJungleClear();
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    OnHarass();
                }

                if (getCheckBoxItem(killstealMenu, "ElTristana.killsteal.Active"))
                {
                    DoKillsteal();
                }

                spells[Spells.Q].Range = 550 + 9 * (Player.Level - 1);
                spells[Spells.E].Range = 625 + 9 * (Player.Level - 1);
                spells[Spells.R].Range = 517 + 9 * (Player.Level - 1);
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        private static void DoKillsteal()
        {
            try
            {
                foreach (
                    var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(spells[Spells.R].Range) && !x.IsDead && !x.IsZombie).OrderBy(x => x.Health))
                {
                    if (enemy.IsValidTarget(spells[Spells.R].Range) && spells[Spells.R].GetDamage(enemy) > enemy.Health)
                    {
                        if (!getCheckBoxItem(killstealMenu, "ElTristana.Killsteal.R"))
                        {
                            return;
                        }

                        spells[Spells.R].Cast(enemy);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            float damage = 0;

            if (!Orbwalker.IsAutoAttacking)
            {
                damage += ObjectManager.Player.GetAutoAttackDamage(enemy, true);
            }

            if (enemy.HasBuff("tristanaecharge"))
            {
                damage += (float)(spells[Spells.E].GetDamage(enemy) * (enemy.GetBuffCount("tristanaecharge") * 0.30)) +
                          spells[Spells.E].GetDamage(enemy);
            }

            if (spells[Spells.R].IsReady())
            {
                damage += spells[Spells.R].GetDamage(enemy);
            }

            return damage;
        }

        #endregion
    }
}