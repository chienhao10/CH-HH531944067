using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using PortAIO.Properties;
using Spell = LeagueSharp.Common.Spell;

namespace Slutty_ryze
{
    internal class Program
    {
        private static readonly Random Seeder = new Random();
        private static bool _casted;
        private static int _lastw;

        private static void Printmsg(string message)
        {
            Chat.Print(
                "<font color='#6f00ff'>[Slutty Ryze]:</font> <font color='#FFFFFF'>" + message + "</font>");
        }

        private static void Printmsg1(string message)
        {
            Chat.Print(
                "<font color='#ff00ff'>[Slutty Ryze]:</font> <font color='#FFFFFF'>" + message + "</font>");
        }

        private static void Printmsg2(string message)
        {
            Chat.Print(
                "<font color='#00abff'>[Slutty Ryze]:</font> <font color='#FFFFFF'>" + message + "</font>");
        }

        #region onload

        public static Menu
            _config = MenuManager._config,
            humanizerMenu = MenuManager.humanizerMenu,
            combo1Menu = MenuManager.combo1Menu,
            mixedMenu = MenuManager.mixedMenu,
            laneMenu = MenuManager.laneMenu,
            jungleMenu = MenuManager.jungleMenu,
            lastMenu = MenuManager.lastMenu,
            passiveMenu = MenuManager.passiveMenu,
            itemMenu = MenuManager.itemMenu,
            eventMenu = MenuManager.eventMenu,
            ksMenu = MenuManager.ksMenu,
            chase = MenuManager.chase;

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
            if (GlobalManager.GetHero.ChampionName != Champion.ChampName)
                return;

            Console.WriteLine(@"Loading Your Slutty Ryze");

            Humanizer.AddAction("generalDelay", 35.0f);

            Champion.Q = new Spell(SpellSlot.Q, 865);
            Champion.Qn = new Spell(SpellSlot.Q, 865);
            Champion.W = new Spell(SpellSlot.W, 585);
            Champion.E = new Spell(SpellSlot.E, 585);
            Champion.R = new Spell(SpellSlot.R);

            Champion.Q.SetSkillshot(0.25f, 50f, 1700f, true, SkillshotType.SkillshotLine);
            Champion.Qn.SetSkillshot(0.25f, 50f, 1700f, false, SkillshotType.SkillshotLine);

            //assign menu from MenuManager to Config
            Console.WriteLine(@"Loading Your Slutty Menu...");
            MenuManager.GetMenu();
            Printmsg("Ryze Assembly Loaded! Make sure to test new combo!");
            Printmsg1("Current Version: " + typeof (Program).Assembly.GetName().Version);
            Printmsg2("Don't Forget To " + "<font color='#00ff00'>[Upvote]</font> <font color='#FFFFFF'>" +
                      "The Assembly In The Databse" + "</font>");

            Game.OnUpdate += Game_OnUpdate;
            AntiGapcloser.OnEnemyGapcloser += Champion.OnGapClose;
            Interrupter2.OnInterruptableTarget += Champion.RyzeInterruptableSpell;
            Orbwalker.OnPreAttack += Champion.Orbwalking_BeforeAttack;
            ShowDisplayMessage();
        }

        #endregion

        #region onGameUpdate

        private static void ShowDisplayMessage()
        {
            var r = new Random();

            var txt = Resources.display.Split('\n');
            switch (r.Next(1, 3))
            {
                case 2:
                    txt = Resources.display2.Split('\n');
                    break;
                case 3:
                    txt = Resources.display3.Split('\n');
                    break;
            }

            foreach (var s in txt)
                Console.WriteLine(s);
        }

