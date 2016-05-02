using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using KA_Lux.DMGHandler;
using Color = System.Drawing.Color;

// ReSharper disable ConvertPropertyToExpressionBody
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming
// ReSharper disable MemberHidesStaticFromOuterClass

namespace KA_Lux
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
            public static readonly Menu SettingsMenu;

            static Modes()
            {
                SpellsMenu = Menu.AddSubMenu("::技能菜单::");
                Combo.Initialize();
                Harass.Initialize();

                FarmMenu = Menu.AddSubMenu("::农兵菜单::");
                LaneClear.Initialize();
                JungleClear.Initialize();
                LastHit.Initialize();

                MiscMenu = Menu.AddSubMenu("::杂项::");
                Misc.Initialize();

                DrawMenu = Menu.AddSubMenu("::线圈::");
                Draw.Initialize();

                SettingsMenu = Menu.AddSubMenu("::设置::");
                Settings.Initialize();
            }

            public static void Initialize()
            {
            }

            public static class Combo
            {
                private static readonly CheckBox _useQ;
                private static readonly CheckBox _useE;
                private static readonly CheckBox _useESnared;
                private static readonly CheckBox _useR;
                private static readonly CheckBox _useRSnared;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }

                public static bool UseE
                {
                    get { return _useE.CurrentValue; }
                }

                public static bool UseESnared
                {
                    get { return _useESnared.CurrentValue; }
                }

                public static bool UseR
                {
                    get { return _useR.CurrentValue; }
                }

                public static bool UseRSnared
                {
                    get { return _useRSnared.CurrentValue; }
                }

                static Combo()
                {
                    // Initialize the menu values
                    SpellsMenu.AddGroupLabel("连招:");
                    _useQ = SpellsMenu.Add("comboQ", new CheckBox("使用 Q?"));
                    _useE = SpellsMenu.Add("comboE", new CheckBox("使用 E?"));
                    _useESnared = SpellsMenu.Add("comboESnared", new CheckBox("敌人被定身才使用E ?", false));
                    _useR = SpellsMenu.Add("comboR", new CheckBox("使用 R?"));
                    _useRSnared = SpellsMenu.Add("comboRSnared", new CheckBox("敌人被定身才使用R ?"));
                }

                public static void Initialize()
                {
                }
            }

            public static class Harass
            {
                private static readonly CheckBox _useQ;
                private static readonly CheckBox _useW;
                private static readonly CheckBox _useE;
                private static readonly Slider _manaHarass;

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

                public static int ManaHarass
                {
                    get { return _manaHarass.CurrentValue; }
                }

                static Harass()
                {
                    SpellsMenu.AddGroupLabel("骚扰:");
                    _useQ = SpellsMenu.Add("harassQ", new CheckBox("使用 Q?"));
                    _useW = SpellsMenu.Add("harassW", new CheckBox("使用 W?"));
                    _useE = SpellsMenu.Add("harassE", new CheckBox("使用 E?"));
                    SpellsMenu.AddGroupLabel("骚扰设置:");
                    _manaHarass = SpellsMenu.Add("harassMana", new Slider("只使用技能清线当蓝量大于 ({0}).", 30));
                }

                public static void Initialize()
                {
                }
            }

            public static class LaneClear
            {
                private static readonly CheckBox _useQ;
                private static readonly CheckBox _useE;
                private static readonly Slider _laneMana;
                private static readonly Slider _eCount;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }

                public static bool UseE
                {
                    get { return _useE.CurrentValue; }
                }

                public static int LaneMana
                {
                    get { return _laneMana.CurrentValue; }
                }

                public static int ECount
                {
                    get { return _eCount.CurrentValue; }
                }

                static LaneClear()
                {
                    FarmMenu.AddGroupLabel("清线:");
                    _useQ = FarmMenu.Add("laneclearQ", new CheckBox("使用 Q?"));
                    _useE = FarmMenu.Add("laneclearE", new CheckBox("使用 E?"));
                    FarmMenu.AddGroupLabel("清线设置:");
                    _laneMana = FarmMenu.Add("laneMana", new Slider("只使用技能清线当蓝量大于 ({0}).", 30));
                    _eCount = FarmMenu.Add("eCountLaneClear", new Slider("能命中 ({0}) 小兵才使用E.", 3, 1, 6));
                }

                public static void Initialize()
                {
                }
            }

            public static class JungleClear
            {
                private static readonly CheckBox _useQ;
                private static readonly CheckBox _useE;
                private static readonly Slider _laneMana;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }

                public static bool UseE
                {
                    get { return _useE.CurrentValue; }
                }

                public static int LaneMana
                {
                    get { return _laneMana.CurrentValue; }
                }

                static JungleClear()
                {
                    FarmMenu.AddGroupLabel("清野:");
                    _useQ = FarmMenu.Add("jungleclearQ", new CheckBox("使用 Q ?"));
                    _useE = FarmMenu.Add("jungleclearE", new CheckBox("使用 E ?"));
                    FarmMenu.AddGroupLabel("清野设置:");
                    _laneMana = FarmMenu.Add("junglelaneMana", new Slider("只使用技能清野当蓝量大于 ({0}).", 30));
                }

                public static void Initialize()
                {
                }
            }

            public static class LastHit
            {
                private static readonly CheckBox _useQ;
                private static readonly CheckBox _useE;
                private static readonly Slider _lastMana;
                private static readonly Slider _eCount;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }

                public static bool UseE
                {
                    get { return _useE.CurrentValue; }
                }

                public static int LastMana
                {
                    get { return _lastMana.CurrentValue; }
                }

                public static int ECount
                {
                    get { return _eCount.CurrentValue; }
                }


                static LastHit()
                {
                    FarmMenu.AddGroupLabel("尾兵:");
                    _useQ = FarmMenu.Add("lasthitQ", new CheckBox("使用 Q?"));
                    _useE = FarmMenu.Add("lasthitE", new CheckBox("使用 E?"));
                    FarmMenu.AddGroupLabel("尾兵设置:");
                    _lastMana = FarmMenu.Add("lastMana", new Slider("只使用技能尾兵当蓝量大于 ({0}).", 30));
                    _eCount = FarmMenu.Add("eCountLastHit", new Slider("能命中 ({0}) 小兵才使用E.", 2, 1, 6));
                }

                public static void Initialize()
                {
                }
            }

            public static class Misc
            {
                private static readonly CheckBox _interruptSpell;
                private static readonly CheckBox _antiGapCloserSpell;
                private static readonly Slider _miscMana;
                //KS
                private static readonly CheckBox _killStealQ;
                private static readonly CheckBox _killStealE;
                private static readonly CheckBox _killStealR;
                private static readonly Slider _ksMana;
                //W Settings
                private static readonly CheckBox _wDefense;
                private static readonly CheckBox _wDefenseAlly;
                private static readonly Slider _wMana;
                //JungleSteal Settings
                private static readonly CheckBox _jugSteal;
                private static readonly CheckBox _jugStealBlue;
                private static readonly CheckBox _jugStealRed;
                private static readonly CheckBox _jugStealDragon;
                private static readonly CheckBox _jugStealBaron;

                public static bool InterruptSpell
                {
                    get { return _interruptSpell.CurrentValue; }
                }

                public static bool AntiGapCloser
                {
                    get { return _antiGapCloserSpell.CurrentValue; }
                }

                public static int MiscMana
                {
                    get { return _miscMana.CurrentValue; }
                }
                //KS
                public static bool KillStealQ
                {
                    get { return _killStealQ.CurrentValue; }
                }

                public static bool KillStealE
                {
                    get { return _killStealE.CurrentValue; }
                }

                public static bool KillStealR
                {
                    get { return _killStealR.CurrentValue; }
                }

                public static int KillStealMana
                {
                    get { return _ksMana.CurrentValue; }
                }
                //W Settings
                public static bool WDefense
                {
                    get { return _wDefense.CurrentValue; }
                }

                public static int WMana
                {
                    get { return _wMana.CurrentValue; }
                }
                //JungleSteal Settings
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
                    _interruptSpell = MiscMenu.Add("interruptQ", new CheckBox("Q 打断技能 ?"));
                    _antiGapCloserSpell = MiscMenu.Add("gapcloserQ", new CheckBox("Q 防突进 ?"));
                    _miscMana = MiscMenu.Add("miscMana", new Slider("最低蓝量使用 技能打断/防突进 ?", 20));
                    MiscMenu.AddGroupLabel("抢头");
                    _killStealQ = MiscMenu.Add("killstealQ", new CheckBox("使用 Q?"));
                    _killStealE = MiscMenu.Add("killstealE", new CheckBox("使用 E?"));
                    _killStealR = MiscMenu.Add("killstealR", new CheckBox("使用 R?"));
                    _ksMana = MiscMenu.Add("killstealMana", new Slider("最低蓝量使用抢头技能 ?", 15));
                    MiscMenu.AddGroupLabel("W 设置");
                    _wDefense = MiscMenu.Add("safetyW", new CheckBox("准备收到伤害时，使用W ?"));
                    _wDefenseAlly = MiscMenu.Add("safetyWAlly", new CheckBox("队友受到伤害时使用 W ?"));
                    _wMana = MiscMenu.Add("wMana", new Slider("最低蓝量使用 W ?", 10));
                    MiscMenu.AddGroupLabel("偷野设置");
                    _jugSteal = MiscMenu.Add("jungleSteal", new CheckBox("使用 R ?"));
                    MiscMenu.AddSeparator(1);
                    _jugStealBlue = MiscMenu.Add("junglestealBlue", new CheckBox("蓝 ?"));
                    _jugStealRed = MiscMenu.Add("junglestealRed", new CheckBox("红 ?", false));
                    _jugStealDragon = MiscMenu.Add("junglestealDrag", new CheckBox("龙 ?"));
                    _jugStealBaron = MiscMenu.Add("junglestealBaron", new CheckBox("男爵 ?"));
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
                private static readonly ColorConfig _rColor;
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
                public static SharpDX.Color RColor
                {
                    get { return _rColor.GetSharpColor(); }
                }

                static Draw()
                {
                    DrawMenu.AddGroupLabel("线圈设置 :");
                    _drawReady = DrawMenu.Add("drawOnlyWhenReady", new CheckBox("只显示无冷却技能 ?"));
                    _drawHealth = DrawMenu.Add("damageIndicatorDraw", new CheckBox("显示伤害指示器 ?"));
                    _drawPercent = DrawMenu.Add("percentageIndicatorDraw", new CheckBox("显示伤害百分比 ?"));
                    _drawStatiscs = DrawMenu.Add("statiscsIndicatorDraw", new CheckBox("显示伤害统计数据 ?"));
                    DrawMenu.AddSeparator(1);
                    _drawQ = DrawMenu.Add("qDraw", new CheckBox("显示 Q 范围 ?"));
                    _drawW = DrawMenu.Add("wDraw", new CheckBox("显示 W 范围 ?"));
                    _drawE = DrawMenu.Add("eDraw", new CheckBox("显示 E 范围 ?"));
                    _drawR = DrawMenu.Add("rDraw", new CheckBox("显示 R 范围 ?"));

                    _healthColor = new ColorConfig(DrawMenu, "healthColorConfig", Color.Orange, "伤害指示器颜色:");
                    _qColor = new ColorConfig(DrawMenu, "qColorConfig", Color.Blue, "Color Q:");
                    _wColor = new ColorConfig(DrawMenu, "wColorConfig", Color.Red, "Color W:");
                    _eColor = new ColorConfig(DrawMenu, "eColorConfig", Color.DeepPink, "Color E:");
                    _rColor = new ColorConfig(DrawMenu, "rColorConfig", Color.Yellow, "Color R:");
                }

                public static void Initialize()
                {
                }
            }

            public static class Settings
            {
                //Danger
                private static readonly Slider EnemyRange;
                private static readonly Slider EnemySlider;
                private static readonly CheckBox Spells;
                private static readonly CheckBox Skillshots;
                private static readonly CheckBox AAs;


                public static int RangeEnemy
                {
                    get { return EnemyRange.CurrentValue; }
                }

                public static int EnemyCount
                {
                    get { return EnemySlider.CurrentValue; }
                }

                public static bool ConsiderSpells
                {
                    get { return Spells.CurrentValue; }
                }

                public static bool ConsiderSkillshots
                {
                    get { return Skillshots.CurrentValue; }
                }

                public static bool ConsiderAttacks
                {
                    get { return AAs.CurrentValue; }
                }

                static Settings()
                {
                    SettingsMenu.AddGroupLabel("危险判断设置");
                    EnemySlider = SettingsMenu.Add("minenemiesinrange", new Slider("最少敌人数量范围判断", 1, 1, 5));
                    EnemyRange = SettingsMenu.Add("minrangeenemy", new Slider("敌人必须在 ({0}) 范围内视为危险", 1000, 600, 2500));
                    Spells = SettingsMenu.Add("considerspells", new CheckBox("考虑技能 ?"));
                    Skillshots = SettingsMenu.Add("considerskilshots", new CheckBox("考虑线形技能 ?"));
                    AAs = SettingsMenu.Add("consideraas", new CheckBox("考虑普攻 ?"));
                    SettingsMenu.AddSeparator();
                    SettingsMenu.AddGroupLabel("危险技能");
                    foreach (var spell in DangerousSpells.Spells.Where(x => EntityManager.Heroes.Enemies.Any(b => b.Hero == x.Hero)))
                    {
                        SettingsMenu.Add(spell.Hero.ToString() + spell.Slot, new CheckBox(spell.Hero + " - " + spell.Slot + ".", spell.DefaultValue));
                    }
                }

                public static void Initialize()
                {
                }
            }
        }
    }
}