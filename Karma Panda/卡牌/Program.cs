namespace TwistedBuddy
{
    using System;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using SharpDX;

    internal class Program
    {
        /// <summary>
        /// Q
        /// </summary>
        public static Spell.Skillshot Q;

        /// <summary>
        /// W
        /// </summary>
        public static Spell.Active W;

        /// <summary>
        /// E
        /// </summary>
        public static Spell.Active E;

        /// <summary>
        /// R
        /// </summary>
        public static Spell.Active R;

        /// <summary>
        /// Twisted Fate's Name
        /// </summary>
        public const string ChampionName = "TwistedFate";

        /// <summary>
        /// Called when program starts
        /// </summary>
        private static void Main()
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        /// <summary>
        /// Called when the game finishes loading.
        /// </summary>
        /// <param name="args">The Args.</param>
        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.BaseSkinName != ChampionName)
            {
                return;
            }

            Q = new Spell.Skillshot(SpellSlot.Q, 1450, SkillShotType.Linear, 0, 1000, 40)
            {
                AllowedCollisionCount = int.MaxValue
            };
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Active(SpellSlot.E);
            R = new Spell.Active(SpellSlot.R, 5500);

            // Menu
            Essentials.MainMenu = MainMenu.AddMenu("CH汉化-卡牌", "TwistedFate");

            // Card Selector Menu
            Essentials.CardSelectorMenu = Essentials.MainMenu.AddSubMenu("选牌菜单", "csMenu");
            Essentials.CardSelectorMenu.AddGroupLabel("选牌设置");
            Essentials.CardSelectorMenu.Add("useY", new KeyBind("使用黄牌", false, KeyBind.BindTypes.HoldActive, "W".ToCharArray()[0]));
            Essentials.CardSelectorMenu.Add("useB", new KeyBind("使用蓝牌", false, KeyBind.BindTypes.HoldActive, "E".ToCharArray()[0]));
            Essentials.CardSelectorMenu.Add("useR", new KeyBind("使用红牌", false, KeyBind.BindTypes.HoldActive, "T".ToCharArray()[0]));

            // Combo
            Essentials.ComboMenu = Essentials.MainMenu.AddSubMenu("连招", "comboMenu");
            Essentials.ComboMenu.AddGroupLabel("连招设置");
            Essentials.ComboMenu.Add("useQ", new CheckBox("使用Q"));
            Essentials.ComboMenu.Add("useCard", new CheckBox("使用W"));
            Essentials.ComboMenu.Add("useQStun", new CheckBox("只对定身目标使用Q", false));
            Essentials.ComboMenu.Add("qPred", new Slider("Q 命中率 %", 75));
            Essentials.ComboMenu.Add("wSlider", new Slider("离敌方 X 距离选牌", 300, 0, 10000));
            Essentials.ComboMenu.Add("manaManagerQ", new Slider("蓝量低于 X 不使用Q", 25));
            Essentials.ComboMenu.AddGroupLabel("选牌设置");
            Essentials.ComboMenu.Add("chooser", new ComboBox("选牌模式", new[] { "智能", "蓝", "红", "黄" }));
            Essentials.ComboMenu.Add("enemyW", new Slider("敌人数量选择红牌 (智能)", 4, 1, 5));
            Essentials.ComboMenu.Add("manaW", new Slider("蓝量低于 X 选择蓝牌 (智能)", 25));

            // Harass Menu
            Essentials.HarassMenu = Essentials.MainMenu.AddSubMenu("骚扰", "harassMenu");
            Essentials.HarassMenu.AddGroupLabel("骚扰设置");
            Essentials.HarassMenu.Add("useQ", new CheckBox("使用Q"));
            Essentials.HarassMenu.Add("useCard", new CheckBox("使用W"));
            Essentials.HarassMenu.Add("qPred", new Slider("Q 命中率 %", 75));
            Essentials.HarassMenu.Add("wSlider", new Slider("离敌方 X 距离选牌", 300, 0, 10000));
            Essentials.HarassMenu.Add("manaManagerQ", new Slider("蓝量低于 X 不使用Q", 25));
            Essentials.HarassMenu.AddGroupLabel("选牌设置");
            Essentials.HarassMenu.Add("chooser", new ComboBox("选牌模式", new[] { "智能", "蓝", "红", "黄" }));
            Essentials.HarassMenu.Add("enemyW", new Slider("敌人数量选择红牌 (智能)", 3, 1, 5));
            Essentials.HarassMenu.Add("manaW", new Slider("蓝量低于 X 选择蓝牌 (智能)", 25));

            // Lane Clear Menu
            Essentials.LaneClearMenu = Essentials.MainMenu.AddSubMenu("清线", "laneclearMenu");
            Essentials.LaneClearMenu.AddGroupLabel("清线设置");
            Essentials.LaneClearMenu.Add("useQ", new CheckBox("使用Q", false));
            Essentials.LaneClearMenu.Add("useCard", new CheckBox("使用W"));
            Essentials.LaneClearMenu.Add("qPred", new Slider("可命中小兵数量 X 使用Q", 3, 1, 5));
            Essentials.LaneClearMenu.Add("manaManagerQ", new Slider("蓝量低于 X 不使用Q", 50));
            Essentials.LaneClearMenu.AddGroupLabel("选牌设置");
            Essentials.LaneClearMenu.Add("chooser", new ComboBox("选牌模式", new[] { "智能", "蓝", "红", "黄" }));
            Essentials.LaneClearMenu.Add("enemyW", new Slider("敌人数量选择红牌 (智能)", 2, 1, 5));
            Essentials.LaneClearMenu.Add("manaW", new Slider("蓝量低于 X 选择蓝牌 (智能)", 25));

            // Jungle Clear Menu
            Essentials.JungleClearMenu = Essentials.MainMenu.AddSubMenu("清野", "jgMenu");
            Essentials.JungleClearMenu.AddGroupLabel("清野设置");
            Essentials.JungleClearMenu.Add("useQ", new CheckBox("使用Q", false));
            Essentials.JungleClearMenu.Add("useCard", new CheckBox("使用W"));
            Essentials.JungleClearMenu.Add("qPred", new Slider("Q 命中率 %", 75));
            Essentials.JungleClearMenu.Add("manaManagerQ", new Slider("蓝量低于 X 不使用Q", 50));
            Essentials.JungleClearMenu.AddGroupLabel("选牌设置");
            Essentials.JungleClearMenu.Add("chooser", new ComboBox("选牌模式", new[] {"智能", "蓝", "红", "黄" }));
            Essentials.JungleClearMenu.Add("enemyW", new Slider("敌人数量选择红牌 (智能)", 2, 1, 5));
            Essentials.JungleClearMenu.Add("manaW", new Slider("蓝量低于 X 选择蓝牌 (智能)", 25));

            // Kill Steal Menu
            Essentials.KillStealMenu = Essentials.MainMenu.AddSubMenu("抢头", "ksMenu");
            Essentials.KillStealMenu.AddGroupLabel("抢头设置");
            Essentials.KillStealMenu.Add("useQ", new CheckBox("使用Q"));
            Essentials.KillStealMenu.Add("qPred", new Slider("Q 命中率 %", 75));
            Essentials.KillStealMenu.Add("manaManagerQ", new Slider("蓝量低于 X 不使用Q", 15));
            Essentials.KillStealMenu.AddSeparator();

            // Drawing Menu
            Essentials.DrawingMenu = Essentials.MainMenu.AddSubMenu("线圈", "drawMenu");
            Essentials.DrawingMenu.AddGroupLabel("线圈竖直");
            Essentials.DrawingMenu.Add("drawQ", new CheckBox("显示 Q 范围"));
            Essentials.DrawingMenu.Add("drawR", new CheckBox("显示 R 范围"));
            Essentials.DrawingMenu.AddSeparator();

            // Misc Menu
            Essentials.MiscMenu = Essentials.MainMenu.AddSubMenu("杂项", "miscMenu");
            Essentials.MiscMenu.AddGroupLabel("杂项设置");
            Essentials.MiscMenu.Add("autoQ", new CheckBox("对不可移动目标自动Q"));
            Essentials.MiscMenu.Add("qPred", new Slider("Q 命中率 %", 75));
            Essentials.MiscMenu.Add("autoY", new CheckBox("R之前自动选黄牌"));
            Essentials.MiscMenu.Add("delay", new CheckBox("跳完三次牌后在选牌（人性化延迟）", false));
            Essentials.MiscMenu.Add("disableAA", new CheckBox("跳牌时屏蔽普攻"));

            // Prints Message
            Chat.Print("TwistedBuddy 2.3 - By KarmaPanda", System.Drawing.Color.Green);

            // Events
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnSpellCast;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        /// <summary>
        /// Called before Auto Attack is casted.
        /// </summary>
        /// <param name="target">The Target</param>
        /// <param name="args"></param>
        private static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (Essentials.MiscMenu["disableAA"].Cast<CheckBox>().CurrentValue &&
                CardSelector.Status == SelectStatus.Selecting)
            {
                args.Process = false;
                return;
            }
            args.Process = true;
        }

        /// <summary>
        /// Called after Spell Cast
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void Obj_AI_Base_OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            var target = args.Target as AIHeroClient;

            if (target == null || args.SData.Name.ToLower() != "goldcardpreattack" || !Q.IsReady() || !Q.IsInRange(target))
            {
                return;
            }

            if (Essentials.MiscMenu["autoQ"].Cast<CheckBox>().CurrentValue)
            {
                var pred = Q.GetPrediction(target);

                if (pred != null && pred.HitChancePercent >= Essentials.MiscMenu["qPred"].Cast<Slider>().CurrentValue)
                {
                    Q.Cast(pred.CastPosition);
                }
                else
                {
                    Essentials.UseStunQ = true;
                    Essentials.StunnedTarget = target;
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                Essentials.ComboMenu["useQStun"].Cast<CheckBox>().CurrentValue)
            {
                var pred = Q.GetPrediction(target);

                if (pred != null && pred.HitChancePercent >= Essentials.ComboMenu["qPred"].Cast<Slider>().CurrentValue)
                {
                    Q.Cast(pred.CastPosition);
                }
                else
                {
                    Essentials.UseStunQ = true;
                    Essentials.StunnedTarget = target;
                }
            }
        }

        /// <summary>
        /// Called on Spell Cast
        /// </summary>
        /// <param name="sender">The Person who casted a spell</param>
        /// <param name="args">The Args</param>
        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (args.SData.Name.ToLower() == "gate" && Essentials.MiscMenu["autoY"].Cast<CheckBox>().CurrentValue)
            {
                CardSelector.StartSelecting(Cards.Yellow);
            }
        }

        /// <summary>
        /// Called when game draws.
        /// </summary>
        /// <param name="args">The Args.</param>
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Essentials.DrawingMenu["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                if (Player.Instance != null)
                {
                    Circle.Draw(Q.IsReady() ? Color.Green : Color.Red, Q.Range, Player.Instance.Position);
                }
            }

            if (!Essentials.DrawingMenu["drawR"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            if (Player.Instance != null)
            {
                Circle.Draw(R.IsReady() ? Color.Green : Color.Red, R.Range, Player.Instance.Position);
            }
        }

        /// <summary>
        /// Called when game updates.
        /// </summary>
        /// <param name="args">The Args.</param>
        private static void Game_OnUpdate(EventArgs args)
        {
            if (!Player.Instance.IsRecalling() && !Player.Instance.IsInShopRange())
            {
                var useY = Essentials.CardSelectorMenu["useY"].Cast<KeyBind>().CurrentValue;
                var useB = Essentials.CardSelectorMenu["useB"].Cast<KeyBind>().CurrentValue;
                var useR = Essentials.CardSelectorMenu["useR"].Cast<KeyBind>().CurrentValue;

                if (useY)
                {
                    CardSelector.StartSelecting(Cards.Yellow);
                }

                if (useB)
                {
                    CardSelector.StartSelecting(Cards.Blue);
                }

                if (useR)
                {
                    CardSelector.StartSelecting(Cards.Red);
                }

                StateManager.AutoQ();
            }

            if (Essentials.KillStealMenu["useQ"].Cast<CheckBox>().CurrentValue)
            {
                StateManager.KillSteal();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                StateManager.Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                StateManager.LaneClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                StateManager.JungleClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                StateManager.Harass();
            }
        }
    }
}
