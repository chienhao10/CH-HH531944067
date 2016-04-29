using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using Spell = LeagueSharp.Common.Spell;

namespace GFUELElise
{
    internal class Elise
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Fired when the game loads.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void OnGameLoad()
        {
            try
            {
                if (Player.ChampionName != "Elise")
                {
                    return;
                }

                var smiteSlot = Player.GetSpell(SpellSlot.Summoner1).Name.ToLower().Contains("smite")
                    ? SpellSlot.Summoner1
                    : Player.GetSpell(SpellSlot.Summoner2).Name.ToLower().Contains("smite")
                        ? SpellSlot.Summoner2
                        : SpellSlot.Unknown;

                if (smiteSlot != SpellSlot.Unknown)
                {
                    SmiteSpell = new Spell(smiteSlot);
                }

                var igniteSlot = Player.GetSpell(SpellSlot.Summoner1).Name.ToLower().Contains("summonerdot")
                    ? SpellSlot.Summoner1
                    : Player.GetSpell(SpellSlot.Summoner2).Name.ToLower().Contains("summonerdot")
                        ? SpellSlot.Summoner2
                        : SpellSlot.Unknown;

                if (igniteSlot != SpellSlot.Unknown)
                {
                    IgniteSpell = new Spell(igniteSlot, 600f);
                }

                Q = new Spell(SpellSlot.Q, 625f);
                W = new Spell(SpellSlot.W, 950f);
                E = new Spell(SpellSlot.E, 1075f);
                R = new Spell(SpellSlot.R, 0);

                SpiderQ = new Spell(SpellSlot.Q, 475f);
                SpiderW = new Spell(SpellSlot.W, Orbwalking.GetRealAutoAttackRange(Player));
                SpiderE = new Spell(SpellSlot.E, 750f);

                W.SetSkillshot(0.25f, 100f, 1000, true, SkillshotType.SkillshotLine);
                E.SetSkillshot(0.25f, 55f, 1300, true, SkillshotType.SkillshotLine);

                GenerateMenu();

                Game.OnUpdate += OnUpdate;
                Drawing.OnDraw += OnDraw;
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
        ///     Gets or sets the E spell
        /// </summary>
        /// <value>
        ///     The E spell
        /// </value>
        private static Spell E { get; set; }

        /// <summary>
        ///     Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The Smitespell
        /// </value>
        private static Spell IgniteSpell { get; set; }

        /// <summary>
        ///     Gets the set player state human/spider
        /// </summary>
        /// <value>
        ///     The player state
        /// </value>
        private static bool IsSpider
        {
            get
            {
                return Player.Spellbook.GetSpell(SpellSlot.Q).Name == "EliseSpiderQCast"
                       || Player.Spellbook.GetSpell(SpellSlot.W).Name == "EliseSpiderW"
                       || Player.Spellbook.GetSpell(SpellSlot.E).Name == "EliseSpiderEInitial";
            }
        }

        /// <summary>
        ///     Gets the set player state human/spider
        /// </summary>
        /// <value>
        ///     The player state
        /// </value>
        private static bool IsHuman
        {
            get
            {
                return Player.Spellbook.GetSpell(SpellSlot.Q).Name == "EliseHumanQ"
                       || Player.Spellbook.GetSpell(SpellSlot.W).Name == "EliseHumanW"
                       || Player.Spellbook.GetSpell(SpellSlot.E).Name == "EliseHumanE";
            }
        }

        /// <summary>
        ///     Gets or sets the menu
        /// </summary>
        /// <value>
        ///     The menu
        /// </value>
        private static Menu Menu { get; set; }

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
        ///     Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The Smitespell
        /// </value>
        private static Spell SmiteSpell { get; set; }

        /// <summary>
        ///     Gets or sets the spider E spell
        /// </summary>
        /// <value>
        ///     The spider E spell
        /// </value>
        private static Spell SpiderE { get; set; }

        /// <summary>
        ///     Gets or sets the spider Q spell
        /// </summary>
        /// <value>
        ///     The spider Q spell
        /// </value>
        private static Spell SpiderQ { get; set; }

        /// <summary>
        ///     Gets or sets the spider W spell
        /// </summary>
        /// <value>
        ///     The spider W spell
        /// </value>
        private static Spell SpiderW { get; set; }

        /// <summary>
        ///     Gets or sets the W spell
        /// </summary>
        /// <value>
        ///     The W spell
        /// </value>
        private static Spell W { get; set; }

        #endregion

        #region Methods

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!IsSpider)
            {
                if (getCheckBoxItem(miscMenu, "GFUELElise.Misc.Antigapcloser") && E.IsReady())
                {
                    if (gapcloser.Sender.IsValidTarget(E.Range))
                    {
                        E.Cast(gapcloser.Sender);
                    }
                }
            }
        }

