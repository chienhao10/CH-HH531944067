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

        private static void Menu()
        {
            try
            {
                _Menu = MainMenu.AddMenu("Blitzcrank", "Blitzcrank");

                ComboMenu = _Menu.AddSubMenu("Combo", "Combo");
                ComboMenu.Add("Blitzcrank_CUse_Q", new CheckBox("Use Q"));
                ComboMenu.Add("Blitzcrank_CUse_W", new CheckBox("Use W"));
                ComboMenu.Add("Blitzcrank_CUse_E", new CheckBox("Use E"));
                ComboMenu.Add("Blitzcrank_CUse_R", new CheckBox("Use R"));
                ComboMenu.AddSeparator();
                ComboMenu.AddLabel("1 : Out of Range");
                ComboMenu.AddLabel("2 : Impossible");
                ComboMenu.AddLabel("3 : Low");
                ComboMenu.AddLabel("4 : Medium");
                ComboMenu.AddLabel("5 : High");
                ComboMenu.AddLabel("6 : Very High");
                ComboMenu.Add("Blitzcrank_CUseQ_Hit", new Slider("Q HitChance", 6, 1, 6));

                HarassMenu = _Menu.AddSubMenu("Harass", "Harass");
                HarassMenu.Add("Blitzcrank_HUse_Q", new CheckBox("Use Q"));
                HarassMenu.Add("Blitzcrank_HUse_W", new CheckBox("Use W"));
                HarassMenu.Add("Blitzcrank_HUse_E", new CheckBox("Use E"));
                HarassMenu.Add("Blitzcrank_AManarate", new Slider("Mana %", 20));

                KSMenu = _Menu.AddSubMenu("KillSteal", "KillSteal");
                KSMenu.Add("Blitzcran_KUse_Q", new CheckBox("Use Q"));
                KSMenu.Add("Blitzcran_KUse_R", new CheckBox("Use R"));

                MiscMenu = _Menu.AddSubMenu("Misc", "Misc");
                MiscMenu.AddGroupLabel("Grab Settings");
                foreach (var enemy in ObjectManager.Get<AIHeroClient>())
                {
                    if (enemy.Team != Player.Team)
                    {
                        MiscMenu.Add("Blitzcrank_GrabSelect" + enemy.ChampionName,
                            new Slider("Grab Mode (0 : Enable | 1 : Don't | 2 : Auto) " + enemy.ChampionName, 0, 0, 2));
                        MiscMenu.AddSeparator();
                    }
                }
                MiscMenu.AddSeparator();
                MiscMenu.AddGroupLabel("Interrupt Settings");
                MiscMenu.Add("Blitzcrank_InterQ", new CheckBox("Use Q"));
                MiscMenu.Add("Blitzcrank_InterE", new CheckBox("Use E"));
                MiscMenu.Add("Blitzcrank_InterR", new CheckBox("Use R"));
                MiscMenu.AddSeparator();
                MiscMenu.Add("Blitzcrank_GrabDash", new CheckBox("Grab to dashing enemy"));

                DrawMenu = _Menu.AddSubMenu("Draw", "Draw");
                DrawMenu.Add("Blitzcrank_Draw_Q", new CheckBox("Use Q", false));
                DrawMenu.Add("Blitzcrank_Draw_R", new CheckBox("Use R", false));
                DrawMenu.Add("Blitzcrank_Indicator", new CheckBox("Use Damage Indicator"));
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
            Menu();
            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Gapcloser.OnGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            Interrupter.OnInterruptableSpell += Interrupter2_OnInterruptableTarget;
            Orbwalker.OnPreAttack += Orbwalking_BeforeAttack;
            EloBuddy.Player.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
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
                        getSliderItem(MiscMenu, "Blitzcrank_GrabSelect" + enemy.ChampionName) == 2 && _Q.IsReady() &&
                        QTarget.ChampionName == enemy.ChampionName)
                    {
                        if (QTarget.CanMove && QTarget.Distance(Player.Position) < _Q.Range*0.9)
                            _Q.CastIfHitchanceEquals(QTarget, FreshCommon.Hitchance("Blitzcrank_CUseQ_Hit"), true);
                        if (!QTarget.CanMove)
                            _Q.CastIfHitchanceEquals(QTarget, FreshCommon.Hitchance("Blitzcrank_CUseQ_Hit"), true);
                    }
                }

                // Combo
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    if (getCheckBoxItem(ComboMenu, "Blitzcrank_CUse_Q") && _Q.IsReady() && QTarget != null &&
                        getSliderItem(MiscMenu, "Blitzcrank_GrabSelect" + QTarget.ChampionName) != 1)
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
                        getSliderItem(MiscMenu, "Blitzcrank_GrabSelect" + QTarget.ChampionName) != 1)
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