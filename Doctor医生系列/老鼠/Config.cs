using System;
using EloBuddy;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberHidesStaticFromOuterClass

namespace Twitch
{
    public static class Config
    {
        private const string MenuName = "CH汉化-Dr图奇";

        private static readonly Menu Menu;

        static Config()
        {
            Menu = MainMenu.AddMenu(MenuName, MenuName.ToLower());
            Menu.AddGroupLabel("Doctor 瘟疫之鼠");
            Menu.AddLabel("Halo!");
            Menu.AddLabel("Good Luck.");
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
                private static readonly Slider _minEStacks;
                private static readonly Slider _minREnemies;
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

                public static int MinEStacks
                {
                    get { return _minEStacks.CurrentValue; }
                }

                public static int MinREnemies
                {
                    get { return _minREnemies.CurrentValue; }
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
                    _useW = MenuModes.Add("comboUseW", new CheckBox("使用 W"));
                    _useE = MenuModes.Add("comboUseE", new CheckBox("使用 E"));
                    _useR = MenuModes.Add("comboUseR", new CheckBox("使用 R"));
                    _useItems = MenuModes.Add("comboUseItems", new CheckBox("使用 弯刀/破败/幽梦"));
                    _minEStacks = MenuModes.Add("comboMinEStacks",
                        new Slider("最低叠加层数使用 E", 6, 1, 6));
                    _minREnemies = MenuModes.Add("comboMinREnemies",
                        new Slider("最低附近敌人数量使用 R", 3, 1, 5));
                    _maxBOTRKHPPlayer = MenuModes.Add("comboMaxBotrkHpPlayer",
                        new Slider("玩家最大生命 % 时使用破败", 80, 0, 100));
                    _maxBOTRKHPEnemy = MenuModes.Add("comboMaxBotrkHpEnemy",
                        new Slider("敌人最大生命 % 时使用破败", 80, 0, 100));
                }

                public static void Initialize()
                {
                }
            }

            public static class Harass
            {
                private static readonly CheckBox _useW;
                private static readonly CheckBox _useE;
                private static readonly Slider _minEStacks;
                
                public static bool UseW
                {
                    get { return _useW.CurrentValue; }
                }

                public static bool UseE
                {
                    get { return _useE.CurrentValue; }
                }

                public static int MinEStacks
                {
                    get { return _minEStacks.CurrentValue; }
                }

                static Harass()
                {
                    MenuModes.AddGroupLabel("骚扰");
                    _useW = MenuModes.Add("harassUseQ", new CheckBox("使用 W"));
                    _useE = MenuModes.Add("harassUseE", new CheckBox("使用 E"));
                    _minEStacks = MenuModes.Add("harassMinEStacks",
                        new Slider("最低叠加层数使用 E", 4, 1, 6));
                }

                public static void Initialize()
                {
                }
            }

            public static class LaneClear
            {
                private static readonly CheckBox _useW;
                private static readonly CheckBox _useE;
                private static readonly Slider _minWTargets;
                private static readonly Slider _minETargets;

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

                static LaneClear()
                {
                    MenuModes.AddGroupLabel("清线");
                    _useW = MenuModes.Add("laneUseW", new CheckBox("使用W"));
                    _useE = MenuModes.Add("laneUseE", new CheckBox("使用E"));
                    _minWTargets = MenuModes.Add("minWTargetsLC", new Slider("最少 X 目标使用 W", 4, 1, 10));
                    _minETargets = MenuModes.Add("minETargetsLC", new Slider("最少 X 目标使用 E", 4, 1, 10));
                }

                public static void Initialize()
                {
                }
            }

            public static class JungleClear
            {
                private static readonly CheckBox _useW;
                private static readonly CheckBox _useE;
                private static readonly Slider _minWTargets;
                private static readonly Slider _minETargets;
                private static readonly Slider _minEStacks;

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

                public static int MinEStacks
                {
                    get { return _minEStacks.CurrentValue; }
                }

                static JungleClear()
                {
                    MenuModes.AddGroupLabel("清野");
                    _useW = MenuModes.Add("jungleUseW", new CheckBox("使用W"));
                    _useE = MenuModes.Add("jungleUseE", new CheckBox("使用E"));
                    _minWTargets = MenuModes.Add("minWTargetsJC", new Slider("最少 X 目标使用 W", 2, 1, 10));
                    _minETargets = MenuModes.Add("minETargetsJC", new Slider("最少 X 目标使用  E", 2, 1, 10));
                    _minEStacks = MenuModes.Add("minEStacksJC", new Slider("每个目标最少层数使用E", 2, 1, 6));
                }

