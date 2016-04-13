using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using Color = System.Drawing.Color;

// ReSharper disable ConvertPropertyToExpressionBody
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming
// ReSharper disable MemberHidesStaticFromOuterClass

namespace KA_Ezreal
{
    public static class Config
    {
        private static readonly string MenuName = "KA " + Player.Instance.ChampionName + "CH汉化";

        private static readonly Menu Menu;

        static Config()
        {
            Menu = MainMenu.AddMenu(MenuName, MenuName.ToLower());
            Menu.AddGroupLabel("KA " + Player.Instance.ChampionName);
            Modes.Initialize();
        }

        public static void Initialize()
        {
        }

        public static class Modes
        {
            private static readonly Menu SpellsMenu, FarmMenu, MiscMenu, DrawMenu;

            static Modes()
            {
                SpellsMenu = Menu.AddSubMenu("::技能菜单::");
                Combo.Initialize();
                Harass.Initialize();

                FarmMenu = Menu.AddSubMenu("::尾兵菜单::");
                LaneClear.Initialize();
                LastHit.Initialize();

                MiscMenu = Menu.AddSubMenu("::杂项::");
                Misc.Initialize();

                DrawMenu = Menu.AddSubMenu("::线圈::");
                Draw.Initialize();
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
                private static readonly Slider _minR;

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

                public static int MinR
                {
                    get { return _minR.CurrentValue; }
                }

                static Combo()
                {
                    // Initialize the menu values
                    SpellsMenu.AddGroupLabel("连招技能:");
                    _useQ = SpellsMenu.Add("comboQ", new CheckBox("使用Q连招 ?"));
                    _useW = SpellsMenu.Add("comboW", new CheckBox("使用W连招 ?"));
                    _useE = SpellsMenu.Add("comboE", new CheckBox("使用E连招 ?"));
                    _useR = SpellsMenu.Add("comboR", new CheckBox("使用R连招 ?"));
                    _minR = SpellsMenu.Add("minR", new Slider("最低敌人数量使用R", 2, 0, 5));
                }

                public static void Initialize()
                {
                }
            }

            public static class Harass
            {
                private static readonly CheckBox _useQ;
                private static readonly CheckBox _useW;
                private static readonly Slider _manaHarass;
                //AutoHarass
                private static readonly CheckBox _autouseQ;
                private static readonly CheckBox _autouseW;
                private static readonly Slider _automanaQ;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }

                public static bool UseW
                {
                    get { return _useW.CurrentValue; }
                }

                public static int ManaHarass
                {
                    get { return _manaHarass.CurrentValue; }
                }

                //AutoHarass
                public static bool UseQAuto
                {
                    get { return _autouseQ.CurrentValue; }
                }

                public static bool UseWAuto
                {
                    get { return _autouseW.CurrentValue; }
                }

                public static int ManaAutoHarass
                {
                    get { return _automanaQ.CurrentValue; }
                }

                static Harass()
                {
                    SpellsMenu.AddGroupLabel("骚扰技能:");
                    _useQ = SpellsMenu.Add("harassQ", new CheckBox("使用Q骚扰 ?"));
                    _useW = SpellsMenu.Add("harassW", new CheckBox("使用W骚扰 ?"));
                    SpellsMenu.AddGroupLabel("骚扰设置:");
                    _manaHarass = SpellsMenu.Add("harassMana", new Slider("蓝量高于({0})时才使用骚扰技能.", 30));
                    SpellsMenu.AddGroupLabel("自动骚扰技能:");
                    //Add Key bind
                    _autouseQ = SpellsMenu.Add("autoHarassQ", new CheckBox("Q自动骚扰 ?"));
                    _autouseW = SpellsMenu.Add("autoHarassW", new CheckBox("W自动骚扰 ?"));
                    SpellsMenu.AddGroupLabel("自动骚扰设置:");
                    _automanaQ = SpellsMenu.Add("autoHarassMana", new Slider("蓝量高于({0})时才使用自动骚扰.", 50));
                }

                public static void Initialize()
                {
                }
            }

            public static class LaneClear
            {
                private static readonly CheckBox _useQ;
                private static readonly Slider _laneMana;
                private static readonly CheckBox _useQJungle;
                private static readonly Slider _jungleMana;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }

                public static int LaneMana
                {
                    get { return _laneMana.CurrentValue; }
                }

                public static bool UseQjungle
                {
                    get { return _useQJungle.CurrentValue; }
                }

                public static int JungleMana
                {
                    get { return _jungleMana.CurrentValue; }
                }

