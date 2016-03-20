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
        private static readonly string MenuName = "KA " + Player.Instance.ChampionName;

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
                SpellsMenu = Menu.AddSubMenu("::SpellsMenu::");
                Combo.Initialize();
                Harass.Initialize();

                FarmMenu = Menu.AddSubMenu("::FarmMenu::");
                LaneClear.Initialize();
                LastHit.Initialize();

                MiscMenu = Menu.AddSubMenu("::Misc::");
                Misc.Initialize();

                DrawMenu = Menu.AddSubMenu("::Drawings::");
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
                    SpellsMenu.AddGroupLabel("Combo Spells:");
                    _useQ = SpellsMenu.Add("comboQ", new CheckBox("Use Q on Combo ?"));
                    _useW = SpellsMenu.Add("comboW", new CheckBox("Use W on Combo ?"));
                    _useE = SpellsMenu.Add("comboE", new CheckBox("Use E on Combo ?"));
                    _useR = SpellsMenu.Add("comboR", new CheckBox("Use R on Combo ?"));
                    _minR = SpellsMenu.Add("minR", new Slider("Min Enemies to use R", 2, 0, 5));
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
                    SpellsMenu.AddGroupLabel("Harass Spells:");
                    _useQ = SpellsMenu.Add("harassQ", new CheckBox("Use Q on Harass ?"));
                    _useW = SpellsMenu.Add("harassW", new CheckBox("Use W on Harass ?"));
                    SpellsMenu.AddGroupLabel("Harass Settings:");
                    _manaHarass = SpellsMenu.Add("harassMana", new Slider("It will only cast any harass spell if the mana is greater than ({0}).", 30));
                    SpellsMenu.AddGroupLabel("AutoHarass Spells:");
                    //Add Key bind
                    _autouseQ = SpellsMenu.Add("autoHarassQ", new CheckBox("Use Q on AutoHarass ?"));
                    _autouseW = SpellsMenu.Add("autoHarassW", new CheckBox("Use W on AutoHarass ?"));
                    SpellsMenu.AddGroupLabel("AutoHarass Settings:");
                    _automanaQ = SpellsMenu.Add("autoHarassMana", new Slider("It will only cast any harass spell if the mana is greater than ({0}).", 50));
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
                    FarmMenu.AddGroupLabel("LaneClear Spells:");
                    _useQ = FarmMenu.Add("laneclearQ", new CheckBox("Use Q on Laneclear ?"));
                    FarmMenu.AddGroupLabel("LaneClear Settings:");
                    _laneMana = FarmMenu.Add("laneMana", new Slider("It will only cast any laneclear spell if the mana is greater than ({0}).", 30));
                    FarmMenu.AddGroupLabel("JungleClear Spells:");
                    _useQJungle = FarmMenu.Add("jungleclearQ", new CheckBox("Use Q on JungleClear ?"));
                    FarmMenu.AddGroupLabel("JungleClear Settings:");
                    _jungleMana = FarmMenu.Add("jungleMana", new Slider("It will only cast any jungleclear spell if the mana is greater than ({0}).", 30));
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
                    FarmMenu.AddGroupLabel("LastHit Spells:");
                    _useQ = FarmMenu.Add("lasthitQ", new CheckBox("Use Q on LastHit ?"));
                    FarmMenu.AddGroupLabel("LastHit Settings:");
                    _lastMana = FarmMenu.Add("lastMana", new Slider("It will only cast any lasthit spell if the mana is greater than ({0}).", 30));
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
                    MiscMenu.AddGroupLabel("Miscellenous");
                    _fleeE = MiscMenu.Add("fleeE", new CheckBox("Use E when you press the orbwalker key to flee ?"));
                    _stunR = MiscMenu.Add("stunUlt", new CheckBox("Use R when target has some kind of CC (Ex: Stun)?"));
                    _gapE = MiscMenu.Add("gapE", new CheckBox("Use E to safety when an enemy use a gapcloser spell on you ?"));
                    MiscMenu.AddLabel("Auto Tear Stack Settings");
                    _tearAutoStack = MiscMenu.Add("tearstackbox", new CheckBox("Use spells to auto stack tear ?"));
                    _minManaToAutoStack = MiscMenu.Add("manaAutoStack",
                        new Slider("It will only autostack if mana is greater than ({0}).", 90, 1));

                    MiscMenu.AddGroupLabel("R Settings");
                    _minKsR = MiscMenu.Add("ksminR", new Slider("It will only cast R if the enemie is not in ({0}).", 600, 300, 2000));
                    _maxKsR = MiscMenu.Add("ksmaxR", new Slider("It will only cast R if the enemie is in ({0}).", 1500, 300, 30000));
                    MiscMenu.AddLabel("KS Settings");
                    _ksR = MiscMenu.Add("ksR", new CheckBox("Use R to KillSteal ?"));
                    _minHealthKsR = MiscMenu.Add("kshealthR",
                        new Slider(
                            "Overkill R, it will cast the ultimate only if the target`s health is greater than ({0}).",
                            150, 0, 650));

                    MiscMenu.AddGroupLabel("Jungle Steal");
                    _jugSteal = MiscMenu.Add("jungleSteal", new CheckBox("JungleSteal using R ?"));
                    MiscMenu.AddSeparator(1);
                    _jugStealBlue = MiscMenu.Add("junglestealBlue", new CheckBox("JungleSteal Blue ?"));
                    _jugStealRed = MiscMenu.Add("junglestealRed", new CheckBox("JungleSteal Red ?"));
                    _jugStealDragon = MiscMenu.Add("junglestealDrag", new CheckBox("JungleSteal Dragon ?"));
                    _jugStealBaron = MiscMenu.Add("junglestealBaron", new CheckBox("JungleSteal Baron ?"));
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
                    DrawMenu.AddGroupLabel("Spell drawings Settings :");
                    _drawReady = DrawMenu.Add("drawOnlyWhenReady", new CheckBox("Draw the spells only if they are ready ?"));
                    _drawHealth = DrawMenu.Add("damageIndicatorDraw", new CheckBox("Draw damage indicator ?"));
                    _drawPercent = DrawMenu.Add("percentageIndicatorDraw", new CheckBox("Draw damage percentage ?"));
                    _drawStatiscs = DrawMenu.Add("statiscsIndicatorDraw", new CheckBox("Draw damage statistics ?"));
                    DrawMenu.AddSeparator(1);
                    _drawQ = DrawMenu.Add("qDraw", new CheckBox("Draw Q spell range ?"));
                    _drawW = DrawMenu.Add("wDraw", new CheckBox("Draw W spell range ?"));
                    _drawE = DrawMenu.Add("eDraw", new CheckBox("Draw E spell range ?"));
                    _drawR = DrawMenu.Add("rDraw", new CheckBox("Draw R spell range ?"));

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