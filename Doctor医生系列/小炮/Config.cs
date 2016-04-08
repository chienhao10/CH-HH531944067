using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberHidesStaticFromOuterClass

namespace Tristana
{
    public static class Config
    {
        private const string MenuName = "CH汉化-Dr小炮";

        private static readonly Menu Menu;

        static Config()
        {
            Menu = MainMenu.AddMenu(MenuName, MenuName.ToLower());
            Menu.AddGroupLabel("CH汉化-Doctor 小炮");
            Menu.AddLabel("Good Luck");
            Menu.AddLabel("Upvote for me if u like addon.");
            ModesMenu.Initialize();
            PredictionMenu.Initialize();
            ManaManagerMenu.Initialize();
            MiscMenu.Initialize();
            DrawingMenu.Initialize();
            DebugMenu.Initialize();
        }

        public static void Initialize()
        {
        }

        public static class ModesMenu
        {
            private static readonly Menu MenuModes;

            static ModesMenu()
            {
                MenuModes = Config.Menu.AddSubMenu("模式");

                Combo.Initialize();
                MenuModes.AddSeparator();

                Harass.Initialize();
                MenuModes.AddSeparator();

                LaneClear.Initialize();
                MenuModes.AddSeparator();

                JungleClear.Initialize();
                MenuModes.AddSeparator();

                Flee.Initialize();
            }

            public static void Initialize()
            {
            }

            public static class Combo
            {
                private static readonly CheckBox _useQ;
                private static readonly CheckBox _useW;
                private static readonly CheckBox _useE;
                private static readonly CheckBox _useR;
                private static readonly CheckBox _useItems;
                private static readonly Slider _maxBOTRKHPEnemy;
                private static readonly Slider _maxBOTRKHPPlayer;

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

                public static bool UseItems
                {
                    get { return _useItems.CurrentValue; }
                }


                public static int MaxBOTRKHPPlayer
                {
                    get { return _maxBOTRKHPPlayer.CurrentValue; }
                }

                public static int MaxBOTRKHPEnemy
                {
                    get { return _maxBOTRKHPEnemy.CurrentValue; }
                }

                static Combo()
                {
                    MenuModes.AddGroupLabel("连招");
                    _useQ = MenuModes.Add("comboUseQ", new CheckBox("使用 Q"));
                    _useW = MenuModes.Add("comboUseW", new CheckBox("使用 W", false));
                    _useE = MenuModes.Add("comboUseE", new CheckBox("使用 E"));
                    _useR = MenuModes.Add("comboUseR", new CheckBox("使用 R (使用R最后一击)"));
                    _useItems = MenuModes.Add("comboUseItems", new CheckBox("使用 弯刀/破败/幽梦"));
                    _maxBOTRKHPPlayer = MenuModes.Add("comboMaxBotrkHpPlayer",
                        new Slider("玩家最大生命 % 时使用破败", 80, 0, 100));
                    _maxBOTRKHPEnemy = MenuModes.Add("comboMaxBotrkHpEnemy",
                        new Slider("敌人最大生命 % 时使用破败", 100, 0, 100));
                }

                public static void Initialize()
                {
                }
            }

            public static class Harass
            {
                private static readonly CheckBox _useQ;
                private static readonly CheckBox _useE;
                
                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }
                
                public static bool UseE
                {
                    get { return _useE.CurrentValue; }
                }
                
                static Harass()
                {
                    MenuModes.AddGroupLabel("骚扰");
                    _useQ = MenuModes.Add("harassUseQ", new CheckBox("使用 Q"));
                    _useE = MenuModes.Add("harassUseE", new CheckBox("使用 E"));
                }

                public static void Initialize()
                {
                }
            }

            public static class LaneClear
            {

                private static readonly CheckBox _useQ;
                private static readonly CheckBox _useW;
                private static readonly CheckBox _useE;
                private static readonly Slider _minWTargets;
                private static readonly Slider _minETargets;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }

                public static bool UseE
                {
                    get { return _useE.CurrentValue; }
                }

                public static bool UseW
                {
                    get { return _useW.CurrentValue; }
                }
                
                public static int MinWTargets
                {
                    get { return _minWTargets.CurrentValue; }
                }
                
                public static int MinETargets
                {
                    get { return _minETargets.CurrentValue; }
                }

                static LaneClear()
                {
                    MenuModes.AddGroupLabel("清线");
                    _useQ = MenuModes.Add("laneUseQ", new CheckBox("使用 Q"));
                    _useW = MenuModes.Add("laneUseW", new CheckBox("使用 W", false));
                    _useE = MenuModes.Add("laneUseE", new CheckBox("使用 E"));
                    _minWTargets = MenuModes.Add("minWTargetsLC", new Slider("最少目标使用W", 9, 1, 10));
                    _minETargets = MenuModes.Add("minETargetsLC", new Slider("最少目标使用E", 4, 1, 10));
                }

