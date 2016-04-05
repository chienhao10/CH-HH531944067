using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace LelBlanc
{
    internal class Config
    {
        /// <summary>
        /// Contains all the Menu's
        /// </summary>
        public static Menu ConfigMenu,
            ComboMenu,
            HarassMenu,
            LaneClearMenu,
            JungleClearMenu,
            KillStealMenu,
            DrawingMenu,
            MiscMenu;

        /// <summary>
        /// Contains Different Modes
        /// </summary>
        private static readonly string[] LogicModes = {"爆发模式", "双链条(TM) 模式"};

        /// <summary>
        /// Creates the Menu
        /// </summary>
        public static void Initialize()
        {
            ConfigMenu = MainMenu.AddMenu("CH汉化-妖姬", "LelBlanc");
            ConfigMenu.AddGroupLabel("作者：Karma Panda");
            ConfigMenu.AddGroupLabel(
                "任何盗用或者修改将会由严重的后果");
            ConfigMenu.AddGroupLabel("感谢选择使用本脚本，祝你玩的开心!");

            ComboMenu = ConfigMenu.AddSubMenu("连招", "cMenu");
            ComboMenu.AddLabel("技能设置");
            ComboMenu.Add("useQ", new CheckBox("使用Q"));
            ComboMenu.Add("useW", new CheckBox("使用W"));
            ComboMenu.Add("useReturn", new CheckBox("使用W 退回"));
            ComboMenu.Add("useE", new CheckBox("使用E"));
            ComboMenu.AddLabel("R 设置");
            ComboMenu.Add("useQR", new CheckBox("使用QR"));
            ComboMenu.Add("useWR", new CheckBox("使用WR", false));
            ComboMenu.Add("useReturn2", new CheckBox("使用WR 退回", false));
            ComboMenu.Add("useER", new CheckBox("使用ER", false));
            ComboMenu.AddLabel("额外设置");
            ComboMenu.Add("mode", new ComboBox("连招模式", 0, LogicModes));

            HarassMenu = ConfigMenu.AddSubMenu("骚扰", "hMenu");
            HarassMenu.AddLabel("技能设置");
            HarassMenu.Add("useQ", new CheckBox("使用Q"));
            HarassMenu.Add("useW", new CheckBox("使用W"));
            HarassMenu.Add("useReturn", new CheckBox("使用W 退回"));
            HarassMenu.Add("useE", new CheckBox("使用E"));
            HarassMenu.AddLabel("R 设置");
            HarassMenu.Add("useQR", new CheckBox("使用QR"));
            HarassMenu.Add("useWR", new CheckBox("使用WR", false));
            HarassMenu.Add("useReturn2", new CheckBox("使用WR 退回"));
            HarassMenu.Add("useER", new CheckBox("使用ER", false));
            HarassMenu.AddLabel("额外设置");
            HarassMenu.Add("mode", new ComboBox("骚扰模式", 1, LogicModes));

            LaneClearMenu = ConfigMenu.AddSubMenu("清线", "lcMenu");
            LaneClearMenu.AddLabel("技能设置");
            LaneClearMenu.Add("useQ", new CheckBox("使用Q", false));
            LaneClearMenu.Add("useW", new CheckBox("使用W"));
            LaneClearMenu.Add("sliderW", new Slider("使用W 如果可杀 {0} 个小兵", 3, 1, 5));
            LaneClearMenu.AddLabel("R 设置");
            LaneClearMenu.Add("useQR", new CheckBox("使用QR", false));
            LaneClearMenu.Add("useWR", new CheckBox("使用WR"));
            LaneClearMenu.Add("sliderWR", new Slider("使用WR 如果可杀 {0} 个小兵", 5, 1, 5));

            JungleClearMenu = ConfigMenu.AddSubMenu("清野", "jcMenu");
            JungleClearMenu.AddLabel("技能设置");
            JungleClearMenu.Add("useQ", new CheckBox("使用Q"));
            JungleClearMenu.Add("useW", new CheckBox("使用W"));
            JungleClearMenu.Add("useE", new CheckBox("使用E"));
            JungleClearMenu.Add("sliderW", new Slider("使用W 如果可击中 {0} 个小兵", 3, 1, 5));
            JungleClearMenu.AddLabel("R 设置");
            JungleClearMenu.Add("useQR", new CheckBox("使用QR"));
            JungleClearMenu.Add("useWR", new CheckBox("使用WR"));
            JungleClearMenu.Add("useER", new CheckBox("使用ER"));
            JungleClearMenu.Add("sliderWR", new Slider("使用WR 如果可击中 {0} 个小兵", 5, 1, 5));

            KillStealMenu = ConfigMenu.AddSubMenu("抢头", "ksMenu");
            KillStealMenu.AddLabel("技能设置");
            KillStealMenu.Add("useQ", new CheckBox("使用Q"));
            KillStealMenu.Add("useW", new CheckBox("使用W"));
            KillStealMenu.Add("useReturn", new CheckBox("使用W 退回"));
            KillStealMenu.Add("useE", new CheckBox("使用E"));
            KillStealMenu.AddLabel("R 设置");
            KillStealMenu.Add("useQR", new CheckBox("使用QR"));
            KillStealMenu.Add("useWR", new CheckBox("使用WR"));
            KillStealMenu.Add("useReturn2", new CheckBox("使用WR 退回"));
            KillStealMenu.Add("useER", new CheckBox("使用ER"));
            KillStealMenu.AddLabel("杂项设置");
            KillStealMenu.Add("useIgnite", new CheckBox("使用点燃"));
            KillStealMenu.Add("toggle", new CheckBox("开启抢头"));

            DrawingMenu = ConfigMenu.AddSubMenu("线圈", "dMenu");
            DrawingMenu.AddLabel("线圈设置");
            DrawingMenu.Add("drawQ", new CheckBox("显示 Q 范围", false));
            DrawingMenu.Add("drawW", new CheckBox("显示 W 范围", false));
            DrawingMenu.Add("drawE", new CheckBox("显示 E 范围", false));
            DrawingMenu.AddLabel("伤害指示器");
            DrawingMenu.Add("draw.Damage", new CheckBox("显示伤害"));
            DrawingMenu.Add("draw.Q", new CheckBox("计算 Q 伤害"));
            DrawingMenu.Add("draw.W", new CheckBox("计算 W 伤害"));
            DrawingMenu.Add("draw.E", new CheckBox("计算 E 伤害"));
            DrawingMenu.Add("draw.R", new CheckBox("计算 R 伤害"));
            DrawingMenu.Add("draw.Ignite", new CheckBox("计算点燃伤害"));

            MiscMenu = ConfigMenu.AddSubMenu("杂项菜单", "mMenu");
            MiscMenu.AddLabel("杂项");
            MiscMenu.Add("pet", new CheckBox("自动移动克隆"));
        }
    }
}