        public static void SpellSequence(Obj_AI_Base target, string Q, string W, string E, string R)
        {
            var qSpell = false;
            var wSpell = false;
            var eSpell = false;
            var rSpell = false;

            qSpell = getCheckBoxItem(laneMenu, "useQj");
            wSpell = getCheckBoxItem(laneMenu, "useW2L");
            eSpell = getCheckBoxItem(laneMenu, "useE2L");
            rSpell = getCheckBoxItem(laneMenu, "useRl");

            switch (getBoxItem(combo1Menu, "combooptions"))
            {
                case 0:
                    if (GlobalManager.GetPassiveBuff <= 1 && !GlobalManager.GetHero.HasBuff("ryzepassivecharged"))
                    {
                        if (qSpell)
                            Champion.Q.Cast(target);
                        if (eSpell)
                            Champion.E.Cast(target);
                        if (wSpell)
                            Champion.W.Cast(target);
                        if (rSpell)
                            Champion.R.Cast(target);
                    }

                    if (GlobalManager.GetPassiveBuff == 2)
                    {
                        if (qSpell)
                            Champion.Q.Cast(target);
                        if (wSpell)
                            Champion.W.Cast(target);
                        if (eSpell)
                            Champion.E.Cast(target);
                        if (rSpell)
                            Champion.R.Cast(target);
                    }


                    if (GlobalManager.GetPassiveBuff == 3)
                    {
                        if (qSpell)
                            Champion.Q.Cast(target);
                        if (eSpell)
                            Champion.E.Cast(target);
                        if (wSpell)
                            Champion.W.Cast(target);
                        if (rSpell)
                            Champion.R.Cast(target);
                    }

                    if (GlobalManager.GetPassiveBuff == 4)
                    {
                        if (wSpell)
                            Champion.W.Cast(target);
                        if (qSpell)
                            Champion.Q.Cast(target);
                        if (eSpell)
                            Champion.E.Cast(target);
                        if (rSpell)
                            Champion.R.Cast(target);
                    }

                    if (GlobalManager.GetHero.HasBuff("ryzepassivecharged"))
                    {
                        if (wSpell)
                            Champion.W.Cast(target);
                        if (qSpell)
                            Champion.Q.Cast(target);
                        if (eSpell)
                            Champion.E.Cast(target);
                        if (rSpell)
                            Champion.R.Cast(target);
                    }

                    if (Champion.R.IsReady() &&
                        (GlobalManager.GetPassiveBuff == 4 || GlobalManager.GetHero.HasBuff("ryzepassivecharged")) &&
                        rSpell)
                    {
                        if (!Champion.Q.IsReady() && !Champion.W.IsReady() && !Champion.E.IsReady())
                        {
                            Champion.R.Cast();
                        }
                    }

                    break;

                case 1:
                    if (target.LSIsValidTarget(Champion.Q.Range))
                    {
                        if (Champion.R.IsReady() || !Champion.R.IsReady())
                        {
                            if (GlobalManager.GetPassiveBuff <= 1 &&
                                !GlobalManager.GetHero.HasBuff("ryzepassivecharged"))
                            {
                                if (rSpell)
                                    Champion.R.Cast(target);
                                if (eSpell)
                                    Champion.E.Cast(target);
                                if (qSpell)
                                    Champion.Q.Cast(target);
                                if (wSpell)
                                    Champion.W.Cast(target);
                            }

                            if (GlobalManager.GetPassiveBuff == 2)
                            {
                                if (rSpell)
                                    Champion.R.Cast(target);
                                if (eSpell)
                                    Champion.E.Cast(target);
                                if (wSpell)
                                    Champion.W.Cast(target);
                                if (qSpell)
                                    Champion.Q.Cast(target);
                            }


                            if (GlobalManager.GetPassiveBuff == 3)
                            {
                                if (wSpell)
                                    Champion.W.Cast(target);
                                if (rSpell)
                                    Champion.R.Cast(target);
                                if (qSpell)
                                    Champion.Q.Cast(target);
                                if (eSpell)
                                    Champion.E.Cast(target);
                            }

                            if (GlobalManager.GetPassiveBuff == 4)
                            {
                                if (eSpell)
                                    Champion.E.Cast(target);
                                if (rSpell)
                                    Champion.R.Cast(target);
                                if (wSpell)
                                    Champion.W.Cast(target);
                                if (qSpell)
                                    Champion.Q.Cast(target);
                            }

                            if (GlobalManager.GetHero.HasBuff("ryzepassivecharged"))
                            {
                                if (rSpell)
                                    Champion.R.Cast(target);
                                if (wSpell)
                                    Champion.W.Cast(target);
                                if (qSpell)
                                    Champion.Q.Cast(target);
                                if (eSpell)
                                    Champion.E.Cast(target);
                            }
                        }


                        if (Champion.R.IsReady() &&
                            (GlobalManager.GetPassiveBuff == 4 ||
                             GlobalManager.GetHero.HasBuff("ryzepassivecharged")) &&
                            rSpell)
                        {
                            if (!Champion.Q.IsReady() && !Champion.W.IsReady() && !Champion.E.IsReady())
                            {
                                Champion.R.Cast();
                            }
                        }
                    }
                    break;
            }
        }