                static LaneClear()
                {
                    FarmMenu.AddGroupLabel("清线技能:");
                    _useQ = FarmMenu.Add("laneclearQ", new CheckBox("使用Q清线 ?"));
                    FarmMenu.AddGroupLabel("清线设置:");
                    _laneMana = FarmMenu.Add("laneMana", new Slider("蓝量高于({0})时才使用清线技能.", 30));
                    FarmMenu.AddGroupLabel("清野技能:");
                    _useQJungle = FarmMenu.Add("jungleclearQ", new CheckBox("清野使用Q ?"));
                    FarmMenu.AddGroupLabel("清野设置:");
                    _jungleMana = FarmMenu.Add("jungleMana", new Slider("蓝量高于({0})时才使用清野技能.", 30));
                }

                public static void Initialize()
                {
                }
            }

            public static class LastHit
            {
                private static readonly CheckBox _useQ;
                private static readonly Slider _lastMana;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }

                public static int LastMana
                {
                    get { return _lastMana.CurrentValue; }
                }


                static LastHit()
                {
                    FarmMenu.AddGroupLabel("尾兵技能:");
                    _useQ = FarmMenu.Add("lasthitQ", new CheckBox("使用Q尾兵 ?"));
                    FarmMenu.AddGroupLabel("尾兵设置:");
                    _lastMana = FarmMenu.Add("lastMana", new Slider("蓝量高于({0})时才使用尾兵技能.", 30));
                }

