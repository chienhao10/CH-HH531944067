namespace KappaKindred
{
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    internal class Menu
    {
        private static EloBuddy.SDK.Menu.Menu menuIni;

        public static EloBuddy.SDK.Menu.Menu ComboMenu { get; private set; }

        public static EloBuddy.SDK.Menu.Menu DrawMenu { get; private set; }

        public static EloBuddy.SDK.Menu.Menu HarassMenu { get; private set; }

        public static EloBuddy.SDK.Menu.Menu JungleMenu { get; private set; }

        public static EloBuddy.SDK.Menu.Menu LaneMenu { get; private set; }

        public static EloBuddy.SDK.Menu.Menu ManaMenu { get; private set; }

        public static EloBuddy.SDK.Menu.Menu FleeMenu { get; private set; }

        public static EloBuddy.SDK.Menu.Menu UltMenu { get; private set; }

        public static void Load()
        {
            menuIni = MainMenu.AddMenu("千玗", "Kindred");
            menuIni.AddGroupLabel("欢迎使用最渣千玗脚本!");

            UltMenu = menuIni.AddSubMenu("大招");
            UltMenu.AddGroupLabel("大招设置");
            UltMenu.Add("Rally", new CheckBox("R 拯救队友 / 自身"));
            UltMenu.Add("Rallyh", new Slider("R 队友血量 %", 20, 0, 100));
            UltMenu.AddGroupLabel("不使用R: ");
            foreach (var ally in ObjectManager.Get<AIHeroClient>())
            {
                CheckBox cb = new CheckBox(ally.BaseSkinName) { CurrentValue = false };
                if (ally.Team == ObjectManager.Player.Team)
                {
                    UltMenu.Add("DontUlt" + ally.BaseSkinName, cb);
                }
            }

            ComboMenu = menuIni.AddSubMenu("连招");
            ComboMenu.AddGroupLabel("连招设置");
            ComboMenu.Add("Q", new CheckBox("使用 Q"));
            ComboMenu.Add("W", new CheckBox("使用 W"));
            ComboMenu.Add("E", new CheckBox("使用 E"));
            ComboMenu.AddGroupLabel("额外设置");
            ComboMenu.Add("Qmode", new ComboBox("Q 模式", 0, "至目标", "至鼠标"));
            ComboMenu.Add("QW", new CheckBox("只Q 当W激活时", false));
            ComboMenu.Add("QAA", new CheckBox("不Q 当目标在普攻范围", false));
            ComboMenu.Add("Emark", new CheckBox("集火有 E的目标"));
            ComboMenu.Add("Pmark", new CheckBox("集火有 被标记的目标"));
            ComboMenu.Add("Pspells", new CheckBox("不攻击 R 中低于 15%血量的目标", false));

            HarassMenu = menuIni.AddSubMenu("骚扰");
            HarassMenu.AddGroupLabel("骚扰设置");
            HarassMenu.Add("Q", new CheckBox("使用 Q"));
            HarassMenu.Add("W", new CheckBox("使用 W", false));
            HarassMenu.Add("E", new CheckBox("使用 E"));

            LaneMenu = menuIni.AddSubMenu("清线");
            LaneMenu.AddGroupLabel("清线设置");
            LaneMenu.Add("Q", new CheckBox("使用 Q"));
            LaneMenu.Add("W", new CheckBox("使用 W", false));
            LaneMenu.Add("E", new CheckBox("使用 E", false));

            JungleMenu = menuIni.AddSubMenu("清野");
            JungleMenu.AddGroupLabel("清野设置");
            JungleMenu.Add("Q", new CheckBox("使用 Q"));
            JungleMenu.Add("W", new CheckBox("使用 W", false));
            JungleMenu.Add("E", new CheckBox("使用 E", false));

            FleeMenu = menuIni.AddSubMenu("逃跑");
            FleeMenu.AddGroupLabel("逃跑设置");
            FleeMenu.Add("Q", new CheckBox("使用 Q"));
            FleeMenu.Add("Qgap", new CheckBox("使用 Q 防突进"));

            ManaMenu = menuIni.AddSubMenu("蓝量控制器");
            ManaMenu.AddGroupLabel("骚扰");
            ManaMenu.Add("harassmana", new Slider("骚扰蓝量 %", 75, 0, 100));
            ManaMenu.AddGroupLabel("清线");
            ManaMenu.Add("lanemana", new Slider("清线蓝量 %", 60, 0, 100));

            DrawMenu = menuIni.AddSubMenu("线圈");
            DrawMenu.AddGroupLabel("线圈设置");
            DrawMenu.Add("Q", new CheckBox("显示 Q"));
            DrawMenu.Add("W", new CheckBox("显示 W"));
            DrawMenu.Add("E", new CheckBox("显示 E"));
            DrawMenu.Add("R", new CheckBox("显示 R"));
            DrawMenu.Add("debug", new CheckBox("调试", false));
        }
    }
}