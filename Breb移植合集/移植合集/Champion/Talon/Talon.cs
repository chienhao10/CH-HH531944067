using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using Damage = LeagueSharp.Common.Damage;
using Geometry = LeagueSharp.Common.Geometry;
using ItemData = LeagueSharp.Common.Data.ItemData;
using Spell = LeagueSharp.Common.Spell;

namespace GFUELTalon
{
    internal class Talon
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
                if (Player.ChampionName != "Talon")
                {
                    return;
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

                Q = new Spell(SpellSlot.Q, Orbwalking.GetRealAutoAttackRange(Player) + 100);
                W = new Spell(SpellSlot.W, 600f);
                E = new Spell(SpellSlot.E, 700f);
                R = new Spell(SpellSlot.R, 650f);

                W.SetSkillshot(0.25f, 75, 2300, false, SkillshotType.SkillshotLine);

                GenerateMenu();

                Game.OnUpdate += OnUpdate;
                Drawing.OnDraw += OnDraw;
                AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
                Orbwalker.OnPostAttack += OrbwalkingAfterAttack;
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
        ///     Gets Ravenous Hydra
        /// </summary>
        /// <value>
        ///     Ravenous Hydra
        /// </value>
        private static Items.Item Hydra
        {
            get { return ItemData.Ravenous_Hydra_Melee_Only.GetItem(); }
        }

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
        ///     Gets Tiamat Item
        /// </summary>
        /// <value>
        ///     Tiamat Item
        /// </value>
        private static Items.Item Tiamat
        {
            get { return ItemData.Tiamat_Melee_Only.GetItem(); }
        }

        /// <summary>
        ///     Gets Titanic Hydra
        /// </summary>
        /// <value>
        ///     Titanic Hydra
        /// </value>
        private static Items.Item Titanic
        {
            get { return ItemData.Titanic_Hydra_Melee_Only.GetItem(); }
        }

        /// <summary>
        ///     Gets or sets the W spell
        /// </summary>
        /// <value>
        ///     The W spell
        /// </value>
        private static Spell W { get; set; }

        /// <summary>
        ///     Gets Youmuus Ghostblade
        /// </summary>
        /// <value>
        ///     Youmuus Ghostblade
        /// </value>
        private static Items.Item Youmuu
        {
            get { return ItemData.Youmuus_Ghostblade.GetItem(); }
        }

        #endregion

        #region Methods

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            try
            {
                if (getCheckBoxItem(miscellaneousMenu, "GFUELTalon.Misc.Antigapcloser") && E.IsReady())
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
        ///     Combo logic
        /// </summary>
        private static void DoCombo()
        {
            try
            {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                if (target == null)
                {
                    return;
                }

                if (getCheckBoxItem(comboMenu, "GFUELTalon.Combo.E") && target.IsValidTarget(E.Range) && E.IsReady())
                {
                    if (getCheckBoxItem(comboMenu, "GFUELTalon.Combo.Towercheck"))
                    {
                        var underTower = target.UnderTurret();
                        if (underTower)
                        {
                            return;
                        }
                    }

                    E.Cast(target);
                }

                if (getCheckBoxItem(comboMenu, "GFUELTalon.Combo.R") && R.IsReady())
                {
                    if (getCheckBoxItem(comboMenu, "GFUELTalon.Combo.Killability"))
                    {
                        if (getCheckBoxItem(comboMenu, "GFUELTalon.Combo.Overkill.R"))
                        {
                            if (Player.GetSpellDamage(target, SpellSlot.R) > target.Health + 75
                                && (Q.IsReady() || E.IsReady() || W.IsReady()))
                            {
                                return;
                            }
                        }

                        if (GetComboDamage(target) > target.Health && target.IsValidTarget(R.Range - 50))
                        {
                            R.Cast();
                        }
                    }

                    foreach (
                        var x in
                            HeroManager.Enemies.Where(hero => !hero.IsDead && hero.IsValidTarget(R.Range - 50)))
                    {
                        var pred = R.GetPrediction(x);
                        if (pred.AoeTargetsHitCount >= getSliderItem(comboMenu, "GFUELTalon.Combo.Count"))
                        {
                            R.Cast();
                        }
                    }
                }

                if (getCheckBoxItem(comboMenu, "GFUELTalon.Combo.W") && target.Distance(Player.Position) < W.Range &&
                    W.IsReady())
                {
                    var prediction = W.GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.High)
                    {
                        W.Cast(target);
                    }
                }

                if (getCheckBoxItem(comboMenu, "GFUELTalon.Combo.Q") && target.IsValidTarget(Q.Range) && Q.IsReady())
                {
                    Q.Cast();
                }

                if (Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) >= target.Health)
                {
                    if (IgniteSpell.Slot != SpellSlot.Unknown)
                    {
                        Player.Spellbook.CastSpell(IgniteSpell.Slot, target);
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
                var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                if (target == null)
                {
                    return;
                }

                if (getCheckBoxItem(harassMenu, "GFUELTalon.Harass.W") && target.Distance(Player.Position) < W.Range &&
                    W.IsReady())
                {
                    var prediction = W.GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.High)
                    {
                        W.Cast(target);
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
                        E.Range,
                        MinionTypes.All,
                        MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth).OrderByDescending(x => x.MaxHealth).FirstOrDefault();

                if (minion == null)
                {
                    return;
                }

                if (Player.ManaPercent < getSliderItem(jungleclearMenu, "GFUELTalon.jungleclear.Mana"))
                {
                    return;
                }


                if (getCheckBoxItem(jungleclearMenu, "GFUELTalon.jungleclear.E") && E.IsReady())
                {
                    E.CastOnUnit(minion);
                }

                if (getCheckBoxItem(jungleclearMenu, "GFUELTalon.jungleclear.W") && W.IsReady() &&
                    minion.IsValidTarget(W.Range))
                {
                    W.Cast(minion);
                }

                if (getCheckBoxItem(jungleclearMenu, "GFUELTalon.jungleclear.Q") && Q.IsReady() &&
                    minion.IsValidTarget(Q.Range))
                {
                    Q.Cast();
                }

                var min =
                    MinionManager.GetMinions(
                        Orbwalking.GetRealAutoAttackRange(Player),
                        MinionTypes.All,
                        MinionTeam.NotAlly).Count();

                if (min >= 1)
                {
                    if (Tiamat.IsReady())
                    {
                        Tiamat.Cast();
                    }

                    if (Hydra.IsReady())
                    {
                        Hydra.Cast();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static void DoLaneclear()
        {
            try
            {
                var minion = MinionManager.GetMinions(Player.Position, E.Range).MinOrDefault(x => x.Health);
                if (minion == null)
                {
                    return;
                }

                if (Player.ManaPercent < getSliderItem(laneclearMenu, "GFUELTalon.laneclear.Mana"))
                {
                    return;
                }

                if (getCheckBoxItem(laneclearMenu, "GFUELTalon.laneclear.W") && W.IsReady())
                {
                    if (GetCenterMinion().IsValidTarget())
                    {
                        W.Cast(GetCenterMinion());
                    }
                }

                var min =
                    MinionManager.GetMinions(
                        Orbwalking.GetRealAutoAttackRange(Player),
                        MinionTypes.All,
                        MinionTeam.NotAlly).Count();

                if (min >= 3)
                {
                    if (Tiamat.IsReady())
                    {
                        Tiamat.Cast();
                    }

                    if (Hydra.IsReady())
                    {
                        Hydra.Cast();
                    }
                }

                if (getCheckBoxItem(laneclearMenu, "GFUELTalon.laneclear.E") && E.IsReady())
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
        public static Menu miscellaneousMenu, killstealMenu, jungleclearMenu, laneclearMenu, harassMenu, comboMenu;

        private static void GenerateMenu()
        {
            try
            {
                Menu = MainMenu.AddMenu("GFUEL TALON", "GFUELTALON");

                comboMenu = Menu.AddSubMenu("Combo", "Combo");
                comboMenu.Add("GFUELTalon.Combo.Q", new CheckBox("Use Q"));
                comboMenu.Add("GFUELTalon.Combo.W", new CheckBox("Use W"));
                comboMenu.Add("GFUELTalon.Combo.E", new CheckBox("Use E"));
                comboMenu.Add("GFUELTalon.Combo.Items", new CheckBox("Use Items"));
                comboMenu.Add("GFUELTalon.Combo.Towercheck", new CheckBox("Check under tower", false));
                comboMenu.AddGroupLabel("R Settings");
                comboMenu.Add("GFUELTalon.Combo.R", new CheckBox("Use R"));
                comboMenu.Add("GFUELTalon.Combo.Killability", new CheckBox("R on killability"));
                comboMenu.Add("GFUELTalon.Combo.Count", new Slider("R when ult can hit", 1, 1, 5));
                comboMenu.Add("GFUELTalon.Combo.Overkill.R", new CheckBox("Check R overkill", false));

                harassMenu = Menu.AddSubMenu("Harass", "Harass");
                harassMenu.Add("GFUELTalon.Harass.W", new CheckBox("Use W"));

                laneclearMenu = Menu.AddSubMenu("Laneclear", "Laneclear");
                laneclearMenu.Add("GFUELTalon.laneclear.E", new CheckBox("Use E", false));
                laneclearMenu.Add("GFUELTalon.laneclear.W", new CheckBox("Use W"));
                laneclearMenu.Add("GFUELTalon.laneclear.count", new Slider("Minimum minion count", 3, 2, 6));
                laneclearMenu.Add("GFUELTalon.laneclear.Mana", new Slider("Minimum mana", 20));

                jungleclearMenu = Menu.AddSubMenu("Jungleclear", "Jungleclear");
                jungleclearMenu.Add("GFUELTalon.jungleclear.Q", new CheckBox("Use Q", false));
                jungleclearMenu.Add("GFUELTalon.jungleclear.E", new CheckBox("Use E", false));
                jungleclearMenu.Add("GFUELTalon.jungleclear.W", new CheckBox("Use W"));
                jungleclearMenu.Add("GFUELTalon.jungleclear.Mana", new Slider("Minimum mana", 20));

                killstealMenu = Menu.AddSubMenu("Killsteal", "Killsteal");
                killstealMenu.Add("GFUELTalon.Killsteal.W", new CheckBox("Killsteal W", false));

                miscellaneousMenu = Menu.AddSubMenu("Miscellaneous", "Miscellaneous");
                miscellaneousMenu.Add("GFUELTalon.Draw.Off", new CheckBox("Disable drawings", false));
                miscellaneousMenu.Add("GFUELTalon.Draw.W", new CheckBox("Draw E"));
                miscellaneousMenu.Add("GFUELTalon.Misc.Antigapcloser", new CheckBox("Use E - Antigapcloser"));
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

        private static Obj_AI_Base GetCenterMinion()
        {
            var minions = MinionManager.GetMinions(E.Range);
            var centerlocation =
                MinionManager.GetBestCircularFarmLocation(minions.Select(x => x.Position.To2D()).ToList(), 500, E.Range);

            return centerlocation.MinionsHit >= getSliderItem(laneclearMenu, "GFUELTalon.laneclear.count")
                ? MinionManager.GetMinions(1000)
                    .OrderBy(x => x.Distance(centerlocation.Position))
                    .FirstOrDefault()
                : null;
        }

        private static float GetComboDamage(Obj_AI_Base enemy)
        {
            try
            {
                float damage = 0;

                if (!Orbwalker.IsAutoAttacking)
                {
                    damage += Player.GetAutoAttackDamage(enemy, true);
                }

                if (IgniteSpell.IsReady() && IgniteSpell.Slot != SpellSlot.Unknown)
                {
                    damage += (float) Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
                }

                if (Hydra.IsReady())
                {
                    damage += (float) Player.GetItemDamage(enemy, Damage.DamageItems.Hydra);
                }

                if (Tiamat.IsReady())
                {
                    damage += (float) Player.GetItemDamage(enemy, Damage.DamageItems.Tiamat);
                }

                if (Youmuu.IsReady())
                {
                    damage += Player.GetAutoAttackDamage(enemy, true)*2;
                }

                if (Q.IsReady())
                {
                    damage += Q.GetDamage(enemy);
                }

                if (E.IsReady())
                {
                    damage += E.GetDamage(enemy);
                }

                if (W.IsReady())
                {
                    damage += W.GetDamage(enemy);
                }

                if (R.IsReady())
                {
                    damage += R.GetDamage(enemy);
                }

                return damage;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return 0;
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

                if (getCheckBoxItem(miscellaneousMenu, "GFUELTalon.Draw.W") && W.IsReady())
                {
                    var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);

                    if (target.IsValidTarget() == false)
                    {
                        return;
                    }

                    var polygon = new Geometry.Polygon.Sector(
                        ObjectManager.Player.Position,
                        target.Position,
                        54*(float) Math.PI/180,
                        700);
                    polygon.UpdatePolygon();
                    polygon.Draw(Color.Aqua);
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
                    var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range) && !x.IsDead && !x.IsZombie))
                {
                    if (enemy.IsValidTarget(W.Range) && enemy.Health < W.GetDamage(enemy))
                    {
                        var prediction = W.GetPrediction(enemy);
                        if (prediction.Hitchance >= HitChance.High)
                        {
                            W.Cast(enemy);
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

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    DoCombo();
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    DoHarass();
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) ||
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                {
                    DoJungleclear();
                    DoLaneclear();
                }

                if (getCheckBoxItem(killstealMenu, "GFUELTalon.Killsteal.W"))
                {
                    OnKillsteal();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static void OrbwalkingAfterAttack(AttackableUnit target, EventArgs args)
        {
            try
            {
                var enemy = target as Obj_AI_Base;
                if (enemy == null || !(target is AIHeroClient))
                {
                    return;
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)
                    || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    if (target.IsValidTarget(Q.Range))
                    {
                        Q.Cast();
                    }
                }

                if (getCheckBoxItem(comboMenu, "GFUELTalon.Combo.Items"))
                {
                    UseItems();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Credits to Trees
        private static bool UseItems()
        {
            if (Player.IsDashing() || Orbwalker.IsAutoAttacking)
            {
                return false;
            }

            var youmuus = Youmuu;
            if (Youmuu.IsReady() && youmuus.Cast())
            {
                return true;
            }

            var heroes = Player.GetEnemiesInRange(385).Count;
            var count = heroes;

            var tiamat = Tiamat;
            if (tiamat.IsReady() && count > 0 && tiamat.Cast())
            {
                return true;
            }

            var hydra = Hydra;
            if (Hydra.IsReady() && count > 0 && hydra.Cast())
            {
                return true;
            }

            var titanic = Titanic;
            return titanic.IsReady() && count > 0 && titanic.Cast();
        }

        #endregion
    }
}