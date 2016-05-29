using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using static FreshBooster.FreshCommon;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;
using Spell = LeagueSharp.Common.Spell;

namespace FreshBooster.Champion
{
    class Leblanc
    {
        // Default Setting
        public static int ErrorTime;
        public const string ChampName = "Leblanc";   // Edit
        public static AIHeroClient Player;
        public static Spell _Q, _W, _E, _R;
        public static Menu menu, Combo, Harass, KillSteal, Misc, Draw;
        // Default Setting

        public static List<Spell> SpellList = new List<Spell>();
        public static List<int> SpellUseTree = new List<int>();
        public static string[] SpellName = new string[]
        {
            "LeblancChaosOrb","LeblancChaosOrbM",   // 0,1
            "LeblancSlide", "LeblancSlideM", "leblancslidereturn","leblancslidereturnm",    // 2,3,4,5
            "LeblancSoulShackle","LeblancSoulShackleM"     // 6,7
        };
        public static int SpellUseCnt = 0, SpellUseCnt1 = 0, ERCC = 0;
        public static int SpellUseTime = 0, ERTIME = 0, WTime = 0, RTime = 0, ReturnTime = 0, PetTime = 0;

        private void SkillSet()
        {
            try
            {
                _Q = new Spell(SpellSlot.Q, 720);
                _Q.SetTargetted(0.5f, 1500f);
                _W = new Spell(SpellSlot.W, 700);
                _W.SetSkillshot(0.6f, 220f, 1450f, false, SkillshotType.SkillshotCircle);
                _E = new Spell(SpellSlot.E, 900);
                _E.SetSkillshot(0.3f, 55f, 1650f, true, SkillshotType.SkillshotLine);
                _R = new Spell(SpellSlot.R, 720);
                {
                    SpellList.Add(_Q);
                    SpellList.Add(_W);
                    SpellList.Add(_E);
                    SpellList.Add(_R);
                }
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
                menu = MainMenu.AddMenu("FreshBooster LeBlanc", "FreshBoosterLeBlanc");

                Combo = menu.AddSubMenu("Combo", "Combo");
                {
                    Combo.Add("Braum_CUse_Q", new CheckBox("Use Q"));
                    Combo.Add("Braum_CUse_W", new CheckBox("Use W"));
                    Combo.Add("Leblanc_CUse_WReturn", new CheckBox("Use W Return"));
                    Combo.Add("Braum_CUse_E", new CheckBox("Use E"));
                    Combo.AddLabel("R Settings");
                    Combo.Add("Leblanc_CUse_Q2", new CheckBox("Use Q + R"));
                    Combo.Add("Leblanc_CUse_W2", new CheckBox("Use W + R"));
                    Combo.Add("Leblanc_CUse_W2Return", new CheckBox("Use W Return"));
                    Combo.Add("Leblanc_CUse_E2", new CheckBox("Use E + R"));
                    Combo.Add("Leblanc_CUseE_Hit", new Slider("E HitChance", 3, 1, 6));
                    Combo.Add("Leblanc_ComboMode", new KeyBind("Combo Mode is Teamfight", false, KeyBind.BindTypes.PressToggle, 'N'));
                }

                Harass = menu.AddSubMenu("Harass", "Harass");
                {
                    Harass.Add("Leblanc_AUse_Q", new CheckBox("Use Q"));
                    Harass.Add("Leblanc_AUse_W", new CheckBox("Use W"));
                    Harass.Add("Leblanc_AUse_E", new CheckBox("Use E"));
                    Harass.Add("Leblanc_AManarate", new Slider("Mana %", 20, 0, 100));
                    Harass.Add("Leblanc_AHToggle", new CheckBox("Auto Enable", false));

                }

                KillSteal = menu.AddSubMenu("KillSteal", "KillSteal");
                {
                    KillSteal.Add("Leblanc_KUse_Q", new CheckBox("Use Q"));
                    KillSteal.Add("Leblanc_KUse_W", new CheckBox("Use W"));
                    KillSteal.Add("Leblanc_KUse_E", new CheckBox("Use E"));
                    KillSteal.Add("Leblanc_KUse_Q2", new CheckBox("Use R+Q"));
                    KillSteal.Add("Leblanc_KUse_W2", new CheckBox("Use R+W"));
                    KillSteal.Add("Leblanc_KUse_E2", new CheckBox("Use R+E"));

                }


                Misc = menu.AddSubMenu("Misc", "Misc");
                {
                    Misc.Add("Leblanc_ERCC", new KeyBind("Use _E + _R CC", false, KeyBind.BindTypes.HoldActive, 'G'));
                    Misc.Add("Leblanc_Flee", new KeyBind("Flee & W + R", false, KeyBind.BindTypes.HoldActive, 'Z'));
                    Misc.Add("Leblanc_Pet", new CheckBox("Passive will be locating between Me & Enemy"));

                }

                Draw = menu.AddSubMenu("Draw", "Draw");
                {

                    Draw.Add("Leblanc_Draw_Q", new CheckBox("Draw Q"));
                    Draw.Add("Leblanc_Draw_W", new CheckBox("Draw W"));
                    Draw.Add("Leblanc_Draw_E", new CheckBox("Draw E"));
                    Draw.Add("Leblanc_Draw_WR", new CheckBox("Draw W + R"));
                    Draw.Add("Leblanc_WTimer", new CheckBox("Indicate _W Timer"));
                    Draw.Add("Leblanc_Draw_ComboMode", new CheckBox("Draw Combo Mode"));
                    Draw.Add("Leblanc_REnable", new CheckBox("Show _R Status"));


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
                if (Player.IsDead)
                    return;
                if (getCheckBoxItem(Draw, "Leblanc_Draw_Q"))
                    Render.Circle.DrawCircle(Player.Position, _Q.Range, Color.White, 1);
                if (getCheckBoxItem(Draw, "Leblanc_Draw_W"))
                    Render.Circle.DrawCircle(Player.Position, _W.Range, Color.White, 1);
                if (getCheckBoxItem(Draw, "Leblanc_Draw_E"))
                    Render.Circle.DrawCircle(Player.Position, _E.Range, Color.White, 1);
                if (getCheckBoxItem(Draw, "Leblanc_Draw_WR"))
                    Render.Circle.DrawCircle(Player.Position, 1400, Color.White, 1);
                if (WTime > Environment.TickCount && getCheckBoxItem(Draw, "Leblanc_WTimer"))
                    Drawing.DrawText(Drawing.WorldToScreen(Player.Position)[0] - 20, Drawing.WorldToScreen(Player.Position)[1] + 20, Color.YellowGreen, "W Time is : " + ((WTime - Environment.TickCount) / 10));
                if (RTime > Environment.TickCount && getCheckBoxItem(Draw, "Leblanc_WTimer"))
                    Drawing.DrawText(Drawing.WorldToScreen(Player.Position)[0] - 20, Drawing.WorldToScreen(Player.Position)[1] + 30, Color.YellowGreen, "R Time is : " + ((RTime - Environment.TickCount) / 10));
                if (getCheckBoxItem(Draw, "Leblanc_REnable"))
                {
                    string Status_R = string.Empty;
                    if (Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancChaosOrbM")
                        Status_R = "_Q";
                    if (Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSlideM")
                        Status_R = "_W";
                    if (Player.Spellbook.GetSpell(SpellSlot.R).Name == "leblancslidereturnm")
                        Status_R = "_W (Return)";
                    if (Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSoulShackleM")
                        Status_R = "_E";
                    Drawing.DrawText(Player.HPBarPosition.X + 30, Player.HPBarPosition.Y - 40, Color.IndianRed, "R: " + Status_R);
                }
                if (getCheckBoxItem(Draw, "Leblanc_Draw_ComboMode"))
                {
                    var drawtext = getKeyBindItem(Combo, "Leblanc_ComboMode") ? "TeamFight" : "Private";
                    Drawing.DrawText(Player.HPBarPosition.X + 40, Player.HPBarPosition.Y + 30, Color.White, drawtext);
                }
                if (TargetSelector.GetTarget(_Q.Range, DamageType.Magical) != null)
                    Drawing.DrawCircle(TargetSelector.GetTarget(1400, DamageType.Magical).Position, 150, Color.Green);
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
                        if (_R.IsReady() || _W.IsReady() || _E.IsReady())
                        {
                            damage += (_Q.GetDamage(enemy) * 2);
                        }
                        else
                        {
                            damage += _Q.GetDamage(enemy);
                        }
                    if (_W.IsReady())
                        damage += _W.GetDamage(enemy);
                    if (_E.IsReady())
                        damage += (_E.GetDamage(enemy));
                    if (_R.IsReady())
                        if (_Q.IsReady())
                        {
                            damage += (_Q.GetDamage(enemy) * 1.5f * 2f);
                        }
                        else
                        {
                            damage += _R.GetDamage(enemy);
                        }
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
        public Leblanc()
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
        }

        private static void OnGameUpdate(EventArgs args)
        {
            try
            {
                if (Player.IsDead) return;
                if (getKeyBindItem(Misc, "Leblanc_Flee"))
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);  // 커서방향 이동
                    if (_W.IsReady() && Player.Spellbook.GetSpell(SpellSlot.W).Name == "LeblancSlide")
                        _W.Cast(Game.CursorPos, true);
                    if (_R.IsReady() && Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSlideM")
                        _R.Cast(Game.CursorPos, true);
                }

                if (getKeyBindItem(Misc, "Leblanc_ERCC")) // ERCC
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);  // 커서방향 이동
                    var ETarget = TargetSelector.GetTarget(_E.Range, DamageType.Magical);
                    if (_E.IsReady() && ETarget != null && Environment.TickCount > ERTIME)
                    {
                        _E.CastIfHitchanceEquals(ETarget, HitChance.Low, true);
                    }
                    else if (ETarget != null && _R.IsReady() && Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSoulShackleM" && Environment.TickCount > ERTIME)
                    {
                        _R.CastIfHitchanceEquals(ETarget, HitChance.Low, true);
                    }
                }

                // Pet
                if (getCheckBoxItem(Misc, "Leblanc_Pet") && Player.Pet != null && Player.Pet.IsValid && !Player.Pet.IsDead)
                {
                    var Enemy = ObjectManager.Get<AIHeroClient>().OrderBy(x => x.LSDistance(Player)).FirstOrDefault(x => x.IsEnemy && !x.IsDead);
                    Random Ran = new Random();
                    var PetLocate = Player.ServerPosition.LSExtend(Enemy.ServerPosition, Ran.Next(150, 300));
                    PetLocate.X = PetLocate.X + Ran.Next(0, 20);
                    PetLocate.Y = PetLocate.Y + Ran.Next(0, 20);
                    PetLocate.Z = PetLocate.Z + Ran.Next(0, 20);
                    if (PetTime < Environment.TickCount)
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MovePet, PetLocate);
                        PetTime = TickCount(750);
                    }

                }

                if (getCheckBoxItem(KillSteal, "Leblanc_KUse_Q") && _Q.IsReady())
                {
                    var QTarget = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);
                    if (QTarget == null) return;
                    if (QTarget.Health <= _Q.GetDamage(QTarget))
                    {
                        _Q.Cast(QTarget, true);
                        return;
                    }
                }
                if (getCheckBoxItem(KillSteal, "Leblanc_KUse_W") && _W.IsReady() && Player.Spellbook.GetSpell(SpellSlot.W).Name == "LeblancSlide")
                {
                    var WTarget = TargetSelector.GetTarget(_W.Range, DamageType.Magical);
                    if (WTarget == null) return;
                    if (WTarget.Health <= _W.GetDamage(WTarget))
                    {
                        _W.Cast(WTarget.ServerPosition, true);
                        return;
                    }
                }
                if (getCheckBoxItem(KillSteal, "Leblanc_KUse_E") && _E.IsReady())
                {
                    var ETarget = TargetSelector.GetTarget(_E.Range, DamageType.Magical);
                    if (ETarget == null) return;
                    if (ETarget.Health <= _E.GetDamage(ETarget))
                    {
                        _E.CastIfHitchanceEquals(ETarget, HitChance.Low, true);
                        return;
                    }
                }
                if (getCheckBoxItem(KillSteal, "Leblanc_KUse_Q2") && _R.IsReady() && Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancChaosOrbM")
                {
                    var QTarget = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);
                    if (QTarget == null) return;
                    if (QTarget.Health <= _Q.GetDamage(QTarget))
                    {
                        _R.Cast(QTarget, true);
                        return;
                    }
                }
                if (getCheckBoxItem(KillSteal, "Leblanc_KUse_W2") && _R.IsReady() && Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSlideM")
                {
                    var QTarget = TargetSelector.GetTarget(_W.Range, DamageType.Magical);
                    if (QTarget == null) return;
                    if (QTarget.Health <= _W.GetDamage(QTarget))
                    {
                        _R.Cast(QTarget.ServerPosition, true);
                        return;
                    }
                }
                if (getCheckBoxItem(KillSteal, "Leblanc_KUse_E2") && _R.IsReady() && Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSoulShackleM")
                {
                    var QTarget = TargetSelector.GetTarget(_E.Range, DamageType.Magical);
                    if (QTarget == null) return;
                    if (QTarget.Health <= _E.GetDamage(QTarget))
                    {
                        _R.CastIfHitchanceEquals(QTarget, HitChance.Low, true);
                        return;
                    }
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) // Combo
                {
                    var QTarget = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);
                    var WTarget = TargetSelector.GetTarget(_W.Range, DamageType.Magical);
                    var ETarget = TargetSelector.GetTarget(_E.Range, DamageType.Magical);
                    var TargetSel = TargetSelector.SelectedTarget;
                    if (QTarget == null && WTarget == null && ETarget == null) return;

                    // Q
                    if (QTarget != null
                            && getCheckBoxItem(Combo, "Leblanc_CUse_Q") && _Q.IsReady()
                            && Player.Spellbook.GetSpell(SpellSlot.Q).Name == "LeblancChaosOrb"
                            )
                    {
                        ReturnTime = TickCount(1000);
                        _Q.Cast(QTarget, true);
                        return;
                    }

                    if (Player.Level > 5 && TargetSel == null && getKeyBindItem(Combo, "Leblanc_ComboMode"))   // teamcombo
                    {
                        // W
                        if (WTarget != null
                                && getCheckBoxItem(Combo, "Leblanc_CUse_W") && _W.IsReady()
                                && Player.Spellbook.GetSpell(SpellSlot.W).Name == "LeblancSlide"
                                )
                        {
                            _W.Cast(WTarget.ServerPosition, true);
                            return;
                        }

                        // W2
                        if (WTarget != null
                                && getCheckBoxItem(Combo, "Leblanc_CUse_W2") && _R.IsReady()
                                && Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSlideM")
                        {
                            _R.Cast(WTarget.ServerPosition, true);
                            return;
                        }
                    }

                    // Q2
                    if (QTarget != null
                            && getCheckBoxItem(Combo, "Leblanc_CUse_Q2") && _R.IsReady()
                            && Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancChaosOrbM"
                            )
                    {
                        _R.Cast(QTarget, true);
                        return;
                    }
                    // W
                    if (WTarget != null
                            && getCheckBoxItem(Combo, "Leblanc_CUse_W") && _W.IsReady()
                            && Player.Spellbook.GetSpell(SpellSlot.W).Name == "LeblancSlide"
                            )
                    {
                        _W.Cast(WTarget.ServerPosition, true);
                        return;
                    }

                    // W2
                    if (WTarget != null
                            && getCheckBoxItem(Combo, "Leblanc_CUse_W2") && _R.IsReady()
                            && Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSlideM")
                    {
                        _R.Cast(WTarget.ServerPosition, true);
                        return;
                    }

                    // E
                    if (ETarget != null
                            && getCheckBoxItem(Combo, "Leblanc_CUse_E") && _E.IsReady()
                            && Player.Spellbook.GetSpell(SpellSlot.E).Name == "LeblancSoulShackle" && Player.Spellbook.GetSpell(SpellSlot.R).Name != "LeblancSlideM"
                            )
                    {
                        _E.CastIfHitchanceEquals(ETarget, Hitchance("Leblanc_CUseE_Hit"), true);
                        return;
                    }

                    // E2
                    if (ETarget != null
                            && getCheckBoxItem(Combo, "Leblanc_CUse_E2") && _E.IsReady()
                            && Player.Spellbook.GetSpell(SpellSlot.E).Name == "LeblancSoulShackleM"
                            )
                    {
                        _R.CastIfHitchanceEquals(ETarget, Hitchance("Leblanc_CUseE_Hit"), true);
                        return;
                    }

                    // WReturn
                    if (ETarget.Buffs.Find(buff => (buff.Name == "LeblancSoulShackle" || buff.Name == "LeblancSoulShackleM") && buff.IsValidBuff()) == null
                        && getCheckBoxItem(Combo, "Leblanc_CUse_WReturn")
                        && _W.IsReady() && Player.Spellbook.GetSpell(SpellSlot.W).Name == "leblancslidereturn" && !_Q.IsReady() && !_R.IsReady() && ReturnTime < Environment.TickCount)
                    {
                        _W.Cast(true);
                        return;
                    }

                    // WR Return
                    if (ETarget.Buffs.Find(buff => (buff.Name == "LeblancSoulShackle" || buff.Name == "LeblancSoulShackleM") && buff.IsValidBuff()) == null
                        && getCheckBoxItem(Combo, "Leblanc_CUse_W2Return")
                        && _W.IsReady() && Player.Spellbook.GetSpell(SpellSlot.R).Name == "leblancslidereturnm" && !_Q.IsReady() && !_W.IsReady() && ReturnTime < Environment.TickCount)
                    {
                        _R.Cast(true);
                        return;
                    }

                }
                if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) || getCheckBoxItem(Harass, "Leblanc_AHToggle"))
                    && getSliderItem(Harass, "Leblanc_AManarate") < Player.ManaPercent) // Harass
                {

                    var QTarget = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);
                    var WTarget = TargetSelector.GetTarget(_W.Range, DamageType.Magical);
                    var ETarget = TargetSelector.GetTarget(_E.Range, DamageType.Magical);
                    // Q
                    if (QTarget != null
                            && getCheckBoxItem(Harass, "Leblanc_AUse_Q") && _Q.IsReady()
                            && Player.Spellbook.GetSpell(SpellSlot.Q).Name == "LeblancChaosOrb"
                            )
                    {
                        _Q.Cast(QTarget, true);
                        return;
                    }

                    // W
                    if (WTarget != null
                            && getCheckBoxItem(Harass, "Leblanc_AUse_W") && _W.IsReady()
                            && Player.Spellbook.GetSpell(SpellSlot.W).Name == "LeblancSlide"
                            )
                    {
                        _W.Cast(WTarget.ServerPosition, true);
                        return;
                    }

                    // E
                    if (ETarget != null
                            && getCheckBoxItem(Harass, "Leblanc_AUse_E") && _E.IsReady()
                            && Player.Spellbook.GetSpell(SpellSlot.E).Name == "LeblancSoulShackle"
                            )
                    {
                        _E.CastIfHitchanceEquals(ETarget, Hitchance("Leblanc_CUseE_Hit"), true);
                        return;
                    }

                    // WW
                    if (WTarget != null
                            && getCheckBoxItem(Harass, "Leblanc_AUse_W") && _W.IsReady()
                            && Player.Spellbook.GetSpell(SpellSlot.W).Name == "leblancslidereturn"
                            )
                    {
                        _W.Cast(true);
                        return;
                    }

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
                if (sender.IsMe)
                {
                    if (args.SData.Name == "LeblancSlide")
                        WTime = TickCount(4000);
                    if (args.SData.Name == "LeblancSlideM")
                        RTime = TickCount(4000);
                    if (getKeyBindItem(Misc, "Leblanc_ERCC"))
                    {
                        if (ERCC == 0 && args.SData.Name == "LeblancSoulShackle")
                        {
                            ERTIME = TickCount(2000);
                        }
                        if (ERCC == 1 && args.SData.Name == "LeblancSoulShackleM")
                        {
                            ERTIME = TickCount(2000);
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
        public void Orbwalking_BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
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
    }
}
