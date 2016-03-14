using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberHidesStaticFromOuterClass
namespace PurifierVayne
{
    // I can't really help you with my layout of a good config class
    // since everyone does it the way they like it most, go checkout my
    // config classes I make on my GitHub if you wanna take over the
    // complex way that I use
    public static class Config
    {
        private const string MenuName = "淨化者薇恩";

        private static readonly Menu Menu;

        static Config()
        {
            // Initialize the menu
            Menu = MainMenu.AddMenu(MenuName, MenuName.ToLower());
            Menu.AddGroupLabel("欢迎使用淨化者薇恩 by TopGunner");
            Menu.AddGroupLabel("CARRY合集由CH汉化");
            Menu.AddGroupLabel("挂机的话请不要勾开局买装备以及逃跑Q/任何Q至鼠标的功能");


            // Initialize the modes
            Misc.Initialize();
            Modes.Initialize();
            ESettings.Initialize();
            QSettings.Initialize();

        }

        public static void Initialize()
        {
        }

        public static class QSettings
        {
            private static readonly Menu Menu;
            private static readonly CheckBox _useQToMouse;
            private static readonly CheckBox _gapcloserQ;
            private static readonly CheckBox _fleeQ;
            private static readonly Slider _defQ;
            private static readonly Slider _lowHPQ;
            private static readonly CheckBox _unkillableMinion;
            private static readonly CheckBox _useQToMouseCombo;
            private static readonly CheckBox _useQToMouseHarass;
            private static readonly CheckBox _useQToMouseLaneClear;
            private static readonly CheckBox _useQToMouseLastHit;

            public static bool useQToMouse
            {
                get { return _useQToMouse.CurrentValue; }
            }
            public static bool unkillableMinion
            {
                get { return _unkillableMinion.CurrentValue; }
            }
            public static bool comboQToMouse
            {
                get { return _useQToMouseCombo.CurrentValue; }
            }
            public static bool harassQToMouse
            {
                get { return _useQToMouseHarass.CurrentValue; }
            }
            public static bool laneClearQToMouse
            {
                get { return _useQToMouseLaneClear.CurrentValue; }
            }
            public static bool lastHitQToMouse
            {
                get { return _useQToMouseLastHit.CurrentValue; }
            }
            public static bool GapcloserQ
            {
                get { return _gapcloserQ.CurrentValue; }
            }
            public static bool fleeQ
            {
                get { return _fleeQ.CurrentValue; }
            }
            public static int defQ
            {
                get { return _defQ.CurrentValue; }
            }
            public static int lowHPQ
            {
                get { return _lowHPQ.CurrentValue; }
            }



            static QSettings()
            {
                Menu = Config.Menu.AddSubMenu("Q 设置");
                Menu.AddSeparator();
                _useQToMouse = Menu.Add("useQToMouse", new CheckBox("Q至鼠标位置", false));
                _gapcloserQ = Menu.Add("gapcloserQ", new CheckBox("使用Q造成间距"));
                _fleeQ = Menu.Add("fleeQ", new CheckBox("逃跑Q (至鼠标位置)"));
                _unkillableMinion = Menu.Add("unkillableMinion", new CheckBox("普攻无法杀死小兵时使用Q尾兵"));
                Menu.AddSeparator();
                _useQToMouseCombo = Menu.Add("useQToMouseCombo", new CheckBox("连招Q至鼠标位置", false));
                _useQToMouseHarass = Menu.Add("useQToMouseHarass", new CheckBox("骚扰Q至鼠标位置", false));
                _useQToMouseLaneClear = Menu.Add("useQToMouseLaneClear", new CheckBox("清线Q至鼠标位置", false));
                _useQToMouseLastHit = Menu.Add("useQToMouselastHit", new CheckBox("尾兵Q至鼠标位置", false));
                Menu.AddSeparator();
                _defQ = Menu.Add("defQ", new Slider("防守型 Q 当 ({0}) 个敌人多于友军", 1, 0, 4));
                _lowHPQ = Menu.Add("lowHPQ", new Slider("低血量 Q 当自身血量少于 ({0}%)", 10, 1, 100));
            }

            public static void Initialize()
            {
            }
        }
        public static class ESettings
        {
            private static readonly Menu Menu;
            private static readonly CheckBox _harassEProcW;
            private static readonly CheckBox _harassPinToWall;
            private static readonly CheckBox _comboEProcW;
            private static readonly CheckBox _comboPinToWall;
            public static readonly CheckBox _ksE;
            public static readonly CheckBox _interruptE;
            public static readonly CheckBox _useEOnGapcloser;