                public static void Initialize()
                {
                }
            }

            public static class Misc
            {
                private static readonly CheckBox _fleeE;
                private static readonly CheckBox _stunR;
                private static readonly CheckBox _gapE;
                private static readonly CheckBox _ksR;
                private static readonly Slider _minKsR;
                private static readonly Slider _maxKsR;
                private static readonly Slider _minHealthKsR;
                //Auto Tear Stack
                private static readonly CheckBox _tearAutoStack;
                private static readonly Slider _minManaToAutoStack;
                //JungleSteal
                //JungleSteal Settings
                private static readonly CheckBox _jugSteal;
                private static readonly CheckBox _jugStealBlue;
                private static readonly CheckBox _jugStealRed;
                private static readonly CheckBox _jugStealDragon;
                private static readonly CheckBox _jugStealBaron;

                public static bool UseE
                {
                    get { return _fleeE.CurrentValue; }
                }

                public static bool GapE
                {
                    get { return _gapE.CurrentValue; }
                }

                public static bool KSR
                {
                    get { return _ksR.CurrentValue; }
                }

                public static bool CCedR
                {
                    get { return _stunR.CurrentValue; }
                }

                public static int minR
                {
                    get { return _minKsR.CurrentValue; }
                }

                public static int maxR
                {
                    get { return _maxKsR.CurrentValue; }
                }

                public static int MinHealthR
                {
                    get { return _minHealthKsR.CurrentValue; }
                }
                //Auto Tear Stack
                public static bool AutoTearStack
                {
                    get { return _tearAutoStack.CurrentValue; }
                }

                public static int MinManaToAutoStack
                {
                    get { return _minManaToAutoStack.CurrentValue; }
                }
                //JungleSteal
                public static bool JungleSteal
                {
                    get { return _jugSteal.CurrentValue; }
                }

                public static bool JungleStealBlue
                {
                    get { return _jugStealBlue.CurrentValue; }
                }

                public static bool JungleStealRed
                {
                    get { return _jugStealRed.CurrentValue; }
                }

                public static bool JungleStealDrag
                {
                    get { return _jugStealDragon.CurrentValue; }
                }

                public static bool JungleStealBaron
                {
                    get { return _jugStealBaron.CurrentValue; }
                }

                static Misc()
                {
                    // Initialize the menu values
                    MiscMenu.AddGroupLabel("杂项");
                    _fleeE = MiscMenu.Add("fleeE", new CheckBox("按逃跑按键时使用E逃跑 ?"));
                    _stunR = MiscMenu.Add("stunUlt", new CheckBox("对无法移动敌方单位使用R (比如: 晕眩)?"));
                    _gapE = MiscMenu.Add("gapE", new CheckBox("敌方英雄突击时智能E至安全地点 ?"));
                    MiscMenu.AddLabel("女神之泪叠加设置");
                    _tearAutoStack = MiscMenu.Add("tearstackbox", new CheckBox("使用技能叠加女神 ?"));
                    _minManaToAutoStack = MiscMenu.Add("manaAutoStack",
                        new Slider("蓝量高于({0})时才使用自动叠加女神.", 90, 1));

                    MiscMenu.AddGroupLabel("R 设置");
                    _minKsR = MiscMenu.Add("ksminR", new Slider("当敌人在({0})最小范围内才使用R.", 600, 300, 2000));
                    _maxKsR = MiscMenu.Add("ksmaxR", new Slider("当敌人在({0})最大范围内才使用R.", 1500, 300, 30000));
                    MiscMenu.AddLabel("抢头设置");
                    _ksR = MiscMenu.Add("ksR", new CheckBox("使用R抢头 ?"));
                    _minHealthKsR = MiscMenu.Add("kshealthR",
                        new Slider(
                            "只有在目标血量高于({0})才使用R.",
                            150, 0, 650));

                    MiscMenu.AddGroupLabel("偷野设置");
                    _jugSteal = MiscMenu.Add("jungleSteal", new CheckBox("使用R偷野 ?"));
                    MiscMenu.AddSeparator(1);
                    _jugStealBlue = MiscMenu.Add("junglestealBlue", new CheckBox("使用R偷蓝 ?"));
                    _jugStealRed = MiscMenu.Add("junglestealRed", new CheckBox("使用R偷红 ?"));
                    _jugStealDragon = MiscMenu.Add("junglestealDrag", new CheckBox("使用R偷龙 ?"));
                    _jugStealBaron = MiscMenu.Add("junglestealBaron", new CheckBox("使用R偷男爵 ?"));
                }

                public static void Initialize()
                {
                }
            }

            public static class Draw
            {
                private static readonly CheckBox _drawReady;
                private static readonly CheckBox _drawHealth;
                private static readonly CheckBox _drawPercent;
                private static readonly CheckBox _drawStatiscs;
                private static readonly CheckBox _drawQ;
                private static readonly CheckBox _drawW;
                private static readonly CheckBox _drawE;
                private static readonly CheckBox _drawR;
                //Color Config
                private static readonly ColorConfig _qColor;
                private static readonly ColorConfig _wColor;
                private static readonly ColorConfig _eColor;
                private static readonly ColorConfig _minrColor;
                private static readonly ColorConfig _maxrColor;
                private static readonly ColorConfig _healthColor;

                //CheckBoxes
                public static bool DrawReady
                {
                    get { return _drawReady.CurrentValue; }
                }

                public static bool DrawHealth
                {
                    get { return _drawHealth.CurrentValue; }
                }

                public static bool DrawPercent
                {
                    get { return _drawPercent.CurrentValue; }
                }

                public static bool DrawStatistics
                {
                    get { return _drawStatiscs.CurrentValue; }
                }

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
                //Colors
                public static Color HealthColor
                {
                    get { return _healthColor.GetSystemColor(); }
                }

                public static SharpDX.Color QColor
                {
                    get { return _qColor.GetSharpColor(); }
                }

                public static SharpDX.Color WColor
                {
                    get { return _wColor.GetSharpColor(); }
                }

                public static SharpDX.Color EColor
                {
                    get { return _eColor.GetSharpColor(); }
                }
                public static SharpDX.Color minRColor
                {
                    get { return _minrColor.GetSharpColor(); }
                }
                public static SharpDX.Color MaxRColor
                {
                    get { return _maxrColor.GetSharpColor(); }
                }

                static Draw()
                {
                    DrawMenu.AddGroupLabel("技能线圈设置 :");
                    _drawReady = DrawMenu.Add("drawOnlyWhenReady", new CheckBox("显示无冷却技能线圈 ?"));
                    _drawHealth = DrawMenu.Add("damageIndicatorDraw", new CheckBox("伤害显示 ?"));
                    _drawPercent = DrawMenu.Add("percentageIndicatorDraw", new CheckBox("伤害百分比显示 ?"));
                    _drawStatiscs = DrawMenu.Add("statiscsIndicatorDraw", new CheckBox("显示伤害数据 ?"));
                    DrawMenu.AddSeparator(1);
                    _drawQ = DrawMenu.Add("qDraw", new CheckBox("显示Q距离 ?"));
                    _drawW = DrawMenu.Add("wDraw", new CheckBox("显示W距离 ?"));
                    _drawE = DrawMenu.Add("eDraw", new CheckBox("显示E距离 ?"));
                    _drawR = DrawMenu.Add("rDraw", new CheckBox("显示R距离 ?"));

                    _healthColor = new ColorConfig(DrawMenu, "healthColorConfig", Color.Orange, "Color Damage Indicator:");
                    _qColor = new ColorConfig(DrawMenu, "qColorConfig", Color.Blue, "Color Q:");
                    _wColor = new ColorConfig(DrawMenu, "wColorConfig", Color.Red, "Color W:");
                    _eColor = new ColorConfig(DrawMenu, "eColorConfig", Color.DeepPink, "Color E:");
                    _minrColor = new ColorConfig(DrawMenu, "rminColorConfig", Color.Yellow, "Color Minimun range R:");
                    _maxrColor = new ColorConfig(DrawMenu, "rmaxColorConfig", Color.Purple, "Color Maximun range R:");
                }

                public static void Initialize()
                {
                }
            }
        }
    }
}