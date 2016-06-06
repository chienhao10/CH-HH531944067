using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System.Reflection;
namespace KappaLeBlanc
{
    class LBMenu : Helper
    {
        public static Menu Menu, ComboM, LCM, KSM, AntiGapcloserM, HSM,DrawM, FLM, Misc;
        static readonly string[] Modes = new string[] { "Move to nearest ally tower", "Move to mouse", "Move to enemy" };
        public static void StartMenu()
        {
            Menu = MainMenu.AddMenu("Kappa 妖姬", "menu");
            Menu.AddGroupLabel("Kappa Leblanc Reworked");
            Menu.AddLabel("By Capitão Addon");
            Menu.AddSeparator();
            Menu.AddSeparator();
            Menu.AddSeparator();
            Menu.AddLabel("Current Version: " + Assembly.GetExecutingAssembly().GetName().Version);

            DrawingsMenu();
            ComboMenu();
            LaneClearMenu();
            KillstealMenu();
            AntiGapMenu();
            Flee();
            MiscMenu();
        }
        private static void DrawingsMenu()
        {
            DrawM = Menu.AddSubMenu("线圈", "draw");
            DrawM.Add("Q", new CheckBox("显示 Q 范围"));
            DrawM.Add("W", new CheckBox("显示 W 范围", false));
            DrawM.Add("WPos", new CheckBox("显示 W 位置", false));
            DrawM.Add("E", new CheckBox("显示 E 范围", false));
            DrawM.AddSeparator();
            DrawM.Add("line", new CheckBox("显示 线至可击杀目标"));
            DrawM.Add("dist", new Slider("最大线形距离", 1000, 500, 3000));   

        }
        private static void ComboMenu()
        {
            ComboM = Menu.AddSubMenu("连招", "combo");
            ComboM.Add("Q", new CheckBox("使用 Q"));
            ComboM.Add("W", new CheckBox("使用 W"));
            ComboM.Add("extW", new CheckBox("延长 W (W to 防突进)", false));
            ComboM.Add("E", new CheckBox("使用 E"));
            ComboM.Add("R", new CheckBox("使用 R"));
            ComboM.Add("RQ", new CheckBox("使用 R (Q)"));
            ComboM.Add("RW", new CheckBox("使用 R (W)", false));
            ComboM.Add("RE", new CheckBox("使用 R (E)"));

        }
        private static void LaneClearMenu()
        {
            LCM = Menu.AddSubMenu("清线", "laneclear");
            LCM.Add("Q", new CheckBox("使用 Q", false));
            LCM.Add("QMana", new Slider("最低 % 蓝量Q", 20, 0, 100));
            LCM.Add("W", new CheckBox("使用 W"));         
            LCM.Add("WMana", new Slider("最低 % 蓝量W", 20, 0, 100));
            LCM.Add("W2", new CheckBox("自动 W2"));
            LCM.Add("WMin", new Slider("最低小兵数量 W", 4, 1, 10));
            LCM.AddSeparator();
            LCM.Add("R", new CheckBox("使用 R (W)", false));
            LCM.Add("RMin", new Slider("最低小兵数量 R (W)", 6, 1, 10));

            //Who uses E in laneclear -.-

        }
        private static void KillstealMenu()
        {
            KSM = Menu.AddSubMenu("抢头", "ks");
            KSM.Add("Q", new CheckBox("使用 Q 抢头"));
            KSM.Add("W", new CheckBox("使用 W 抢头"));
            KSM.Add("extW", new CheckBox("使用 延长 W (or R) 抢头 (W + Q 或者 E)"));
            KSM.Add("wr", new CheckBox("使用 W+R + Q/E 抢头"));
            KSM.Add("E", new CheckBox("使用 E 抢头"));
            KSM.Add("R", new CheckBox("使用 R 抢头"));

        }
        private static void AntiGapMenu()
        {
            AntiGapcloserM = Menu.AddSubMenu("防突进", "antigap");
            AntiGapcloserM.Add("E", new CheckBox("E 防突进"));
            AntiGapcloserM.Add("RE", new CheckBox("R (E) 防突进", false));

            HarassMenu();
        }
        private static void HarassMenu()
        {
            HSM = Menu.AddSubMenu("骚扰");

            HSM.Add("Q", new CheckBox("使用 Q"));
            HSM.Add("QMana", new Slider("最低 % 蓝量 Q", 40, 0, 100));
            HSM.AddSeparator();
            HSM.Add("W", new CheckBox("使用 W"));
            HSM.Add("extW", new CheckBox("延长 W (W 接近)", false));
            HSM.Add("AutoW", new CheckBox("骚扰后自动 W2"));
            HSM.Add("WMana", new Slider("最低 % 蓝量 W", 40, 0, 100));
            HSM.AddSeparator();
            HSM.Add("E", new CheckBox("使用 E", false));
            HSM.Add("EMana", new Slider("最低 % 蓝量 E", 40, 0, 100));
            HSM.AddSeparator();
            HSM.Add("Auto", new KeyBind("自动骚扰按键开关", false, KeyBind.BindTypes.PressToggle, 'N'));

        }
        private static void Flee()
        {
            FLM = Menu.AddSubMenu("逃跑");

            FLM.Add("E", new CheckBox("使用 E"));
            FLM.Add("W", new CheckBox("使用 W 至鼠标位置"));
            FLM.Add("R", new CheckBox("使用 R 至鼠标位置", false));
        }
        private static void MiscMenu()
        {
            Misc = Menu.AddSubMenu("杂项");

            Misc.Add("AutoW", new Slider("自动 W2 当血量低于 %", 20, 0, 100));
            Misc.Add("Clone", new CheckBox("控制克隆模式"));
            var a = Misc.Add("CloneMode", new Slider("", 1, 0, 2));
            a.DisplayName = Modes[a.CurrentValue];
            a.OnValueChange += delegate
                (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs Args)
            {
                sender.DisplayName = Modes[Args.NewValue];
            };
        }
    }
}