        private static void AutoE()
        {
            if (IsHuman)
            {
                if (E.IsReady() && getCheckBoxItem(miscMenu, "GFUELElise.Auto.E"))
                {
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>())
                    {
                        if (enemy.IsValidTarget(E.Range))
                        {
                            var pred = E.GetPrediction(enemy);
                            if (pred.Hitchance >= HitChance.Immobile)
                            {
                                E.Cast(pred.CastPosition);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Combo logic
        /// </summary>
        private static void DoCombo()
        {
            try
            {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
                if (target == null)
                {
                    return;
                }

                if (IsHuman)
                {
                    if (target.HasBuff("buffelisecocoon") && SpiderQ.IsReady() && target.IsValidTarget(SpiderQ.Range))
                    {
                        R.Cast();
                    }

                    if (getCheckBoxItem(comboMenu, "GFUELElise.Combo.E") && target.Distance(Player.Position) <= E.Range &&
                        E.IsReady())
                    {
                        var prediction = E.GetPrediction(target);
                        if (prediction.Hitchance >= HitChance.High)
                        {
                            E.Cast(prediction.CastPosition);
                        }
                    }

                    if (getCheckBoxItem(comboMenu, "GFUELElise.Combo.Q") && Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        Q.CastOnUnit(target);
                    }

                    if (getCheckBoxItem(comboMenu, "GFUELElise.Combo.W") && W.IsReady() && target.IsValidTarget(W.Range))
                    {
                        var prediction = W.GetPrediction(target);
                        if (prediction.CollisionObjects.Count == 0)
                        {
                            W.Cast(target.ServerPosition);
                        }
                    }

                    if (getCheckBoxItem(comboMenu, "GFUELElise.Combo.R"))
                    {
                        if (Player.ManaPercent < getSliderItem(comboMenu, "GFUELElise.R.nein"))
                        {
                            R.Cast();
                        }

                        if (Player.Distance(target) <= 750 && R.IsReady()
                            && (!Q.IsReady() && !W.IsReady() && !E.IsReady()
                                || !Q.IsReady() && !W.IsReady() && !E.IsReady()))
                        {
                            R.Cast();
                        }

                        if (SpiderQ.IsReady() && target.IsValidTarget(SpiderQ.Range)
                            && target.IsValidTarget(SpiderQ.Range))
                        {
                            R.Cast();
                        }
                    }
                }

                if (IsSpider)
                {
                    if (getCheckBoxItem(comboMenu, "GFUELElise.ComboSpider.Q") && SpiderQ.IsReady())
                    {
                        if (target.IsValidTarget(SpiderQ.Range))
                        {
                            SpiderQ.Cast(target);
                        }
                    }

                    if (getCheckBoxItem(comboMenu, "GFUELElise.ComboSpider.W") && Player.Distance(target) <= 140 &&
                        SpiderW.IsReady())
                    {
                        if (target.IsValidTarget(SpiderW.Range))
                        {
                            SpiderW.Cast();
                        }
                    }

                    if (getCheckBoxItem(comboMenu, "GFUELElise.ComboSpider.E") &&
                        Player.Distance(target) <= SpiderE.Range && Player.Distance(target) > SpiderQ.Range &&
                        SpiderE.IsReady())
                    {
                        if (target.IsValidTarget(SpiderQ.Range)) return;
                        SpiderE.Cast(target);
                    }

                    if (getCheckBoxItem(comboMenu, "GFUELElise.Combo.R"))
                    {
                        if (Player.ManaPercent < getSliderItem(comboMenu, "GFUELElise.R.nein"))
                        {
                            return;
                        }

                        if (R.IsReady() && !target.IsValidTarget(SpiderQ.Range) && !SpiderE.IsReady())
                        {
                            R.Cast();
                        }

                        if (!SpiderQ.IsReady() && !SpiderW.IsReady() && R.IsReady())
                        {
                            R.Cast();
                        }

                        if (!SpiderQ.IsReady() && !SpiderE.IsReady() && !SpiderW.IsReady()
                            || !SpiderQ.IsReady() && Q.IsReady() && Q.GetDamage(target) > target.Health)
                        {
                            R.Cast();
                        }
                    }

                    if (getCheckBoxItem(comboMenu, "GFUELElise.ComboSpider.E") &&
                        Player.Distance(target) > SpiderQ.Range && SpiderE.IsReady())
                    {
                        SpiderE.Cast(target);
                    }
                }

                if (getCheckBoxItem(comboMenu, "GFUELElise.Combo.R"))
                {
                    if (!Q.IsReady() && !W.IsReady() && !R.IsReady()
                        || (Q.IsReady() && Q.GetDamage(target) >= target.Health)
                        || W.IsReady() && W.GetDamage(target) >= target.Health)
                    {
                        if (SpiderQ.IsReady() && target.IsValidTarget(SpiderQ.Range))
                        {
                            return;
                        }

                        R.Cast();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        ///     Harass logic
        /// </summary>
        private static void DoHarass()
        {
            try
            {
                var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
                if (target == null)
                {
                    return;
                }

                if (!IsSpider)
                {
                    if (getCheckBoxItem(harassMenu, "GFUELElise.Harass.Q") && Q.IsReady() &&
                        target.IsValidTarget(Q.Range))
                    {
                        Q.Cast(target);
                    }

                    if (getCheckBoxItem(harassMenu, "GFUELElise.Harass.W") && W.IsReady() &&
                        target.IsValidTarget(W.Range))
                    {
                        var prediction = W.GetPrediction(target);
                        if (prediction.CollisionObjects.Count == 0)
                        {
                            W.Cast(target.ServerPosition);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void DoLaneclear()
        {
            try
            {
                var minion = MinionManager.GetMinions(Player.Position, Q.Range + W.Width).FirstOrDefault();
                if (minion == null)
                {
                    return;
                }

                if (!IsSpider)
                {
                    if (Player.ManaPercent < getSliderItem(laneClearMenu, "GFUELElise.laneclear.Mana"))
                    {
                        if (getCheckBoxItem(laneClearMenu, "GFUELElise.laneclear.SwitchR") && R.IsReady())
                        {
                            R.Cast();
                        }
                    }

                    if (getCheckBoxItem(laneClearMenu, "GFUELElise.laneclear.Q") && Q.IsReady())
                    {
                        Q.Cast(minion);
                    }

                    if (getCheckBoxItem(laneClearMenu, "GFUELElise.laneclear.W") && W.IsReady() &&
                        minion.IsValidTarget(W.Range))
                    {
                        W.Cast(minion.Position);
                    }
                    if (getCheckBoxItem(laneClearMenu, "GFUELElise.laneclear.SwitchR") && !Q.IsReady() && !W.IsReady() ||
                        Player.ManaPercent < getSliderItem(laneClearMenu, "GFUELElise.laneclear.Mana"))
                    {
                        R.Cast();
                    }
                }

                if (IsSpider)
                {
                    if (getCheckBoxItem(laneClearMenu, "GFUELElise.laneclear.SpiderQ") && SpiderQ.IsReady())
                    {
                        SpiderQ.Cast(minion);
                    }

                    if (getCheckBoxItem(laneClearMenu, "GFUELElise.laneclear.SpiderW") && W.IsReady() &&
                        minion.IsValidTarget(SpiderW.Range))
                    {
                        SpiderW.Cast();
                    }


                    if (getCheckBoxItem(laneClearMenu, "GFUELElise.laneclear.SwitchR") && R.IsReady() && Q.IsReady() &&
                        !SpiderQ.IsReady() && !SpiderW.IsReady())
                    {
                        R.Cast();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static void DoJungleclear()
        {
            try
            {
                var minion =
                    MinionManager.GetMinions(
                        Player.ServerPosition,
                        Q.Range + W.Width,
                        MinionTypes.All,
                        MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth).FirstOrDefault();

                if (minion == null)
                {
                    return;
                }

                if (!IsSpider)
                {
                    if (getCheckBoxItem(jungleClearMenu, "GFUELElise.jungleclear.Q") && Q.IsReady())
                    {
                        Q.Cast(minion);
                    }

                    if (getCheckBoxItem(jungleClearMenu, "GFUELElise.jungleclear.W") && W.IsReady() &&
                        minion.IsValidTarget(W.Range))
                    {
                        W.Cast(minion.Position);
                    }

                    if (getCheckBoxItem(jungleClearMenu, "GFUELElise.jungleclear.SwitchR") && !Q.IsReady() &&
                        !W.IsReady() ||
                        Player.ManaPercent < getSliderItem(jungleClearMenu, "GFUELElise.jungleclear.Mana"))
                    {
                        R.Cast();
                    }
                }

                if (IsSpider)
                {
                    if (getCheckBoxItem(jungleClearMenu, "GFUELElise.jungleclear.SpiderQ") && SpiderQ.IsReady())
                    {
                        SpiderQ.Cast(minion);
                    }

                    if (getCheckBoxItem(jungleClearMenu, "GFUELElise.jungleclear.SpiderW") && W.IsReady() &&
                        minion.IsValidTarget(SpiderW.Range))
                    {
                        SpiderW.Cast();
                    }

                    if (getCheckBoxItem(jungleClearMenu, "GFUELElise.jungleclear.SwitchR") && R.IsReady() && Q.IsReady() &&
                        !SpiderQ.IsReady() && !SpiderW.IsReady())
                    {
                        R.Cast();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        ///     Creates the menu
        /// </summary>
        /// <value>
        ///     Creates the menu
        /// </value>
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

        public static Menu comboMenu, harassMenu, laneClearMenu, jungleClearMenu, smiteMenu, miscMenu;

        private static void GenerateMenu()
        {
            try
            {
                Menu = MainMenu.AddMenu("GFUEL 蜘蛛", "GFUELELISE");

                comboMenu = Menu.AddSubMenu("连招", "Combo");
                comboMenu.Add("GFUELElise.Combo.Q", new CheckBox("使用 Q"));
                comboMenu.Add("GFUELElise.Combo.W", new CheckBox("使用 W"));
                comboMenu.Add("GFUELElise.Combo.E", new CheckBox("使用 E"));
                comboMenu.Add("GFUELElise.ComboSpider.Q", new CheckBox("使用 蜘蛛 Q"));
                comboMenu.Add("GFUELElise.ComboSpider.W", new CheckBox("使用 蜘蛛 W"));
                comboMenu.Add("GFUELElise.ComboSpider.E", new CheckBox("使用 蜘蛛 E"));
                comboMenu.Add("GFUELElise.Combo.R", new CheckBox("自动切换形态"));
                comboMenu.Add("GFUELElise.Combo.Semi.E", new KeyBind("使用按键 E", false, KeyBind.BindTypes.HoldActive, 'T'));
                comboMenu.Add("GFUELElise.R.nein", new Slider("蓝量低于% 不切换成人类形态", 10));

                harassMenu = Menu.AddSubMenu("骚扰", "Harass");
                harassMenu.Add("GFUELElise.Harass.Q", new CheckBox("使用 Q"));
                harassMenu.Add("GFUELElise.Harass.W", new CheckBox("使用 W"));

                laneClearMenu = Menu.AddSubMenu("清线", "Laneclear");
                laneClearMenu.Add("GFUELElise.laneclear.Q", new CheckBox("使用 Q"));
                laneClearMenu.Add("GFUELElise.laneclear.W", new CheckBox("使用 W"));
                laneClearMenu.Add("GFUELElise.laneclear.SpiderQ", new CheckBox("使用 蜘蛛 Q"));
                laneClearMenu.Add("GFUELElise.laneclear.SpiderW", new CheckBox("使用 蜘蛛 W"));
                laneClearMenu.Add("GFUELElise.laneclear.SwitchR", new CheckBox("切换 R"));
                laneClearMenu.Add("GFUELElise.laneclear.Mana", new Slider("最低蓝量", 20));

                jungleClearMenu = Menu.AddSubMenu("清野", "Jungleclear");
                jungleClearMenu.Add("GFUELElise.jungleclear.Q", new CheckBox("使用 Q"));
                jungleClearMenu.Add("GFUELElise.jungleclear.W", new CheckBox("使用 W"));
                jungleClearMenu.Add("GFUELElise.jungleclear.SpiderQ", new CheckBox("使用 蜘蛛 Q"));
                jungleClearMenu.Add("GFUELElise.jungleclear.SpiderW", new CheckBox("使用 蜘蛛 W"));
                jungleClearMenu.Add("GFUELElise.jungleclear.SwitchR", new CheckBox("切换 R"));
                jungleClearMenu.Add("GFUELElise.jungleclear.Mana", new Slider("Minimum mana", 20));

                smiteMenu = Menu.AddSubMenu("惩戒", "Smite");
                smiteMenu.Add("GFUELElise.Smite.Nope", new CheckBox("人类形态不使用惩戒", false));
                smiteMenu.Add("GFUELElise.Smite", new CheckBox("蜘蛛连招模式下使用惩戒"));

                miscMenu = Menu.AddSubMenu("杂项", "Miscellaneous");
                miscMenu.Add("GFUELElise.Auto.E", new CheckBox("自动 E 不可移动目标"));
                miscMenu.Add("GFUELElise.Draw.Off", new CheckBox("屏蔽 线圈", false));
                miscMenu.Add("GFUELElise.Draw.Q", new CheckBox("显示 Q"));
                miscMenu.Add("GFUELElise.Draw.W", new CheckBox("显示 W"));
                miscMenu.Add("GFUELElise.Draw.E", new CheckBox("显示 E"));
                miscMenu.Add("GFUELElise.Misc.Antigapcloser", new CheckBox("使用 E - 防突进"));
                miscMenu.Add("GFUELElise.Misc.Interrupter", new CheckBox("使用 E - 技能打断"));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static void Interrupter2_OnInterruptableTarget(
            AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!IsSpider)
            {
                if (getCheckBoxItem(miscMenu, "GFUELElise.Misc.Interrupter") && E.IsReady())
                {
                    if (sender.IsValidTarget(E.Range))
                    {
                        E.Cast(sender);
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
                if (Player.IsDead)
                {
                    return;
                }


                if (getCheckBoxItem(miscMenu, "GFUELElise.Draw.Off"))
                {
                    return;
                }

                if (getCheckBoxItem(miscMenu, "GFUELElise.Draw.Q"))
                {
                    if (Q.Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.OrangeRed);
                    }
                }

                if (getCheckBoxItem(miscMenu, "GFUELElise.Draw.W"))
                {
                    if (W.Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Color.DeepSkyBlue);
                    }
                }

                if (getCheckBoxItem(miscMenu, "GFUELElise.Draw.E"))
                {
                    if (E.Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.DeepSkyBlue);
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
                    DoCombo();
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    DoHarass();
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    DoJungleclear();
                    DoLaneclear();
                }

                if (getKeyBindItem(comboMenu, "GFUELElise.Combo.Semi.E"))
                {
                    SemiE();
                }

                if (getCheckBoxItem(miscMenu, "GFUELElise.Auto.E"))
                {
                    AutoE();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static void SemiE()
        {
            Orbwalker.OrbwalkTo(Game.CursorPos);

            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            if (!E.IsReady() || !target.IsValidTarget(E.Range))
            {
                return;
            }

            var prediction = E.GetPrediction(target);
            E.Cast(prediction.CastPosition);
        }

        #endregion
    }
}