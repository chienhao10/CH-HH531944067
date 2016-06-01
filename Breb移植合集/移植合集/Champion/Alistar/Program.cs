using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using Spell = LeagueSharp.Common.Spell;

namespace PortAIO.Champion.Alistar
{
    internal class Program
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Fired when the game loads.
        /// </summary>
        public static void OnGameLoad()
        {
            try
            {
                if (Player.ChampionName != "Alistar")
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

                FlashSlot = Player.GetSpellSlot("summonerflash");

                Q = new Spell(SpellSlot.Q, 365f);
                W = new Spell(SpellSlot.W, 650f);
                E = new Spell(SpellSlot.E, 575f);
                R = new Spell(SpellSlot.R);

                GenerateMenu();
                Game.OnUpdate += OnUpdate;
                Drawing.OnDraw += OnDraw;
                AttackableUnit.OnDamage += AttackableUnit_OnDamage;
                Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
                AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
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
        ///     Gets or sets the menu
        /// </summary>
        /// <value>
        ///     The menu
        /// </value>
        private static Menu Menu { get; set; }

        private static Menu comboMenu { get; set; }
        private static Menu flashMenu { get; set; }
        private static Menu healMenu { get; set; }
        private static Menu interrupterMenu { get; set; }
        private static Menu miscellaneousMenu { get; set; }

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
        ///     Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The IgniteSpell
        /// </value>
        public static Spell IgniteSpell { get; set; }

        /// <summary>
        ///     FlashSlot
        /// </summary>
        public static SpellSlot FlashSlot;

        #endregion

        #region Methods

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
                Menu = MainMenu.AddMenu("EL牛头", "ElAlistar");

                comboMenu = Menu.AddSubMenu("连招设置", "Combo");
                comboMenu.Add("ElAlistar.Combo.Q", new CheckBox("使用 Q"));
                comboMenu.Add("ElAlistar.Combo.W", new CheckBox("使用 W"));
                comboMenu.Add("ElAlistar.Combo.R", new CheckBox("使用 R"));
                comboMenu.Add("ElAlistar.Combo.RHeal.HP", new Slider("生命百分比为X时，连招使用R", 60));
                comboMenu.Add("ElAlistar.Combo.RHeal.Damage", new Slider("受到 X 百分比的伤害时，连招使用R", 60));


                flashMenu = Menu.AddSubMenu("闪现设置", "Flash");
                flashMenu.Add("ElAlistar.Flash.Click", new CheckBox("左键点击 [开] 目标选择器 [关]"));
                flashMenu.Add("ElAlistar.Combo.FlashQ", new KeyBind("闪现 Q", false, KeyBind.BindTypes.HoldActive, 'T'));


                healMenu = Menu.AddSubMenu("治疗设置", "Heal");
                healMenu.Add("ElAlistar.Heal.E", new CheckBox("使用治疗"));
                healMenu.Add("Heal.HP", new Slider("使用治疗百分比", 80));
                healMenu.Add("Heal.Damage", new Slider("收到 X 百分比伤害时，使用治疗", 80));
                healMenu.Add("ElAlistar.Heal.Mana", new Slider("最低蓝量限制", 20));
                healMenu.AddSeparator();
                foreach (var x in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly))
                {
                    healMenu.Add("healon" + x.NetworkId, new CheckBox("使用 " + x.ChampionName));
                }


                interrupterMenu = Menu.AddSubMenu("技能打断", "Interrupter");
                interrupterMenu.Add("ElAlistar.Interrupter.Q", new CheckBox("使用 Q"));
                interrupterMenu.Add("ElAlistar.Interrupter.W", new CheckBox("使用 W"));
                interrupterMenu.Add("ElAlistar.GapCloser", new CheckBox("防突进"));

                miscellaneousMenu = Menu.AddSubMenu("Miscellaneous", "杂项");
                miscellaneousMenu.Add("ElAlistar.Ignite", new CheckBox("使用点燃"));
                miscellaneousMenu.Add("ElAlistar.Drawings.W", new CheckBox("显示 W 范围"));
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
                if (getCheckBoxItem(miscellaneousMenu, "ElAlistar.Drawings.W"))
                {
                    if (W.Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Color.DeepSkyBlue);
                    }
                }
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
                            !hero.IsZombie &&
                            Player.GetSummonerSpellDamage(hero, DamageLibrary.SummonerSpells.Ignite) >= hero.Health);

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


        /// <summary>
        ///     Returns the mana
        /// </summary>
        private static bool HasEnoughMana()
        {
            return Player.Mana >
                   Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana + Player.Spellbook.GetSpell(SpellSlot.W).SData.Mana;
        }

        /// <summary>
        ///     Combo logic
        /// </summary>
        private static void OnCombo()
        {
            try
            {
                var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
                if (target == null)
                {
                    return;
                }
                if (getCheckBoxItem(comboMenu, "ElAlistar.Combo.Q") && getCheckBoxItem(comboMenu, "ElAlistar.Combo.W") &&
                    Q.IsReady() && W.IsReady())
                {
                    if (target.LSIsValidTarget(W.Range) && HasEnoughMana())
                    {
                        if (target.LSIsValidTarget(Q.Range))
                        {
                            Q.Cast();
                            return;
                        }

                        if (W.Cast(target).IsCasted())
                        {
                            var comboTime = Math.Max(0, Player.LSDistance(target) - 365)/1.2f - 25;
                            LeagueSharp.Common.Utility.DelayAction.Add((int) comboTime, () => Q.Cast());
                        }
                    }
                }

                if (getCheckBoxItem(comboMenu, "ElAlistar.Combo.Q") && target.LSIsValidTarget(Q.Range))
                {
                    Q.Cast();
                }

                if (getCheckBoxItem(comboMenu, "ElAlistar.Combo.W"))
                {
                    if (target.LSIsValidTarget(W.Range) && W.GetDamage(target) > target.Health)
                    {
                        W.Cast(target);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsAlly || sender.IsMe)
            {
                return;
            }

            if (args.DangerLevel != Interrupter2.DangerLevel.High || sender.LSDistance(Player) > W.Range)
            {
                return;
            }

            if (sender.LSIsValidTarget(Q.Range) && Q.IsReady() && getCheckBoxItem(interrupterMenu, "ElAlistar.Interrupter.Q"))
            {
                Q.Cast();
            }

            if (sender.LSIsValidTarget(W.Range) && W.IsReady() &&
                getCheckBoxItem(interrupterMenu, "ElAlistar.Interrupter.W"))
            {
                W.Cast(sender);
            }
        }

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly || gapcloser.Sender.IsMe)
            {
                return;
            }
            if (getCheckBoxItem(interrupterMenu, "ElAlistar.GapCloser"))
            {
                if (Q.IsReady() && gapcloser.Sender.LSDistance(Player) < Q.Range)
                {
                    Q.Cast();
                }

                if (W.IsReady() && gapcloser.Sender.LSDistance(Player) < W.Range)
                {
                    W.Cast(gapcloser.Sender);
                }
            }
        }

        private static void AttackableUnit_OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            var obj = ObjectManager.GetUnitByNetworkId<GameObject>((uint) args.Target.NetworkId);

            if (obj.Type != GameObjectType.AIHeroClient)
            {
                return;
            }

            var hero = (AIHeroClient) obj;

            if (hero.IsEnemy)
            {
                return;
            }

            if (getCheckBoxItem(comboMenu, "ElAlistar.Combo.R"))
            {
                if (
                    ObjectManager.Get<AIHeroClient>()
                        .Any(
                            x =>
                                x.IsAlly && x.IsMe && !x.IsDead &&
                                ((int) (args.Damage/x.MaxHealth*100) >
                                 getSliderItem(comboMenu, "ElAlistar.Combo.RHeal.Damage") ||
                                 x.HealthPercent < getSliderItem(comboMenu, "ElAlistar.Combo.RHeal.HP") &&
                                 x.LSCountEnemiesInRange(1000) >= 1)))
                {
                    R.Cast();
                }
            }

            if (getCheckBoxItem(healMenu, "ElAlistar.Heal.E") &&
                Player.ManaPercent > getSliderItem(healMenu, "ElAlistar.Heal.Mana"))
            {
                if (
                    ObjectManager.Get<AIHeroClient>()
                        .Any(
                            x =>
                                x.IsAlly && !x.IsDead &&
                                getCheckBoxItem(healMenu, string.Format("healon{0}", x.NetworkId)) &&
                                ((int) (args.Damage/x.MaxHealth*100) > getSliderItem(healMenu, "Heal.Damage") ||
                                 x.HealthPercent < getSliderItem(healMenu, "Heal.HP")) && x.LSDistance(Player) < E.Range &&
                                x.LSCountEnemiesInRange(1000) >= 1))
                {
                    E.Cast();
                }
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
                if (Player.IsDead || Player.LSIsRecalling() || Player.InFountain())
                {
                    return;
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    OnCombo();
                }

                if (getCheckBoxItem(miscellaneousMenu, "ElAlistar.Ignite"))
                {
                    HandleIgnite();
                }

                if (getKeyBindItem(flashMenu, "ElAlistar.Combo.FlashQ") && Q.IsReady())
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

                    var target = getCheckBoxItem(flashMenu, "ElAlistar.Flash.Click")
                        ? TargetSelector.SelectedTarget
                        : TargetSelector.GetTarget(W.Range, DamageType.Magical);

                    if (!target.LSIsValidTarget(W.Range))
                    {
                        return;
                    }

                    Player.Spellbook.CastSpell(FlashSlot, target.ServerPosition);
                    LeagueSharp.Common.Utility.DelayAction.Add(50, () => Q.Cast());
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion
    }
}