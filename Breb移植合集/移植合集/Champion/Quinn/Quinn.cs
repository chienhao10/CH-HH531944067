using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;
using Spell = LeagueSharp.Common.Spell;

namespace GFUELQuinn
{
    internal class Quinn
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
                if (Player.ChampionName != "Quinn")
                {
                    return;
                }

                GenerateMenu();

                var igniteSlot = Player.GetSpell(SpellSlot.Summoner1).Name.ToLower().Contains("summonerdot")
                    ? SpellSlot.Summoner1
                    : Player.GetSpell(SpellSlot.Summoner2).Name.ToLower().Contains("summonerdot")
                        ? SpellSlot.Summoner2
                        : SpellSlot.Unknown;

                if (igniteSlot != SpellSlot.Unknown)
                {
                    IgniteSpell = new Spell(igniteSlot, 600f);
                }

                Q = new Spell(SpellSlot.Q, 1025f);
                W = new Spell(SpellSlot.W, 2100f);
                E = new Spell(SpellSlot.E, 675f);
                R = new Spell(SpellSlot.R, 0);

                Q.SetSkillshot(313f, 60f, 1550, true, SkillshotType.SkillshotLine);

                Game.OnUpdate += OnUpdate;
                Drawing.OnDraw += OnDraw;
                AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
                Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
                Orbwalker.OnPreAttack += OrbwalkingBeforeAttack;
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
            try
            {
                if (getCheckBoxItem(miscellaneousMenu, "GFUELQuinn.Misc.Antigapcloser") && E.IsReady())
                {
                    if (gapcloser.Sender.IsValidTarget(E.Range))
                    {
                        E.CastOnUnit(gapcloser.Sender);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        ///     Combo logic test
        /// </summary>
        private static void DoCombo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (target == null)
            {
                return;
            }

            var passiveTarget = HeroManager.Enemies.Find(x => x.HasBuff("quinnw") && x.IsValidTarget(Q.Range));
            Orbwalker.ForcedTarget = passiveTarget ?? null;
            if (getCheckBoxItem(comboMenu, "GFUELQuinn.Combo.Ghostblade"))
            {
                var ghostBlade = ItemData.Youmuus_Ghostblade.GetItem();
                if (ghostBlade.IsReady() && ghostBlade.IsOwned(Player)
                    && target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player) + 100))
                {
                    ghostBlade.Cast();
                }
            }

            if (getCheckBoxItem(comboMenu, "GFUELQuinn.Combo.ForceE"))
            {
                if (isBirdForm)
                {
                    if (getCheckBoxItem(comboMenu, "GFUELQuinn.Combo.E") && E.IsInRange(target) && E.IsReady())
                    {
                        E.CastOnUnit(target);
                    }
                }
                else
                {
                    if (getCheckBoxItem(comboMenu, "GFUELQuinn.Combo.Q") && Q.IsInRange(target) && Q.IsReady())
                    {
                        Q.Cast(target);
                    }
                    if (getCheckBoxItem(comboMenu, "GFUELQuinn.Combo.E") && E.IsInRange(target) && E.IsReady())
                    {
                        E.CastOnUnit(target);
                    }
                }
            }
            else
            {
                if (getCheckBoxItem(comboMenu, "GFUELQuinn.Combo.E") && target.Distance(Player.Position) < E.Range && E.IsReady())
                {
                    E.CastOnUnit(target);
                }

                if (getCheckBoxItem(comboMenu, "GFUELQuinn.Combo.Q") && target.Distance(Player.Position) < Q.Range && Q.IsReady())
                {
                    Q.Cast(target);
                }
            }
        }

