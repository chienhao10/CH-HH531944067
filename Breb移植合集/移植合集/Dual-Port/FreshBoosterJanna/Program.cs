using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy.SDK;
using Color = System.Drawing.Color;
using static FreshBooster.FreshCommon;
using Spell = LeagueSharp.Common.Spell;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace FreshBooster.Champion
{
    class Janna
    {
        // Default Setting
        public static int ErrorTime;
        public const string ChampName = "Janna";   // Edit
        public static AIHeroClient Player;
        public static Spell _Q, _W, _E, _R;
        // Default Setting

        public static bool QSpell = false;
        public static int SpellTime = 0, RTime;

        private void SkillSet()
        {
            try
            {
                _Q = new Spell(SpellSlot.Q, 850f);
                _Q.SetSkillshot(0.25f, 120f, 900f, false, SkillshotType.SkillshotLine);
                _W = new Spell(SpellSlot.W, 600);
                _E = new Spell(SpellSlot.E, 800f);
                _R = new Spell(SpellSlot.R, 875f);
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 01");
                    ErrorTime = TickCount(10000);
                }
            }
        }

        public static Menu menu, comboMenu, miscMenu, drawMenu;

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

        private void Menu()
        {
            try
            {
                menu = MainMenu.AddMenu("FreshBooster Janna", "fbjanna");

                comboMenu = menu.AddSubMenu("Combo", "Combo");
                comboMenu.Add("Janna_CUse_Q", new CheckBox("Use Q"));
                comboMenu.Add("Janna_CUse_W", new CheckBox("Use W"));

                miscMenu = menu.AddSubMenu("Misc", "Misc");
                miscMenu.Add("Janna_AutoE", new CheckBox("Auto E"));
                miscMenu.Add("Janna_AutoE1", new CheckBox("Auto E(Dont use on me)"));
                miscMenu.Add("Janna_AutoREnable", new CheckBox("Enable Auto R"));
                miscMenu.Add("Janna_AutoRHP", new Slider("Min HP % for Auto R", 10, 1, 100));
                miscMenu.Add("Janna_InterQ", new CheckBox("Use Q Interrupt"));
                miscMenu.Add("Janna_InterR", new CheckBox("Use R Interrupt"));
                miscMenu.Add("Janna_GapQ", new CheckBox("Use Q GapCloser"));
                miscMenu.Add("Janna_GapR", new CheckBox("Use R GapCloser"));

                drawMenu = menu.AddSubMenu("Draw", "Draw");
                drawMenu.Add("Janna_Draw_Q", new CheckBox("Draw Q", false));
                drawMenu.Add("Janna_Draw_W", new CheckBox("Draw W", false));
                drawMenu.Add("Janna_Draw_E", new CheckBox("Draw E", false));
                drawMenu.Add("Janna_Draw_R", new CheckBox("Draw R", false));

            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 02");
                    ErrorTime = TickCount(10000);
                }
            }
        }
        public static void Drawing_OnDraw(EventArgs args)
        {
            try
            {
                if (Player.IsDead) return;
                if (getCheckBoxItem(drawMenu, "Janna_Draw_Q"))
                    Render.Circle.DrawCircle(Player.Position, _Q.Range, Color.White, 1);
                if (getCheckBoxItem(drawMenu, "Janna_Draw_W"))
                    Render.Circle.DrawCircle(Player.Position, _W.Range, Color.White, 1);
                if (getCheckBoxItem(drawMenu, "Janna_Draw_E"))
                    Render.Circle.DrawCircle(Player.Position, _E.Range, Color.White, 1);
                if (getCheckBoxItem(drawMenu, "Janna_Draw_R"))
                    Render.Circle.DrawCircle(Player.Position, _R.Range, Color.White, 1);
                var QTarget = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);
                if (QTarget != null)
                    Drawing.DrawCircle(QTarget.Position, 150, Color.Green);
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 03");
                    ErrorTime = TickCount(10000);
                }
            }
        }

        static float getComboDamage(Obj_AI_Base enemy)
        {
            try
            {
                if (enemy != null)
                {
                    float damage = 0;
                    if (_Q.IsReady())
                        damage += _Q.GetDamage(enemy);
                    if (_W.IsReady())
                        damage += _W.GetDamage(enemy);
                    if (!Player.Spellbook.IsAutoAttacking)
                        damage += (float)Player.GetAutoAttackDamage(enemy, true);
                    return damage;
                }
                return 0;
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 04");
                    ErrorTime = TickCount(10000);
                }
                return 0;
            }
        }

        // OnLoad
        public Janna()
        {
            Player = ObjectManager.Player;
            SkillSet();
            Menu();
            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Orbwalker.OnPreAttack += Orbwalking_BeforeAttack;
            EloBuddy.Player.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
        }

        private static void OnGameUpdate(EventArgs args)
        {
            try
            {
                if (Player.IsDead) return;
                // Set Target
                var QTarget = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);
                var WTarget = TargetSelector.GetTarget(_W.Range, DamageType.Magical);

                if (QSpell && _Q.IsCharging)
                {
                    _Q.Cast(true);
                    QSpell = false;
                }
                if (getCheckBoxItem(miscMenu, "Janna_AutoREnable"))
                {
                    var AutoR = ObjectManager.Get<AIHeroClient>().OrderByDescending(x => x.Health).FirstOrDefault(x => x.HealthPercent < getSliderItem(miscMenu, "Janna_AutoRHP") && x.Distance(Player.ServerPosition) < _R.Range && !x.IsDead && x.IsAlly);
                    if (AutoR != null && _R.IsReady() && !Player.IsRecalling())
                    {
                        _R.Cast(true);
                        RTime = TickCount(2000);
                    }
                }
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)) // Flee
                {
                   EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    if (WTarget != null && _W.IsReady() && WTarget.Distance(Player.ServerPosition) < 400)
                        _W.CastOnUnit(WTarget, true);
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) // Combo
                {
                    if (QTarget != null && getCheckBoxItem(comboMenu, "Janna_CUse_Q") && _Q.IsReady())
                    {
                        var Prediction = _Q.GetPrediction(QTarget);
                        if (!_Q.IsCharging && Prediction.Hitchance >= HitChance.Medium)
                        {
                            _Q.Cast(Prediction.CastPosition, true);
                        }
                        if (_Q.IsCharging)
                        {
                            _Q.Cast(true);
                        }
                    }
                    if (WTarget != null && _W.IsReady())
                        _W.Cast(WTarget, true);
                }
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    //Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 06");
                    ErrorTime = TickCount(10000);
                }
            }
        }
        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            try
            {
                if (getCheckBoxItem(miscMenu, "Janna_GapQ") && _Q.IsReady() && gapcloser.Sender.ServerPosition.Distance(Player.ServerPosition) < 850 && !_Q.IsCharging)
                {
                    _Q.Cast(gapcloser.Sender.ServerPosition, true);
                    QSpell = true;
                    SpellTime = TickCount(1000);
                }

                if (getCheckBoxItem(miscMenu, "Janna_GapR") && _R.IsReady() && gapcloser.Sender.ServerPosition.Distance(Player.ServerPosition) < 875 && SpellTime < Environment.TickCount && !Player.IsRecalling())
                {
                    _R.Cast(true);
                    RTime = TickCount(2000);
                }
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 07");
                    ErrorTime = TickCount(10000);
                }
            }
        }
        private static void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {

                if (args.Target is Obj_AI_Minion || !(sender is AIHeroClient))
                    return;
                if (getCheckBoxItem(miscMenu, "Janna_AutoE"))
                {
                    if (sender.IsEnemy)
                    {
                        var StartPos = args.Start;
                        var EndPos = args.End;
                        var NonTRange = new LeagueSharp.Common.Geometry.Polygon.Rectangle(StartPos, EndPos, sender.BoundingRadius + 30);
                        var Target = HeroManager.Allies.FirstOrDefault(f => f.Position.Distance(Player.Position) <= _E.Range && NonTRange.IsInside(f.Position));
                        if (Target == Player && getCheckBoxItem(miscMenu, "Janna_AutoE1")) return;
                        if (Target != null)
                        {
                            _E.CastOnUnit(Target, true);
                            return;
                        }
                        if (args.Target != null && args.Target.Position.Distance(Player.Position) <= _E.Range && args.Target is AIHeroClient)
                        {
                            var ShieldTarget = HeroManager.Allies.FirstOrDefault(f => f.Position.Distance(args.Target.Position) <= 10);
                            _E.CastOnUnit(ShieldTarget, true);
                            return;
                        }
                    }
                    if (sender.IsAlly && args.Target is AIHeroClient)
                    {
                        if (sender.Position.Distance(Player.Position) <= _E.Range && args.Target != null && args.SData.Name.ToLower().Contains("attack"))
                        {
                            _E.CastOnUnit(sender, true);
                            return;
                        }
                    }
                }
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 08");
                    ErrorTime = TickCount(10000);
                }
            }

        }
        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            try
            {
                if (!sender.IsEnemy || !sender.IsValid<AIHeroClient>())
                    return;
                if (getCheckBoxItem(miscMenu, "Janna_InterQ") && _Q.IsReady() && sender.ServerPosition.Distance(Player.ServerPosition) < 850 && !_Q.IsCharging)
                {
                    _Q.Cast(sender.ServerPosition, true);
                    QSpell = true;
                    SpellTime = TickCount(1000);
                }

                if (getCheckBoxItem(miscMenu, "Janna_InterR") && _R.IsReady() && sender.ServerPosition.Distance(Player.ServerPosition) < 875 && SpellTime < Environment.TickCount && !Player.IsRecalling())
                {
                    _R.Cast(true);
                    RTime = TickCount(2000);
                }
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 09");
                    ErrorTime = TickCount(10000);
                }
            }

        }
        public static void Orbwalking_BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            try
            {

            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 10");
                    ErrorTime = TickCount(10000);
                }
            }

        }
        private static void Obj_AI_Base_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            try
            {
                if (sender.IsMe && RTime > Environment.TickCount)
                    args.Process = false;
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 11");
                    ErrorTime = TickCount(10000);
                }
            }

        }
        private static void GameObject_OnCreate(GameObject obj, EventArgs args)
        {
            try
            {

            }
            catch (Exception e)
            {
                Console.Write(e);
                Chat.Print("FreshPoppy is not working. plz send message by KorFresh (Code 13)");
            }
        }

        private static void GameObject_OnDelete(GameObject obj, EventArgs args)
        {
            try
            {

            }
            catch (Exception e)
            {
                Console.Write(e);
                Chat.Print("FreshPoppy is not working. plz send message by KorFresh (Code 14)");
            }
        }
    }
}
