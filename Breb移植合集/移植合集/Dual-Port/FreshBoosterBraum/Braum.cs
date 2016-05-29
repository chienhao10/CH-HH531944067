using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using static FreshBooster.FreshCommon;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;

namespace FreshBooster.Champion
{
    class Braum
    {
        // Default Setting
        public static int ErrorTime;
        public const string ChampName = "Braum";   // Edit
        public static AIHeroClient Player;
        public static LeagueSharp.Common.Spell _Q, _W, _E, _R;
        // Default Setting

        public static bool QSpell = false;
        public static int SpellTime = 0;
        public static Menu menu, Combo, Harass, KillSteal, Misc, Draw;

        private void SkillSet()
        {
            try
            {
                _Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1000f);
                _Q.SetSkillshot(0.25f, 120f, 1400f, true, SkillshotType.SkillshotLine);
                _W = new LeagueSharp.Common.Spell(SpellSlot.W, 650);
                _E = new LeagueSharp.Common.Spell(SpellSlot.E, 0);
                _R = new LeagueSharp.Common.Spell(SpellSlot.R, 1250f);
                _R.SetSkillshot(0.5f, 120f, 1200f, false, SkillshotType.SkillshotLine);
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
        private void Menu()
        {
            try
            {
                menu = MainMenu.AddMenu("FreshBooster Braum", "FreshBoosterBraum");

                Combo = menu.AddSubMenu("Combo", "Combo");
                {
                    Combo.Add("Braum_CUse_Q", new CheckBox("Use Q"));
                    Combo.Add("Braum_CUse_Q_Hit", new Slider("Q HitChance", 3, 1, 6));
                    Combo.Add("Braum_CUse_R", new CheckBox("Use R"));
                }

                Harass = menu.AddSubMenu("Harass", "Harass");
                {
                    Harass.Add("Braum_HUse_Q", new CheckBox("Use Q"));
                    Harass.Add("Braum_Auto_HEnable", new CheckBox("Auto Harass"));
                    Harass.Add("Braum_HMana", new Slider("Min. Mana %", 50));
                }

                KillSteal = menu.AddSubMenu("KillSteal", "KillSteal");
                {
                    KillSteal.Add("Braum_KUse_Q", new CheckBox("Use Q"));
                    KillSteal.Add("Braum_KUse_R", new CheckBox("Use R"));
                }

                Misc = menu.AddSubMenu("Misc", "Misc");
                {
                    Misc.Add("Braum_Flee", new KeyBind("Flee Key", false, KeyBind.BindTypes.HoldActive, 'G'));
                    Misc.Add("Braum_AutoW", new CheckBox("Auto W"));
                    Misc.Add("Braum_AutoE", new CheckBox("Auto E"));
                    Misc.Add("Braum_InterR", new CheckBox("Interrupt w/ Use R"));
                    Misc.Add("Braum_GapQ", new CheckBox("Gap w/ Use Q"));
                    Misc.Add("Braum_GapR", new CheckBox("Gap w/ Use R"));
                }

                Draw = menu.AddSubMenu("Draw", "Draw");
                {
                    Draw.Add("Braum_Draw_Q", new CheckBox("Draw Q", false));
                    Draw.Add("Braum_Draw_W", new CheckBox("Draw W", false));
                    Draw.Add("Braum_Draw_R", new CheckBox("Draw R", false));
                }
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

        public static void Drawing_OnDraw(EventArgs args)
        {
            try
            {
                if (Player.IsDead) return;
                if (getCheckBoxItem(Draw, "Braum_Draw_Q"))
                    Render.Circle.DrawCircle(Player.Position, _Q.Range, Color.White, 1);
                if (getCheckBoxItem(Draw, "Braum_Draw_W"))
                    Render.Circle.DrawCircle(Player.Position, _W.Range, Color.White, 1);
                if (getCheckBoxItem(Draw, "Braum_Draw_R"))
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
                    if (_R.IsReady())
                        damage += _R.GetDamage(enemy);
                    if (!Player.Spellbook.IsAutoAttacking)
                        damage += (float)Player.LSGetAutoAttackDamage(enemy, true);
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
        public static void Drawing_OnEndScene(EventArgs args)
        {
            try
            {
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 05");
                    ErrorTime = TickCount(10000);
                }
            }
        }

        // OnLoad
        public Braum()
        {
            Player = ObjectManager.Player;
            SkillSet();
            Menu();
            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
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
                var RTarget = TargetSelector.GetTarget(_R.Range, DamageType.Magical);

                // Kill
                if (getCheckBoxItem(KillSteal, "Braum_KUse_Q") && QTarget != null && _Q.IsReady() && _Q.GetDamage(QTarget) > QTarget.Health)
                {
                    _Q.CastIfHitchanceEquals(QTarget, HitChance.Low, true);
                    return;
                }
                if (getCheckBoxItem(KillSteal, "Braum_KUse_R") && QTarget != null && _R.IsReady() && _R.GetDamage(RTarget) > RTarget.Health)
                {
                    _R.CastIfHitchanceEquals(QTarget, HitChance.VeryHigh, true);
                    return;
                }

                // Flee
                if (getKeyBindItem(Misc, "Braum_Flee"))
                {
                    MovingPlayer(Game.CursorPos);
                    if (QTarget != null && _Q.IsReady())
                        _Q.CastIfHitchanceEquals(QTarget, HitChance.Low, true);
                }

                // Combo
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    if (getCheckBoxItem(Combo, "Braum_CUse_R") && _R.IsReady() && RTarget != null)
                        _R.CastIfHitchanceEquals(RTarget, HitChance.VeryHigh, true);
                    if (getCheckBoxItem(Combo, "Braum_CUse_Q") && _Q.IsReady() && QTarget != null)
                        _Q.CastIfHitchanceEquals(QTarget, Hitchance("Braum_CUse_Q_Hit"), true);
                }

                // Harass
                if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) || getCheckBoxItem(Harass, "Braum_Auto_HEnable"))
                    && getSliderItem(Harass, "Braum_HMana") < Player.ManaPercent)
                {
                    if (getCheckBoxItem(Harass, "Braum_HUse_Q") && _Q.IsReady() && QTarget != null)
                        _Q.CastIfHitchanceEquals(QTarget, HitChance.VeryHigh, true);
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
                if (getCheckBoxItem(Misc, "Braum_GapQ") && _Q.IsReady() && gapcloser.Sender.Position.LSDistance(Player.Position) < _Q.Range)
                {
                    _Q.CastIfHitchanceEquals(gapcloser.Sender, HitChance.Low, true);
                    return;
                }
                if (getCheckBoxItem(Misc, "Braum_GapR") && _R.IsReady() && gapcloser.Sender.Position.LSDistance(Player.Position) < _R.Range)
                {
                    _R.CastIfHitchanceEquals(gapcloser.Sender, HitChance.VeryHigh, true);
                    return;
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
                if (!(sender is AIHeroClient) || Player.LSIsRecalling())
                    return;
                // Auto W
                if (getCheckBoxItem(Misc, "Braum_AutoW") && _W.IsReady())
                {
                    if (!(sender is AIHeroClient) || !sender.IsEnemy)
                        return;
                    if (args.Target != null)
                        if (args.SData.Name.ToLower().Contains("attack") && args.Target.Position.LSDistance(Player.Position) < _W.Range)
                            if (args.Target.IsAlly && args.Target is AIHeroClient)
                            {
                                if (args.Target.IsMe && Player.HealthPercent < 20)
                                {
                                    _W.CastOnUnit((Obj_AI_Base)args.Target, true);
                                }
                                else
                                {
                                    _W.CastOnUnit((Obj_AI_Base)args.Target, true);
                                }
                            }
                }
                // Auto E
                if (getCheckBoxItem(Misc, "Braum_AutoE") && _E.IsReady())
                {
                    if (!(sender is AIHeroClient) || !sender.IsEnemy || !Orbwalker.CanAutoAttack)
                        return;
                    var enemyskill = new LeagueSharp.Common.Geometry.Polygon.Rectangle(args.Start, args.End, args.SData.BounceRadius + 20);
                    var myteam = HeroManager.Allies.Where(f => f.LSDistance(Player.Position) < 200);
                    var count = myteam.Count(f => enemyskill.IsInside(f.Position));
                    if (args.Target != null && args.Target.Position.LSDistance(Player.Position) < 200)
                    {
                        if (args.Target.Name == Player.Name && Player.HealthPercent < 20)
                        {
                            _E.Cast(sender.Position, true);
                        }
                        else if (args.Target.Position.LSDistance(Player.Position) < 200 && args.Target is AIHeroClient)
                        {
                            if (_W.IsReady() && args.Target.Position.LSDistance(Player.Position) < _W.Range)
                                _W.CastOnUnit((Obj_AI_Base)args.Target, true);
                            _E.Cast(sender.Position, true);
                        }
                    }
                    else if (args.Target == null)
                    {
                        if (Player.HealthPercent < 20 && count == 1)
                        {
                            _E.Cast(sender.Position, true);
                        }
                        else if (count >= 2)
                        {
                            _E.Cast(sender.Position, true);
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
                if (getCheckBoxItem(Misc, "Braum_InterR") && _R.IsReady() && sender.IsEnemy && sender.Position.LSDistance(Player.Position) < _R.Range * 0.9)
                    _R.CastIfHitchanceEquals(sender, HitChance.VeryHigh, true);
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