        /// <summary>
        ///     Harass logic
        /// </summary>
        private static void DoHarass()
        {
            try
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (target == null)
                {
                    return;
                }

                var passiveTarget = HeroManager.Enemies.Find(x => x.HasBuff("quinnw") && x.IsValidTarget(Q.Range));
                Orbwalker.ForcedTarget = passiveTarget ?? null;

                if (getCheckBoxItem(harassMenu, "GFUELQuinn.Harass.Q") && target.Distance(Player.Position) < Q.Range &&
                    Q.IsReady())
                {
                    var prediction = Q.GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.High)
                    {
                        Q.Cast(prediction.CastPosition);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void DoJungleclear()
        {
            try
            {
                var minion =
                    MinionManager.GetMinions(
                        Player.ServerPosition,
                        Q.Range,
                        MinionTypes.All,
                        MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth).OrderByDescending(x => x.MaxHealth).FirstOrDefault();

                if (minion == null)
                {
                    return;
                }

                if (Player.ManaPercent < getSliderItem(jungleclearMenu, "GFUELQuinn.jungleclear.Mana"))
                {
                    return;
                }

                var passiveTarget =
                    MinionManager.GetMinions(Player.Position, Q.Range + Q.Width)
                        .Find(x => x.HasBuff("quinnw") && x.IsValidTarget(Q.Range));
                Orbwalker.ForcedTarget = passiveTarget ?? null;

                if (getCheckBoxItem(jungleclearMenu, "GFUELQuinn.jungleclear.Q"))
                {
                    Q.Cast(minion);
                }

                if (getCheckBoxItem(jungleclearMenu, "GFUELQuinn.jungleclear.Q"))
                {
                    E.CastOnUnit(minion);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
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

        private static void DoLaneclear()
        {
            try
            {
                var minion = MinionManager.GetMinions(Player.Position, Q.Range + Q.Width).FirstOrDefault();
                if (minion == null)
                {
                    return;
                }

                if (Player.ManaPercent < getSliderItem(laneclearMenu, "GFUELQuinn.laneclear.Mana"))
                {
                    return;
                }

                var passiveTarget =
                    MinionManager.GetMinions(Player.Position, Q.Range + Q.Width)
                        .Find(x => x.HasBuff("quinnw") && x.IsValidTarget(Q.Range));
                Orbwalker.ForcedTarget = passiveTarget ?? null;

                if (getCheckBoxItem(laneclearMenu, "GFUELQuinn.laneclear.Q"))
                {
                    if (GetCenterMinion().IsValidTarget())
                    {
                        Q.Cast(GetCenterMinion());
                    }
                }

                if (getCheckBoxItem(laneclearMenu, "GFUELQuinn.laneclear.E"))
                {
                    if (E.GetDamage(minion) > minion.Health)
                    {
                        E.CastOnUnit(minion);
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
        public static Menu comboMenu, harassMenu, laneclearMenu, jungleclearMenu, killstealMenu, miscellaneousMenu;

        private static void GenerateMenu()
        {
            try
            {
                Menu = MainMenu.AddMenu("GFUEL QUINN", "GFUELQUINN");

                comboMenu = Menu.AddSubMenu("Combo", "Combo");
                comboMenu.Add("GFUELQuinn.Combo.Q", new CheckBox("Use Q"));
                comboMenu.Add("GFUELQuinn.Combo.E", new CheckBox("Use E"));
                comboMenu.Add("GFUELQuinn.Combo.ForceE", new CheckBox("Force E Before Q in Bird"));
                comboMenu.Add("GFUELQuinn.Combo.Ghostblade", new CheckBox("Use Ghostblade"));

                harassMenu = Menu.AddSubMenu("Harass", "Harass");
                harassMenu.Add("GFUELQuinn.Harass.Q", new CheckBox("Use Q"));

                laneclearMenu = Menu.AddSubMenu("Laneclear", "Laneclear");
                laneclearMenu.Add("GFUELQuinn.laneclear.Q", new CheckBox("Use Q"));
                laneclearMenu.Add("GFUELQuinn.laneclear.E", new CheckBox("Use E", false));
                laneclearMenu.Add("GFUELQuinn.laneclear.count", new Slider("Minimum minion count", 3, 2, 6));
                laneclearMenu.Add("GFUELQuinn.laneclear.Mana", new Slider("Minimum mana", 20, 0, 10));

                jungleclearMenu = Menu.AddSubMenu("Jungleclear", "Jungleclear");
                jungleclearMenu.Add("GFUELQuinn.jungleclear.Q", new CheckBox("Use Q"));
                jungleclearMenu.Add("GFUELQuinn.jungleclear.E", new CheckBox("Use E"));
                jungleclearMenu.Add("GFUELQuinn.jungleclear.Mana", new Slider("Minimum mana", 20));

                killstealMenu = Menu.AddSubMenu("Killsteal", "Killsteal");
                killstealMenu.Add("GFUELElise.Killsteal.Q", new CheckBox("Killsteal Q"));

                miscellaneousMenu = Menu.AddSubMenu("Miscellaneous", "Miscellaneous");
                miscellaneousMenu.Add("GFUELQuinn.Draw.Off", new CheckBox("Disable drawings", false));
                miscellaneousMenu.Add("GFUELQuinn.Draw.Q", new CheckBox("Draw Q"));
                miscellaneousMenu.Add("GFUELQuinn.Draw.E", new CheckBox("Draw E"));
                miscellaneousMenu.AddSeparator();
                miscellaneousMenu.Add("GFUELQuinn.Misc.Antigapcloser", new CheckBox("Use E - Antigapcloser"));
                miscellaneousMenu.Add("GFUELQuinn.Misc.Interrupter", new CheckBox("Use E - interrupter"));
                miscellaneousMenu.AddSeparator();
                miscellaneousMenu.Add("GFUELQuinn.Auto.R", new CheckBox("Auto R in base"));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Credits to Legacy :-]
        private static Obj_AI_Base GetCenterMinion()
        {
            var minions = MinionManager.GetMinions(Q.Range + 500);
            var centerlocation =
                MinionManager.GetBestCircularFarmLocation(
                    minions.Select(x => x.Position.To2D()).ToList(),
                    500,
                    Q.Range);

            return centerlocation.MinionsHit >= getSliderItem(laneclearMenu, "GFUELQuinn.laneclear.count")
                ? MinionManager.GetMinions(1000)
                    .OrderBy(x => x.Distance(centerlocation.Position))
                    .FirstOrDefault()
                : null;
        }

        private static void Interrupter2_OnInterruptableTarget(
            AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            try
            {
                if (getCheckBoxItem(miscellaneousMenu, "GFUELQuinn.Misc.Interrupter") && E.IsReady())
                {
                    if (sender.IsValidTarget(E.Range))
                    {
                        E.CastOnUnit(sender);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
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
                if (getCheckBoxItem(miscellaneousMenu, "GFUELQuinn.Draw.Off"))
                {
                    return;
                }

                if (getCheckBoxItem(miscellaneousMenu, "GFUELQuinn.Draw.Q"))
                {
                    if (Q.Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.White);
                    }
                }

                if (getCheckBoxItem(miscellaneousMenu, "GFUELQuinn.Draw.E"))
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
        ///     Killsteal logic
        /// </summary>
        private static void OnKillsteal()
        {
            try
            {
                foreach (
                    var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && !x.IsDead && !x.IsZombie))
                {
                    if (enemy.IsValidTarget(Q.Range) && enemy.Health < Q.GetDamage(enemy))
                    {
                        var prediction = Q.GetPrediction(enemy);
                        if (prediction.Hitchance >= HitChance.High)
                        {
                            Q.Cast(prediction.CastPosition);
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

                if (Player.IsInShopRange() && getCheckBoxItem(miscellaneousMenu, "GFUELQuinn.Auto.R") && !R.Instance.Name.Contains("Return") && R.Instance.Name == "QuinnR")
                {
                    if (!isBirdForm)
                        R.Cast();
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

                if (getCheckBoxItem(killstealMenu, "GFUELElise.Killsteal.Q"))
                {
                    OnKillsteal();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static bool isBirdForm
        {
            get
            {
                return ObjectManager.Player.HasBuff("QuinnR");
            }
        }

        // QuinnR

        private static void OrbwalkingBeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
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

                    var targeta = HeroManager.Enemies.Find(x => x.HasBuff("quinnw") && x.IsValidTarget(Q.Range));
                    if (targeta == null)
                    {
                        return;
                    }
                    if (Orbwalking.InAutoAttackRange(targeta))
                    {
                        Orbwalker.ForcedTarget = targeta;
                    }
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit)
                    || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                {
                    var minion = args.Target as Obj_AI_Minion;
                    if (minion != null && minion.HasBuff("quinnw"))
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
    }
}