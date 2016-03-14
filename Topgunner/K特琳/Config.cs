using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberHidesStaticFromOuterClass
namespace Kitelyn
{
    // I can't really help you with my layout of a good config class
    // since everyone does it the way they like it most, go checkout my
    // config classes I make on my GitHub if you wanna take over the
    // complex way that I use
    public static class Config
    {
        private const string MenuName = "K特琳";

        private static readonly Menu Menu;

        static Config()
        {
            // Initialize the menu
            Menu = MainMenu.AddMenu(MenuName, MenuName.ToLower());
            Menu.AddGroupLabel("Welcome to Kitelyn by TopGunner");
            Menu.AddGroupLabel("CARRY合集由CH汉化");
            Menu.AddGroupLabel("女警是挂机神器！");

            // Initialize the modes
            Modes.Initialize();

        }

        public static void Initialize()
        {
        }


        public static class Misc
        {

            private static readonly Menu Menu;
            public static readonly CheckBox _drawQ;
            public static readonly CheckBox _drawW;
            public static readonly CheckBox _drawE;
            public static readonly CheckBox _drawR;
            public static readonly CheckBox _drawCombo;
            private static readonly CheckBox _useR;
            private static readonly CheckBox _useRAlways;
            private static readonly CheckBox _useScryingOrbMarker;
            private static readonly CheckBox _useHeal;
            private static readonly CheckBox _useQSS;
            private static readonly CheckBox _useWOnTP;
            private static readonly CheckBox _useWOnZhonyas;
            private static readonly CheckBox _useWOnGapcloser;
            private static readonly CheckBox _useEOnGapcloser;
            public static readonly CheckBox _useEFlee;
            private static readonly CheckBox _autoBuyStartingItems;
            private static readonly CheckBox _autolevelskills;
            private static readonly Slider _skinId;
            public static readonly CheckBox _useSkinHack;
            private static readonly CheckBox _cleanseStun;
            private static readonly Slider _cleanseEnemies;
            private static readonly CheckBox _forceAAOnTrap;
            private static readonly CheckBox[] _useHealOn = { new CheckBox("", false), new CheckBox("", false), new CheckBox("", false), new CheckBox("", false), new CheckBox("", false) };