                public static void Initialize()
                {
                }


            }

            public static class Flee
            {
                private static readonly CheckBox _useQ;
                private static readonly CheckBox _useW;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }
                
                public static bool UseW
                {
                    get { return _useW.CurrentValue; }
                }

                static Flee()
                {
                    MenuModes.AddGroupLabel("逃跑");
                    _useQ = MenuModes.Add("fleeUseQ", new CheckBox("使用Q"));
                    _useW = MenuModes.Add("fleeUseW", new CheckBox("使用W", false));
                }

                public static void Initialize()
                {
                }
            }
        }

        public static class MiscMenu
        {
            private static readonly Menu MenuMisc;
            private static readonly CheckBox _gapcloserW;
            private static readonly CheckBox _potion;
            private static readonly CheckBox _ksE;
            private static readonly CheckBox _ksIgnite;
            private static readonly CheckBox _autoQ;
            private static readonly CheckBox _stealthRecall;
            private static readonly CheckBox _showHPBar;
            private static readonly CheckBox _showStats;
            private static readonly CheckBox _showPercentage;
            private static readonly CheckBox _calculateQ;
            private static readonly CheckBox _calculateW;
            private static readonly CheckBox _calculateE;
            private static readonly CheckBox _calculateR;
            private static readonly Slider _potionMinHP;
            private static readonly Slider _potionMinMP;
            private static readonly Slider _autoQMinEnemies;
            
            public static bool GapcloserUseW
            {
                get { return _gapcloserW.CurrentValue; }
            }
            public static bool KsE
            {
                get { return _ksE.CurrentValue; }
            }
            public static bool KsIgnite
            {
                get { return _ksIgnite.CurrentValue; }
            }
            public static bool AutoQ
            {
                get { return _autoQ.CurrentValue; }
            }
            public static bool Potion
            {
                get { return _potion.CurrentValue; }
            }
            public static bool StealthRecall
            {
                get { return _stealthRecall.CurrentValue; }
            }
            public static bool ShowDamageIndicator
            {
                get { return _showHPBar.CurrentValue; }
            }
            public static bool ShowStats
            {
                get { return _showStats.CurrentValue; }
            }
            public static bool ShowPercentage
            {
                get { return _showPercentage.CurrentValue; }
            }
            public static bool CalculateQ
            {
                get { return _calculateQ.CurrentValue; }
            }
            public static bool CalculateW
            {
                get { return _calculateW.CurrentValue; }
            }
            public static bool CalculateE
            {
                get { return _calculateE.CurrentValue; }
            }
            public static bool CalculateR
            {
                get { return _calculateR.CurrentValue; }
            }
            public static int potionMinHP
            {
                get { return _potionMinHP.CurrentValue; }
            }
            public static int potionMinMP
            {
                get { return _potionMinMP.CurrentValue; }
            }
            public static int AutoQMinEnemies
            {
                get { return _autoQMinEnemies.CurrentValue; }
            }

            static MiscMenu()
            {
                MenuMisc = Config.Menu.AddSubMenu("杂项");
                MenuMisc.AddGroupLabel("间距");
                _gapcloserW = MenuMisc.Add("gapcloserW", new CheckBox("使用W造成间距（防突进）", false));
                MenuMisc.AddGroupLabel("抢头");
                _ksE = MenuMisc.Add("ksE", new CheckBox("抢头 E"));
                _ksIgnite = MenuMisc.Add("ksIgnite", new CheckBox("点燃抢头"));
                MenuMisc.AddGroupLabel("智能Q");
                _autoQ = MenuMisc.Add("autoQ", new CheckBox("当 X 名敌人在附近使用Q", false));
                _autoQMinEnemies = MenuMisc.Add("autoQMinEnemiesAround", new Slider("最低附近敌人数量使用", 3, 1, 5));
                MenuMisc.AddGroupLabel("自动喝药");
                _potion = MenuMisc.Add("potion", new CheckBox("使用药水"));
                _potionMinHP = MenuMisc.Add("potionminHP", new Slider("最低血量 % 使用药水", 70));
                _potionMinMP = MenuMisc.Add("potionMinMP", new Slider("最低蓝量 % 使用药水", 20));
                MenuMisc.AddGroupLabel("其他");
                _stealthRecall = MenuMisc.Add("stealthRecall", new CheckBox("隐身回城", false));
                MenuMisc.AddGroupLabel("伤害显示");
                _showHPBar = MenuMisc.Add("showHPBar", new CheckBox("显示生命血条"));
                _showStats = MenuMisc.Add("showStats", new CheckBox("显示层数", false));
				_showPercentage = MenuMisc.Add("showPercentage", new CheckBox("显示百分比"));
				MenuMisc.AddGroupLabel("计算");
				_calculateQ = MenuMisc.Add("calculateQ", new CheckBox("计算Q", false));
				_calculateW = MenuMisc.Add("calculateW", new CheckBox("计算W", false));
				_calculateE = MenuMisc.Add("calculateE", new CheckBox("计算E", false));
				_calculateR = MenuMisc.Add("calculateR", new CheckBox("计算R", false));
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
                _minQMana = MenuManaManager.Add("minQMana", new Slider("最低蓝量 % 使用 Q", 0, 0, 100));
                _minWMana = MenuManaManager.Add("minWMana", new Slider("最低蓝量 % 使用 W", 50, 0, 100));
                _minEMana = MenuManaManager.Add("minEMana", new Slider("最低蓝量 % 使用 E", 0, 0, 100));
                _minRMana = MenuManaManager.Add("minRMana", new Slider("最低蓝量 % 使用 R", 10, 0, 100));
            }

            public static void Initialize()
            {
            }
        }

        public static class DrawingMenu
        {
            private static readonly Menu MenuDrawing;
            private static readonly CheckBox _drawQ;
            private static readonly CheckBox _drawW;
            private static readonly CheckBox _drawE;
            private static readonly CheckBox _drawR;
            private static readonly CheckBox _drawIgnite;
            private static readonly CheckBox _drawOnlyReady;

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
            public static bool DrawOnlyReady
            {
                get { return _drawOnlyReady.CurrentValue; }
            }
            public static bool DrawIgnite
            {
                get { return _drawIgnite.CurrentValue; }
            }

            static DrawingMenu()
            {
                MenuDrawing = Config.Menu.AddSubMenu("线圈");
                _drawQ = MenuDrawing.Add("drawQ", new CheckBox("显示 Q"));
                _drawW = MenuDrawing.Add("drawW", new CheckBox("显示 W"));
                _drawE = MenuDrawing.Add("drawE", new CheckBox("显示 E"));
                _drawR = MenuDrawing.Add("drawR", new CheckBox("显示 R"));
                _drawIgnite = MenuDrawing.Add("drawIgnite", new CheckBox("显示点燃"));
                _drawOnlyReady = MenuDrawing.Add("drawOnlyReady", new CheckBox("只显示无冷却技能"));
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
                MenuDebug = Config.Menu.AddSubMenu("调试");
                MenuDebug.AddLabel("开发者模式.");
                _debugChat = MenuDebug.Add("debugChat", new CheckBox("在聊天显示调试信息", false));
                _debugConsole = MenuDebug.Add("debugConsole", new CheckBox("控制台显示调试信息", false));
            }

            public static void Initialize()
            {

            }
        }

        public static class PredictionMenu
        {
            private static readonly Menu MenuPrediction;
            private static readonly Slider _minWHCCombo;
            private static readonly Slider _minWHCHarass;
            private static readonly Slider _minWHCFlee;

            public static HitChance MinWHCCombo
            {
                get { return Util.GetHCSliderHitChance(_minWHCCombo); }
            }

            public static HitChance MinWHCHarass
            {
                get { return Util.GetHCSliderHitChance(_minWHCHarass); }
            }

            public static HitChance MinWHCFlee
            {
                get { return Util.GetHCSliderHitChance(_minWHCFlee); }
            }

            static PredictionMenu()
            {
                MenuPrediction = Config.Menu.AddSubMenu("预判");
                MenuPrediction.AddLabel("这里可以调整技能的最低命中率，进行的计算检查.");
                MenuPrediction.AddGroupLabel("W 预判");
                MenuPrediction.AddGroupLabel("连招");
                _minWHCCombo = Util.CreateHCSlider("comboMinWHitChance", "连招", HitChance.High, MenuPrediction);
                MenuPrediction.AddGroupLabel("骚扰");
                _minWHCHarass = Util.CreateHCSlider("harassMinWHitChance", "骚扰", HitChance.High, MenuPrediction);
                MenuPrediction.AddGroupLabel("逃跑");
                _minWHCFlee = Util.CreateHCSlider("fleeMinWHitChance", "逃跑", HitChance.Low, MenuPrediction);
            }

            public static void Initialize()
            {

            }
        }
    }
}
