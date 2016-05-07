using System;
using System.Drawing;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using FreshBooster;
using LeagueSharp.Common;
using Gapcloser = EloBuddy.SDK.Events.Gapcloser;
using Interrupter = EloBuddy.SDK.Events.Interrupter;
using Spell = LeagueSharp.Common.Spell;

namespace PortAIO.Champion.Blitzcrank
{
    internal class Program
    {
        public const string ChampName = "Blitzcrank"; // Edit
        // Default Setting
        public static int ErrorTime;
        public static AIHeroClient Player;
        public static Spell _Q, _W, _E, _R;
        private static SpellSlot FlashSlot;

        public static Menu _Menu, ComboMenu, HarassMenu, KSMenu, MiscMenu, DrawMenu;
        // Default Setting

        private static void SkillSet()
        {
            try
            {
                _Q = new Spell(SpellSlot.Q, 950f);
                _Q.SetSkillshot(0.25f, 70f, 1800f, true, SkillshotType.SkillshotLine);
                _W = new Spell(SpellSlot.W, 700f);
                _E = new Spell(SpellSlot.E, 150f);
                _R = new Spell(SpellSlot.R, 540f);
            }
            catch (Exception)
            {
                if (FreshCommon.NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 01");
                    ErrorTime = FreshCommon.TickCount(10000);
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

        public static int getQHitChance()
        {
            return ComboMenu["Blitzcrank_CUseQ_Hit"].Cast<Slider>().CurrentValue;
        }

        private static void flashq()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            var target = TargetSelector.GetTarget(_Q.Range + 425, DamageType.Magical);
            if (target == null || !_Q.IsReady())
                return;
            var x = target.Position.Extend(target, 450f).To3D();
            var pred = _Q.GetPrediction(target).CastPosition;
            Player.Spellbook.CastSpell(FlashSlot, x);
            _Q.Cast(pred);
        }

        private static void Menu()
        {
            try
            {
                _Menu = MainMenu.AddMenu("机器人", "Blitzcrank");

                ComboMenu = _Menu.AddSubMenu("连招", "Combo");
                ComboMenu.Add("Blitzcrank_CUse_Q", new CheckBox("使用 Q"));
                ComboMenu.Add("Blitzcrank_CUse_W", new CheckBox("使用 W"));
                ComboMenu.Add("Blitzcrank_CUse_E", new CheckBox("使用 E"));
                ComboMenu.Add("Blitzcrank_CUse_R", new CheckBox("使用 R"));
                ComboMenu.Add("Blitzcrank_CUse_FlashQ", new KeyBind("闪现 Q (不好用）", false, KeyBind.BindTypes.HoldActive, 'T'));
                ComboMenu.AddSeparator();
                ComboMenu.AddLabel("1 : 范围外");
                ComboMenu.AddLabel("2 : 不可能");
                ComboMenu.AddLabel("3 : 低");
                ComboMenu.AddLabel("4 : 中");
                ComboMenu.AddLabel("5 : 高");
                ComboMenu.AddLabel("6 : 非常高");
                ComboMenu.Add("Blitzcrank_CUseQ_Hit", new Slider("Q 命中率", 6, 1, 6));

                HarassMenu = _Menu.AddSubMenu("骚扰", "Harass");
                HarassMenu.Add("Blitzcrank_HUse_Q", new CheckBox("使用 Q"));
                HarassMenu.Add("Blitzcrank_HUse_W", new CheckBox("使用 W"));
                HarassMenu.Add("Blitzcrank_HUse_E", new CheckBox("使用 E"));
                HarassMenu.Add("Blitzcrank_AManarate", new Slider("蓝量 %", 20));

                KSMenu = _Menu.AddSubMenu("抢头", "KillSteal");
                KSMenu.Add("Blitzcran_KUse_Q", new CheckBox("使用 Q"));
                KSMenu.Add("Blitzcran_KUse_R", new CheckBox("使用 R"));

                MiscMenu = _Menu.AddSubMenu("杂项", "Misc");
                MiscMenu.AddGroupLabel("抓人设置");
                foreach (var enemy in ObjectManager.Get<AIHeroClient>())
                {
                    if (enemy.Team != Player.Team)
                    {
                        MiscMenu.Add("Blitzcrank_GrabSelect" + enemy.NetworkId,
                            new Slider("抓人模式 (0 : 开启 | 1 : 不抓 | 2 : 自动) " + enemy.ChampionName, 0, 0, 2));
                        MiscMenu.AddSeparator();
                    }
                }
                MiscMenu.AddSeparator();
                MiscMenu.AddGroupLabel("技能打断");
                MiscMenu.Add("Blitzcrank_InterQ", new CheckBox("使用 Q"));
                MiscMenu.Add("Blitzcrank_InterE", new CheckBox("使用 E"));
                MiscMenu.Add("Blitzcrank_InterR", new CheckBox("使用 R"));
                MiscMenu.AddSeparator();
                MiscMenu.Add("Blitzcrank_GrabDash", new CheckBox("抓冲刺的敌人"));

                DrawMenu = _Menu.AddSubMenu("线圈", "Draw");
                DrawMenu.Add("Blitzcrank_Draw_Q", new CheckBox("使用 Q", false));
                DrawMenu.Add("Blitzcrank_Draw_R", new CheckBox("使用 R", false));
                DrawMenu.Add("Blitzcrank_Indicator", new CheckBox("显示伤害指示器"));
            }
            catch (Exception)
            {
                if (FreshCommon.NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 02");
                    ErrorTime = FreshCommon.TickCount(10000);
                }
            }
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            try
            {
                if (Player.IsDead) return;
                if (getCheckBoxItem(DrawMenu, "Blitzcrank_Draw_Q"))
                    Render.Circle.DrawCircle(Player.Position, _Q.Range, Color.White, 1);
                if (getCheckBoxItem(DrawMenu, "Blitzcrank_Draw_R"))
                    Render.Circle.DrawCircle(Player.Position, _R.Range, Color.White, 1);
                var QTarget = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);
                if (QTarget != null)
                    Drawing.DrawCircle(QTarget.Position, 150, Color.Green);
            }
            catch (Exception)
            {
                if (FreshCommon.NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 03");
                    ErrorTime = FreshCommon.TickCount(10000);
                }
            }
        }

        // OnLoad
        public static void OnLoad()
        {
            Player = ObjectManager.Player;
            SkillSet();
            FlashSlot = Player.GetSpellSlot("SummonerFlash");
            Menu();
            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Gapcloser.OnGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            Interrupter.OnInterruptableSpell += Interrupter2_OnInterruptableTarget;
            Orbwalker.OnPreAttack += Orbwalking_BeforeAttack;
            EloBuddy.Player.OnIssueOrder += Obj_AI_Base_OnIssueOrder;

            Chat.Print("Flash Q is pretty bad, keep that in mind when using it.");
        }

        private static void OnGameUpdate(EventArgs args)
        {
            try
            {
                if (Player.IsDead) return;
                var QTarget = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);
                var WTarget = TargetSelector.GetTarget(1500, DamageType.Physical);
                var RTarget = TargetSelector.GetTarget(_R.Range, DamageType.Magical);

                if (getCheckBoxItem(MiscMenu, "Blitzcrank_GrabDash") && _Q.IsReady())
                    if (QTarget != null && _Q.GetPrediction(QTarget).Hitchance == HitChance.Dashing)
                        _Q.CastIfHitchanceEquals(QTarget, HitChance.Dashing, true);

                //killsteal
                if (getCheckBoxItem(KSMenu, "Blitzcran_KUse_Q") && QTarget != null &&
                    QTarget.Health < _Q.GetDamage(QTarget) && _Q.IsReady())
                {
                    _Q.CastIfHitchanceEquals(QTarget, HitChance.VeryHigh, true);
                    return;
                }
                if (getCheckBoxItem(KSMenu, "Blitzcran_KUse_R") && RTarget != null &&
                    RTarget.Health < _E.GetDamage(RTarget) && _R.IsReady())
                {
                    _R.Cast(true);
                    return;
                }

                if (QTarget == null) return; // auto grab
                foreach (var enemy in ObjectManager.Get<AIHeroClient>())
                {
                    if (enemy.Team != Player.Team && QTarget != null &&
                        getSliderItem(MiscMenu, "Blitzcrank_GrabSelect" + enemy.NetworkId) == 2 && _Q.IsReady() &&
                        QTarget.ChampionName == enemy.ChampionName)
                    {
                        if (QTarget.CanMove && QTarget.Distance(Player.Position) < _Q.Range*0.9)
                            _Q.CastIfHitchanceEquals(QTarget, FreshCommon.Hitchance("Blitzcrank_CUseQ_Hit"), true);
                        if (!QTarget.CanMove)
                            _Q.CastIfHitchanceEquals(QTarget, FreshCommon.Hitchance("Blitzcrank_CUseQ_Hit"), true);
                    }
                }

				if (getKeyBindItem(ComboMenu, "Blitzcrank_CUse_FlashQ"))
				{
					flashq();
				}

                // Combo
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    if (getCheckBoxItem(ComboMenu, "Blitzcrank_CUse_Q") && _Q.IsReady() && QTarget != null &&
                        getSliderItem(MiscMenu, "Blitzcrank_GrabSelect" + QTarget.NetworkId) != 1)
                    {
                        _Q.CastIfHitchanceEquals(QTarget, FreshCommon.Hitchance("Blitzcrank_CUseQ_Hit"), true);
                    }
                    if (getCheckBoxItem(ComboMenu, "Blitzcrank_CUse_W") && _W.IsReady() && WTarget != null)
                        _W.Cast(Player, true);
                    if (getCheckBoxItem(ComboMenu, "Blitzcrank_CUse_E") && _E.IsReady() &&
                        QTarget.Distance(Player.ServerPosition) < 230)
                        _E.Cast(Player);
                    if (getCheckBoxItem(ComboMenu, "Blitzcrank_CUse_R") && _R.IsReady() && RTarget != null)
                        _R.Cast();
                }

                // Harass
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) &&
                    getSliderItem(HarassMenu, "Blitzcrank_AManarate") < Player.ManaPercent)
                {
                    if (getCheckBoxItem(HarassMenu, "Blitzcrank_HUse_Q") && _Q.IsReady() && QTarget != null &&
                        getSliderItem(MiscMenu, "Blitzcrank_GrabSelect" + QTarget.NetworkId) != 1)
                    {
                        _Q.CastIfHitchanceEquals(QTarget, FreshCommon.Hitchance("Blitzcrank_CUseQ_Hit"), true);
                    }
                    if (getCheckBoxItem(HarassMenu, "Blitzcrank_HUse_W") && _W.IsReady() && WTarget != null)
                        _W.Cast(Player, true);
                    if (getCheckBoxItem(HarassMenu, "Blitzcrank_HUse_E") && _E.IsReady() &&
                        QTarget.Distance(Player.ServerPosition) < 230)
                        _E.Cast(Player);
                }
            }
            catch (Exception)
            {
                if (FreshCommon.NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 06");
                    ErrorTime = FreshCommon.TickCount(10000);
                }
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            try
            {
            }
            catch (Exception)
            {
                if (FreshCommon.NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 07");
                    ErrorTime = FreshCommon.TickCount(10000);
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
                if (FreshCommon.NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 08");
                    ErrorTime = FreshCommon.TickCount(10000);
                }
            }
        }

        private static void Interrupter2_OnInterruptableTarget(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs e)
        {
            try
            {
                if (Player.IsDead)
                    return;
                if (!sender.IsEnemy || !sender.IsValid<AIHeroClient>())
                    return;

                if (getCheckBoxItem(MiscMenu, "Blitzcrank_InterQ") && _Q.IsReady())
                {
                    if (sender.Distance(Player.ServerPosition, true) <= _Q.RangeSqr)
                        _Q.Cast(sender);
                }
                if (getCheckBoxItem(MiscMenu, "Blitzcrank_InterR") && _R.IsReady())
                {
                    if (sender.Distance(Player.ServerPosition, true) <= _R.RangeSqr)
                        _R.Cast();
                }
                if (getCheckBoxItem(MiscMenu, "Blitzcrank_InterE") && _E.IsReady())
                {
                    if (sender.Distance(Player.ServerPosition, true) <= _E.RangeSqr)
                        _E.CastOnUnit(Player);
                }
            }
            catch (Exception)
            {
                if (FreshCommon.NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 09");
                    ErrorTime = FreshCommon.TickCount(10000);
                }
            }
        }

        private static void Orbwalking_BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            try
            {
            }
            catch (Exception)
            {
                if (FreshCommon.NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 10");
                    ErrorTime = FreshCommon.TickCount(10000);
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
                if (FreshCommon.NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 11");
                    ErrorTime = FreshCommon.TickCount(10000);
                }
            }
        }
    }
}