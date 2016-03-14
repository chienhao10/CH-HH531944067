using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberHidesStaticFromOuterClass

namespace Hellsing.Kalista
{
    public static class Config
    {
        private const string MenuName = "Kalista";
        public static Menu Menu { get; private set; }

        static Config()
        {
            Menu = MainMenu.AddMenu(MenuName, "kalistaMenu");
            Menu.AddGroupLabel("简介");
            Menu.AddLabel("欢迎使用我的第一个脚本！由CH汉化!");

            // All modes
            Modes.Initialize();

            // Misc
            Misc.Initialize();

            // Items
            Items.Initialize();

            // Drawing
            Drawing.Initialize();

            // Specials
            Specials.Initialize();
        }

        public static void Initialize()
        {
        }

        public static class Modes
        {
            private static Menu Menu { get; set; }

            static Modes()
            {
                // Initialize modes menu
                Menu = Config.Menu.AddSubMenu("模式", "modes");

                // Combo
                Combo.Initialize();

                // Harass
                Menu.AddSeparator();
                Harass.Initialize();

                // WaveClear
                Menu.AddSeparator();
                LaneClear.Initialize();

                // JungleClear
                Menu.AddSeparator();
                JungleClear.Initialize();

                // Flee
                Menu.AddSeparator();
                Flee.Initialize();
            }

            public static void Initialize()
            {
            }

            public static class Combo
            {
                private static readonly CheckBox _useQ;
                private static readonly CheckBox _useQAA;
                private static readonly CheckBox _useAA;
                private static readonly CheckBox _useE;
                private static readonly CheckBox _useEslow;
                private static readonly Slider _numE;
                private static readonly Slider _mana;

                private static readonly CheckBox _useItems;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }
                public static bool UseQAA
                {
                    get { return _useQAA.CurrentValue; }
                }
                public static bool UseE
                {
                    get { return _useE.CurrentValue; }
                }
                public static int MinNumberE
                {
                    get { return _numE.CurrentValue; }
                }
                public static bool UseAA
                {
                    get { return _useAA.CurrentValue; }
                }
                public static bool UseESlow
                {
                    get { return _useEslow.CurrentValue; }
                }
                public static bool UseItems
                {
                    get { return _useItems.CurrentValue; }
                }
                public static int ManaQ
                {
                    get { return _mana.CurrentValue; }
                }

                static Combo()
                {
                    Menu.AddGroupLabel("连招");

                    _useQ = Menu.Add("comboUseQ", new CheckBox("使用 Q"));
                    _useQAA = Menu.Add("comboUseQAA", new CheckBox("只在平A后使用Q"));
                    _useE = Menu.Add("comboUseE", new CheckBox("使用 E"));
                    _useEslow = Menu.Add("comboUseEslow", new CheckBox("使用E杀死小兵并减速敌方"));
                    _useAA = Menu.Add("comboUseAA", new CheckBox("攻击小兵进行间距"));
                    _useItems = Menu.Add("comboUseItems", new CheckBox("使用物品"));
                    _numE = Menu.Add("comboNumE", new Slider("最少叠加层数使用 E", 5, 1, 50));
                    _mana = Menu.Add("comboMana", new Slider("最少蓝量 ({0}% 使用Q)", 30));
                }

                public static void Initialize()
                {
                }
            }

            public static class Harass
            {
                private static readonly CheckBox _useQ;
                private static readonly Slider _mana;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }
                public static int MinMana
                {
                    get { return _mana.CurrentValue; }
                }

                static Harass()
                {
                    Menu.AddGroupLabel("骚扰");

                    _useQ = Menu.Add("harassUseQ", new CheckBox("使用 Q"));
                    _mana = Menu.Add("harassMana", new Slider("最少蓝量 %", 30));
                }

                public static void Initialize()
                {
                }
            }

            public static class LaneClear
            {
                private static readonly CheckBox _useQ;
                private static readonly Slider _numQ;
                private static readonly CheckBox _useE;
                private static readonly Slider _numE;
                private static readonly Slider _mana;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }
                public static int MinNumberQ
                {
                    get { return _numQ.CurrentValue; }
                }
                public static bool UseE
                {
                    get { return _useE.CurrentValue; }
                }
                public static int MinNumberE
                {
                    get { return _numE.CurrentValue; }
                }
                public static int MinMana
                {
                    get { return _mana.CurrentValue; }
                }

