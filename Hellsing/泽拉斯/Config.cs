using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberHidesStaticFromOuterClass

namespace Xerath
{
    public static class Config
    {
        public const string MenuName = "CH汉化-泽拉斯";
        private static readonly Menu Menu;

        static Config()
        {
            // Initialize menu
            Menu = MainMenu.AddMenu(MenuName, MenuName + "_hellsing");

            // Initialize sub menus
            Modes.Initialize();
            Ultimate.Initialize();
            Misc.Initialize();
            Drawing.Initialize();
        }

        public static void Initialize()
        {
        }

        public static class Modes
        {
            public const string MenuName = "模式";
            private static readonly Menu Menu;

            static Modes()
            {
                // Initialize menu
                Menu = Config.Menu.AddSubMenu(MenuName);

                // Initialize sub groups
                Combo.Initialize();
                Menu.AddSeparator();
                Harass.Initialize();
                Menu.AddSeparator();
                LaneClear.Initialize();
                Menu.AddSeparator();
                JungleClear.Initialize();
                Menu.AddSeparator();
                Flee.Initialize();
            }

            public static void Initialize()
            {
            }

            public static class Combo
            {
                public const string GroupName = "连招";

                private static readonly CheckBox _useQ;
                private static readonly CheckBox _useW;
                private static readonly CheckBox _useE;
                private static readonly CheckBox _useR;

                private static readonly Slider _extraRangeQ;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }
                public static bool UseW
                {
                    get { return _useW.CurrentValue; }
                }
                public static bool UseE
                {
                    get { return _useE.CurrentValue; }
                }
                public static bool UseR
                {
                    get { return _useR.CurrentValue; }
                }

                public static int ExtraRangeQ
                {
                    get { return _extraRangeQ.CurrentValue; }
                }

                static Combo()
                {
                    // Initialize group
                    Menu.AddGroupLabel(GroupName);

                    _useQ = Menu.Add("comboUseQ", new CheckBox("使用 Q"));
                    _useW = Menu.Add("comboUseW", new CheckBox("使用 W"));
                    _useE = Menu.Add("comboUseE", new CheckBox("使用 E"));
                    _useR = Menu.Add("comboUseR", new CheckBox("使用 R", false));

                    Menu.AddLabel("高级设置:");

                    _extraRangeQ = Menu.Add("comboExtraRangeQ", new Slider("为Q 使用额外射程", 200, 0, 200));
                }

                public static void Initialize()
                {
                }
            }

            public static class Harass
            {
                public const string GroupName = "骚扰";

                private static readonly CheckBox _useQ;
                private static readonly CheckBox _useW;
                private static readonly CheckBox _useE;

                private static readonly Slider _extraRangeQ;
                private static readonly Slider _mana;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }
                public static bool UseW
                {
                    get { return _useW.CurrentValue; }
                }
                public static bool UseE
                {
                    get { return _useE.CurrentValue; }
                }

                public static int ExtraRangeQ
                {
                    get { return _extraRangeQ.CurrentValue; }
                }
                public static int ManaUsage
                {
                    get { return _mana.CurrentValue; }
                }

                static Harass()
                {
                    // Initialize group
                    Menu.AddGroupLabel(GroupName);

                    _useQ = Menu.Add("harassUseQ", new CheckBox("使用 Q"));
                    _useW = Menu.Add("harassUseW", new CheckBox("使用 W"));
                    _useE = Menu.Add("harassUseE", new CheckBox("使用 E"));

                    Menu.AddLabel("高级设置:");

                    _extraRangeQ = Menu.Add("harassExtraRangeQ", new Slider("为Q 使用额外射程", 200, 0, 200));
                    _mana = Menu.Add("harassMana", new Slider("蓝量使用 (%)", 30));
                }

                public static void Initialize()
                {
                }
            }

            public static class LaneClear
            {
                public const string GroupName = "清线";

                private static readonly CheckBox _useQ;
                private static readonly CheckBox _useW;

                private static readonly Slider _hitNumQ;
                private static readonly Slider _hitNumW;
                private static readonly Slider _mana;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }
                public static bool UseW
                {
                    get { return _useW.CurrentValue; }
                }

                public static int HitNumberQ
                {
                    get { return _hitNumQ.CurrentValue; }
                }
                public static int HitNumberW
                {
                    get { return _hitNumW.CurrentValue; }
                }
                public static int ManaUsage
                {
                    get { return _mana.CurrentValue; }
                }

                static LaneClear()
                {
                    // Initialize group
                    Menu.AddGroupLabel(GroupName);

                    _useQ = Menu.Add("laneUseQ", new CheckBox("使用 Q"));
                    _useW = Menu.Add("laneUseW", new CheckBox("使用 W"));

                    Menu.AddLabel("高级设置:");

                    _hitNumQ = Menu.Add("laneHitQ", new Slider("Q 命中小兵数量", 3, 1, 10));
                    _hitNumW = Menu.Add("laneHitW", new Slider("W 命中小兵数量", 3, 1, 10));
                    _mana = Menu.Add("laneMana", new Slider("蓝量使用 (%)", 30));
                }

