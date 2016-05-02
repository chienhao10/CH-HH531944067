namespace Khappa_Zix.Load
{
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    internal class menu
    {
        public static Menu MenuIni, Combo, Harass, Misc, Clear, KillSteal, Mana, Jump, Draw;

        internal static void Load()
        {
            MenuIni = MainMenu.AddMenu("CH汉化-螳螂", "Khappa'Zix");

            Jump = MenuIni.AddSubMenu("跳跃设置 ", "JumpsHandler");
            Jump.AddGroupLabel("E 设置");
            Jump.Add("double", new CheckBox("使用 双跳-E", false));
            Jump.Add("block", new CheckBox("屏蔽E如果会撞墙"));
            Jump.Add("delay", new Slider("第2跳延迟 {0}", 150, 0, 300));
            Jump.AddGroupLabel("第一跳");
            Jump.Add("1jump", new ComboBox("第一跳", 0, "向泉水", "向友军", "至鼠标", "至下一个目标"));
            Jump.AddGroupLabel("第二跳");
            Jump.Add("2jump", new ComboBox("第二跳", 0, "向泉水", "向友军", "至鼠标", "至下一个目标"));
            Jump.AddSeparator();
            Jump.AddGroupLabel("额外设置");
            Jump.AddLabel("跳出塔外");
            Jump.Add("save", new CheckBox("跳出塔攻击距离"));
            Jump.Add("saveh", new Slider("在血量低于 {0}% 时跳出", 15));

            Combo = MenuIni.AddSubMenu("连招 ", "Combo");
            Combo.AddGroupLabel("连招设置");
            Combo.Add("Q", new CheckBox("使用 Q "));
            Combo.Add("W", new CheckBox("使用 W "));
            Combo.Add("E", new CheckBox("使用 E "));
            Combo.AddSeparator();
            Combo.AddGroupLabel("E 设置");
            Combo.Add("Edive", new CheckBox("E 强杀（塔下）"));
            Combo.Add("safe", new Slider("不 E 如果目标附近敌人数量为 {0}", 3, 0, 5));
            Combo.Add("dis", new Slider("使用E 如果目标距离我的范围多余 {0}", 385, 0, 850));
            Combo.AddSeparator();
            Combo.AddGroupLabel("R 设置");
            Combo.Add("useR", new CheckBox("使用 R"));
            Combo.Add("R", new CheckBox("全技能冷却时 使用 R"));
            Combo.Add("NoAA", new CheckBox("R开启时 不普攻"));
            Combo.Add("Rmode", new ComboBox("R 模式", 0, "进行连招距离（接近）", "一直"));
            Combo.Add("danger", new Slider("当附近敌人数量多于 {0}时使用", 3, 1, 5));

            Harass = MenuIni.AddSubMenu("骚扰 ", "Harass");
            Harass.AddGroupLabel("骚扰设置");
            Harass.Add("Q", new CheckBox("使用 Q "));
            Harass.Add("W", new CheckBox("使用 W "));
            Harass.Add("E", new CheckBox("使用 E "));
            Harass.Add("Edive", new CheckBox("E 强杀（塔下）"));

            Clear = MenuIni.AddSubMenu("农兵 ", "Clear");
            Clear.AddGroupLabel("清线设置");
            Clear.Add("Qc", new CheckBox("使用 Q "));
            Clear.Add("Wc", new CheckBox("使用 W "));
            Clear.Add("Ec", new CheckBox("使用 E ", false));
            Clear.AddSeparator();
            Clear.AddGroupLabel("尾兵设置");
            Clear.Add("Qh", new CheckBox("使用 Q "));
            Clear.Add("Wh", new CheckBox("使用 W "));
            Clear.Add("Eh", new CheckBox("使用 E ", false));
            Clear.AddSeparator();
            Clear.AddGroupLabel("清野设置");
            Clear.Add("Qj", new CheckBox("使用 Q "));
            Clear.Add("Wj", new CheckBox("使用 W "));
            Clear.Add("Ej", new CheckBox("使用 E ", false));

            Mana = MenuIni.AddSubMenu("蓝量控制器 ", "ManaManager");
            Mana.AddGroupLabel("骚扰蓝");
            Mana.Add("harass", new Slider("保留 {0}% ", 60));
            Mana.AddSeparator();
            Mana.AddGroupLabel("清线蓝");
            Mana.Add("lane", new Slider("保留 {0}% ", 75));
            Mana.AddSeparator();
            Mana.AddGroupLabel("尾兵蓝");
            Mana.Add("last", new Slider("保留 {0}% ", 50));
            Mana.AddSeparator();
            Mana.AddGroupLabel("清野蓝");
            Mana.Add("jungle", new Slider("保留 {0}% ", 30));

            KillSteal = MenuIni.AddSubMenu("抢头 ", "KillSteal");
            KillSteal.AddGroupLabel("抢头设置");
            KillSteal.Add("Q", new CheckBox("使用 Q "));
            KillSteal.Add("W", new CheckBox("使用 W "));
            KillSteal.Add("E", new CheckBox("使用 E "));

            Draw = MenuIni.AddSubMenu("线圈 ", "Drawings");
            Draw.AddGroupLabel("线圈竖直");
            Draw.Add("Q", new CheckBox("显示 Q "));
            Draw.Add("W", new CheckBox("显示 W "));
            Draw.Add("E", new CheckBox("显示 E "));

            Misc = MenuIni.AddSubMenu("在线 ", "Misc");
            Misc.AddGroupLabel("技能命中率");
            Misc.Add("hitChance", new ComboBox("命中率", 0, "高", "中", "低"));
        }
    }
}