namespace KappAzir
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    using Utility;

    internal static class Menus
    {
        public static Menu Menuini, Auto, JumperMenu, ComboMenu, HarassMenu, LaneClearMenu, JungleClearMenu, KillstealMenu, DrawMenu, ColorMenu;

        public static void Execute()
        {
            Menuini = MainMenu.AddMenu("Kapp沙皇", "KappAzir");
            Auto = Menuini.AddSubMenu("自动");
            JumperMenu = Menuini.AddSubMenu("漂移");
            ComboMenu = Menuini.AddSubMenu("连招");
            HarassMenu = Menuini.AddSubMenu("骚扰");
            LaneClearMenu = Menuini.AddSubMenu("清线");
            JungleClearMenu = Menuini.AddSubMenu("清野");
            KillstealMenu = Menuini.AddSubMenu("抢头");
            DrawMenu = Menuini.AddSubMenu("线圈");
            ColorMenu = Menuini.AddSubMenu("颜色");

            Auto.AddGroupLabel("设置");
            Auto.Add("gap", new CheckBox("防突进"));
            Auto.Add("int", new CheckBox("技能打断"));
            Auto.Add("danger", new ComboBox("技能危险等级打断", 1, "高", "中", "低"));
            Auto.AddGroupLabel("防御塔设置");
            Auto.Add("tower", new CheckBox("召唤防御塔"));
            Auto.Add("Tenemy", new Slider("召唤防御塔当有 [{0}] 敌人", 3, 1, 6));
            Auto.AddGroupLabel("自动防突进技能");
            foreach (var spell in
                from spell in Gapcloser.GapCloserList
                from enemy in EntityManager.Heroes.Enemies.Where(enemy => spell.ChampName == enemy.ChampionName)
                select spell)
            {
                Auto.Add(spell.SpellName, new CheckBox(spell.ChampName + " " + spell.SpellSlot));
            }

            if (EntityManager.Heroes.Enemies.Any(e => e.Hero == Champion.Rengar))
            {
                Auto.Add("rengar", new CheckBox("狮子狗跳跃"));
            }

            JumperMenu.Add("jump", new KeyBind("WEQ 逃跑按键", false, KeyBind.BindTypes.HoldActive, 'A'));
            JumperMenu.Add("normal", new KeyBind("正常漂移推按键", false, KeyBind.BindTypes.HoldActive, 'S'));
            JumperMenu.Add("new", new KeyBind("新漂移推", false, KeyBind.BindTypes.HoldActive, 'Z'));
            JumperMenu.Add("flash", new CheckBox("尝试使用闪现进行大范围伤害"));
            JumperMenu.Add("delay", new Slider("延迟 EQ", 200, 0, 500));
            JumperMenu.Add("range", new Slider("检查士兵距离", 800, 0, 1000));

            ComboMenu.AddGroupLabel("连招设置");
            ComboMenu.Add("key", new KeyBind("连招按键", false, KeyBind.BindTypes.HoldActive, 32));
            ComboMenu.AddSeparator(0);
            ComboMenu.AddGroupLabel("Q 设置");
            ComboMenu.Add("Q", new CheckBox("使用 Q"));
            ComboMenu.Add("WQ", new CheckBox("使用 W > Q"));
            ComboMenu.Add("Qaoe", new CheckBox("使用 Q 范围伤害", false));
            ComboMenu.Add("QS", new Slider("士兵数量使用Q", 1, 1, 3));
            ComboMenu.AddSeparator(0);
            ComboMenu.AddGroupLabel("W 设置");
            ComboMenu.Add("W", new CheckBox("使用 W"));
            ComboMenu.Add("Wsave", new CheckBox("保留 1 个 W 层数", false));
            ComboMenu.Add("WS", new Slider("限制召唤几名士兵", 3, 1, 3));
            ComboMenu.AddSeparator(0);
            ComboMenu.AddGroupLabel("E 设置");
            ComboMenu.Add("E", new CheckBox("使用 E"));
            ComboMenu.Add("Ekill", new CheckBox("E 只用于可击杀敌人"));
            ComboMenu.Add("Edive", new CheckBox("E 越塔", false));
            ComboMenu.Add("EHP", new Slider("只使用 E 当我的血量高于 [{0}%]", 50));
            ComboMenu.Add("Esafe", new Slider("不 E 进 [{0}] 个敌人", 3, 1, 6));
            ComboMenu.AddSeparator(0);
            ComboMenu.AddGroupLabel("R 设置");
            ComboMenu.Add("R", new CheckBox("使用 R"));
            ComboMenu.Add("Rkill", new CheckBox("R 尾头"));
            ComboMenu.Add("insec", new CheckBox("连招尝试进行漂移推"));
            ComboMenu.Add("Raoe", new Slider("R 范围伤害 [{0}] 名敌人", 3, 1, 6));
            ComboMenu.Add("Rsave", new CheckBox("R 自救"));
            ComboMenu.Add("RHP", new Slider("推开敌人当我的血量低于 [{0}%]", 35));

            HarassMenu.AddGroupLabel("骚扰设置");
            HarassMenu.Add("key", new KeyBind("骚扰按键", false, KeyBind.BindTypes.HoldActive, 'C'));
            HarassMenu.Add("toggle", new KeyBind("自动骚扰开关按键", false, KeyBind.BindTypes.PressToggle, 'H'));
            HarassMenu.AddSeparator(0);
            HarassMenu.AddGroupLabel("Q 设置");
            HarassMenu.Add("Q", new CheckBox("使用 Q"));
            HarassMenu.Add("WQ", new CheckBox("使用 W > Q"));
            HarassMenu.Add("QS", new Slider("士兵数量使用Q", 1, 1, 3));
            HarassMenu.Add("Qmana", new Slider("蓝量Q限制 < [{0}%]", 65));
            HarassMenu.AddSeparator(0);
            HarassMenu.AddGroupLabel("W 设置");
            HarassMenu.Add("W", new CheckBox("使用 W"));
            HarassMenu.Add("Wsave", new CheckBox("保留 1 个 W 层数"));
            HarassMenu.Add("WS", new Slider("限制召唤几名士兵", 3, 1, 3));
            HarassMenu.Add("Wmana", new Slider("蓝量W限制 < [{0}%]", 65));
            HarassMenu.AddSeparator(0);
            HarassMenu.AddGroupLabel("E 设置");
            HarassMenu.Add("E", new CheckBox("使用 E"));
            HarassMenu.Add("Edive", new CheckBox("E Dive Turrets", false));
            HarassMenu.Add("EHP", new Slider("只使用 E 当我的血量高于 [{0}%]", 50));
            HarassMenu.Add("Esafe", new Slider("不 E 进 [{0}] 个敌人", 3, 1, 6));
            HarassMenu.Add("Emana", new Slider("蓝量E限制 < [{0}%]", 65));

            LaneClearMenu.AddGroupLabel("清线设置");
            LaneClearMenu.Add("key", new KeyBind("清线按键", false, KeyBind.BindTypes.HoldActive, 'V'));
            LaneClearMenu.Add("Q", new CheckBox("使用 Q"));
            LaneClearMenu.Add("Qmana", new Slider("蓝量Q限制 < [{0}%]", 65));
            LaneClearMenu.Add("W", new CheckBox("使用 W"));
            LaneClearMenu.Add("Wsave", new CheckBox("保留 1 个 W 层数"));
            LaneClearMenu.Add("Wmana", new Slider("蓝量W限制 < [{0}%]", 65));

            JungleClearMenu.AddGroupLabel("清野设置");
            JungleClearMenu.Add("key", new KeyBind("清野按键", false, KeyBind.BindTypes.HoldActive, 'V'));
            JungleClearMenu.Add("Q", new CheckBox("使用 Q"));
            JungleClearMenu.Add("Qmana", new Slider("蓝量Q限制 < [{0}%]", 65));
            JungleClearMenu.Add("W", new CheckBox("使用 W"));
            JungleClearMenu.Add("Wsave", new CheckBox("保留 1 个 W 层数"));
            JungleClearMenu.Add("Wmana", new Slider("蓝量W限制 < [{0}%]", 65));

            KillstealMenu.AddGroupLabel("抢头设置");
            KillstealMenu.Add("Q", new CheckBox("使用 Q"));
            KillstealMenu.Add("E", new CheckBox("使用 E"));
            KillstealMenu.Add("R", new CheckBox("使用 R"));

            foreach (var spell in Azir.SpellList)
            {
                DrawMenu.Add(spell.Slot.ToString(), new CheckBox(spell.Slot + " Range"));
                ColorMenu.Add(spell.Slot.ToString(), new ColorPicker(spell.Slot + " Color", System.Drawing.Color.Chartreuse));
            }

            DrawMenu.Add("insec", new CheckBox("显示漂移推助手"));
        }

        public static int combobox(this Menu m, string id)
        {
            return m[id].Cast<ComboBox>().CurrentValue;
        }

        public static int slider(this Menu m, string id)
        {
            return m[id].Cast<Slider>().CurrentValue;
        }

        public static bool checkbox(this Menu m, string id)
        {
            return m[id].Cast<CheckBox>().CurrentValue;
        }

        public static bool keybind(this Menu m, string id)
        {
            return m[id].Cast<KeyBind>().CurrentValue;
        }

        public static System.Drawing.Color Color(this Menu m, string id)
        {
            return m[id].Cast<ColorPicker>().CurrentValue;
        }
    }
}