                public static void Initialize()
                {
                }
            }

            public static class JungleClear
            {
                public const string GroupName = "清野";

                private static readonly CheckBox _useQ;
                private static readonly CheckBox _useW;
                private static readonly CheckBox _useE;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }
                public static bool UseW
                {
                    get { return _useW.CurrentValue; }
                }
                public static bool UseE
                {
                    get { return _useE.CurrentValue; }
                }

                static JungleClear()
                {
                    // Initialize group
                    Menu.AddGroupLabel(GroupName);

                    _useQ = Menu.Add("jungleUseQ", new CheckBox("使用 Q"));
                    _useW = Menu.Add("jungleUseW", new CheckBox("使用 W"));
                    _useE = Menu.Add("jungleUseE", new CheckBox("使用 E"));
                }

                public static void Initialize()
                {
                }
            }

            public static class Flee
            {
                static Flee()
                {
                }

                public static void Initialize()
                {
                }
            }
        }

        public static class Ultimate
        {
            public const string MenuName = "大招";
            private static readonly Menu Menu;

            public static readonly string[] AvailableModes =
            {
                "智能选择目标",
                "超神模式（明显脚本）",
                "鼠标附近",
                "使用发射按键 (自动)",
                "使用发射按键 (鼠标附近)"
            };

            private static readonly CheckBox _enabled;
            private static readonly Slider _mode;
            private static readonly KeyBind _shootKey;

            public static bool Enabled
            {
                get { return _enabled.CurrentValue; }
            }
            public static int CurrentMode
            {
                get { return _mode.CurrentValue; }
            }
            public static KeyBind ShootKey
            {
                get { return _shootKey; }
            }

            static Ultimate()
            {
                // Initialize menu
                Menu = Config.Menu.AddSubMenu(MenuName);

                _enabled = Menu.Add("enabled", new CheckBox("开启已下设置"));

                _mode = new Slider("Mode: " + AvailableModes[0], 0, 0, AvailableModes.Length - 1);
                _mode.OnValueChange += delegate { _mode.DisplayName = "模式: " + AvailableModes[_mode.CurrentValue]; };
                Menu.Add("mode", _mode);
                Menu.AddLabel("可选模式:");
                for (var i = 0; i < AvailableModes.Length; i++)
                {
                    Menu.AddLabel(string.Format("  - {0}: {1}", i, AvailableModes[i]));
                }

                _shootKey = Menu.Add("keyPress", new KeyBind("R 发射按键", false, KeyBind.BindTypes.HoldActive, 'T'));
            }

            public static void Initialize()
            {
            }
        }

        public static class Misc
        {
            public const string MenuName = "杂项";
            private static readonly Menu Menu;

            private static readonly CheckBox _gapcloser;
            private static readonly CheckBox _interrupter;
            private static readonly CheckBox _alerter;

            public static bool GapcloserE
            {
                get { return _gapcloser.CurrentValue; }
            }
            public static bool InterruptE
            {
                get { return _interrupter.CurrentValue; }
            }
            public static bool Alerter
            {
                get { return _alerter.CurrentValue; }
            }

            static Misc()
            {
                // Initialize menu
                Menu = Config.Menu.AddSubMenu(MenuName);

                _gapcloser = Menu.Add("miscGapcloseE", new CheckBox("E 防突进"));
                _interrupter = Menu.Add("miscInterruptE", new CheckBox("E 技能打断"));
                _alerter = Menu.Add("miscAlerter", new CheckBox("聊天提示R可击杀目标"));
            }

            public static void Initialize()
            {
            }
        }

        public static class Drawing
        {
            public const string MenuName = "线圈";
            private static readonly Menu Menu;

            private static readonly CheckBox _drawQ;
            private static readonly CheckBox _drawW;
            private static readonly CheckBox _drawE;
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
                // Initialize menu
                Menu = Config.Menu.AddSubMenu(MenuName);

                Menu.AddGroupLabel("技能范围");
                _drawQ = Menu.Add("drawQ", new CheckBox("Q 范围"));
                _drawW = Menu.Add("drawW", new CheckBox("W 范围"));
                _drawE = Menu.Add("drawE", new CheckBox("E 范围"));
                _drawR = Menu.Add("drawR", new CheckBox("R 范围", false));

                Menu.AddGroupLabel("伤害指示器");
                _healthbar = Menu.Add("healthbar", new CheckBox("血量显示"));
                _percent = Menu.Add("percent", new CheckBox("伤害百分比"));
            }

            public static void Initialize()
            {
            }
        }
    }
}
