namespace Jinx
{
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    internal class Config
    {
        /// <summary>
        /// Initializes and Contains the Menu.
        /// </summary>
        public static Menu ConfigMenu;

        /// <summary>
        /// Initializes and Contains the Menu.
        /// </summary>
        public static Menu ComboMenu;

        /// <summary>
        /// Initializes and Contains the Menu.
        /// </summary>
        public static Menu LastHitMenu;

        /// <summary>
        /// Initializes and Contains the Menu.
        /// </summary>
        public static Menu HarassMenu;

        /// <summary>
        /// Initializes and Contains the Menu.
        /// </summary>
        public static Menu LaneClearMenu;

        /// <summary>
        /// Initializes and Contains the Menu.
        /// </summary>
        public static Menu KillStealMenu;

        /// <summary>
        /// Initializes and Contains the Menu.
        /// </summary>
        public static Menu JungleClearMenu;

        /// <summary>
        /// Initializes and Contains the Menu.
        /// </summary>
        public static Menu JungleStealMenu;

        /// <summary>
        /// Initializes and Contains the Menu.
        /// </summary>
        public static Menu FleeMenu;

        /// <summary>
        /// Initializes and Contains the Menu.
        /// </summary>
        public static Menu DrawingMenu;

        /// <summary>
        /// Initializes and Contains the Menu.
        /// </summary>
        public static Menu MiscMenu;

        /// <summary>
        /// Creates the Menu
        /// </summary>
        public static void Initialize()
        {
            ConfigMenu = MainMenu.AddMenu("CH汉化-吉茵珂絲", "Jin-XXX");
            ConfigMenu.AddGroupLabel("This addon is made by KarmaPanda and should not be redistributed in any way.");
            ConfigMenu.AddGroupLabel("Any unauthorized redistribution without credits will result in severe consequences.");
            ConfigMenu.AddGroupLabel("Thank you for using this addon and have a fun time!");

            ComboMenu = ConfigMenu.AddSubMenu("连招", "Combo");
            ComboMenu.AddLabel("连招");
            ComboMenu.Add("useQ", new CheckBox("使用 Q"));
            ComboMenu.Add("useW", new CheckBox("使用 W"));
            ComboMenu.Add("useE", new CheckBox("使用 E"));
            ComboMenu.Add("useR", new CheckBox("使用 R"));
            ComboMenu.AddLabel("蓝量控制器");
            ComboMenu.Add("manaQ", new Slider("控制 Q", 25));
            ComboMenu.Add("manaW", new Slider("控制 W", 25));
            ComboMenu.Add("manaE", new Slider("控制 E", 25));
            ComboMenu.Add("manaR", new Slider("控制 R", 25));
            ComboMenu.AddLabel("优先攻击英雄而非小兵");
            ComboMenu.Add("qCountC", new Slider("使用 Q 如果可命中 {0} 英雄", 3, 1, 5));
            ComboMenu.Add("rCountC", new Slider("使用 R 如果可命中 {0} 英雄", 5, 1, 5));
            ComboMenu.AddLabel("预判设定");
            ComboMenu.Add("wSlider", new Slider("使用 W 如果命中率为 {0}%", 80));
            ComboMenu.Add("eSlider", new Slider("使用 E 如果命中率为 {0}%", 80));
            ComboMenu.Add("rSlider", new Slider("使用 R 如果命中率为 {0}%", 80));
            ComboMenu.AddLabel("额外设置");
            ComboMenu.Add("wRange2", new Slider("当目标超出 {0} 范围才使用 W", 150, 0, 1500));
            ComboMenu.Add("eRange", new Slider("当目标超出 {0} 范围才使用 E", 150, 0, 900));
            ComboMenu.Add("eRange2", new Slider("最远 E 范围", 900, 0, 900));
            ComboMenu.Add("rRange2", new Slider("最远 R 范围", 3000, 0, 3000));

            LastHitMenu = ConfigMenu.AddSubMenu("尾兵", "LastHit");
            LastHitMenu.AddGroupLabel("尾兵设置");
            LastHitMenu.Add("useQ", new CheckBox("使用 Q"));
            LastHitMenu.Add("qCountM", new Slider("使用 Q 如果可命中 {0} 个小兵", 3, 1, 7));
            LastHitMenu.AddLabel("蓝量控制器");
            LastHitMenu.Add("manaQ", new Slider("控制 Q", 25));

            HarassMenu = ConfigMenu.AddSubMenu("骚扰", "Harass");
            HarassMenu.AddLabel("骚扰");
            HarassMenu.Add("useQ", new CheckBox("使用 Q"));
            HarassMenu.Add("useW", new CheckBox("使用 W"));
            HarassMenu.AddLabel("蓝量控制器");
            HarassMenu.Add("manaQ", new Slider("控制 Q", 15));
            HarassMenu.Add("manaW", new Slider("控制 W", 35));
            HarassMenu.AddLabel("优先攻击英雄而非小兵");
            HarassMenu.Add("qCountC", new Slider("使用 Q 如果可命中 {0} 英雄", 3, 1, 5));
            HarassMenu.Add("qCountM", new Slider("使用 Q 如果可命中 {0} 小兵", 3, 1, 7));
            HarassMenu.AddLabel("预判设定");
            HarassMenu.Add("wSlider", new Slider("使用 W 如果命中率为 {0}%", 95));
            HarassMenu.AddLabel("额外设置");
            HarassMenu.Add("wRange2", new Slider("不使用 W 当目标在 {0} 范围内", 0, 0, 1450));

            LaneClearMenu = ConfigMenu.AddSubMenu("清线", "LaneClear");
            LaneClearMenu.AddLabel("清线设置");
            LaneClearMenu.Add("useQ", new CheckBox("使用 Q"));
            LaneClearMenu.Add("lastHit", new CheckBox("尾兵范围外的小兵", false));
            LaneClearMenu.Add("manaQ", new Slider("控制蓝量控制 Q", 25));
            LaneClearMenu.Add("qCountM", new Slider("使用 Q 如果可命中 {0} 小兵", 3, 1, 7));

            KillStealMenu = ConfigMenu.AddSubMenu("抢头", "KillSteal");
            KillStealMenu.Add("toggle", new CheckBox("开启抢头"));
            KillStealMenu.Add("useW", new CheckBox("使用 W"));
            KillStealMenu.Add("useR", new CheckBox("使用 R"));
            KillStealMenu.AddLabel("蓝量控制器");
            KillStealMenu.Add("manaW", new Slider("控制 W", 25));
            KillStealMenu.Add("manaR", new Slider("控制 R", 25));
            KillStealMenu.AddLabel("预判设置");
            KillStealMenu.Add("wSlider", new Slider("使用 W 如果命中率为 {0}%", 80));
            KillStealMenu.Add("rSlider", new Slider("使用 R 如果命中率为 {0}%", 80));
            KillStealMenu.AddLabel("技能设置");
            KillStealMenu.Add("wRange", new Slider("使用 W 的最低距离", 450, 0, 1500));
            KillStealMenu.Add("rRange", new Slider("使用 R 的最远距离", 3000, 0, 3000));

            JungleClearMenu = ConfigMenu.AddSubMenu("清野", "JungleClear");
            JungleClearMenu.AddLabel("清野设置");
            JungleClearMenu.Add("useQ", new CheckBox("使用 Q"));
            JungleClearMenu.Add("useW", new CheckBox("使用 W", false));
            JungleClearMenu.AddLabel("蓝量控制器");
            JungleClearMenu.Add("manaQ", new Slider("控制 Q", 25));
            JungleClearMenu.Add("manaW", new Slider("控制 W", 25));
            JungleClearMenu.AddLabel("杂项设置");
            JungleClearMenu.Add("wSlider", new Slider("使用 W 如果命中率为 {0}%", 85));

            JungleStealMenu = ConfigMenu.AddSubMenu("偷野", "JungleSteal");
            JungleStealMenu.AddLabel("偷野设置，未更新至元素龙");
            JungleStealMenu.Add("toggle", new CheckBox("开启偷野开关", false));
            JungleStealMenu.Add("manaR", new Slider("R 蓝量控制器", 25));
            JungleStealMenu.Add("rRange", new Slider("距离野怪 X 使用 R", 3000, 0, 3000));
            if (Game.MapId == GameMapId.SummonersRift)
            {
                JungleStealMenu.AddLabel("野怪");
                JungleStealMenu.Add("SRU_Baron", new CheckBox("男爵"));
                JungleStealMenu.Add("SRU_Dragon", new CheckBox("小龙"));
                JungleStealMenu.AddLabel("状态");
                JungleStealMenu.Add("SRU_Blue", new CheckBox("蓝", false));
                JungleStealMenu.Add("SRU_Red", new CheckBox("红", false));
                JungleStealMenu.AddLabel("小野怪");
                JungleStealMenu.Add("SRU_Gromp", new CheckBox("青蛙", false));
                JungleStealMenu.Add("SRU_Murkwolf", new CheckBox("狼", false));
                JungleStealMenu.Add("SRU_Krug", new CheckBox("石头人", false));
                JungleStealMenu.Add("SRU_Razorbeak", new CheckBox("4鸟", false));
                JungleStealMenu.Add("Sru_Crab", new CheckBox("河蟹", false));
            }

            if (Game.MapId == GameMapId.TwistedTreeline)
            {
                JungleStealMenu.AddLabel("史诗");
                JungleStealMenu.Add("TT_Spiderboss8.1", new CheckBox("蜘蛛"));
                JungleStealMenu.AddLabel("Camps");
                JungleStealMenu.Add("TT_NWraith1.1", new CheckBox("幽灵", false));
                JungleStealMenu.Add("TT_NWraith4.1", new CheckBox("幽灵2", false));
                JungleStealMenu.Add("TT_NGolem2.1", new CheckBox("石头人", false));
                JungleStealMenu.Add("TT_NGolem5.1", new CheckBox("石头人2", false));
                JungleStealMenu.Add("TT_NWolf3.1", new CheckBox("狼", false));
                JungleStealMenu.Add("TT_NWolf6.1", new CheckBox("狼2", false));
            }

            FleeMenu = ConfigMenu.AddSubMenu("逃跑", "Flee");
            FleeMenu.AddLabel("逃跑设置");
            FleeMenu.Add("useW", new CheckBox("使用 W"));
            FleeMenu.Add("useE", new CheckBox("使用 E"));
            FleeMenu.Add("wSlider", new Slider("使用 W 如果命中率为 {0}%", 75));
            FleeMenu.Add("eSlider", new Slider("使用 E 如果命中率为 {0}%", 75));

            DrawingMenu = ConfigMenu.AddSubMenu("线圈", "Drawing");
            DrawingMenu.AddLabel("线圈设置");
            DrawingMenu.Add("drawQ", new CheckBox("显示 Q 范围"));
            DrawingMenu.Add("drawW", new CheckBox("显示 W 范围"));
            DrawingMenu.Add("drawE", new CheckBox("显示 E 范围", false));
            DrawingMenu.AddLabel("显示预判");
            DrawingMenu.Add("predW", new CheckBox("显示 W 预判"));
            DrawingMenu.Add("predR", new CheckBox("显示 R 预判 (R预计施放位置)", false));
            DrawingMenu.AddLabel("伤害指示器");
            DrawingMenu.Add("draw.Damage", new CheckBox("显示 伤害"));
            DrawingMenu.Add("draw.Q", new CheckBox("计算 Q 伤害", false));
            DrawingMenu.Add("draw.W", new CheckBox("计算 W 伤害"));
            DrawingMenu.Add("draw.E", new CheckBox("计算 E 伤害", false));
            DrawingMenu.Add("draw.R", new CheckBox("计算 R 伤害"));
            DrawingMenu.AddLabel("伤害指示器颜色");
            DrawingMenu.Add("draw_Alpha", new Slider("Alpha: ", 255, 0, 255));
            DrawingMenu.Add("draw_Red", new Slider("Red: ", 255, 0, 255));
            DrawingMenu.Add("draw_Green", new Slider("Green: ", 0, 0, 255));
            DrawingMenu.Add("draw_Blue", new Slider("Blue: ", 0, 0, 255));

            MiscMenu = ConfigMenu.AddSubMenu("杂项菜单", "Misc");
            MiscMenu.AddLabel("技能打断");
            MiscMenu.Add("interruptE", new CheckBox("使用 E"));
            MiscMenu.Add("interruptmanaE", new Slider("最低蓝量剩余使用 E", 25));
            MiscMenu.AddLabel("防突进");
            MiscMenu.Add("gapcloserE", new CheckBox("使用 E"));
            MiscMenu.Add("gapclosermanaE", new Slider("最低蓝量剩余使用 E", 25));
            MiscMenu.AddLabel("技能设置");
            MiscMenu.Add("autoW", new CheckBox("某些情况下自动 W"));
            MiscMenu.Add("autoE", new CheckBox("某些情况下自动"));
            MiscMenu.Add("wRange", new CheckBox("普工范围下只使用 W", false));
            MiscMenu.Add("rRange", new Slider("不使用 R 如果玩家距离至目标为 {0}", 800, 0, 3000));
            MiscMenu.AddLabel("自动W 设置（必须开启自动W）)");
            MiscMenu.Add("stunW", new CheckBox("对被晕眩的敌人使用", false));
            MiscMenu.Add("charmW", new CheckBox("对被魅惑的敌人使用", false));
            //MiscMenu.Add("tauntW", new CheckBox("Use W on Taunted Enemy", false));
            MiscMenu.Add("fearW", new CheckBox("对被恐的惧敌人使用", false));
            MiscMenu.Add("snareW", new CheckBox("被禁锢的敌人使用", false));
            MiscMenu.Add("wRange2", new Slider("只使用 W 如果玩家距离至目标为 {0}", 450, 0, 1500));
            MiscMenu.AddLabel("预判设置");
            MiscMenu.Add("wSlider", new Slider("使用 W 如果命中率为 {0}%", 75));
            MiscMenu.Add("eSlider", new Slider("使用 E 如果命中率为 {0}%", 75));
            /*MiscMenu.AddLabel("Allah Akbar");
            MiscMenu.Add("allahAkbarT", new CheckBox("Play Allah Akbar after casting R", false));*/
        }
    }
}
