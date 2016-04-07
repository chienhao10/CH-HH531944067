using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace Ryze
{
    class Program
    {
        public static Spell.Skillshot Q;
        public static Spell.Skillshot QC;
        public static Spell.Targeted E;
        public static Spell.Targeted W;
        public static Spell.Active R;
        public static string Author = "Doctor";
        public static string AddonName = "CH汉化-Dr瑞兹";
        public static Menu menu;
        public static string[] interrupt;
        public static string[] notarget;
        public static string[] gapcloser;
        public static Dictionary<string, Menu> SubMenu = new Dictionary<string, Menu>() { };
        public static AIHeroClient myHero { get { return ObjectManager.Player; } }
        public static float ManaPercent { get { return myHero.Mana / myHero.MaxMana * 100; } }
        public static float HealthPercent { get { return myHero.Health / myHero.MaxHealth * 100; } }

        static void Main(string[] args)
        {
            if (Hacks.RenderWatermark)
                Hacks.RenderWatermark = false;

            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        public static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (myHero.Hero != Champion.Ryze) { return; }
            Chat.Print("<font color=\"#F20000\"><b>Doctor Ryze:</b></font> Loaded!");

            Q = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Linear, 250, 1700, 100);
            Q.AllowedCollisionCount = 0;
            QC = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Linear, 250, 1700, 100);
            QC.AllowedCollisionCount = int.MaxValue;
            W = new Spell.Targeted(SpellSlot.W, 600);
            E = new Spell.Targeted(SpellSlot.E, 600);
            R = new Spell.Active(SpellSlot.R);

            menu = MainMenu.AddMenu(AddonName, AddonName + " by " + Author + " v1.0");
            menu.AddLabel(AddonName + " made by " + Author);
            menu.AddLabel("CH汉化-瑞兹!");
            menu.AddLabel("谢谢使用!");

            SubMenu["Combo"] = menu.AddSubMenu("连招", "Combo");
            SubMenu["Combo"].AddLabel("WQER 1层被动连招使用R，2层被动连招使用R，3层被动连招使用R，或者3层被动不R");
            SubMenu["Combo"].AddSeparator();
            SubMenu["Combo"].AddLabel("QEWR 2层被动使用连招（R无冷却）");
            SubMenu["Combo"].AddSeparator();
            SubMenu["Combo"].AddLabel("QEW 2层被动连招不使用R");
            SubMenu["Combo"].AddSeparator();
            SubMenu["Combo"].AddLabel("QEQ 3层被动配合R（R无冷却）(等级6 防守型逻辑) ");
            SubMenu["Combo"].AddSeparator();
            SubMenu["Combo"].AddLabel("WQRE 3层被动配合R (双W逻辑)");
            SubMenu["Combo"].AddSeparator();

            SubMenu["Combo"].Add("Q", new CheckBox("使用 Q", true));
            SubMenu["Combo"].Add("W", new CheckBox("使用 W", true));
            SubMenu["Combo"].Add("E", new CheckBox("使用 E", true));
            SubMenu["Combo"].Add("R", new CheckBox("使用 R", true));
            var mode = SubMenu["Combo"].Add("Mode", new Slider("连招模式", 0, 0, 4));
            var modeDisplay = new[] { "WQER 1-2-3 层数", "QEWR 2 层数", "QEW 2 层数(无R)", "QEQ 3 层数", "WQRE 3 层数" };
            mode.DisplayName = modeDisplay[mode.CurrentValue];

            mode.OnValueChange +=
                delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
                {
                    sender.DisplayName = modeDisplay[changeArgs.NewValue];
                };
            var switcher = SubMenu["Combo"].Add("Switcher", new KeyBind("连招逻辑切换", false, KeyBind.BindTypes.HoldActive, (uint)'K'));
            switcher.OnValueChange += delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs e)
            {
                if (e.NewValue == true)
                {
                    var cast = SubMenu["Combo"]["Mode"].Cast<Slider>();
                    if (cast.CurrentValue == cast.MaxValue)
                    {
                        cast.CurrentValue = 0;
                    }
                    else
                    {
                        cast.CurrentValue++;
                    }
                }
            };

            SubMenu["Harass"] = menu.AddSubMenu("骚扰", "Harass");
            SubMenu["Harass"].Add("Q", new CheckBox("使用 Q", true));
            SubMenu["Harass"].Add("W", new CheckBox("使用 W", true));
            SubMenu["Harass"].Add("E", new CheckBox("使用 E", true));
            SubMenu["Harass"].Add("HarassMana", new Slider("蓝量%", 50, 0, 100));

            SubMenu["LastHit"] = menu.AddSubMenu("尾兵", "LastHit");
            SubMenu["LastHit"].Add("Q", new CheckBox("使用 Q", true));
            SubMenu["LastHit"].Add("W", new CheckBox("使用 W", true));
            SubMenu["LastHit"].Add("E", new CheckBox("使用 E", true));
            SubMenu["LastHit"].Add("LastHitMana", new Slider("蓝量%", 50, 0, 100));

            SubMenu["LaneClear"] = menu.AddSubMenu("清线", "LaneClear");
            SubMenu["LaneClear"].Add("LC", new KeyBind("清线按键", false, KeyBind.BindTypes.HoldActive, 'V'));
            SubMenu["LaneClear"].Add("Q", new CheckBox("使用 Q", true));
            SubMenu["LaneClear"].Add("W", new CheckBox("使用 W", true));
            SubMenu["LaneClear"].Add("E", new CheckBox("使用 E", true));
            SubMenu["LaneClear"].Add("R", new CheckBox("使用 R", true));
            SubMenu["LaneClear"].Add("LaneClearMana", new Slider("蓝量%", 50, 0, 100));

            SubMenu["JungleClear"] = menu.AddSubMenu("清野", "JungleClear");
            SubMenu["JungleClear"].Add("JC", new KeyBind("清野按键", false, KeyBind.BindTypes.HoldActive, 'V'));
            SubMenu["JungleClear"].Add("Q", new CheckBox("使用 Q", true));
            SubMenu["JungleClear"].Add("W", new CheckBox("使用 W", true));
            SubMenu["JungleClear"].Add("E", new CheckBox("使用 E", true));
            SubMenu["JungleClear"].Add("R", new CheckBox("使用 R", true));
            SubMenu["JungleClear"].Add("JungleClearMana", new Slider("蓝量%", 50, 0, 100));

            SubMenu["Misc"] = menu.AddSubMenu("杂项", "Misc");


            SubMenu["Misc"].AddSeparator(10);
            SubMenu["Misc"].AddGroupLabel("抢头设置");
            SubMenu["Misc"].Add("KsQ", new CheckBox("使用 Q", true));
            SubMenu["Misc"].Add("KsW", new CheckBox("使用 W", true));
            SubMenu["Misc"].Add("KsE", new CheckBox("使用 E", true));

            SubMenu["Misc"].AddSeparator(10);
            SubMenu["Misc"].AddGroupLabel("技能打断/防突进");
            SubMenu["Misc"].Add("Interrupt", new CheckBox("使用W技能打断/防突进", true));

            SubMenu["Misc"].AddSeparator(10);
            SubMenu["Misc"].AddGroupLabel("大天使设置");
            SubMenu["Misc"].Add("seraph", new CheckBox("使用大天使护盾", true));
            SubMenu["Misc"].Add("seraphHp", new Slider("我血量低于%时", 50, 0, 101));
            SubMenu["Misc"].Add("seraphCount", new Slider("范围敌人数量大于等于 X", 2, 0, 5));

            SubMenu["Misc"].AddSeparator(10);
            SubMenu["Misc"].AddGroupLabel("中亚设置");
            SubMenu["Misc"].Add("zhonyas", new CheckBox("使用中亚", true));
            SubMenu["Misc"].Add("zhonyasHp", new Slider("我血量低于%时", 50, 0, 101));
            SubMenu["Misc"].Add("zhonyasCount", new Slider("范围敌人数量大于等于 X", 2, 0, 5));

            SubMenu["Misc"].AddSeparator(10);
            SubMenu["Misc"].AddGroupLabel("皮肤设置");
            var skin = SubMenu["Misc"].Add("sID", new Slider("皮肤", 8, 0, 8));
            var sID = new[] { "Skin#1", "Skin#2", "Skin#3", "Skin#4", "Skin#5", "Skin#6", "Skin#7", "Skin#8", "Skin#9", };
            skin.DisplayName = sID[skin.CurrentValue];

            skin.OnValueChange +=
                delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
                {
                    sender.DisplayName = sID[changeArgs.NewValue];
                };

            SubMenu["Misc"].AddSeparator(10);
            SubMenu["Misc"].AddGroupLabel("线圈设置");
            SubMenu["Misc"].Add("DrawQ", new CheckBox("显示 Q", true));
            SubMenu["Misc"].Add("DrawW", new CheckBox("显示 W", true));
            SubMenu["Misc"].Add("DrawE", new CheckBox("显示 E", true));

            SubMenu["gap"] = menu.AddSubMenu("防突进列表", "gap");
            SubMenu["gap2"] = menu.AddSubMenu("防突进列表 2", "gap2");
            SubMenu["int"] = menu.AddSubMenu("技能打断列表", "int");

            gapcloser = new[]
            {
                "阿卡丽R", "牛头W", "皎月R", "刀妹Q", "贾科斯Q", "杰斯突击",
                "茂凯W", "猴子E", "潘森W", "波比E", "奎恩E",
                "赵信E", "blindmonkqtwo", "小鱼人Q", "狮子狗跳"
            };
            notarget = new[]
            {
                "剑魔Q", "男抢E", "男抢2", "人马R", "皇子Q", "皇子R", "螳螂E",
                "螳螂2", "妖姬W", "妖姬W2", "日女E", "石头人R", "鳄鱼E",
                "猪女Q", "慎E", "小炮W", "slashCast"
            };
            interrupt = new[]
            {
                "卡特R", "加里奥R", "稻草人R", "稻草人W", "努努R", "慎R", "螃蟹R",
                "蚱蜢R", "J8脸R", "潘森R", "韦鲁斯Q", "女警R",
                "赏金R", "狼人R", "卢锡安R"
            };
            for (int i = 0; i < gapcloser.Length; i++)
            {
                SubMenu["gap"].Add(gapcloser[i], new CheckBox(gapcloser[i], true));
            }
            for (int i = 0; i < notarget.Length; i++)
            {
                SubMenu["gap2"].Add(notarget[i], new CheckBox(notarget[i], true));
            }
            for (int i = 0; i < interrupt.Length; i++)
            {
                SubMenu["int"].Add(interrupt[i], new CheckBox(interrupt[i], true));
            }

            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Game.OnTick += Game_OnTick;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (SubMenu["Misc"]["Interrupt"].Cast<CheckBox>().CurrentValue && interrupt.Any(x => x.Contains(args.SData.Name)) &&
               SubMenu["int"][args.SData.Name].Cast<CheckBox>().CurrentValue && myHero.Distance(sender) <= 550)
            {
                W.Cast(sender);
            }

            if (gapcloser.Any(str => str.Contains(args.SData.Name)) && SubMenu["gap"][args.SData.Name].Cast<CheckBox>().CurrentValue && args.Target.IsMe)
            {
                W.Cast(sender);
            }

            if (notarget.Any(str => str.Contains(args.SData.Name)) &&
                Vector3.Distance(args.End, ObjectManager.Player.Position) <= 300 && sender.IsValidTarget(550f) &&
                SubMenu["gap2"][args.SData.Name].Cast<CheckBox>().CurrentValue)
            {
                if (ObjectManager.Player.Distance(args.End) < ObjectManager.Player.Distance(sender.Position))
                {
                    W.Cast(sender);
                }
            }

            if (sender is AIHeroClient)
            {
                var pant = (AIHeroClient)sender;
                if (pant.IsValidTarget() && pant.ChampionName == "Pantheon" && pant.GetSpellSlotFromName(args.SData.Name) == SpellSlot.W)
                {
                    if (SubMenu["Misc"]["Interrupt"].Cast<CheckBox>().CurrentValue && args.Target.IsMe)
                    {
                        if (pant.IsValidTarget(E.Range))
                        {
                            W.Cast(pant);
                        }
                    }
                }
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            KillSteal();
            sChoose();

            if (SubMenu["LaneClear"]["LC"].Cast<KeyBind>().CurrentValue && ManaPercent > SubMenu["LaneClear"]["LaneClearMana"].Cast<Slider>().CurrentValue)
            {
                Farm.LaneClear();
            }
            if (SubMenu["JungleClear"]["JC"].Cast<KeyBind>().CurrentValue && ManaPercent > SubMenu["JungleClear"]["JungleClearMana"].Cast<Slider>().CurrentValue)
            {
                Farm.JungleClear();
            }

            switch (Orbwalker.ActiveModesFlags)
            {
                case Orbwalker.ActiveModes.Harass:
                    Harass();
                    break;
                case Orbwalker.ActiveModes.LastHit:
                    Farm.LastHit();
                    break;
                case Orbwalker.ActiveModes.Combo:
                    ComboMode();
                    Zhonyas();
                    Seraph();
                    break;
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (myHero.IsDead) { return; }
            if (SubMenu["Misc"]["DrawQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                Circle.Draw(Color.Green, Q.Range, myHero.Position);
            }
            if (SubMenu["Misc"]["DrawW"].Cast<CheckBox>().CurrentValue && W.IsReady())
            {
                Circle.Draw(Color.Green, W.Range, myHero.Position);
            }
            if (SubMenu["Misc"]["DrawE"].Cast<CheckBox>().CurrentValue && E.IsReady())
            {
                Circle.Draw(Color.Green, E.Range, myHero.Position);
            }
            if (!myHero.IsDead)
            {
                var DisplayName = SubMenu["Combo"]["Mode"].DisplayName;
                var heroPos = Drawing.WorldToScreen(myHero.Position);
                var dimension = Drawing.GetTextEntent(DisplayName, 20);
                Drawing.DrawText(heroPos.X - dimension.Width / 2, heroPos.Y, System.Drawing.Color.LightYellow, DisplayName);
            }
        }

        public static void QCast()
        {
            var TsTarget = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            PredictionResult QPred = Q.GetPrediction(TsTarget);
            {
                Q.Cast(QPred.CastPosition);
            }
        }

        public static void ComboWQER()
        {
            var useQ = SubMenu["Combo"]["Q"].Cast<CheckBox>().CurrentValue;
            var useW = SubMenu["Combo"]["W"].Cast<CheckBox>().CurrentValue;
            var useE = SubMenu["Combo"]["E"].Cast<CheckBox>().CurrentValue;
            var useR = SubMenu["Combo"]["R"].Cast<CheckBox>().CurrentValue;

            var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            Orbwalker.ForcedTarget = target;
            if (target != null && target.IsValidTarget())
            {
                if (useW && W.IsReady())
                    W.Cast(target);
                else if (!W.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useR && R.IsReady() && myHero.GetBuffCount("ryzepassivestack") == 4 || myHero.GetBuffCount("ryzepassivestack") == 2 || myHero.GetBuffCount("ryzepassivestack") == 3)
                    R.Cast();
                else if (!R.IsReady() && useW && W.IsReady())
                    W.Cast(target);
                else if (!W.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useW && W.IsReady())
                    W.Cast(target);
                else if (!W.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useW && W.IsReady())
                    W.Cast(target);
                else if (!W.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useW && W.IsReady())
                    W.Cast(target);
                else if (!W.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useW && W.IsReady())
                    W.Cast(target);
                else if (!W.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useW && W.IsReady())
                    W.Cast(target);
                else if (!W.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useW && W.IsReady())
                    W.Cast(target);
            }
        }

        public static void ComboQEWR()
        {
            var useQ = SubMenu["Combo"]["Q"].Cast<CheckBox>().CurrentValue;
            var useW = SubMenu["Combo"]["W"].Cast<CheckBox>().CurrentValue;
            var useE = SubMenu["Combo"]["E"].Cast<CheckBox>().CurrentValue;
            var useR = SubMenu["Combo"]["R"].Cast<CheckBox>().CurrentValue;

            var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            Orbwalker.ForcedTarget = target;
            if (target != null && target.IsValidTarget())
            {
                if (useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useW && W.IsReady())
                    W.Cast(target);
                else if (!E.IsReady() && useR && R.IsReady())
                    R.Cast();
                if (!R.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useW && W.IsReady())
                    W.Cast(target);
                else if (!W.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useW && W.IsReady())
                    W.Cast(target);
                else if (!W.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useW && W.IsReady())
                    W.Cast(target);
                else if (!W.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useW && W.IsReady())
                    W.Cast(target);
                else if (!W.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useW && W.IsReady())
                    W.Cast(target);
            }
        }

        public static void ComboQEW()
        {
            var useQ = SubMenu["Combo"]["Q"].Cast<CheckBox>().CurrentValue;
            var useW = SubMenu["Combo"]["W"].Cast<CheckBox>().CurrentValue;
            var useE = SubMenu["Combo"]["E"].Cast<CheckBox>().CurrentValue;
            var useR = SubMenu["Combo"]["R"].Cast<CheckBox>().CurrentValue;

            var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            Orbwalker.ForcedTarget = target;
            if (target != null && target.IsValidTarget())
            {
                if (useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useW && W.IsReady())
                    W.Cast(target);
                if (!W.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useW && W.IsReady())
                    W.Cast(target);
                else if (!W.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useW && W.IsReady())
                    W.Cast(target);
                else if (!W.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useW && W.IsReady())
                    W.Cast(target);
                else if (!W.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useW && W.IsReady())
                    W.Cast(target);
            }
        }

        public static void ComboQEQ()
        {
            var useQ = SubMenu["Combo"]["Q"].Cast<CheckBox>().CurrentValue;
            var useW = SubMenu["Combo"]["W"].Cast<CheckBox>().CurrentValue;
            var useE = SubMenu["Combo"]["E"].Cast<CheckBox>().CurrentValue;
            var useR = SubMenu["Combo"]["R"].Cast<CheckBox>().CurrentValue;

            var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            Orbwalker.ForcedTarget = target;
            if (target != null && target.IsValidTarget())
            {
                if (useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useW && W.IsReady())
                    W.Cast(target);
                else if (!W.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useR && R.IsReady())
                    R.Cast();
                else if (!R.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useW && W.IsReady())
                    W.Cast(target);
                else if (!W.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useW && W.IsReady())
                    W.Cast(target);
                else if (!W.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useW && W.IsReady())
                    W.Cast(target);
            }
        }

        public static void ComboWQRE()
        {
            var useQ = SubMenu["Combo"]["Q"].Cast<CheckBox>().CurrentValue;
            var useW = SubMenu["Combo"]["W"].Cast<CheckBox>().CurrentValue;
            var useE = SubMenu["Combo"]["E"].Cast<CheckBox>().CurrentValue;
            var useR = SubMenu["Combo"]["R"].Cast<CheckBox>().CurrentValue;

            var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            Orbwalker.ForcedTarget = target;
            if (target != null && target.IsValidTarget())
            {
                if (W.IsReady() && useW)
                    W.Cast(target);
                else if (!W.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useR && R.IsReady())
                    R.Cast();
                else if (!R.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useW && W.IsReady())
                    W.Cast(target);
                else if (!W.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useW && W.IsReady())
                    W.Cast(target);
                else if (!W.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useW && W.IsReady())
                    W.Cast(target);
                else if (!W.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
            }
        }

        public static void Flee()
        {

        }

        public static void Harass()
        {
            var useQ = SubMenu["Harass"]["Q"].Cast<CheckBox>().CurrentValue;
            var useW = SubMenu["Harass"]["W"].Cast<CheckBox>().CurrentValue;
            var useE = SubMenu["Harass"]["E"].Cast<CheckBox>().CurrentValue;

            var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            Orbwalker.ForcedTarget = target;
            if (target != null && target.IsValidTarget() && ManaPercent > SubMenu["Harass"]["HarassMana"].Cast<Slider>().CurrentValue)
            {
                if (useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useW && W.IsReady())
                    W.Cast(target);
                if (!W.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useW && W.IsReady())
                    W.Cast(target);
                else if (!W.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useW && W.IsReady())
                    W.Cast(target);
                else if (!W.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useW && W.IsReady())
                    W.Cast(target);
                else if (!W.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useE && E.IsReady())
                    E.Cast(target);
                else if (!E.IsReady() && useQ && Q.IsReady())
                    QCast();
                else if (!Q.IsReady() && useW && W.IsReady())
                    W.Cast(target);
            }
        }

        public static void KillSteal()
        {
            foreach (AIHeroClient enemy in EntityManager.Heroes.Enemies)
            {
                if (enemy.IsValidTarget(Q.Range))
                {
                    if (Q.IsReady() && SubMenu["Misc"]["KsQ"].Cast<CheckBox>().CurrentValue && (myHero.GetSpellDamage(enemy, SpellSlot.Q) >= enemy.Health))
                    {
                        QC.Cast(enemy);
                    }
                    if (W.IsReady() && SubMenu["Misc"]["KsW"].Cast<CheckBox>().CurrentValue && (myHero.GetSpellDamage(enemy, SpellSlot.W) >= enemy.Health))
                    {
                        W.Cast(enemy);
                    }
                    if (E.IsReady() && SubMenu["Misc"]["KsE"].Cast<CheckBox>().CurrentValue && (myHero.GetSpellDamage(enemy, SpellSlot.E) >= enemy.Health))
                    {
                        E.Cast(enemy);
                    }
                }
            }
        }

        private static void Seraph()
        {
            if (!myHero.IsDead && SubMenu["Misc"]["seraph"].Cast<CheckBox>().CurrentValue)
            {
                if (Item.HasItem((int)ItemId.Archangels_Staff, myHero) && Item.CanUseItem((int)ItemId.Archangels_Staff)
                   && HealthPercent <= SubMenu["Misc"]["seraphHp"].Cast<Slider>().CurrentValue && myHero.CountEnemiesInRange(Q.Range) >= SubMenu["Misc"]["seraphCount"].Cast<Slider>().CurrentValue)
                {
                    Item.UseItem((int)ItemId.Archangels_Staff);
                }
            }
        }

        private static void Zhonyas()
        {
            if (!myHero.IsDead && SubMenu["Misc"]["zhonyas"].Cast<CheckBox>().CurrentValue)
            {
                if (Item.HasItem((int)ItemId.Zhonyas_Hourglass, myHero) && Item.CanUseItem((int)ItemId.Zhonyas_Hourglass)
                   && HealthPercent <= SubMenu["Misc"]["zhonyasHp"].Cast<Slider>().CurrentValue && myHero.CountEnemiesInRange(Q.Range) >= SubMenu["Misc"]["zhonyasCount"].Cast<Slider>().CurrentValue)
                {
                    Item.UseItem((int)ItemId.Zhonyas_Hourglass);
                }
            }
        }

        private static void sChoose()
        {
            var style = SubMenu["Misc"]["sID"].DisplayName;


            switch (style)
            {
                case "Skin#1":
                    Player.SetSkinId(0);
                    break;
                case "Skin#2":
                    Player.SetSkinId(1);
                    break;
                case "Skin#3":
                    Player.SetSkinId(2);
                    break;
                case "Skin#4":
                    Player.SetSkinId(3);
                    break;
                case "Skin#5":
                    Player.SetSkinId(4);
                    break;
                case "Skin#6":
                    Player.SetSkinId(5);
                    break;
                case "Skin#7":
                    Player.SetSkinId(6);
                    break;
                case "Skin#8":
                    Player.SetSkinId(7);
                    break;
                case "Skin#9":
                    Player.SetSkinId(8);
                    break;
            }
        }
        
        private static void ComboMode()
        {
            var Mode = SubMenu["Combo"]["Mode"].DisplayName;

            switch (Mode)
            {
                case "WQER 1-2-3层逻辑":
                    ComboWQER();
                    break;
                case "QEWR 2层逻辑":
                    ComboQEWR();
                    break;
                case "QEW 2层(无R)":
                    ComboQEW();
                    break;
                case "QEQ 3层逻辑":
                    ComboQEQ();
                    break;
                case "WQRE 3层逻辑":
                    ComboWQRE();
                    break;
            }
        }
    }
}