            public static bool useHealOnI(int i)
            {
                return _useHealOn[i].CurrentValue;
            }
            public static bool UseR
            {
                get { return _useR.CurrentValue; }
            }
            public static bool UseRAlways
            {
                get { return _useRAlways.CurrentValue; }
            }
            public static bool useScryingOrbMarker
            {
                get { return _useScryingOrbMarker.CurrentValue; }
            }
            public static bool useHeal
            {
                get { return _useHeal.CurrentValue; }
            }
            public static bool useQSS
            {
                get { return _useQSS.CurrentValue; }
            }
            public static bool useWOnTP
            {
                get { return _useWOnTP.CurrentValue; }
            }
            public static bool useWOnZhonyas
            {
                get { return _useWOnZhonyas.CurrentValue; }
            }
            public static bool useWOnGapcloser
            {
                get { return _useWOnGapcloser.CurrentValue; }
            }
            public static bool useEOnGapcloser
            {
                get { return _useEOnGapcloser.CurrentValue; }
            }
            public static bool useEFlee
            {
                get { return _useEFlee.CurrentValue; }
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
            public static bool drawComboDmg
            {
                get { return _drawCombo.CurrentValue; }
            }
            public static bool forceAAOnTrap
            {
                get { return _forceAAOnTrap.CurrentValue; }
            }


            static Misc()
            {
                // Initialize the menu values
                Menu = Config.Menu.AddSubMenu("杂项");
                _drawQ = Menu.Add("drawQ", new CheckBox("显示 Q"));
                _drawW = Menu.Add("drawW", new CheckBox("显示 W"));
                _drawE = Menu.Add("drawE", new CheckBox("显示 E"));
                _drawR = Menu.Add("drawR", new CheckBox("显示 R"));
                _drawCombo = Menu.Add("drawCombo", new CheckBox("显示 连招伤害"));
                Menu.AddSeparator();
                _useR = Menu.Add("useR", new CheckBox("使用 R 杀死距离外的敌人"));
                _useRAlways = Menu.Add("useRAlways", new CheckBox("总是使用R如果可杀死", false));
                _useScryingOrbMarker = Menu.Add("useScryingOrbMarker", new CheckBox("使用视野眼照亮敌人作为R预备"));
                Menu.AddSeparator();
                _useHeal = Menu.Add("useHeal", new CheckBox("智能使用治疗"));
                _useQSS = Menu.Add("useQSS", new CheckBox("使用水银"));
                Menu.AddSeparator();
                for (int i = 0; i < EntityManager.Heroes.Allies.Count; i++)
                {
                    _useHealOn[i] = Menu.Add("useHeal" + i, new CheckBox("使用治疗给 " + EntityManager.Heroes.Allies[i].ChampionName));
                }
                Menu.AddSeparator();
                _useWOnTP = Menu.Add("useWOnTP", new CheckBox("给传送的敌人放置W"));
                _useWOnZhonyas = Menu.Add("useWOnZhonyas", new CheckBox("给中亚的敌人放置W"));
                _useWOnGapcloser = Menu.Add("useWOnGapcloser", new CheckBox("使用W造成间距"));
                _useEOnGapcloser = Menu.Add("useEOnGapcloser", new CheckBox("使用E造成间距", false));
                _useEFlee = Menu.Add("useEFlee", new CheckBox("逃跑模式使用E至鼠标"));
                _forceAAOnTrap = Menu.Add("forceAAOnTrap", new CheckBox("优先攻击陷阱内的敌人"));
                Menu.AddSeparator();
                _autolevelskills = Menu.Add("autolevelskills", new CheckBox("自动加点"));
                _autoBuyStartingItems = Menu.Add("autoBuyStartingItems", new CheckBox("开局自动购买物品 (召唤师峡谷)"));
                Menu.AddSeparator();
                _useSkinHack = Menu.Add("useSkinHack", new CheckBox("换肤"));
                _skinId = Menu.Add("skinId", new Slider("Skin ID", 6, 1, 13));
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
                private static readonly CheckBox _useQNotStunned;
                private static readonly CheckBox _useQStunned;
                private static readonly CheckBox _useW;
                private static readonly CheckBox _useE;
                private static readonly CheckBox _useBOTRK;
                private static readonly CheckBox _useYOUMOUS;
                private static readonly CheckBox _useWVision;
                private static readonly CheckBox _useWardVision;
                private static readonly CheckBox _useTrinketVision;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }
                public static bool UseQNotStunned
                {
                    get { return _useQNotStunned.CurrentValue; }
                }
                public static bool UseQStunned
                {
                    get { return _useQStunned.CurrentValue; }
                }
                public static int ManaQAlways
                {
                    get { return Menu["comboManaQAlways"].Cast<Slider>().CurrentValue; }
                }
                public static bool UseW
                {
                    get { return _useW.CurrentValue; }
                }
                public static bool useWVision
                {
                    get { return _useWVision.CurrentValue; }
                }
                public static bool useWardVision
                {
                    get { return _useWardVision.CurrentValue; }
                }
                public static bool useTrinketVision
                {
                    get { return _useTrinketVision.CurrentValue; }
                }
                public static bool UseE
                {
                    get { return _useE.CurrentValue; }
                }
                public static bool useBOTRK
                {
                    get { return _useBOTRK.CurrentValue; }
                }
                public static bool useYOUMOUS
                {
                    get { return _useYOUMOUS.CurrentValue; }
                }
                public static int StockW
                {
                    get { return Menu["comboStockW"].Cast<Slider>().CurrentValue; }
                }


                static Combo()
                {
                    // Initialize the menu values
                    Menu.AddGroupLabel("连招");
                    _useQ = Menu.Add("comboUseQ", new CheckBox("使用 Q"));
                    _useW = Menu.Add("comboUseW", new CheckBox("智能W"));
                    Menu.Add("comboStockW", new Slider("保留 X 陷阱", 1, 0, 5));
                    _useQNotStunned = Menu.Add("comboUseQNotStunned", new CheckBox("总使用Q", false));
                    _useQStunned = Menu.Add("comboUseQStunned", new CheckBox("总对晕眩的目标使用Q（就算伤害不高）"));
                    Menu.Add("comboManaQAlways", new Slider("总使用Q当蓝量 > ", 75, 0, 100));
                    _useE = Menu.Add("comboUseE", new CheckBox("使用智能E"));
                    _useBOTRK = Menu.Add("useBotrk", new CheckBox("使用 破败（智能）和弯刀"));
                    _useYOUMOUS = Menu.Add("useYoumous", new CheckBox("使用 幽梦"));
                    _useWVision = Menu.Add("useWVision", new CheckBox("使用W做视野"));
                    _useWardVision = Menu.Add("useWardVision", new CheckBox("使用眼做视野"));
                    _useTrinketVision = Menu.Add("useTrinketVision", new CheckBox("使用饰品做视野"));
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
                public static bool UseQNotStunned
                {
                    get { return Menu["harassUseQNotStunned"].Cast<CheckBox>().CurrentValue; }
                }
                public static bool UseQStunned
                {
                    get { return Menu["harassUseQStunned"].Cast<CheckBox>().CurrentValue; }
                }
                public static int ManaQAlways
                {
                    get { return Menu["harassManaQAlways"].Cast<Slider>().CurrentValue; }
                }
                public static bool UseW
                {
                    get { return Menu["harassUseW"].Cast<CheckBox>().CurrentValue; }
                }
                public static bool UseE
                {
                    get { return Menu["harassUseE"].Cast<CheckBox>().CurrentValue; }
                }
                public static bool UseR
                {
                    get { return Menu["harassUseR"].Cast<CheckBox>().CurrentValue; }
                }
                public static int Mana
                {
                    get { return Menu["harassMana"].Cast<Slider>().CurrentValue; }
                }
                public static int StockW
                {
                    get { return Menu["harassStockW"].Cast<Slider>().CurrentValue; }
                }

                static Harass()
                {
                    // Here is another option on how to use the menu, but I prefer the
                    // way that I used in the combo class
                    Menu.AddGroupLabel("骚扰");
                    Menu.Add("harassUseQ", new CheckBox("使用 Q"));
                    Menu.Add("harassUseQNotStunned", new CheckBox("总使用Q"));
                    Menu.Add("harassUseQStunned", new CheckBox("总对晕眩的目标使用Q（就算伤害不高）"));
                    Menu.Add("harassManaQAlways", new Slider("总使用Q当蓝量 > ", 75, 0, 100));
                    Menu.Add("harassUseW", new CheckBox("使用智能W"));
                    Menu.Add("harassStockW", new Slider("保留 X 陷阱", 1, 0, 5));
                    Menu.Add("harassUseR", new CheckBox("使用 R", false)); // Default false

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
                private static readonly Slider _mana;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
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
                }

                public static void Initialize()
                {
                }
            }
        }
    }
}