                public static void Initialize()
                {
                }
            }

            public static class JungleClear
            {
                private static readonly CheckBox _useQ;
                private static readonly CheckBox _useW;
                private static readonly CheckBox _useE;
                private static readonly Slider _minWTargets;
                private static readonly Slider _minETargets;

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

                public static int MinWTargets
                {
                    get { return _minWTargets.CurrentValue; }
                }

                public static int MinETargets
                {
                    get { return _minETargets.CurrentValue; }
                }

                static JungleClear()
                {
                    MenuModes.AddGroupLabel("清野");
                    _useQ = MenuModes.Add("jungleUseQ", new CheckBox("使用 Q"));
                    _useW = MenuModes.Add("jungleUseW", new CheckBox("使用 W", false));
                    _useE = MenuModes.Add("jungleUseE", new CheckBox("使用 E"));
                    _minWTargets = MenuModes.Add("minWTargetsJC", new Slider("最少目标使用W", 6, 1, 10));
                    _minETargets = MenuModes.Add("minETargetsJC", new Slider("最少目标使用E", 2, 1, 10));
                }

                public static void Initialize()
                {
                }
            }

            public static class Flee
            {
                private static readonly CheckBox _useW;
                private static readonly CheckBox _useR;

                public static bool UseW
                {
                    get { return _useW.CurrentValue; }
                }
                
                public static bool UseR
                {
                    get { return _useR.CurrentValue; }
                }

                static Flee()
                {
                    MenuModes.AddGroupLabel("逃跑");
                    _useW = MenuModes.Add("fleeUseW", new CheckBox("使用 W"));
                    _useR = MenuModes.Add("fleeUseR", new CheckBox("使用 R", false));
                }

