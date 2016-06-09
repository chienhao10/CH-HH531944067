using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using static FreshBooster.FreshCommon;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using Spell = LeagueSharp.Common.Spell;

namespace FreshBooster.Champion
{
    class Bard
    {
        // Default Setting
        public static int ErrorTime;
        public const string ChampName = "Bard";   // Edit
        public static AIHeroClient Player;
        public static Spell _Q, _W, _E, _R;
        // Default Setting

        public static int cnt = 0;
        public static Obj_AI_Base BardQTarget1, BardQTarget2;
        public static LeagueSharp.Common.Geometry.Polygon.Rectangle Range1, Range2;
        public static int RCnt;

        private void SkillSet()
        {
            try
            {
                _Q = new Spell(SpellSlot.Q, 950f);
                _Q.SetSkillshot(0.4f, 120f, 1400f, false, SkillshotType.SkillshotLine);
                _W = new Spell(SpellSlot.W, 1000);
                _E = new Spell(SpellSlot.E, 900);
                _R = new Spell(SpellSlot.R, 3400f);
                _R.SetSkillshot(0.5f, 340f, 1400f, false, SkillshotType.SkillshotCircle);
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

        public static Menu menu, comboMenu, harassMenu, ksMenu, miscMenu, drawMenu;

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
                menu = MainMenu.AddMenu("FreshBooster Bard", "bardogas");

                comboMenu = menu.AddSubMenu("Combo", "Combo");
                {
                    comboMenu.Add("Bard_CUse_Q", new CheckBox("Use Q"));
                    comboMenu.Add("Bard_CUse_Q_Hit", new Slider("Q HitChance", 3, 1, 6));
                    comboMenu.Add("Bard_CUse_OnlyQ", new CheckBox("Only use Q sturn"));

                }

                harassMenu = menu.AddSubMenu("Harass", "Harass");
                {
                    harassMenu.Add("Bard_HUse_Q", new CheckBox("Use Q"));
                    harassMenu.Add("Bard_HUse_OnlyQ", new CheckBox("Only use Q sturn"));
                    harassMenu.Add("Bard_AManarate", new Slider("Mana %", 20, 0, 100));
                    harassMenu.Add("Bard_Auto_HEnable", new CheckBox("Auto Harass"));
                }

                ksMenu = menu.AddSubMenu("KillSteal", "KillSteal");

                {
                    ksMenu.Add("Bard_KUse_Q", new CheckBox("Use Q"));
                }

                miscMenu = menu.AddSubMenu("Misc", "Misc");
                {
                    miscMenu.AddGroupLabel("Heal W");
                    miscMenu.Add("Bard_HealWMin", new Slider("Min HP %", 20, 0, 100));
                    miscMenu.Add("Bard_HealWMinEnable", new CheckBox("Enable"));
                    miscMenu.Add("Bard_Anti", new CheckBox("Anti-Gabcloser Q"));
                    miscMenu.Add("Bard_Inter", new CheckBox("Interrupt R"));
                    miscMenu.Add("BardRKey", new KeyBind("Ult R", false, KeyBind.BindTypes.HoldActive, 'G'));

                }


                drawMenu = menu.AddSubMenu("Draw", "Draw");
                {
                    drawMenu.Add("Bard_Draw_Q", new CheckBox("Draw Q", false));
                    drawMenu.Add("Bard_Draw_W", new CheckBox("Draw W", false));
                    drawMenu.Add("Bard_Draw_E", new CheckBox("Draw E", false));
                    drawMenu.Add("Bard_Draw_R", new CheckBox("Draw R", false));
                    drawMenu.Add("Bard_Draw_R1", new CheckBox("Draw R in MiniMap", false));

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
        public static void Drawing_OnDraw(EventArgs args)
        {
            try
            {
                if (Player.IsDead) return;
                if (getCheckBoxItem(drawMenu, "Bard_Draw_Q"))
                    Render.Circle.DrawCircle(Player.Position, _Q.Range, Color.White, 1);
                if (getCheckBoxItem(drawMenu, "Bard_Draw_W"))
                    Render.Circle.DrawCircle(Player.Position, _W.Range, Color.White, 1);
                if (getCheckBoxItem(drawMenu, "Bard_Draw_E"))
                    Render.Circle.DrawCircle(Player.Position, _W.Range, Color.White, 1);
                if (getCheckBoxItem(drawMenu, "Bard_Draw_R"))
                    Render.Circle.DrawCircle(Player.Position, _R.Range, Color.White, 1);
                if (getCheckBoxItem(drawMenu, "Bard_Draw_R1"))
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, _R.Range, Color.White, 1, 23, true);
                var QTarget = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);
                if (QTarget != null)
                    Drawing.DrawCircle(QTarget.Position, 150, Color.Green);
                if (getKeyBindItem(miscMenu, "BardRKey"))
                {
                    Render.Circle.DrawCircle(Game.CursorPos, 340, Color.Aqua);
                    //Drawing.DrawCircle(Game.CursorPos, _R.Range, Color.AliceBlue);
                }
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
        public Bard()
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
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        private static void OnGameUpdate(EventArgs args)
        {
            try
            {
                if (Player.IsDead) return;

                // Set Target
                var QTarget = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);

                //Kill
                if (getCheckBoxItem(ksMenu, "Bard_KUse_Q"))
                    if (QTarget != null)
                        if (_Q.IsReady())
                            if (QTarget.Health < _Q.GetDamage(QTarget))
                                _Q.CastIfHitchanceEquals(QTarget, HitChance.High, true);

                // W
                if (getCheckBoxItem(miscMenu, "Bard_HealWMinEnable") && !Player.IsRecalling())
                {
                    var ally = HeroManager.Allies.OrderBy(f => f.Health).FirstOrDefault(f => f.Distance(Player.Position) < _W.Range && !f.IsDead && !f.IsZombie && f.HealthPercent < getSliderItem(miscMenu, "Bard_HealWMin"));
                    if (ally != null && _W.IsReady() && !ally.InFountain(LeagueSharp.Common.Utility.FountainType.OwnFountain))
                        _W.CastOnUnit(ally, true);
                }

                //R
                if (getKeyBindItem(miscMenu, "BardRKey") && _R.IsReady())
                {
                    RCnt = 0;
                    var range = HeroManager.AllHeroes.OrderBy(f => Game.CursorPos.Distance(f.Position) < 340f);
                    if (range == null)
                        return;
                    foreach (var item in range)
                    {
                        RCnt++;
                    }
                    if (RCnt == 0)
                        return;
                    var target = range.FirstOrDefault(f => f.Distance(Game.CursorPos) < 340f);
                    _R.SetSkillshot(Player.Distance(target.Position) * 3400 / 1.4f, 340f, 1400f, false, SkillshotType.SkillshotCircle);
                    if (target != null)
                        _R.CastIfHitchanceEquals(target, HitChance.Medium, true);
                }

                // Combo
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    if (getCheckBoxItem(comboMenu, "Bard_CUse_Q") && _Q.IsReady() && QTarget != null)
                    {
                        BardQ(QTarget, true);
                        if (getCheckBoxItem(comboMenu, "Bard_CUse_OnlyQ"))
                        {
                            if (cnt == 2)
                                if (BardQTarget1 is AIHeroClient || BardQTarget2 is AIHeroClient)
                                    _Q.CastIfHitchanceEquals(BardQTarget1, Hitchance("Bard_CUse_Q_Hit"), true);
                            if (cnt == 1 && BardQTarget1 is AIHeroClient && BardQTarget1.Position.Extend(Player.Position, -450).IsWall())
                                _Q.CastIfHitchanceEquals(BardQTarget1, Hitchance("Bard_CUse_Q_Hit"), true);
                        }
                        else
                        {
                            BardQ(QTarget, false);
                            if (BardQTarget1 == QTarget || BardQTarget2 == QTarget)
                                _Q.CastIfHitchanceEquals(BardQTarget1, Hitchance("Bard_CUse_Q_Hit"), true);
                        }
                    }
                }

                // Harass
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) || getCheckBoxItem(harassMenu, "Bard_Auto_HEnable"))
                {
                    if (getCheckBoxItem(harassMenu, "Bard_HUse_Q") && _Q.IsReady() && QTarget != null && getSliderItem(harassMenu, "Bard_AManarate") < Player.ManaPercent)
                    {
                        BardQ(QTarget, true);
                        // Sturn
                        if (getCheckBoxItem(harassMenu, "Bard_HUse_OnlyQ"))
                        {
                            if (cnt == 2 && Range2 != null)
                                if ((BardQTarget1 is AIHeroClient && BardQTarget1 != Player) || (BardQTarget2 is AIHeroClient && BardQTarget2 != Player))
                                {
                                    _Q.CastIfHitchanceEquals(BardQTarget1, Hitchance("Bard_CUse_Q_Hit"), true);
                                    Console.WriteLine("1");
                                }

                            if (cnt == 1 && BardQTarget1 is AIHeroClient && BardQTarget1.Position.Extend(Player.Position, -400).IsWall())
                            {
                                _Q.CastIfHitchanceEquals(BardQTarget1, Hitchance("Bard_CUse_Q_Hit"), true);
                                Console.WriteLine("2");
                            }
                        }
                        else
                        {
                            BardQ(QTarget, false);
                            if (BardQTarget1 == QTarget || BardQTarget2 == QTarget)
                            {
                                _Q.CastIfHitchanceEquals(BardQTarget1, Hitchance("Bard_CUse_Q_Hit"), true);
                                Console.WriteLine("3");
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    //Game.PrintChat(ChampName + " in FreshBooster isn't Load. Error Code 06");
                    ErrorTime = TickCount(10000);
                }
            }
        }
        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            try
            {
                if (getCheckBoxItem(miscMenu, "Bard_Anti") && _Q.IsReady() && gapcloser.Sender.Distance(Player.Position) < _Q.Range)
                    _Q.CastIfHitchanceEquals(gapcloser.Sender, HitChance.Medium, true);
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
        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {

        }
        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            try
            {
                if (getCheckBoxItem(miscMenu, "Bard_Inter") && _R.IsReady() && sender.Distance(Player.Position) < _R.Range)
                {
                    _R.SetSkillshot(Player.Distance(sender.Position) * 3400 / 1.5f, 340f, 1400f, false, SkillshotType.SkillshotCircle);
                    _R.CastIfHitchanceEquals(sender, HitChance.Medium, true);
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

        private static void BardQ(AIHeroClient Target, bool Type, bool Draw = false)
        {
            // Type 0: no sturn / 1: only sturn
            // If Draw is true, return draw
            /* return
            target1, target2, type
            */
            Range1 = new LeagueSharp.Common.Geometry.Polygon.Rectangle(Player.Position, Player.Position.LSExtend(Target.Position, _Q.Range), _Q.Width);
            Range2 = null;
            if (Draw)
                Range1.Draw(Color.Red);
            cnt = 0;
            BardQTarget1 = Player;
            BardQTarget2 = Player;
            foreach (var item in ObjectManager.Get<Obj_AI_Base>().OrderBy(f => f.Distance(f.Position)))
            {
                if (item.Distance(Player.Position) < _Q.Range)
                    if (item is AIHeroClient || item is Obj_AI_Minion)
                        if (item.IsEnemy && !item.IsDead)
                        {
                            if (cnt == 2)
                                break;
                            if (cnt == 0 && Range1.IsInside(item.Position))
                            {
                                BardQTarget1 = item;
                                Range2 = new LeagueSharp.Common.Geometry.Polygon.Rectangle(Player.Position.Extend(BardQTarget1.Position, Player.Distance(BardQTarget1.Position)),
                                    Player.Position.Extend(BardQTarget1.Position, Player.Distance(BardQTarget1.Position) + 450), _Q.Width);
                                if (Draw)
                                    Range2.Draw(Color.Yellow);
                                cnt++;
                            }
                            if (cnt == 1 && Range2.IsInside(item.Position))
                            {
                                BardQTarget2 = item;
                                cnt++;
                            }
                        }
            }
        }
    }
}
