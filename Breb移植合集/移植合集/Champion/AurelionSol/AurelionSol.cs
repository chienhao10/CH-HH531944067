using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using Damage = LeagueSharp.Common.Damage;
using Geometry = LeagueSharp.Common.Geometry;
using Spell = LeagueSharp.Common.Spell;

namespace ElAurelion_Sol
{
    internal class AurelionSol
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The Smitespell
        /// </value>
        public static Spell IgniteSpell { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Fired when the game loads.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void OnGameLoad()
        {
            try
            {
                if (Player.ChampionName != "AurelionSol")
                {
                    return;
                }

                var igniteSlot = Player.GetSpellSlot("summonerdot");

                if (igniteSlot != SpellSlot.Unknown)
                {
                    IgniteSpell = new Spell(igniteSlot, 600f);
                }


                Q = new Spell(SpellSlot.Q, 650f);
                W1 = new Spell(SpellSlot.W, 350f);
                W = new Spell(SpellSlot.W, 600f);
                new Spell(SpellSlot.E, 400f);
                R = new Spell(SpellSlot.R, 1420f);

                Q.SetSkillshot(0.25f, 180, 850, false, SkillshotType.SkillshotLine);
                R.SetSkillshot(0.25f, 300, 4500, false, SkillshotType.SkillshotLine);

                GenerateMenu();

                Game.OnUpdate += OnUpdate;
                Drawing.OnDraw += OnDraw;
                Orbwalker.OnPreAttack += OrbwalkingBeforeAttack;
                AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
                Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        /// <summary>
        ///     Gets or sets the Q spell
        /// </summary>
        /// <value>
        ///     The Q spell
        /// </value>
        private static Spell Q { get; set; }

        /// <summary>
        ///     Gets or sets the R spell.
        /// </summary>
        /// <value>
        ///     The R spell
        /// </value>
        private static Spell R { get; set; }

        /// <summary>
        ///     Gets or sets the W spell
        /// </summary>
        /// <value>
        ///     The W spell
        /// </value>
        private static Spell W { get; set; }

        /// <summary>
        ///     Gets or sets the W1 spell
        /// </summary>
        /// <value>
        ///     The W1 spell
        /// </value>
        private static Spell W1 { get; set; }

        private static bool HasPassive()
        {
            return Player.HasBuff("AurelionSolWActive");
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets or sets the menu
        /// </summary>
        /// <value>
        ///     The menu
        /// </value>
        private static Menu menu, comboMenu, harassMenu, laneClearMenu, jungleClearMenu, ksMenu, miscMenu, drawMenu;

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

        /// <summary>
        ///     Creates the menu
        /// </summary>
        /// <value>
        ///     Creates the menu
        /// </value>
        private static void GenerateMenu()
        {
            try
            {
                menu = MainMenu.AddMenu("El翱銳龍獸", "AurelionSol");

                comboMenu = menu.AddSubMenu("连招", "Combo");
                comboMenu.Add("Combo.Q", new CheckBox("使用 Q"));
                comboMenu.Add("Combo.W", new CheckBox("使用 W"));
                comboMenu.Add("Combo.R", new CheckBox("使用 R"));
                comboMenu.Add("Combo.R.Multiple", new CheckBox("R 复数目标:"));
                comboMenu.Add("Combo.R.Count", new Slider("自动 R 敌人数量:", 3, 2, 4));

                harassMenu = menu.AddSubMenu("骚扰", "Harass");
                harassMenu.Add("Harass.Q", new CheckBox("使用 Q"));

                laneClearMenu = menu.AddSubMenu("清线", "Laneclear");
                laneClearMenu.Add("laneclear.Q", new CheckBox("使用 Q"));
                laneClearMenu.Add("laneclear.minionshit", new Slider("最低小兵命中数 (Q)", 2, 1, 5));
                laneClearMenu.Add("laneclear.Mana", new Slider("最低蓝量", 20));

                jungleClearMenu = menu.AddSubMenu("清野", "Jungleclear");
                jungleClearMenu.Add("jungleclear.Q", new CheckBox("使用 Q"));
                jungleClearMenu.Add("aneclear.minionshit", new Slider("最低小兵命中数 (Q)", 1, 1, 5));
                jungleClearMenu.Add("jungleclear.Mana", new Slider("最低蓝量", 20));

                ksMenu = menu.AddSubMenu("抢头", "Killsteal");
                ksMenu.Add("Killsteal.Active", new CheckBox("开启抢头"));
                ksMenu.Add("Killsteal.R", new CheckBox("使用 R"));
                ksMenu.Add("Ignite", new CheckBox("使用点燃"));

                miscMenu = menu.AddSubMenu("杂项", "Misc");
                miscMenu.Add("Misc.Auto.W", new CheckBox("自动结束 W"));
                miscMenu.Add("AA.Block", new CheckBox("连招时不普攻", false));
                miscMenu.Add("inter", new CheckBox("防技能打断"));
                miscMenu.Add("gapcloser", new CheckBox("防突击"));

                drawMenu = menu.AddSubMenu("线圈", "Drawings");
                drawMenu.Add("Draw.Off", new CheckBox("屏蔽线圈", false));
                drawMenu.Add("Draw.Q", new CheckBox("显示 Q"));
                drawMenu.Add("Draw.W", new CheckBox("显示 W"));
                drawMenu.Add("Draw.R", new CheckBox("显示 R"));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        ///     The ignite killsteal logic
        /// </summary>
        private static void HandleIgnite()
        {
            try
            {
                var kSableEnemy =
                    HeroManager.Enemies.FirstOrDefault(
                        hero =>
                            hero.LSIsValidTarget(550) && ShieldCheck(hero) && !hero.HasBuff("summonerdot") &&
                            !hero.IsZombie
                            && Player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite) >= hero.Health);

                if (kSableEnemy != null && IgniteSpell.Slot != SpellSlot.Unknown)
                {
                    Player.Spellbook.CastSpell(IgniteSpell.Slot, kSableEnemy);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        ///     Calculates the R damage
        /// </summary>
        /// <param name="target"></param>
        /// <returns>
        ///     The R damage
        /// </returns>
        private static double RDamage(Obj_AI_Base target)
        {
            return Player.CalcDamage(target, DamageType.Magical,
                (float) new double[] {200, 400, 600}[R.Level - 1] + 0.70f*Player.TotalMagicalDamage);
        }

        /// <summary>
        ///     Calculates the Q damage
        /// </summary>
        /// <param name="target"></param>
        /// <returns>
        ///     The Q damage
        /// </returns>
        private static double QDamage(Obj_AI_Base target)
        {
            return Player.CalcDamage(target, DamageType.Magical,
                (float) new double[] {70, 110, 150, 190, 230}[Q.Level - 1] + 0.65f*Player.TotalMagicalDamage);
        }

        /// <summary>
        ///     Combo logic
        /// </summary>
        private static void OnCombo()
        {
            try
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                if (target == null)
                {
                    return;
                }

                if (Q.IsReady() && getCheckBoxItem(comboMenu, "Combo.Q"))
                {
                    var prediction = Q.GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.High)
                    {
                        Q.Cast(target);
                    }
                }

                if (W.IsReady() && getCheckBoxItem(comboMenu, "Combo.W"))
                {
                    if (!HasPassive())
                    {
                        if (target.LSIsValidTarget(W1.Range))
                        {
                            return;
                        }

                        if (Player.LSDistance(target) > W1.Range && Player.LSDistance(target) < W.Range)
                        {
                            W.Cast();
                        }
                    }
                    else if (HasPassive())
                    {
                        if (Player.LSDistance(target) > W1.Range && Player.LSDistance(target) < W.Range + 100)
                        {
                            return;
                        }

                        if (Player.LSDistance(target) > W1.Range + 150)
                        {
                            W.Cast();
                        }
                    }
                }

                if (R.IsReady() && getCheckBoxItem(comboMenu, "Combo.R") &&
                    RDamage(target) > target.Health + target.AttackShield)
                {
                    var prediction = R.GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.High)
                    {
                        R.Cast(target);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="args"></param>
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
                    args.Process = !(Q.IsReady() || Player.LSDistance(args.Target) >= 1000);
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void Interrupter2_OnInterruptableTarget(
            AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!getCheckBoxItem(miscMenu, "inter"))
            {
                return;
            }

            if (args.DangerLevel != Interrupter2.DangerLevel.High
                || sender.LSDistance(Player) > Q.Range)
            {
                return;
            }

            if (sender.LSIsValidTarget(Q.Range) && args.DangerLevel == Interrupter2.DangerLevel.High
                && Q.IsReady())
            {
                var prediction = Q.GetPrediction(sender);
                if (prediction.Hitchance >= HitChance.High)
                {
                    Q.Cast(sender);
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="gapcloser"></param>
        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!getCheckBoxItem(miscMenu, "gapcloser"))
            {
                return;
            }

            if (Q.IsReady() && gapcloser.Sender.LSDistance(Player) < Q.Range)
            {
                if (gapcloser.Sender.LSIsValidTarget(Q.Range) && Q.IsReady())
                {
                    var prediction = Q.GetPrediction(gapcloser.Sender);
                    if (prediction.Hitchance >= HitChance.High)
                    {
                        Q.Cast(gapcloser.Sender);
                    }
                }
            }
        }

        /// <summary>
        ///     Called when the game draws itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void OnDraw(EventArgs args)
        {
            try
            {
                if (getCheckBoxItem(drawMenu, "Draw.Off"))
                {
                    return;
                }

                if (getCheckBoxItem(drawMenu, "Draw.W"))
                {
                    if (!HasPassive())
                    {
                        if (W.Level > 0)
                        {
                            Render.Circle.DrawCircle(ObjectManager.Player.Position, W1.Range, Color.Red);
                        }
                    }
                    else
                    {
                        if (W.Level > 0)
                        {
                            Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Color.MediumVioletRed);
                        }
                    }
                }
                if (getCheckBoxItem(drawMenu, "Draw.W"))
                {
                    if (Q.Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Goldenrod);
                    }
                }

                if (getCheckBoxItem(drawMenu, "Draw.R"))
                {
                    if (R.Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, Color.DeepSkyBlue);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        /// <summary>
        ///     Harass logic
        /// </summary>
        private static void OnHarass()
        {
            try
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                if (target == null)
                {
                    return;
                }

                if (Q.IsReady() && getCheckBoxItem(harassMenu, "Harass.Q"))
                {
                    var prediction = Q.GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.High)
                    {
                        Q.Cast(target);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        ///     The jungleclear "logic"
        /// </summary>
        private static void OnJungleclear()
        {
            try
            {
                var minion = MinionManager.GetMinions(
                    Player.ServerPosition,
                    Q.Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (minion == null)
                {
                    return;
                }

                if (Player.ManaPercent < getSliderItem(jungleClearMenu, "jungleclear.Mana"))
                {
                    return;
                }

                if (getCheckBoxItem(jungleClearMenu, "jungleclear.Q") && Q.IsReady())
                {
                    var prediction = Q.GetCircularFarmLocation(minion);
                    if (prediction.MinionsHit >= 1)
                    {
                        Q.Cast(prediction.Position);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        ///     Killsteal logic
        /// </summary>
        private static void OnKillsteal()
        {
            try
            {
                foreach (
                    var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(R.Range) && !x.IsDead && !x.IsZombie))
                {
                    if (Q.IsReady() && enemy.LSIsValidTarget(Q.Range) && enemy.Health < QDamage(enemy))
                    {
                        var prediction = Q.GetPrediction(enemy);
                        if (prediction.Hitchance >= HitChance.High)
                        {
                            Q.Cast(enemy);
                        }
                    }

                    if (R.IsReady() && enemy.LSIsValidTarget(R.Range) && enemy.Health < RDamage(enemy))
                    {
                        var prediction = R.GetPrediction(enemy);
                        if (prediction.Hitchance >= HitChance.High)
                        {
                            R.Cast(enemy);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        ///     The laneclear "logic"
        /// </summary>
        private static void OnLaneclear()
        {
            try
            {
                if (Player.ManaPercent < getSliderItem(laneClearMenu, "laneclear.Mana"))
                {
                    return;
                }

                var minion = MinionManager.GetMinions(Player.Position, Q.Range);
                if (minion == null)
                {
                    return;
                }

                if (getCheckBoxItem(laneClearMenu, "laneclear.Q") && Q.IsReady())
                {
                    var prediction = Q.GetCircularFarmLocation(minion);
                    if (prediction.MinionsHit >= 2)
                    {
                        Q.Cast(prediction.Position);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        ///     Called when the game updates
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void OnUpdate(EventArgs args)
        {
            try
            {
                if (Player.IsDead)
                {
                    return;
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    OnCombo();
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    OnHarass();
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    OnLaneclear();
                    OnJungleclear();
                }

                if (getCheckBoxItem(ksMenu, "Killsteal.Active"))
                {
                    OnKillsteal();
                }

                if (getCheckBoxItem(ksMenu, "Ignite"))
                {
                    HandleIgnite();
                }

                if (getCheckBoxItem(miscMenu, "Misc.Auto.W"))
                {
                    if (HasPassive() && Player.GetEnemiesInRange(2000f).Count == 0)
                    {
                        W.Cast();
                    }
                }

                if (getCheckBoxItem(comboMenu, "Combo.R.Multiple"))
                {
                    float RDistance = 1420;
                    float RWidth = 120;
                    var minREnemies = getSliderItem(comboMenu, "Combo.R.Count");
                    foreach (var enemy in HeroManager.Enemies)
                    {
                        var startPos = enemy.ServerPosition;
                        var endPos = Player.ServerPosition.LSExtend(startPos, Player.LSDistance(enemy) + RDistance);
                        var rectangle = new Geometry.Polygon.Rectangle(startPos, endPos, RWidth);

                        if (HeroManager.Enemies.Count(x => rectangle.IsInside(x)) >= minREnemies)
                        {
                            R.Cast(enemy);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        ///     The shield checker
        /// </summary>
        private static bool ShieldCheck(Obj_AI_Base hero)
        {
            try
            {
                return !hero.HasBuff("summonerbarrier") || !hero.HasBuff("BlackShield")
                       || !hero.HasBuff("SivirShield") || !hero.HasBuff("BansheesVeil")
                       || !hero.HasBuff("ShroudofDarkness");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return false;
        }

        #endregion
    }
}