            public static bool harassEProcW
            {
                get { return _harassEProcW.CurrentValue; }
            }
            public static bool harassPinToWall
            {
                get { return _harassPinToWall.CurrentValue; }
            }
            public static bool comboEProcW
            {
                get { return _comboEProcW.CurrentValue; }
            }
            public static bool comboPinToWall
            {
                get { return _comboPinToWall.CurrentValue; }
            }
            public static bool ksE
            {
                get { return _ksE.CurrentValue; }
            }
            public static bool interruptE
            {
                get { return _interruptE.CurrentValue; }
            }
            public static bool useEOnGapcloser
            {
                get { return _useEOnGapcloser.CurrentValue; }
            }
            public static bool condemnAfterNextAA
            {
                get { return Menu["CondemnHotkey"].Cast<KeyBind>().CurrentValue; }
            }
            static ESettings()
            {
                Menu = Config.Menu.AddSubMenu("E 设置");
                Menu.AddGroupLabel("骚扰");
                _harassEProcW = Menu.Add("harassEProcW", new CheckBox("尝试叠加W骚扰", false));
                _harassPinToWall = Menu.Add("harassPinToWall", new CheckBox("骚扰定墙"));
                Menu.AddGroupLabel("连招");
                _comboEProcW = Menu.Add("comboEProcW", new CheckBox("尝试叠加W连招", false));
                _comboPinToWall = Menu.Add("comboPinToWall", new CheckBox("连招定墙"));
                Menu.AddGroupLabel("杂项");
                _interruptE = Menu.Add("interruptE", new CheckBox("使用E技能打断"));
                _ksE = Menu.Add("ksE", new CheckBox("E抢人头", false));
                _useEOnGapcloser = Menu.Add("useEOnGapcloser", new CheckBox("使用Q+E造成间距", false));
                Menu.Add("CondemnHotkey", new KeyBind("下一个AA后使用E", false, KeyBind.BindTypes.HoldActive, 'Y'));
            }

            public static void Initialize()
            {
            }
        }

        public static class Misc
        {

            private static readonly Menu Menu;
            public static readonly CheckBox _drawQ;
            public static readonly CheckBox _drawE;
            public static readonly CheckBox _drawReady;
            private static readonly CheckBox _useHeal;
            private static readonly CheckBox _useQSS;
            private static readonly CheckBox _useQOnGapcloser;
            private static readonly CheckBox _autoBuyStartingItems;
            private static readonly CheckBox _autolevelskills;
            private static readonly Slider _skinId;
            public static readonly CheckBox _useSkinHack;
            private static readonly CheckBox _cleanseStun;
            private static readonly Slider _cleanseEnemies;
            private static readonly CheckBox[] _useHealOn = { new CheckBox("", false), new CheckBox("", false), new CheckBox("", false), new CheckBox("", false), new CheckBox("", false) };

            public static bool useHealOnI(int i)
            {
                return _useHealOn[i].CurrentValue;
            }
            public static bool useHeal
            {
                get { return _useHeal.CurrentValue; }
            }
            public static bool useQSS
            {
                get { return _useQSS.CurrentValue; }
            }
            public static bool useQOnGapcloser
            {
                get { return _useQOnGapcloser.CurrentValue; }
            }
            public static bool autoBuyStartingItems
            {
                get { return _autoBuyStartingItems.CurrentValue; }
            }
            public static bool autolevelskills
            {
                get { return _autolevelskills.CurrentValue; }
            }
            public static int skinId
            {
                get { return _skinId.CurrentValue; }
            }
            public static bool UseSkinHack
            {
                get { return _useSkinHack.CurrentValue; }
            }
            public static int cleanseEnemies
            {
                get { return _cleanseEnemies.CurrentValue; }
            }
            public static bool cleanseStun
            {
                get { return _cleanseStun.CurrentValue; }
            }
            public static bool drawReady
            {
                get { return _drawReady.CurrentValue; }
            }


            static Misc()
            {
                // Initialize the menu values
                Menu = Config.Menu.AddSubMenu("杂项");
                _drawQ = Menu.Add("drawQ", new CheckBox("显示 Q"));
                _drawE = Menu.Add("drawE", new CheckBox("显示 E"));
                _drawReady = Menu.Add("drawReady", new CheckBox("显示无冷却技能线圈"));
                Menu.AddSeparator();
                _useHeal = Menu.Add("useHeal", new CheckBox("使用治疗"));
                _useQSS = Menu.Add("useQSS", new CheckBox("使用水银"));
                Menu.AddSeparator();
                for (int i = 0; i < EntityManager.Heroes.Allies.Count; i++)
                {
                    _useHealOn[i] = Menu.Add("useHeal" + i, new CheckBox("对友军使用治疗 " + EntityManager.Heroes.Allies[i].ChampionName));
                }
                Menu.AddSeparator();
                _useQOnGapcloser = Menu.Add("useQOnGapcloser", new CheckBox("使用Q造成间距", false));
                Menu.AddSeparator();
                _autolevelskills = Menu.Add("autolevelskills", new CheckBox("自动加点"));
                _autoBuyStartingItems = Menu.Add("autoBuyStartingItems", new CheckBox("开局自动买物品 (召唤师峡谷)", false));
                Menu.AddSeparator();
                _useSkinHack = Menu.Add("useSkinHack", new CheckBox("换肤", false));
                _skinId = Menu.Add("skinId", new Slider("Skin ID", 6, 1, 10));
            }