        private static void Game_OnUpdate(EventArgs args)
        {
            try // lazy
            {
                if (getKeyBindItem(_config, "test"))
                {
                    Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    var targets = TargetSelector.GetTarget(Champion.W.Range, DamageType.Magical);
                    if (targets == null)
                        return;
                    if (Champion.W.IsReady())
                    {
                        LaneOptions.CastW(targets);
                        {
                            _lastw = Environment.TickCount;
                        }
                    }

                    if (Environment.TickCount - _lastw >= 700 - Game.Ping)
                    {
                        if (Champion.Q.IsReady())
                        {
                            LaneOptions.CastQn(targets);
                            _casted = true;
                        }
                    }

                    if (_casted)
                    {
                        LaneOptions.CastE(targets);
                        LaneOptions.CastQn(targets);
                        _casted = false;
                    }
                }

                if (getKeyBindItem(chase, "chase"))
                {
                    Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    var targets = TargetSelector.GetTarget(Champion.W.Range + 200, DamageType.Magical);
                    if (targets == null)
                        return;

                    if (getCheckBoxItem(chase, "usewchase"))
                        LaneOptions.CastW(targets);

                    if (getCheckBoxItem(chase, "chaser") &&
                        targets.Distance(GlobalManager.GetHero) > Champion.W.Range + 200)
                        Champion.R.Cast();
                }

                if (GlobalManager.GetHero.IsDead)
                {
                    return;
                }

                if (GlobalManager.GetHero.IsRecalling())
                {
                    return;
                }

                if (Champion.casted == false)
                {
                    Orbwalker.DisableAttacking = false;
                }

                var target = TargetSelector.GetTarget(Champion.Q.Range, DamageType.Magical);

                if (getCheckBoxItem(humanizerMenu, "doHuman"))
                {
                    if (!Humanizer.CheckDelay("generalDelay")) // Wait for delay for all other events
                    {
                        return;
                    }
                    var nDelay = Seeder.Next(getSliderItem(humanizerMenu, "minDelay"),
                        getSliderItem(humanizerMenu, "maxDelay")); // set a new random delay :D
                    Humanizer.ChangeDelay("generalDelay", nDelay);
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    var expires = GlobalManager.GetHero.Spellbook.GetSpell(SpellSlot.Q).CooldownExpires;

                    LaneOptions.COMBO();
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    LaneOptions.Mixed();
                    Orbwalker.DisableAttacking = false;
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    LaneOptions.JungleClear();
                    if (getKeyBindItem(laneMenu, "disablelane"))
                    {
                        Console.WriteLine("1");
                        if (GlobalManager.GetPassiveBuff == 4 && !GlobalManager.GetHero.HasBuff("RyzeR") &&
                            getCheckBoxItem(laneMenu, "passiveproc"))
                        {
                            Console.WriteLine("2");
                            return;
                        }

                        Console.WriteLine("3");
                        var qlchSpell = getCheckBoxItem(laneMenu, "useQlc");
                        var elchSpell = getCheckBoxItem(laneMenu, "useElc");
                        var wlchSpell = getCheckBoxItem(laneMenu, "useWlc");

                        var rSpell = getCheckBoxItem(laneMenu, "useRl");
                        var rSlider = getSliderItem(laneMenu, "rMin");
                        var minMana = getSliderItem(laneMenu, "useEPL");

                        var minionCount =
                            EntityManager.MinionsAndMonsters.GetLaneMinions()
                                .Where(x => !x.IsAlly && Champion.Q.IsInRange(x));

                        if (GlobalManager.GetHero.ManaPercent <= minMana)
                        {
                            Console.WriteLine("4");
                            return;
                        }

                        foreach (var minion in minionCount)
                        {
                            Console.WriteLine("5");
                            if (!GlobalManager.CheckMinion(minion))
                            {
                                continue;
                            }

                            Console.WriteLine("123123123");

                            var minionHp = minion.Health;

                            SpellSequence(minion, "useQ2L", "useE2L", "useW2L", "useRl");

                            if (qlchSpell && Champion.Q.IsReady() && minion.IsValidTarget(Champion.Q.Range) &&
                                minionHp <= Champion.Q.GetDamage(minion) && GlobalManager.CheckMinion(minion))
                            {
                                Champion.Q.Cast(minion);
                            }

                            else if (wlchSpell && Champion.W.IsReady() && minion.IsValidTarget(Champion.W.Range) &&
                                     minionHp <= Champion.W.GetDamage(minion) && GlobalManager.CheckMinion(minion))
                            {
                                Champion.W.CastOnUnit(minion);
                            }

                            else if (elchSpell && Champion.E.IsReady() && minion.IsValidTarget(Champion.E.Range) &&
                                     minionHp <= Champion.E.GetDamage(minion) && GlobalManager.CheckMinion(minion))
                            {
                                Champion.E.CastOnUnit(minion);
                            }

                            if (rSpell && Champion.R.IsReady() && minion.IsValidTarget(Champion.Q.Range) &&
                                minionCount.Count() > rSlider && GlobalManager.CheckMinion(minion))
                            {
                                Champion.R.Cast();
                            }
                        }
                    }
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                {
                    LaneOptions.LastHit();
                }


                if (getKeyBindItem(itemMenu, "tearS"))
                {
                    ItemManager.TearStack();
                }

                if (getKeyBindItem(passiveMenu, "autoPassive"))
                {
                    var minions = MinionManager.GetMinions(GlobalManager.GetHero.ServerPosition, Champion.Q.Range,
                        MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);
                    if (ObjectManager.Player.ManaPercent < getSliderItem(passiveMenu, "ManapSlider"))
                    {
                        return;
                    }
                    if (ObjectManager.Player.IsRecalling())
                    {
                        return;
                    }
                    if (minions.Count >= 1)
                    {
                        return;
                    }
                    var stackSliders = getSliderItem(passiveMenu, "stackSlider");
                    if (ObjectManager.Player.InFountain())
                    {
                        return;
                    }

                    if (GlobalManager.GetPassiveBuff >= stackSliders)
                    {
                        return;
                    }

                    if (Utils.TickCount - Champion.Q.LastCastAttemptT >=
                        getSliderItem(passiveMenu, "autoPassiveTimer")*1000 - (100 + Game.Ping/2) &&
                        Champion.Q.IsReady())
                    {
                        if (!Game.CursorPos.IsZero)
                        {
                            Champion.Q.Cast(Game.CursorPos);
                        }
                    }
                }

                Orbwalker.DisableAttacking = false;

                if (getCheckBoxItem(mixedMenu, "UseQauto") && target != null)
                {
                    if (Champion.Q.IsReady() && target.IsValidTarget(Champion.Q.Range))
                        Champion.Q.Cast(target);
                }

                Champion.KillSteal();
            }
            catch
            {
                // ignored
            }
        }

        #endregion
    }
}