                public static void Initialize()
                {
                }
            }
        }

        public static class MiscMenu
        {
            private static readonly Menu MenuMisc;
            private static readonly CheckBox _interruptR;
            private static readonly CheckBox _gapcloserR;
            private static readonly CheckBox _potion;
            private static readonly CheckBox _ksW;
            private static readonly CheckBox _ksE;
            private static readonly CheckBox _ksR;
            private static readonly CheckBox _ksIgnite;
            private static readonly KeyBind _WToCursor;
            private static readonly Slider _potionMinHP;
            private static readonly Slider _potionMinMP;

            public static bool InterruptR
            {
                get { return _interruptR.CurrentValue; }
            }
            public static bool GapcloserR
            {
                get { return _gapcloserR.CurrentValue; }
            }
            public static bool KsW
            {
                get { return _ksW.CurrentValue; }
            }
            public static bool KsE
            {
                get { return _ksE.CurrentValue; }
            }
            public static bool KsR
            {
                get { return _ksR.CurrentValue; }
            }
            public static bool KsIgnite
            {
                get { return _ksIgnite.CurrentValue; }
            }
            public static bool Potion
            {
                get { return _potion.CurrentValue; }
            }
            public static int potionMinHP
            {
                get { return _potionMinHP.CurrentValue; }
            }
            public static int potionMinMP
            {
                get { return _potionMinMP.CurrentValue; }
            }
            public static bool WToCursor
            {
                get { return _WToCursor.CurrentValue; }
            }

            static MiscMenu()
            {
                MenuMisc = Config.Menu.AddSubMenu("杂项");
                MenuMisc.AddGroupLabel("防突进");
                _gapcloserR = MenuMisc.Add("gapcloserR", new CheckBox("使用R造成间距", false));
                MenuMisc.AddGroupLabel("技能打断");
                _interruptR = MenuMisc.Add("interruptR", new CheckBox("使用R打断危险技能", false));
                MenuMisc.AddGroupLabel("抢头");
                _ksW = MenuMisc.Add("ksW", new CheckBox("抢头 W", false));
                _ksE = MenuMisc.Add("ksE", new CheckBox("抢头 E"));
                _ksR = MenuMisc.Add("ksR", new CheckBox("抢头 R"));
                _ksIgnite = MenuMisc.Add("ksIgnite", new CheckBox("点燃抢头"));
                MenuMisc.AddGroupLabel("自动喝药");
                _potion = MenuMisc.Add("potion", new CheckBox("使用 药水"));
                _potionMinHP = MenuMisc.Add("potionminHP", new Slider("最低血量 % 使用药水", 70));
                _potionMinMP = MenuMisc.Add("potionMinMP", new Slider("最低蓝量 % 使用药水", 20));
                 MenuMisc.AddGroupLabel("其他");
                _WToCursor = MenuMisc.Add("WToCuror",
                    new KeyBind("跳至鼠标", false, KeyBind.BindTypes.HoldActive, 'H'));
            }

            public static void Initialize()
            {
            }
        }

        public static class ManaManagerMenu
        {
            private static readonly Menu MenuManaManager;
            private static readonly Slider _minQMana;
            private static readonly Slider _minWMana;
            private static readonly Slider _minEMana;
            private static readonly Slider _minRMana;

            public static int MinQMana
            {
                get { return _minQMana.CurrentValue; }
            }
            public static int MinWMana
            {
                get { return _minWMana.CurrentValue; }
            }
            public static int MinEMana
            {
                get { return _minEMana.CurrentValue; }
            }
            public static int MinRMana
            {
                get { return _minRMana.CurrentValue; }
            }

            static ManaManagerMenu()
            {
                MenuManaManager = Config.Menu.AddSubMenu("蓝量管理器");
                _minQMana = MenuManaManager.Add("minQMana", new Slider("最低蓝量 % 使用 Q", 25, 0, 100));
                _minWMana = MenuManaManager.Add("minWMana", new Slider("最低蓝量 % 使用 W", 0, 0, 100));
                _minEMana = MenuManaManager.Add("minEMana", new Slider("最低蓝量 % 使用  E", 35, 0, 100));
                _minRMana = MenuManaManager.Add("minRMana", new Slider("最低蓝量 % 使用  R", 0, 0, 100));
            }

            public static void Initialize()
            {
            }
        }

        public static class PredictionMenu
        {
            private static readonly Menu MenuPrediction;
            private static readonly Slider _minWHCCombo;
            private static readonly Slider _minWHCKillSteal;

            public static HitChance MinWHCCombo
            {
                get { return Util.GetHitChanceSliderValue(_minWHCCombo); }
            }
            public static HitChance MinWHCKillSteal
            {
                get { return Util.GetHitChanceSliderValue(_minWHCKillSteal); }
            }

            static PredictionMenu()
            {
                MenuPrediction = Config.Menu.AddSubMenu("预判");
                MenuPrediction.AddLabel("这里可以调整技能的最低命中率，进行的计算检查.");
                MenuPrediction.AddGroupLabel("W 预判");
                MenuPrediction.AddGroupLabel("连招");
                _minWHCCombo = Util.CreateHitChanceSlider("comboMinWHitChance", "连招", HitChance.High, MenuPrediction);
                MenuPrediction.AddGroupLabel("抢头");
                _minWHCKillSteal = Util.CreateHitChanceSlider("killStealMinWHitChance", "抢头", HitChance.Medium, MenuPrediction);
            }

            public static void Initialize()
            {

            }
        }

        public static class DrawingMenu
        {
            private static readonly Menu MenuDrawing;
            private static readonly CheckBox _drawW;
            private static readonly CheckBox _drawE;
            private static readonly CheckBox _drawR;
            private static readonly CheckBox _drawIgnite;
            private static readonly CheckBox _drawOnlyReady;
            
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
            public static bool DrawIgnite
            {
                get { return _drawIgnite.CurrentValue; }
            }
            public static bool DrawOnlyReady
            {
                get { return _drawOnlyReady.CurrentValue; }
            }

            static DrawingMenu()
            {
                MenuDrawing = Config.Menu.AddSubMenu("线圈");
                _drawW = MenuDrawing.Add("drawW", new CheckBox("显示 W"));
                _drawE = MenuDrawing.Add("drawE", new CheckBox("显示 E"));
                _drawR = MenuDrawing.Add("drawR", new CheckBox("显示 R"));
                _drawIgnite = MenuDrawing.Add("drawIgnite", new CheckBox("显示 点燃"));
                _drawOnlyReady = MenuDrawing.Add("drawOnlyReady", new CheckBox("显示无冷却技能线圈"));
            }

            public static void Initialize()
            {
            }
        }

        public static class DebugMenu
        {
            private static readonly Menu MenuDebug;
            private static readonly CheckBox _debugChat;
            private static readonly CheckBox _debugConsole;

            public static bool DebugChat
            {
                get { return _debugChat.CurrentValue; }
            }
            public static bool DebugConsole
            {
                get { return _debugConsole.CurrentValue; }
            }

            static DebugMenu()
            {
                MenuDebug = Config.Menu.AddSubMenu("测试");
                MenuDebug.AddLabel("开发者模式.");
                _debugChat = MenuDebug.Add("debugChat", new CheckBox("在聊天显示测试信息", false));
                _debugConsole = MenuDebug.Add("debugConsole", new CheckBox("控制台显示测试信息", false));
            }

            public static void Initialize()
            {

            }
        }
    }
}