            public static void Initialize()
            {
            }

        }

        public static class Modes
        {
            private static readonly Menu Menu;

            static Modes()
            {
                // Initialize the menu
                Menu = Config.Menu.AddSubMenu("模式");

                // Initialize all modes
                // Combo
                Combo.Initialize();
                Menu.AddSeparator();

                // Harass
                Harass.Initialize();
                LaneClear.Initialize();
                JungleClear.Initialize();
            }

            public static void Initialize()
            {
            }

            public static class Combo
            {
                private static readonly CheckBox _useQ;
                private static readonly CheckBox _useE;
                private static readonly CheckBox _useR;
                private static readonly Slider _useREnemies;
                private static readonly CheckBox _useBOTRK;
                private static readonly CheckBox _useYOUMOUS;
                private static readonly CheckBox _useWardVision;
                private static readonly CheckBox _useTrinketVision;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }
                public static bool UseE
                {
                    get { return _useE.CurrentValue; }
                }
                public static bool UseR
                {
                    get { return _useR.CurrentValue; }
                }
                public static int UseREnemies
                {
                    get { return _useREnemies.CurrentValue; }
                }
                public static bool useWardVision
                {
                    get { return _useWardVision.CurrentValue; }
                }
                public static bool useTrinketVision
                {
                    get { return _useTrinketVision.CurrentValue; }
                }
                public static bool useBOTRK
                {
                    get { return _useBOTRK.CurrentValue; }
                }
                public static bool useYOUMOUS
                {
                    get { return _useYOUMOUS.CurrentValue; }
                }


                static Combo()
                {
                    // Initialize the menu values
                    Menu.AddGroupLabel("连招");
                    _useQ = Menu.Add("comboUseQ", new CheckBox("使用 Q"));
                    _useE = Menu.Add("comboUseE", new CheckBox("使用 E"));
                    _useR = Menu.Add("comboUseR", new CheckBox("使用 R"));
                    _useREnemies = Menu.Add("useREnemies", new Slider("附近有 X名敌人连招使用R", 2, 1, 5));
                    _useBOTRK = Menu.Add("useBotrk", new CheckBox("使用破败（智能）和小破败"));
                    _useYOUMOUS = Menu.Add("useYoumous", new CheckBox("使用幽梦"));
                    _useWardVision = Menu.Add("useWardVision", new CheckBox("插眼视野"));
                    _useTrinketVision = Menu.Add("useTrinketVision", new CheckBox("使用饰品视野"));
                }

                public static void Initialize()
                {
                }
            }

            public static class Harass
            {
                public static bool UseQ
                {
                    get { return Menu["harassUseQ"].Cast<CheckBox>().CurrentValue; }
                }
                public static bool UseE
                {
                    get { return Menu["harassUseE"].Cast<CheckBox>().CurrentValue; }
                }
                public static int Mana
                {
                    get { return Menu["harassMana"].Cast<Slider>().CurrentValue; }
                }

                static Harass()
                {
                    // Here is another option on how to use the menu, but I prefer the
                    // way that I used in the combo class
                    Menu.AddGroupLabel("骚扰");
                    Menu.Add("harassUseQ", new CheckBox("使用 Q"));
                    Menu.Add("harassUseE", new CheckBox("使用 E"));

                    // Adding a slider, we have a little more options with them, using {0} {1} and {2}
                    // in the display name will replace it with 0=current 1=min and 2=max value
                    Menu.Add("harassMana", new Slider("最大蓝量使用百分比 ({0}%)", 40));
                }
                public static void Initialize()
                {
                }
            }

            public static class LaneClear
            {
                private static readonly CheckBox _useQ;
                private static readonly Slider _mana;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }
                public static int mana
                {
                    get { return _mana.CurrentValue; }
                }

                static LaneClear()
                {
                    // Initialize the menu values
                    Menu.AddGroupLabel("清线");
                    _useQ = Menu.Add("clearUseQ", new CheckBox("使用 Q"));
                    _mana = Menu.Add("clearMana", new Slider("最大蓝量使用百分比 ({0}%)", 40));
                }

                public static void Initialize()
                {
                }
            }

            public static class JungleClear
            {
                private static readonly CheckBox _useQ;
                private static readonly CheckBox _useE;
                private static readonly Slider _mana;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }
                public static bool UseE
                {
                    get { return _useE.CurrentValue; }
                }
                public static int mana
                {
                    get { return _mana.CurrentValue; }
                }

                static JungleClear()
                {
                    // Initialize the menu values
                    Menu.AddGroupLabel("清野");
                    _useQ = Menu.Add("jglUseQ", new CheckBox("使用 Q"));
                    _mana = Menu.Add("jglMana", new Slider("最大蓝量使用百分比 ({0}%)", 40));
                    _useE = Menu.Add("jglUseE", new CheckBox("使用 E"));
                }

                public static void Initialize()
                {
                }
            }
        }
    }
}
