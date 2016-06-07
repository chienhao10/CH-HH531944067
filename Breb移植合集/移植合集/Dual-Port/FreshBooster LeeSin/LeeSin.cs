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
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace FreshBooster.Champion
{
    class LeeSin
    {
        // Default Setting
        public static int ErrorTime;
        public const string ChampName = "LeeSin";   // Edit
        public static AIHeroClient Player;
        public static LeagueSharp.Common.Spell _Q, _W, _E, _R;
        // Default Setting

        private static Vector3 InsecST, InsecED, InsecPOS;
        private static float Ward_Time, QTime, WTime, ETime, InsecTime = 0;
        private static bool WW = true;
        private static string InsecType = "Wait";

        private void SkillSet()
        {
            try
            {
                _Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1100f);
                _Q.SetSkillshot(0.25f, 65f, 1800f, true, SkillshotType.SkillshotLine);
                _W = new LeagueSharp.Common.Spell(SpellSlot.W, 700f);
                _E = new LeagueSharp.Common.Spell(SpellSlot.E, 330f);
                _R = new LeagueSharp.Common.Spell(SpellSlot.R, 375f);
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

        public static Menu menu, comboMenu, harassMenu, laneClearMenu, jungleClearMenu, miscMenu, drawMenu;

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
                menu = MainMenu.AddMenu("FreshBooster LeeSin", "leesinogas");

                comboMenu = menu.AddSubMenu("Combo", "Combo");
                comboMenu.Add("LeeSin_CUse_Q", new CheckBox("Use Q"));
                comboMenu.Add("LeeSin_CUse_W", new CheckBox("Use W"));
                comboMenu.Add("LeeSin_CUse_E", new CheckBox("Use E"));
                comboMenu.Add("LeeSin_CUse_R", new CheckBox("Use R"));
                comboMenu.AddSeparator();
                comboMenu.AddLabel("1 : Out of Range");
                comboMenu.AddLabel("2 : Impossible");
                comboMenu.AddLabel("3 : Low");
                comboMenu.AddLabel("4 : Medium");
                comboMenu.AddLabel("5 : High");
                comboMenu.AddLabel("6 : Very High");
                comboMenu.Add("LeeSin_CUseQ_Hit", new Slider("Q HitChance", 4, 1, 6));

                harassMenu = menu.AddSubMenu("Harass", "Harass");
                harassMenu.Add("LeeSin_HUse_Q", new CheckBox("Use Q"));
                harassMenu.Add("LeeSin_HUse_W", new CheckBox("Use W"));
                harassMenu.Add("LeeSin_HUse_E", new CheckBox("Use E"));

                laneClearMenu = menu.AddSubMenu("LaneClear", "LaneClear");
                laneClearMenu.Add("LeeSin_LUse_Q", new CheckBox("Use Q"));
                laneClearMenu.Add("LeeSin_LUse_W", new CheckBox("Use W"));
                laneClearMenu.Add("LeeSin_LUse_E", new CheckBox("Use E"));

                jungleClearMenu = menu.AddSubMenu("JungleClear", "JungleClear");
                jungleClearMenu.Add("LeeSin_JUse_Q", new CheckBox("Use Q"));
                jungleClearMenu.Add("LeeSin_JUse_W", new CheckBox("Use W"));
                jungleClearMenu.Add("LeeSin_JUse_E", new CheckBox("Use E"));

                miscMenu = menu.AddSubMenu("Misc", "Misc");
                miscMenu.Add("LeeSin_Ward_W", new KeyBind("W to Ward", false, KeyBind.BindTypes.HoldActive, 'A'));
                miscMenu.Add("LeeSin_InsecKick", new KeyBind("Insec Kick", false, KeyBind.BindTypes.HoldActive, 'G'));
                miscMenu.Add("LeeSin_KickAndFlash", new CheckBox("When use InsecKick, Do you use Flash + R?"));
                miscMenu.Add("LeeSin_KUse_R", new CheckBox("KillSteal, Use R"));
                miscMenu.Add("LeeSin_AutoKick", new Slider("When possible hit something, Use _R", 3, 0, 5));
                miscMenu.Add("LeeSin_AntiGab", new Slider("Anti-Gapcloser, If My hp < someone, Use _R", 15, 0, 100));

                drawMenu = menu.AddSubMenu("Draw", "Draw");
                drawMenu.Add("LeeSin_Draw_Q", new CheckBox("Draw Q", false));
                drawMenu.Add("LeeSin_Draw_W", new CheckBox("Draw W", false));
                drawMenu.Add("LeeSin_Draw_E", new CheckBox("Draw E", false));
                drawMenu.Add("LeeSin_Draw_Ward", new CheckBox("Draw Ward"));
                drawMenu.Add("LeeSin_PredictR", new CheckBox("_R Prediction Location"));
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
                if (getCheckBoxItem(drawMenu, "LeeSin_Draw_Q"))
                    Render.Circle.DrawCircle(Player.Position, _Q.Range, Color.White, 1);
                if (getCheckBoxItem(drawMenu, "LeeSin_Draw_W"))
                    Render.Circle.DrawCircle(Player.Position, _W.Range, Color.White, 1);
                if (getCheckBoxItem(drawMenu, "LeeSin_Draw_E"))
                    Render.Circle.DrawCircle(Player.Position, _E.Range, Color.White, 1);
                if (getCheckBoxItem(drawMenu, "LeeSin_Draw_Ward"))
                    Render.Circle.DrawCircle(Player.Position, 625, Color.White, 1);
                if (getKeyBindItem(miscMenu, "LeeSin_InsecKick") && TargetSelector.SelectedTarget != null && getCheckBoxItem(drawMenu, "LeeSin_PredictR"))
                {
                    var GetTarget = TargetSelector.SelectedTarget;
                    if (GetTarget == null || GetTarget.IsDead) return;
                    var Turrets = ObjectManager.Get<Obj_Turret>()
                    .OrderBy(obj => obj.Position.LSDistance(Player.Position))
                    .FirstOrDefault(obj => obj.IsAlly && !obj.IsDead);
                    var AllyChampion = ObjectManager.Get<AIHeroClient>().FirstOrDefault(obj => obj.IsAlly && !obj.IsMe && !obj.IsDead && obj.LSDistance(Player.Position) < 2000);
                    if (Turrets == null && AllyChampion == null) return;
                    if (AllyChampion != null)
                    { var InsecPOS = InsecST.LSExtend(InsecED, +InsecED.LSDistance(InsecST) + 230); }
                    Render.Circle.DrawCircle(InsecPOS, 50, Color.Gold);
                    if (GetTarget.LSDistance(Player.Position) < 625)
                    {
                        Render.Circle.DrawCircle(Player.Position, 525, Color.LightGreen);
                    }
                    else
                    {
                        Render.Circle.DrawCircle(Player.Position, 525, Color.IndianRed);
                    }
                    Drawing.DrawLine(Drawing.WorldToScreen(InsecST)[0], Drawing.WorldToScreen(InsecST)[1], Drawing.WorldToScreen(InsecED)[0], Drawing.WorldToScreen(InsecED)[1], 2, Color.Green);
                }
                if (_Q.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name == "blindmonkqtwo")
                {
                    var target = ObjectManager.Get<AIHeroClient>().FirstOrDefault(f => f.IsEnemy && !f.IsZombie && f.LSDistance(Player.Position) <= _Q.Range && f.HasBuff("BlindMonkQOne"));
                    if (target != null)
                        Render.Circle.DrawCircle(target.Position, 175, Color.YellowGreen);
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
                    if (_W.IsReady())
                        damage += _W.GetDamage(enemy);
                    if (_E.IsReady())
                        damage += _E.GetDamage(enemy);
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
        public LeeSin()
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
                //킬스틸 타겟
                var KTarget = ObjectManager.Get<AIHeroClient>().OrderByDescending(x => x.Health).FirstOrDefault(x => x.IsEnemy && x.LSDistance(Player) < 375);
                if (KTarget != null && getCheckBoxItem(miscMenu, "LeeSin_KUse_R") && KTarget.Health < _R.GetDamage(KTarget) && _R.IsReady())
                    _R.Cast(KTarget, true);
                if (InsecTime < Environment.TickCount) InsecType = "Wait"; // 인섹킥 초기화
                if (Ward_Time < Environment.TickCount) WW = true;   // 와드방호 초기화
                if (getSliderItem(miscMenu, "LeeSin_AutoKick") != 0 && _R.Level > 0 && _R.IsReady() && !getKeyBindItem(miscMenu, "LeeSin_InsecKick"))
                {
                    AutoKick();  // 오토 킥
                }
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) // Combo
                {
                    var QTarget = TargetSelector.GetTarget(_Q.Range, DamageType.Physical);
                    var ETarget = TargetSelector.GetTarget(_E.Range, DamageType.Physical);
                    if (QTarget != null && _Q.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name == "BlindMonkQOne" && getCheckBoxItem(comboMenu, "LeeSin_CUse_Q") && QTime < Environment.TickCount)
                    {
                        var HC = HitChance.Medium;
                        switch (getSliderItem(comboMenu, "LeeSin_CUseQ_Hit"))
                        {
                            case 1:
                                HC = HitChance.OutOfRange;
                                break;
                            case 2:
                                HC = HitChance.Impossible;
                                break;
                            case 3:
                                HC = HitChance.Low;
                                break;
                            case 4:
                                HC = HitChance.Medium;
                                break;
                            case 5:
                                HC = HitChance.High;
                                break;
                            case 6:
                                HC = HitChance.VeryHigh;
                                break;

                        }
                        var prediction = _Q.GetPrediction(QTarget);
                        if (prediction.Hitchance >= HC)
                        {
                            _Q.Cast(prediction.CastPosition);
                        }
                        QTime = TickCount(2000);
                    }

                    if (QTarget != null && QTarget.HasBuff("BlindMonkSonicWave") && _Q.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name != "BlindMonkQOne" && getCheckBoxItem(comboMenu, "LeeSin_CUse_Q"))
                    {
                        _Q.Cast();
                    }

                    if (ETarget != null && _E.IsReady() && !Orbwalker.CanAutoAttack && Orbwalker.CanMove && ETime < Environment.TickCount && getCheckBoxItem(comboMenu, "LeeSin_CUse_E"))
                    {
                        _E.Cast(true);
                        ETime = TickCount(1000);
                    }
                    if (!_Q.IsReady() && !_E.IsReady() && !Orbwalker.CanAutoAttack && Orbwalker.CanMove && WTime < Environment.TickCount && getCheckBoxItem(comboMenu, "LeeSin_CUse_W"))
                    {
                        _W.Cast(Player, true);
                        WTime = TickCount(1000);
                    }
                }
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) // Hafass
                {
                    var QTarget = TargetSelector.GetTarget(_Q.Range, DamageType.Physical);
                    var ETarget = TargetSelector.GetTarget(_E.Range, DamageType.Physical);
                    if (QTarget != null && _Q.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name == "BlindMonkQOne" && getCheckBoxItem(harassMenu, "LeeSin_HUse_Q") && QTime < Environment.TickCount)
                    {
                        var HC = HitChance.Medium;
                        _Q.CastIfHitchanceEquals(QTarget, HC, true);
                        QTime = TickCount(2000);
                    }
                    if (ETarget != null && _E.IsReady() && !Orbwalker.CanAutoAttack && Orbwalker.CanMove && ETime < Environment.TickCount && getCheckBoxItem(harassMenu, "LeeSin_HUse_E"))
                    {
                        _E.Cast(true);
                        ETime = TickCount(1000);
                    }
                    if (!_Q.IsReady() && !_E.IsReady() && !Orbwalker.CanAutoAttack && Orbwalker.CanMove && WTime < Environment.TickCount && getCheckBoxItem(harassMenu, "LeeSin_HUse_W"))
                    {
                        _W.Cast(Player, true);
                        WTime = TickCount(1000);
                    }
                }
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) // LaneClear
                {
                    var JungleTarget = MinionManager.GetMinions(1100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                    foreach (var minion in JungleTarget)
                    {
                        if (_Q.IsReady() && getCheckBoxItem(jungleClearMenu, "LeeSin_JUse_Q") && minion != null && Environment.TickCount > QTime)
                        {
                            _Q.CastIfHitchanceEquals(minion, HitChance.Medium, true);
                            QTime = TickCount(1500);
                        }
                        if (_E.IsReady() && getCheckBoxItem(jungleClearMenu, "LeeSin_JUse_E") && minion != null && Environment.TickCount > ETime
                            && !Orbwalker.CanAutoAttack && Orbwalker.CanMove)
                        {
                            _E.Cast(true);
                            ETime = TickCount(1500);
                        }
                        if (_W.IsReady() && getCheckBoxItem(jungleClearMenu, "LeeSin_JUse_W") && minion != null && Environment.TickCount > WTime
                            && !Orbwalker.CanAutoAttack && Orbwalker.CanMove)
                        {
                            _W.Cast(Player, true);
                            WTime = TickCount(1500);
                        }
                    }

                    var MinionTarget = MinionManager.GetMinions(1100, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);
                    foreach (var minion in MinionTarget)
                    {
                        if (_Q.IsReady() && getCheckBoxItem(laneClearMenu, "LeeSin_LUse_Q") && minion != null && Environment.TickCount > QTime)
                        {
                            _Q.CastIfHitchanceEquals(minion, HitChance.Medium, true);
                            QTime = TickCount(1000);
                        }
                        if (_E.IsReady() && getCheckBoxItem(laneClearMenu, "LeeSin_LUse_E") && minion != null && Environment.TickCount > ETime
                            && !Orbwalker.CanAutoAttack && Orbwalker.CanMove)
                        {
                            _E.Cast(true);
                            ETime = TickCount(1000);
                        }
                        if (_W.IsReady() && getCheckBoxItem(laneClearMenu, "LeeSin_LUse_W") && minion != null && Environment.TickCount > WTime && !Orbwalker.CanAutoAttack && Orbwalker.CanMove)
                        {
                            _W.Cast(Player, true);
                            WTime = TickCount(1000);
                        }
                    }
                }
                if (getKeyBindItem(miscMenu, "LeeSin_InsecKick"))    // 인섹킥
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

                    var GetTarget = TargetSelector.SelectedTarget;

                    if (GetTarget == null || GetTarget.IsDead) return;

                    if (_Q.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name == "BlindMonkQOne" && _Q.GetPrediction(GetTarget).Hitchance >= HitChance.Low)
                        _Q.Cast(GetTarget);
                    else if (_Q.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name != "BlindMonkQOne" && GetTarget.HasBuff("BlindMonkSonicWave"))
                        _Q.Cast();

                    var Turrets = ObjectManager.Get<Obj_Turret>()
                    .OrderBy(obj => obj.Position.LSDistance(Player.Position))
                    .FirstOrDefault(obj => obj.IsAlly && obj.Health > 1);
                    var AllyChampion = ObjectManager.Get<AIHeroClient>().FirstOrDefault(obj => obj.IsAlly && !obj.IsMe && !obj.IsDead && obj.LSDistance(Player.Position) < 2000);
                    if (Turrets == null && AllyChampion == null) return;
                    if (AllyChampion != null)
                    {
                        InsecST = AllyChampion.Position;
                    }
                    else
                    {
                        InsecST = Turrets.Position;
                    }
                    InsecED = GetTarget.Position;
                    InsecPOS = InsecST.LSExtend(InsecED, +InsecED.LSDistance(InsecST) + 230);
                    MovingPlayer(InsecPOS);
                    if (!_R.IsReady())
                        return;

                    if (getCheckBoxItem(miscMenu, "LeeSin_KickAndFlash") && InsecPOS.LSDistance(Player.Position) < 425
                        && GetTarget.LSDistance(Player.Position) < 375 && InsecType == "Wait" && _R.Level > 0 && _R.IsReady() &&
                        InsecType != "WF" && InsecType != "WF1" && Player.GetSpellSlot("SummonerFlash").IsReady())
                    {
                        InsecTime = TickCount(2000);
                        InsecType = "RF";
                        _R.Cast(GetTarget, true);
                        return;
                    }
                    if (InsecPOS.LSDistance(Player.Position) < 625 && _R.Level > 0 && _R.IsReady() && InsecType != "RF")
                    {
                        if (InsecType == "Wait" && InsecType != "WF" && InsecType != "WF1" && _W.IsReady())
                        {
                            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "blindmonkwtwo") return;
                            InsecTime = TickCount(2000);
                            InsecType = "WF";
                            var Ward = Items.GetWardSlot();
                            Player.Spellbook.CastSpell(Ward.SpellSlot, InsecPOS);
                        }
                        if (InsecType == "WF" && _W.IsReady())
                        {
                            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "blindmonkwtwo") return;
                            var WardObj = ObjectManager.Get<Obj_AI_Base>()  // 커서근처 와드 유무
                                    .OrderBy(obj => obj.LSDistance(InsecPOS))
                                    .FirstOrDefault(obj => obj.IsAlly && !obj.IsMe
                                    && obj.LSDistance(InsecPOS) <= 110 && obj.Name.ToLower().Contains("ward"));
                            if (WardObj != null)
                            {
                                InsecType = "WF1";
                                _W.Cast(WardObj, true);
                            }
                        }
                        if (InsecType == "WF1")
                        {
                            if (GetTarget.LSDistance(Player.Position) < 375)
                            {
                                _R.Cast(GetTarget, true);
                            }
                            else
                            {
                                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                            }
                        }
                        return;
                    }

                    // 플 425, 와드 625
                }
                if (getKeyBindItem(miscMenu, ("LeeSin_Ward_W")))   // 와드 방호
                {
                    //와드방호는 WW로 정의
                    var Cursor = Game.CursorPos;
                    var Ward = Items.GetWardSlot();
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Cursor);
                    //Console.WriteLine(ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name);
                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "blindmonkwtwo") return;
                    if (Player.LSDistance(Cursor) > 700) Cursor = Game.CursorPos.LSExtend(Player.Position, +Player.LSDistance(Game.CursorPos) - 700);
                    //Render.Circle.DrawCircle(Cursor, 50, Color.Black, 2);
                    //Drawing.DrawText(200, 200, Color.White, "WW is: " + WW.ToString());                    
                    if (_W.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "blindmonkwone")
                    {
                        var Object = ObjectManager.Get<AIHeroClient>().FirstOrDefault(obj => obj.IsAlly && !obj.IsMe && obj.LSDistance(Cursor) < 110); // 커서근처 챔프유무
                        var Minion = MinionManager.GetMinions(Cursor, 110, MinionTypes.All, MinionTeam.Ally); // 아군 미니언 유무
                        var WardObj = ObjectManager.Get<Obj_AI_Base>()  // 커서근처 와드 유무
                                .OrderBy(obj => obj.LSDistance(Cursor))
                                .FirstOrDefault(obj => obj.IsAlly && !obj.IsMe
                                && obj.LSDistance(Cursor) <= 110 && obj.Name.ToLower().Contains("ward"));
                        if (WardObj != null && WTime < Environment.TickCount)
                        {
                            _W.Cast(WardObj, true);
                            Ward_Time = TickCount(2000);
                            WW = true;
                            WTime = TickCount(2000);
                            return;
                        }
                        if (Object != null && WTime < Environment.TickCount)
                        {
                            _W.Cast(Object, true);
                            Ward_Time = TickCount(2000);
                            WW = true;
                            WTime = TickCount(2000);
                            return;
                        }
                        if (Minion != null && WTime < Environment.TickCount)
                        {
                            foreach (var minion in Minion)
                            {
                                if (minion != null)
                                {
                                    _W.Cast(minion, true);
                                    Ward_Time = TickCount(2000);
                                    WW = true;
                                    WTime = TickCount(2000);
                                    return;
                                }
                            }
                        }
                        if (Player.LSDistance(Cursor) > 625) Cursor = Game.CursorPos.LSExtend(Player.Position, +Player.LSDistance(Game.CursorPos) - 625);
                        //Render.Circle.DrawCircle(Cursor, 50, Color.Black, 2);                            
                        if (WW && Ward != null && Ward_Time < Environment.TickCount)
                        {
                            Player.Spellbook.CastSpell(Ward.SpellSlot, Cursor);
                            WW = false;
                            Ward_Time = TickCount(2000);
                        }
                        WardObj = ObjectManager.Get<Obj_AI_Base>()  // 커서근처 와드 유무
                                .OrderBy(obj => obj.LSDistance(Cursor))
                                .FirstOrDefault(obj => obj.IsAlly && !obj.IsMe
                                && obj.LSDistance(Cursor) <= 110 && obj.Name.ToLower().Contains("ward"));
                        if (WardObj != null && WTime < Environment.TickCount)
                        {
                            _W.Cast(WardObj, true);
                            Ward_Time = TickCount(2000);
                            WW = true;
                            WTime = TickCount(2000);
                            return;
                        }
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
                if (getSliderItem(miscMenu, "LeeSin_AntiGab") == 0 || !_R.IsReady()) return;
                var Anti = getSliderItem(miscMenu, "LeeSin_AntiGab");
                var Myhp = Player.HealthPercent;
                var Target = gapcloser.Sender;
                //Console.Write(Target.ChampionName);
                if (Target != null && Player.LSDistance(Target) < 375 && Myhp < Anti && _R.Level > 0 && _R.IsReady()) _R.Cast(Target, true);
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    //Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 07");
                    ErrorTime = TickCount(10000);
                }
            }
        }
        private static void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                foreach (var Me in HeroManager.Allies)
                {
                    if (Me.ChampionName == Player.ChampionName)
                    {
                        //if (MainMenu._MainMenu.Item("KickAndFlash").GetValue<KeyBind>().Active && args.SData.Name == "BlindMonkRKick")                        
                        //if (MainMenu._MainMenu.Item("KickAndFlash").GetValue<KeyBind>().Active && args.SData.Name == "summonerflash") RF = true;
                        if (getKeyBindItem(miscMenu, "LeeSin_InsecKick") && args.SData.Name == "BlindMonkRKick" && InsecType == "RF" && InsecType != "WF" && InsecType != "WF1")
                        {
                            var Flash = Player.GetSpellSlot("SummonerFlash");
                            Player.Spellbook.CastSpell(Flash, InsecPOS, true);
                        }
                        //if (MainMenu._MainMenu.Item("InsecKick").GetValue<KeyBind>().Active && args.SData.Name == "blindmonkwtwo")
                    }
                    if (sender.IsMe)
                    {
                        //Console.Write("Spell Name: " + args.SData.Name + "\n");
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
        }

        private static void GameObject_OnDelete(GameObject obj, EventArgs args)
        {
        }
        private static void AutoKick()
        {
            if (getSliderItem(miscMenu, "LeeSin_AutoKick") == 0 || getKeyBindItem(miscMenu, "LeeSin_InsecKick")) return;

            var target =
                HeroManager.Enemies.Where(x => x.LSDistance(Player) < 375 && !x.IsDead && x.LSIsValidTarget(375))
                    .OrderBy(x => x.LSDistance(Player)).FirstOrDefault();
            if (target == null) return;

            var ultPoly = new LeagueSharp.Common.Geometry.Polygon.Rectangle(Player.ServerPosition,
                Player.ServerPosition.LSExtend(target.Position, 1100),
                target.BoundingRadius + 10);

            var count =
                HeroManager.Enemies.Where(x => x.LSDistance(Player) < 1100 && x.LSIsValidTarget(1100))
                    .Count(h => h.NetworkId != target.NetworkId && ultPoly.IsInside(h.ServerPosition));

            if (count >= getSliderItem(miscMenu, "LeeSin_AutoKick") && _R.IsReady())
            {
                _R.Cast(target);
            }
        }
    }
}