                static LaneClear()
                {
                    Menu.AddGroupLabel("清线");

                    _useQ = Menu.Add("laneUseQ", new CheckBox("使用 Q"));
                    _useE = Menu.Add("laneUseE", new CheckBox("使用 E"));
                    _numQ = Menu.Add("laneNumQ", new Slider("可杀死 X 小兵数量使用 Q", 3, 1, 10));
                    _numE = Menu.Add("laneNumE", new Slider("可杀死 X 小兵数量使用 E", 2, 1, 10));
                    Menu.AddSeparator();
                    _mana = Menu.Add("laneMana", new Slider("最少蓝量%", 30));
                }

                public static void Initialize()
                {
                }
            }

            public static class JungleClear
            {
                private static readonly CheckBox _useE;

                public static bool UseE
                {
                    get { return _useE.CurrentValue; }
                }

                static JungleClear()
                {
                    Menu.AddGroupLabel("清野");

                    _useE = Menu.Add("jungleUseE", new CheckBox("使用 E"));
                }

                public static void Initialize()
                {
                }
            }

            public static class Flee
            {
                private static readonly CheckBox _walljump;
                private static readonly CheckBox _autoAttack;

                public static bool UseWallJumps
                {
                    get { return _walljump.CurrentValue; }
                }
                public static bool UseAutoAttacks
                {
                    get { return _autoAttack.CurrentValue; }
                }

                static Flee()
                {
                    Menu.AddGroupLabel("逃跑");

                    _walljump = Menu.Add("fleeWalljump", new CheckBox("使用跳墙"));
                    _autoAttack = Menu.Add("fleeAutoattack", new CheckBox("使用平A"));
                }

                public static void Initialize()
                {
                }
            }
        }

        public static class Misc
        {
            private static Menu Menu { get; set; }

            private static readonly CheckBox _killsteal;
            private static readonly CheckBox _bigE;
            private static readonly CheckBox _saveSoulbound;
            private static readonly CheckBox _secureE;
            private static readonly CheckBox _harassPlus;
            private static readonly Slider _autoBelowHealthE;
            private static readonly Slider _reductionE;

            public static bool UseKillsteal
            {
                get { return _killsteal.CurrentValue; }
            }
            public static bool UseEBig
            {
                get { return _bigE.CurrentValue; }
            }
            public static bool SaveSouldBound
            {
                get { return _saveSoulbound.CurrentValue; }
            }
            public static bool SecureMinionKillsE
            {
                get { return _secureE.CurrentValue; }
            }
            public static bool UseHarassPlus
            {
                get { return _harassPlus.CurrentValue; }
            }
            public static int AutoEBelowHealth
            {
                get { return _autoBelowHealthE.CurrentValue; }
            }
            public static int DamageReductionE
            {
                get { return _reductionE.CurrentValue; }
            }

            static Misc()
            {
                Menu = Config.Menu.AddSubMenu("杂项");

                Menu.AddGroupLabel("杂项功能");
                _killsteal = Menu.Add("killsteal", new CheckBox("使用E抢人头"));
                _bigE = Menu.Add("bigE", new CheckBox("总对炮兵使用E"));
                _saveSoulbound = Menu.Add("saveSoulbound", new CheckBox("使用R保护灵魂绑定的队友"));
                _secureE = Menu.Add("secureE", new CheckBox("使用E杀死无法平A的小兵"));
                _harassPlus = Menu.Add("harassPlus", new CheckBox("自动使用E当能杀死小兵以及敌人至少有一层E叠加"));
                _autoBelowHealthE = Menu.Add("autoBelowHealthE", new Slider("当你生命低于 ({0}%) 百分比时自动E", 10));
                _reductionE = Menu.Add("reductionE", new Slider("减速E伤害计算 {0} 点", 20));

                // Initialize other misc features
                Sentinel.Initialize();
            }

            public static void Initialize()
            {
            }

            public static class Sentinel
            {
                private static readonly CheckBox _enabled;
                private static readonly CheckBox _noMode;
                private static readonly CheckBox _alert;
                private static readonly Slider _mana;

                private static readonly CheckBox _baron;
                private static readonly CheckBox _dragon;
                private static readonly CheckBox _mid;
                private static readonly CheckBox _blue;
                private static readonly CheckBox _red;

                public static bool Enabled
                {
                    get { return _enabled.CurrentValue; }
                }
                public static bool NoModeOnly
                {
                    get { return _noMode.CurrentValue; }
                }
                public static bool Alert
                {
                    get { return _alert.CurrentValue; }
                }
                public static int Mana
                {
                    get { return _mana.CurrentValue; }
                }

                public static bool SendBaron
                {
                    get { return _baron.CurrentValue; }
                }
                public static bool SendDragon
                {
                    get { return _dragon.CurrentValue; }
                }
                public static bool SendMid
                {
                    get { return _mid.CurrentValue; }
                }
                public static bool SendBlue
                {
                    get { return _blue.CurrentValue; }
                }
                public static bool SendRed
                {
                    get { return _red.CurrentValue; }
                }

                static Sentinel()
                {
                    Menu.AddGroupLabel("使用灵魂哨兵 (W)");

                    if (Game.MapId != GameMapId.SummonersRift)
                    {
                        Menu.AddLabel("对不起，只支持召唤峡谷.");
                    }
                    else
                    {
                        _enabled = Menu.Add("enabled", new CheckBox("启用"));
                        _noMode = Menu.Add("noMode", new CheckBox("只会在无模式下使用"));
                        _alert = Menu.Add("alert", new CheckBox("哨兵受到伤害是警告"));
                        _mana = Menu.Add("mana", new Slider("至少有({0}% 使用 W )", 40));

                        Menu.AddLabel("发送至一以下地点 (无特定顺序):");
                        (_baron = Menu.Add("baron", new CheckBox("男爵 (会使用卡男爵池BUG)"))).OnValueChange += OnValueChange;
                        (_dragon = Menu.Add("dragon", new CheckBox("小龙 (会使用卡龙池BUG)"))).OnValueChange += OnValueChange;
                        (_mid = Menu.Add("mid", new CheckBox("中路草丛"))).OnValueChange += OnValueChange;
                        (_blue = Menu.Add("blue", new CheckBox("蓝爸爸草丛"))).OnValueChange += OnValueChange;
                        (_red = Menu.Add("red", new CheckBox("红妈妈草丛"))).OnValueChange += OnValueChange;
                        SentinelManager.RecalculateOpenLocations();
                    }
                }

                private static void OnValueChange(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                {
                    SentinelManager.RecalculateOpenLocations();
                }

                public static void Initialize()
                {
                }
            }
        }

        public static class Items
        {
            private static Menu Menu { get; set; }

            private static readonly CheckBox _cutlass;
            private static readonly CheckBox _botrk;
            private static readonly CheckBox _ghostblade;

            public static bool UseCutlass
            {
                get { return _cutlass.CurrentValue; }
            }
            public static bool UseBotrk
            {
                get { return _botrk.CurrentValue; }
            }
            public static bool UseGhostblade
            {
                get { return _ghostblade.CurrentValue; }
            }

            static Items()
            {
                Menu = Config.Menu.AddSubMenu("物品");

                _cutlass = Menu.Add("cutlass", new CheckBox("使用弯刀"));
                _botrk = Menu.Add("botrk", new CheckBox("使用破败"));
                _ghostblade = Menu.Add("ghostblade", new CheckBox("使用幽梦"));
            }

            public static void Initialize()
            {
            }
        }

        public static class Drawing
        {
            private static Menu Menu { get; set; }

            private static readonly CheckBox _drawQ;
            private static readonly CheckBox _drawW;
            private static readonly CheckBox _drawE;
            private static readonly CheckBox _drawEleaving;
            private static readonly CheckBox _drawR;

            private static readonly CheckBox _healthbar;
            private static readonly CheckBox _percent;

            public static bool DrawQ
            {
                get { return _drawQ.CurrentValue; }
            }
            public static bool DrawW
            {
                get { return _drawW.CurrentValue; }
            }
            public static bool DrawE
            {
                get { return _drawE.CurrentValue; }
            }
            public static bool DrawELeaving
            {
                get { return _drawEleaving.CurrentValue; }
            }
            public static bool DrawR
            {
                get { return _drawR.CurrentValue; }
            }
            public static bool IndicatorHealthbar
            {
                get { return _healthbar.CurrentValue; }
            }
            public static bool IndicatorPercent
            {
                get { return _percent.CurrentValue; }
            }

            static Drawing()
            {
                Menu = Config.Menu.AddSubMenu("线圈");

                Menu.AddGroupLabel("技能范围");
                _drawQ = Menu.Add("drawQ", new CheckBox("Q 范围"));
                _drawW = Menu.Add("drawW", new CheckBox("W 范围"));
                _drawE = Menu.Add("drawE", new CheckBox("E 范围"));
                _drawEleaving = Menu.Add("drawEleaving", new CheckBox("E 触发范围(看连招设置)", false));
                _drawR = Menu.Add("drawR", new CheckBox("R 范围", false));

                Menu.AddGroupLabel("伤害显示器 (灵魂粉碎 - E)");
                _healthbar = Menu.Add("healthbar", new CheckBox("血条覆盖"));
                _percent = Menu.Add("percent", new CheckBox("伤害百分比信息"));
            }

            public static void Initialize()
            {
            }
        }

        public static class Specials
        {
            private static Menu Menu { get; set; }

            private static CheckBox _useBalista;
            private static CheckBox _balistaComboOnly;
            private static CheckBox _balistaMoreHealth;
            private static Slider _balistaTriggerRange;

            public static bool UseBalista
            {
                get { return _useBalista != null && _useBalista.CurrentValue; }
            }
            public static bool BalistaComboOnly
            {
                get { return _balistaComboOnly.CurrentValue; }
            }
            public static bool BalistaMoreHealthOnly
            {
                get { return _balistaMoreHealth.CurrentValue; }
            }
            public static int BalistaTriggerRange
            {
                get { return _balistaTriggerRange.CurrentValue; }
            }

            static Specials()
            {
                Menu = Config.Menu.AddSubMenu("特技");

                Menu.AddGroupLabel("机器人与滑板鞋的合体<3");
                if (EntityManager.Heroes.Allies.Any(o => o.ChampionName == "Blitzcrank"))
                {
                    Menu.Add("infoLabel", new Label("你还没进行灵魂合体!"));
                    Game.OnTick += BalistaCheckSoulBound;
                }
                else
                {
                    Menu.AddLabel("队里没有机器人，所以不能合体 >///< ");
                }
            }

            private static void BalistaCheckSoulBound(EventArgs args)
            {
                if (SoulBoundSaver.SoulBound != null)
                {
                    Game.OnTick -= BalistaCheckSoulBound;
                    Menu.Remove("infoLabel");

                    if (SoulBoundSaver.SoulBound.ChampionName != "Blitzcrank")
                    {
                        Menu.AddLabel("你没对机器人进行灵魂连接（合体）快去滚床单吧~!<3");
                        Menu.AddLabel("如果你要重新进行灵魂连接，请重新载入脚本（F5）");
                        Menu.AddLabel("确保系统发现你的新对象（连接对象），祝你们开心合体!");
                        return;
                    }

                    _useBalista = Menu.Add("useBalista", new CheckBox("开启"));
                    Menu.AddSeparator(0);
                    _balistaComboOnly = Menu.Add("balistaComboOnly", new CheckBox("只在连招使用", false));
                    _balistaMoreHealth = Menu.Add("moreHealth", new CheckBox("只在我的生命值更高时"));

                    const int blitzcrankQrange = 925;
                    _balistaTriggerRange = Menu.Add("balistaTriggerRange",
                        new Slider("机器人Q目标和你之间的触发范围", (int) SpellManager.R.Range, (int) SpellManager.R.Range,
                            (int) (SpellManager.R.Range + blitzcrankQrange * 0.8f)));

                    // Handle Blitzcrank hooks in Kalista.OnTickBalistaCheck
                    Obj_AI_Base.OnBuffGain += delegate(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs eventArgs)
                    {
                        if (eventArgs.Buff.DisplayName == "RocketGrab" && eventArgs.Buff.Caster.NetworkId == SoulBoundSaver.SoulBound.NetworkId)
                        {
                            Game.OnTick += Kalista.OnTickBalistaCheck;
                        }
                    };
                    Obj_AI_Base.OnBuffLose += delegate(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs eventArgs)
                    {
                        if (eventArgs.Buff.DisplayName == "RocketGrab" && eventArgs.Buff.Caster.NetworkId == SoulBoundSaver.SoulBound.NetworkId)
                        {
                            Game.OnTick -= Kalista.OnTickBalistaCheck;
                        }
                    };
                }
            }

            public static void Initialize()
            {
            }
        }
    }
}
