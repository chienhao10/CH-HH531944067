namespace Irelia_Reloaded
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Reflection;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK;/// <summary>
                       ///     The program.
                       /// </summary>
    internal class Program
    {
        #region Static Fields

        /// <summary>
        ///     The gatotsu tick
        /// </summary>
        private static int gatotsuTick;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the botrk.
        /// </summary>
        /// <value>
        ///     The botrk.
        /// </value>
        private static Items.Item Botrk { get; set; }

        /// <summary>
        ///     Gets or sets the cutlass.
        /// </summary>
        /// <value>
        ///     The cutlass.
        /// </value>
        private static Items.Item Cutlass { get; set; }

        /// <summary>
        ///     Gets or sets the e.
        /// </summary>
        /// <value>
        ///     The e.
        /// </value>
        private static LeagueSharp.Common.Spell E { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this instance has sheen buff.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has sheen buff; otherwise, <c>false</c>.
        /// </value>
        private static bool HasSheenBuff
            => Player.HasBuff("sheen") || Player.HasBuff("LichBane") || Player.HasBuff("ItemFrozenFist");

        /// <summary>
        ///     Gets or sets the ignite slot.
        /// </summary>
        /// <value>
        ///     The ignite slot.
        /// </value>
        private static SpellSlot IgniteSlot { get; set; }

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        private static Menu Menu { get; set; }

        /// <summary>
        ///     Gets or sets the omen.
        /// </summary>
        /// <value>
        ///     The omen.
        /// </value>
        private static Items.Item Omen { get; set; }

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private static AIHeroClient Player => ObjectManager.Player;

        /// <summary>
        ///     Gets or sets the q.
        /// </summary>
        /// <value>
        ///     The q.
        /// </value>
        private static LeagueSharp.Common.Spell Q { get; set; }

        /// <summary>
        ///     Gets or sets the r.
        /// </summary>
        /// <value>
        ///     The r.
        /// </value>
        private static LeagueSharp.Common.Spell R { get; set; }

        /// <summary>
        ///     Gets a value indicating whether the ult is activated.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the ult is activated; otherwise, <c>false</c>.
        /// </value>
        private static bool UltActivated => Player.HasBuff("IreliaTranscendentBladesSpell");

        /// <summary>
        ///     Gets or sets the w.
        /// </summary>
        /// <value>
        ///     The w.
        /// </value>
        private static LeagueSharp.Common.Spell W { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Fired when the OnGameLoad event is fired.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void GameOnOnGameLoad()
        {
            if (Player.CharData.BaseSkinName != "Irelia")
            {
                return;
            }

            // Setup Spells
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 650);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, Orbwalking.GetRealAutoAttackRange(Player));
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 425);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 1000);

            // Setup Ignite
            IgniteSlot = Player.GetSpellSlot("SummonerDot");

            // Add skillshots
            Q.SetTargetted(0f, 2200);
            R.SetSkillshot(0.5f, 120, 1600, false, SkillshotType.SkillshotLine);

            // Create Items
            Botrk = ItemData.Blade_of_the_Ruined_King.GetItem();
            Cutlass = ItemData.Bilgewater_Cutlass.GetItem();
            Omen = ItemData.Randuins_Omen.GetItem();

            // Create Menu
            SetupMenu();

            // Setup Dmg Indicator
            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = DamageToUnit;
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = true;

            // Subscribe to needed events
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += DrawingOnOnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloserOnOnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += InterrupterOnOnPossibleToInterrupt;

            // to get Q tickcount in least amount of lines.
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Obj_AI_Base.OnBuffGain += (sender, eventArgs) =>
            {
                if (sender.IsMe) Console.WriteLine(eventArgs.Buff.Name);
            };
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fired when there is an incoming gapcloser.
        /// </summary>
        /// <param name="gapcloser">The gapcloser.</param>
        private static void AntiGapcloserOnOnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.LSIsValidTarget() && getCheckBoxItem(miscMenu, "gapcloserE") && E.IsReady())
            {
                E.Cast(gapcloser.Sender);
            }
        }

        /// <summary>
        ///     Does the combo.
        /// </summary>
        private static void Combo()
        {
            var useQ = getCheckBoxItem(comboMenu, "useQ");
            var useW = getCheckBoxItem(comboMenu, "useW");
            var useE = getCheckBoxItem(comboMenu, "useE");
            var useR = getCheckBoxItem(comboMenu, "useR");
            var minQRange = getSliderItem(comboMenu, "minQRange");
            var useEStun = getCheckBoxItem(comboMenu, "useEStun");
            var useQGapclose = getCheckBoxItem(comboMenu, "useQGapclose");
            var useWBeforeQ = getCheckBoxItem(comboMenu, "useWBeforeQ");
            var procSheen = getCheckBoxItem(comboMenu, "procSheen");
            var useIgnite = getCheckBoxItem(comboMenu, "useIgnite");
            var useRGapclose = getCheckBoxItem(comboMenu, "useRGapclose");

            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

            if (target == null && useQGapclose)
            {
                /** var minionQ =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(x => x.LSIsValidTarget())
                        .Where(x => Player.LSGetSpellDamage(x, SpellSlot.Q) > x.Health)
                        .FirstOrDefault(
                            x =>
                                x.LSDistance(TargetSelector.GetTarget(Q.Range * 5, TargetSelector.DamageType.Physical)) <
                                Q.Range);*/
                var minionQ =
                    MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                        .FirstOrDefault(
                            x =>
                            Q.IsKillable(x) && Q.IsInRange(x)
                            && x.LSDistance(HeroManager.Enemies.OrderBy(y => y.LSDistance(Player)).FirstOrDefault())
                            < Player.LSDistance(HeroManager.Enemies.OrderBy(z => z.LSDistance(Player)).FirstOrDefault()));

                if (minionQ != null && Player.Mana > Q.ManaCost * 2)
                {
                    Q.CastOnUnit(minionQ);
                    return;
                }

                if (useRGapclose)
                {
                    var minionR =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(
                                x =>
                                x.LSIsValidTarget() && x.LSDistance(Player) < Q.Range && x.LSCountEnemiesInRange(Q.Range) >= 1)
                            .FirstOrDefault(
                                x =>
                                x.Health - Player.LSGetSpellDamage(x, SpellSlot.R) < Player.LSGetSpellDamage(x, SpellSlot.Q));

                    if (minionR != null)
                    {
                        R.Cast(minionR);
                    }
                }
            }

            // Get target that is in the R range
            var rTarget = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            if (useR && UltActivated && rTarget.LSIsValidTarget())
            {
                if (procSheen)
                {
                    // Fire Ult if player is out of AA range, with Q not up or not in range
                    if (target.LSDistance(Player) > Orbwalking.GetRealAutoAttackRange(Player))
                    {
                        R.Cast(rTarget);
                    }
                    else
                    {
                        if (!HasSheenBuff)
                        {
                            R.Cast(rTarget);
                        }
                    }
                }
                else
                {
                    R.Cast(rTarget);
                }
            }

            if (!target.LSIsValidTarget())
            {
                return;
            }

            if (Botrk.IsReady())
            {
                Botrk.Cast(target);
            }

            if (Cutlass.IsReady())
            {
                Cutlass.Cast(target);
            }

            if (Omen.IsReady() && Omen.IsInRange(target)
                && target.LSDistance(Player) > Orbwalking.GetRealAutoAttackRange(Player))
            {
                Omen.Cast();
            }

            if (useIgnite && target != null && target.LSIsValidTarget(600)
                && (IgniteSlot.IsReady()
                    && Player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite) > target.Health))
            {
                Player.Spellbook.CastSpell(IgniteSlot, target);
            }

            if (useWBeforeQ)
            {
                if (useW && W.IsReady())
                {
                    W.Cast();
                }

                if (useQ && Q.IsReady() && target.LSDistance(Player.ServerPosition) > minQRange)
                {
                    Q.CastOnUnit(target);
                }
            }
            else
            {
                if (useQ && Q.IsReady() && target.LSDistance(Player.ServerPosition) > minQRange)
                {
                    Q.CastOnUnit(target);
                }

                if (useW && W.IsReady())
                {
                    W.Cast();
                }
            }

            if (useEStun)
            {
                if (target.CanStunTarget() && useE && E.IsReady())
                {
                    E.Cast(target);
                }
            }
            else
            {
                if (useE && E.IsReady())
                {
                    E.Cast(target);
                }
            }

            if (useR && R.IsReady() && !UltActivated)
            {
                R.Cast(target);
            }
        }

        /// <summary>
        ///     Get the damage to a hero.
        /// </summary>
        /// <param name="hero">The hero.</param>
        /// <returns>The damage done to the hero.</returns>
        private static float DamageToUnit(AIHeroClient hero)
        {
            float dmg = 0;

            var spells = new List<LeagueSharp.Common.Spell> { Q, W, E, R };
            foreach (var spell in spells.Where(x => x.IsReady()))
            {
                // Account for each blade
                if (spell.Slot == SpellSlot.R)
                {
                    dmg += spell.GetDamage(hero) * 4;
                }
                else
                {
                    dmg += spell.GetDamage(hero);
                }
            }

            if (Botrk.IsReady())
            {
                dmg += (float)Player.GetItemDamage(hero, LeagueSharp.Common.Damage.DamageItems.Botrk);
            }

            if (Cutlass.IsReady())
            {
                dmg += (float)Player.GetItemDamage(hero, LeagueSharp.Common.Damage.DamageItems.Bilgewater);
            }

            return dmg;
        }

        /// <summary>
        ///     Fired when the game redraws itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void DrawingOnOnDraw(EventArgs args)
        {
            var drawQ = getCheckBoxItem(drawMenu, "drawQ");
            var drawE = getCheckBoxItem(drawMenu, "drawE");
            var drawR = getCheckBoxItem(drawMenu, "drawR");
            var drawStunnable = getCheckBoxItem(drawMenu, "drawStunnable");
            var p = Player.Position;

            if (drawQ)
            {
                Render.Circle.DrawCircle(p, Q.Range, Q.IsReady() ? Color.Aqua : Color.Red);
            }

            if (drawE)
            {
                Render.Circle.DrawCircle(p, E.Range, E.IsReady() ? Color.Aqua : Color.Red);
            }

            if (drawR)
            {
                Render.Circle.DrawCircle(p, R.Range, R.IsReady() ? Color.Aqua : Color.Red);
            }

            foreach (var minion in
                MinionManager.GetMinions(Q.Range).Where(x => Player.LSGetSpellDamage(x, SpellSlot.Q) > x.Health))
            {
                Render.Circle.DrawCircle(minion.Position, 65, Color.FromArgb(124, 252, 0), 3);
            }

            if (!drawStunnable)
            {
                return;
            }

            foreach (var unit in
                ObjectManager.Get<AIHeroClient>().Where(x => x.CanStunTarget() && x.LSIsValidTarget()))
            {
                var drawPos = Drawing.WorldToScreen(unit.Position);
                var textSize = 60;
                Drawing.DrawText(drawPos.X - textSize / 2f, drawPos.Y, Color.Aqua, "Stunnable");
            }
        }

        /// <summary>
        ///     Fired when the game updates.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void Game_OnGameUpdate(EventArgs args)
        {
            KillSteal();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LastHit();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear();
                WaveClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
        }

        private static void Harass()
        {
            var useQ = getCheckBoxItem(harassMenu, "UseQHarass");
            var useW = getCheckBoxItem(harassMenu, "UseWHarass");
            var useE = getCheckBoxItem(harassMenu, "UseEHarass");

            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

            if (!target.LSIsValidTarget())
            {
                return;
            }

            if (useQ && Q.IsReady())
            {
                Q.CastOnUnit(target);
            }

            if (useW && W.IsReady() && Orbwalking.InAutoAttackRange(target))
            {
                W.Cast();
            }

            if (useE && E.IsReady())
            {
                E.CastOnUnit(target);
            }
        }

        /// <summary>
        ///     Fired when there is an interruptable target.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Interrupter2.InterruptableTargetEventArgs" /> instance containing the event data.</param>
        private static void InterrupterOnOnPossibleToInterrupt(
            AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            var spell = args;
            var unit = sender;

            if (spell.DangerLevel != Interrupter2.DangerLevel.High || !unit.CanStunTarget())
            {
                return;
            }

            var interruptE = getCheckBoxItem(miscMenu, "interruptE");
            var interruptQe = getCheckBoxItem(miscMenu, "interruptQE");

            if (E.IsReady() && E.IsInRange(unit, E.Range) && interruptE)
            {
                E.Cast(unit);
            }

            if (Q.IsReady() && E.IsReady() && Q.IsInRange(unit, Q.Range) && interruptQe)
            {
                Q.Cast(unit);

                var timeToArrive = (int)(1000 * Player.LSDistance(unit) / Q.Speed + Q.Delay + Game.Ping);
                LeagueSharp.Common.Utility.DelayAction.Add(timeToArrive, () => E.Cast(unit));
            }
        }

        private static void JungleClear()
        {
            var useQ = getCheckBoxItem(jungleClearMenu, "UseQJungleClear");
            var useW = getCheckBoxItem(jungleClearMenu, "UseWJungleClear");
            var useE = getCheckBoxItem(jungleClearMenu, "UseEJungleClear");

            var orbwalkerTarget = Orbwalker.LastTarget;
            var minion = orbwalkerTarget as Obj_AI_Minion;

            if (minion == null || minion.Team != GameObjectTeam.Neutral)
            {
                if (minion != null || !Q.IsReady())
                {
                    return;
                }

                var bestQMinion =
                    MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral)
                        .OrderByDescending(x => x.MaxHealth)
                        .FirstOrDefault();

                if (bestQMinion != null)
                {
                    Q.Cast(bestQMinion);
                }

                return;
            }

            if (useQ && Q.IsReady())
            {
                Q.Cast(minion);
            }

            if (useW && Player.LSDistance(minion) < Orbwalking.GetAttackRange(Player))
            {
                W.Cast();
            }

            if (useE && E.IsReady())
            {
                E.CastOnUnit(minion);
            }
        }

        /// <summary>
        ///     Steals kills.
        /// </summary>
        private static void KillSteal()
        {
            var useQ = getCheckBoxItem(ksMenu, "useQKS");
            var useR = getCheckBoxItem(ksMenu, "useRKS");
            var useIgnite = getCheckBoxItem(ksMenu, "useIgniteKS");

            if (useQ && Q.IsReady())
            {
                var bestTarget =
                    ObjectManager.Get<AIHeroClient>()
                        .Where(x => x.LSIsValidTarget(Q.Range) && Player.LSGetSpellDamage(x, SpellSlot.Q) > x.Health)
                        .OrderBy(x => x.LSDistance(Player))
                        .FirstOrDefault();

                if (bestTarget != null)
                {
                    Q.Cast(bestTarget);
                }
            }

            if (useR && (R.IsReady() || UltActivated))
            {
                var bestTarget =
                    ObjectManager.Get<AIHeroClient>()
                        .Where(x => x.LSIsValidTarget(R.Range))
                        .Where(x => Player.LSGetSpellDamage(x, SpellSlot.Q) > x.Health)
                        .OrderBy(x => x.LSDistance(Player))
                        .FirstOrDefault();

                if (bestTarget != null)
                {
                    R.Cast(bestTarget);
                }
            }

            if (useIgnite && IgniteSlot.IsReady())
            {
                var bestTarget =
                    ObjectManager.Get<AIHeroClient>()
                        .Where(x => x.LSIsValidTarget(600))
                        .Where(x => Player.GetSummonerSpellDamage(x, LeagueSharp.Common.Damage.SummonerSpell.Ignite) / 5 > x.Health)
                        .OrderBy(x => x.ChampionsKilled)
                        .FirstOrDefault();

                if (bestTarget != null)
                {
                    Player.Spellbook.CastSpell(IgniteSlot, bestTarget);
                }
            }
        }

        /// <summary>
        ///     Last hits minions.
        /// </summary>
        private static void LastHit()
        {
            var useQ = getCheckBoxItem(lastHitMenu, "lastHitQ");
            var waitTime = getSliderItem(miscMenu, "gatotsuTime");
            var manaNeeded = getSliderItem(lastHitMenu, "manaNeededQ");
            var dontQUnderTower = getCheckBoxItem(lastHitMenu, "noQMinionTower");

            if (useQ && Player.Mana / Player.MaxMana * 100 > manaNeeded
                && Environment.TickCount - gatotsuTick >= waitTime * 10)
            {
                foreach (var minion in
                    MinionManager.GetMinions(Q.Range).Where(x => Player.LSGetSpellDamage(x, SpellSlot.Q) > x.Health))
                {
                    if (dontQUnderTower && !minion.UnderTurret())
                    {
                        Q.Cast(minion);
                    }
                    else
                    {
                        Q.Cast(minion);
                    }
                }
            }
        }

        /// <summary>
        ///     Fired when the OnProcessSpellCast event is fired.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.SData.Name == "IreliaGatotsu" && sender.IsMe)
            {
                gatotsuTick = Environment.TickCount;
            }
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

        public static Menu comboMenu, harassMenu, ksMenu, lastHitMenu, waveClearMenu, jungleClearMenu, drawMenu, miscMenu;

        /// <summary>
        ///     Setups the menu.
        /// </summary>
        private static void SetupMenu()
        {
            Menu = MainMenu.AddMenu("Irelia Reloaded", "cmIreliaReloaded");

            // Combo
            comboMenu = Menu.AddSubMenu("Combo Settings", "cmCombo");
            comboMenu.AddGroupLabel(":: Q SETTINGS ::");
            comboMenu.Add("useQ", new CheckBox("Use Q"));
            comboMenu.Add("useQGapclose", new CheckBox("Gapclose with Q"));
            comboMenu.Add("minQRange", new Slider("Minimum Q Range", 250, 20, 400));
            comboMenu.AddSeparator();
            comboMenu.AddGroupLabel(":: W SETTINGS ::");
            comboMenu.Add("useW", new CheckBox("Use W"));
            comboMenu.Add("useWBeforeQ", new CheckBox("Use W before Q"));
            comboMenu.AddSeparator();
            comboMenu.AddGroupLabel(":: E SETTINGS ::");
            comboMenu.Add("useE", new CheckBox("Use E"));
            comboMenu.Add("useEStun", new CheckBox("Only Use E to Stun", false));
            comboMenu.AddSeparator();
            comboMenu.AddGroupLabel(":: R SETTINGS ::");
            comboMenu.Add("useR", new CheckBox("Use R"));
            comboMenu.Add("procSheen", new CheckBox("Proc Sheen Before Firing R"));
            comboMenu.Add("useRGapclose", new CheckBox("Use R to Weaken Minion to Gapclose"));
            comboMenu.AddSeparator();
            comboMenu.AddGroupLabel(":: OTHER SETTINGS ::");
            comboMenu.Add("useIgnite", new CheckBox("Use Ignite"));

            // Harass
            harassMenu = Menu.AddSubMenu("Harass Settings", "cmHarass");
            harassMenu.Add("UseQHarass", new CheckBox("Use Q", false));
            harassMenu.Add("UseWHarass", new CheckBox("Use W", false));
            harassMenu.Add("UseEHarass", new CheckBox("Use E", false));
            harassMenu.Add("HarassMana", new Slider("Harass Mana %", 75));

            // KS
            ksMenu = Menu.AddSubMenu("KillSteal Settings", "cmKS");
            ksMenu.Add("useQKS", new CheckBox("KS With Q"));
            ksMenu.Add("useRKS", new CheckBox("KS With R", false));
            ksMenu.Add("useIgniteKS", new CheckBox("KS with Ignite"));

            // Last Hit
            lastHitMenu = Menu.AddSubMenu("Last Hit", "cmLastHit");
            lastHitMenu.Add("lastHitQ", new CheckBox("Last Hit with Q", false));
            lastHitMenu.Add("manaNeededQ", new Slider("Last Hit Mana %", 35));
            lastHitMenu.Add("noQMinionTower", new CheckBox("Don't Q Minion Undertower"));

            // Wave Clear SubMenu
            waveClearMenu = Menu.AddSubMenu("Wave Clear", "cmWaveClear");
            waveClearMenu.Add("waveclearQ", new CheckBox("Use Q"));
            waveClearMenu.Add("waveclearQKillable", new CheckBox("Only Q Killable Minion"));
            waveClearMenu.Add("waveclearW", new CheckBox("Use W"));
            waveClearMenu.Add("waveclearR", new CheckBox("Use R", false));
            waveClearMenu.Add("waveClearMana", new Slider("Wave Clear Mana %", 20));


            // Jungle Clear Menu
            jungleClearMenu = Menu.AddSubMenu("Jungle Clear", "cmJungleClear");
            jungleClearMenu.Add("UseQJungleClear", new CheckBox("Use Q"));
            jungleClearMenu.Add("UseWJungleClear", new CheckBox("Use W"));
            jungleClearMenu.Add("UseEJungleClear", new CheckBox("Use E"));

            // Drawing
            drawMenu = Menu.AddSubMenu("Drawing Settings", "cmDraw");
            drawMenu.Add("drawQ", new CheckBox("Draw Q"));
            drawMenu.Add("drawE", new CheckBox("Draw E"));
            drawMenu.Add("drawR", new CheckBox("Draw R"));
            drawMenu.Add("drawDmg", new CheckBox("Draw Combo Damage"));
            drawMenu.Add("drawStunnable", new CheckBox("Draw Stunnable"));
            drawMenu.Add("drawKillableQ", new CheckBox("Draw Minions Killable with Q", false));

            // Misc
            miscMenu = Menu.AddSubMenu("Miscellaneous Settimgs", "cmMisc");
            miscMenu.Add("gatotsuTime", new Slider("Legit Q Delay (MS) (Farming)", 250, 0, 1500));
            miscMenu.Add("interruptE", new CheckBox("E to Interrupt"));
            miscMenu.Add("interruptQE", new CheckBox("Use Q & E to Interrupt"));
            miscMenu.Add("gapcloserE", new CheckBox("Use E on Gapcloser"));
        }

        /// <summary>
        ///     Does the wave clear.
        /// </summary>
        private static void WaveClear()
        {
            var useQ = getCheckBoxItem(waveClearMenu, "waveclearQ");
            var useQKillable = getCheckBoxItem(waveClearMenu, "waveclearQKillable");
            var useW = getCheckBoxItem(waveClearMenu, "waveclearW");
            var useR = getCheckBoxItem(waveClearMenu, "waveclearR");
            var reqMana = getSliderItem(waveClearMenu, "waveClearMana");
            var waitTime = getSliderItem(miscMenu, "gatotsuTime");
            var dontQUnderTower = getCheckBoxItem(lastHitMenu, "noQMinionTower");

            if (Player.ManaPercent < reqMana)
            {
                return;
            }

            if (useQ && Q.IsReady() && Environment.TickCount - gatotsuTick >= waitTime)
            {
                if (useQKillable)
                {
                    var minion =
                        MinionManager.GetMinions(Q.Range)
                            .FirstOrDefault(
                                x => Q.GetDamage(x) > x.Health && (!dontQUnderTower || !x.UnderTurret(true)));

                    if (minion != null)
                    {
                        Q.Cast(minion);
                    }
                }
                else
                {
                    Q.Cast(MinionManager.GetMinions(Q.Range).FirstOrDefault());
                }
            }

            if (useW && W.IsReady())
            {
                if (Orbwalker.LastTarget is Obj_AI_Minion && W.IsInRange(Orbwalker.LastTarget.Position, W.Range))
                {
                    W.Cast();
                }
            }

            if ((!useR || !R.IsReady())
                && (!R.IsReady() || !UltActivated || Player.LSCountEnemiesInRange(R.Range + 100) != 0))
            {
                return;
            }

            // Get best position for ult
            var pos = R.GetLineFarmLocation(MinionManager.GetMinions(R.Range));
            R.Cast(pos.Position);
        }

        #endregion
    }

    /// <summary>
    ///     Provides helpful extensions
    /// </summary>
    public static class Extension
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Determines whether this instance can stun the target.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <returns><c>true</c> if the Player can stun the unit.</returns>
        public static bool CanStunTarget(this AttackableUnit unit)
        {
            return unit.HealthPercent > ObjectManager.Player.HealthPercent;
        }

        public static SharpDX.Color ToSharpDxColor(this Color color)
        {
            return new SharpDX.Color(color.R, color.G, color.B, color.A);
        }

        #endregion
    }
}