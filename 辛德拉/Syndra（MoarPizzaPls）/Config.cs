using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberHidesStaticFromOuterClass
namespace Syndra
{
    public static class Config
    {
        private const string MenuName = "Syndra";
        public static Menu Menu { get; private set; }

        static Config()
        {
            Menu = MainMenu.AddMenu(MenuName, "syndraMenu");
            Menu.AddGroupLabel("Syndra 脚本");
            Menu.AddLabel("脚本有任何问题请在论坛上回复有建设性的建议，谢谢!");

            // All modes
            Modes.Initialize();

            // Misc
            Misc.Initialize();

            // Drawing
            Drawing.Initialize();
        }

        public static void Initialize()
        {
        }

        public static class Modes
        {
            public static Menu Menu { get; private set; }

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
                Farm.Initialize();

                // JungleClear
                Menu.AddSeparator();
                JungleClear.Initialize();
            }

            public static void Initialize()
            { }

            public static class Combo
            {
                private static readonly CheckBox _useIgnite;
                public static bool UseIgnite { get { return _useIgnite.CurrentValue; } }

                private static readonly CheckBox _disableAA;
                public static bool DisableAA { get { return _disableAA.CurrentValue; } }

                static Combo()
                {
                    Menu.AddGroupLabel("连招");
                    _useIgnite = Menu.Add("comboUseIgnite", new CheckBox("使用点燃"));
                    _useIgnite.CurrentValue = true;
                    //_disableAA = Menu.Add("disableAA", new CheckBox("Disable AA"));
                    //_disableAA.CurrentValue = false;
                }

                public static void Initialize()
                { }
            }

            public static class Harass
            {
                private static readonly CheckBox _useQ;
                public static bool UseQ { get { return _useQ.CurrentValue; } }

                private static readonly CheckBox _useW;
                public static bool UseW { get { return _useW.CurrentValue; } }


                private static readonly CheckBox _useE;
                public static bool UseE { get { return _useE.CurrentValue; } }


                private static readonly KeyBind _autoHarass;
                public static bool AutoHarass { get { return _autoHarass.CurrentValue; } }


                private static readonly KeyBind _autoAaHarass;
                public static bool AutoAaHarass { get { return _autoAaHarass.CurrentValue; } }

                private static readonly Slider _mana;
                public static int MinMana { get { return _mana.CurrentValue; } }

                static Harass()
                {
                    Menu.AddGroupLabel("骚扰");
                    _useQ = Menu.Add("harassUseQ", new CheckBox("使用 Q"));
                    _useQ.CurrentValue = true;
                    _useW = Menu.Add("harassUseW", new CheckBox("使用 W"));
                    _useW.CurrentValue = true;
                    _useE = Menu.Add("harassUseE", new CheckBox("使用 E"));
                    _useE.CurrentValue = false;
                    _mana = Menu.Add("harassMana", new Slider("最低蓝量% 使用自动骚扰", 30));
                    _autoHarass = Menu.Add("autoharass", new KeyBind("自动骚扰", false, KeyBind.BindTypes.PressToggle, 'K'));
                    _autoAaHarass = Menu.Add("enemyaaharass", new KeyBind("敌人普攻时骚扰", false, KeyBind.BindTypes.PressToggle, 'L'));
                }

                public static void Initialize()
                { }
            }

            public static class Farm
            {
                private static readonly CheckBox _useQ;
                public static bool UseQ { get { return _useQ.CurrentValue; } }

                static Farm()
                {
                    Menu.AddGroupLabel("尾兵");
                    _useQ = Menu.Add("farmUseQ", new CheckBox("使用 Q"));
                    _useQ.CurrentValue = false;
                }

                public static void Initialize()
                { }
            }

            public static class JungleClear
            {
                //private static readonly CheckBox _useE;
                //public static bool UseE { get { return _useE.CurrentValue; } }

                static JungleClear()
                {
                   // Menu.AddGroupLabel("JungleClear");
                   // _useE = Menu.Add("jungleUseE", new CheckBox("Use E"));
                }

                public static void Initialize()
                { }
            }
        }

        public static class Misc
        {
            public static Menu Menu { get; private set; }

            public static void Initialize()
            {
                if (Menu == null)
                {
                    Menu = Config.Menu.AddSubMenu("杂项");

                    Menu.AddGroupLabel("杂项");
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>())
                    {
                        CheckBox cb = new CheckBox("不使用大招/点燃 " + enemy.BaseSkinName);
                        cb.CurrentValue = false;
                        if(enemy.Team != Player.Instance.Team)
                            Menu.Add( "DontUlt" + enemy.BaseSkinName, cb);
                    }




                    Menu.AddSeparator();

                    CheckBox cb1 = new CheckBox("抢头", true);
                    Menu.Add("autokill", cb1); 
                    cb1 = new CheckBox("技能打断", true);
                    Menu.Add("interrupt", cb1); 
                    cb1 = new CheckBox("防突击",true);
                    Menu.Add("antigap", cb1);

                    Menu.AddSeparator();

                    cb1 = new CheckBox("调试信息", false);
                    Menu.Add("debug", cb1);

                }
            }
        }

        public static class Drawing
        {
            private static readonly CheckBox _drawQ;
            public static bool DrawQ { get { return _drawQ.CurrentValue; } }

            private static readonly CheckBox _drawW;
            public static bool DrawW { get { return _drawW.CurrentValue; } }

            private static readonly CheckBox _drawQE;
            public static bool DrawQE { get { return _drawQE.CurrentValue; } }

            private static readonly CheckBox _drawQELines;
            public static bool DrawQELines { get { return _drawQELines.CurrentValue; } }

            private static readonly CheckBox _removeHarassText;
            public static bool RemoveHarassText { get { return _removeHarassText.CurrentValue; } }

            static Drawing()
            {

                Menu = Config.Menu.AddSubMenu("线圈");

                Menu.AddGroupLabel("线圈");
                _drawQ = Menu.Add("drawQ", new CheckBox("显示 Q 范围"));
                _drawW = Menu.Add("drawW", new CheckBox("显示 W 范围"));
                _drawQE = Menu.Add("drawQE", new CheckBox("显示 QE 范围"));
                _drawQELines = Menu.Add("drawQELines", new CheckBox("显示 QE 路线",false));
                _removeHarassText = Menu.Add("removeHarassText", new CheckBox("移除自动骚扰文字", false));
            }

            public static void Initialize()
            { }
        }
    }
}
