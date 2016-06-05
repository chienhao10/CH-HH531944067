using System.Linq;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace VladimirTheTroll
{
    internal static class VladimirTheTrollMeNu
    {
        private static Menu _myMenu;
        public static Menu ComboMenu, DrawMeNu, HarassMeNu, Activator, FarmMeNu, MiscMeNu;

        public static void LoadMenu()
        {
            MyVladimirTheTrollPage();
            DrawMeNuPage();
            ComboMenuPage();
            FarmMeNuPage();
            HarassMeNuPage();
            ActivatorPage();
            MiscMeNuPage();
        }

        private static void MyVladimirTheTrollPage()
        {
            _myMenu = MainMenu.AddMenu("CH汉化-吸血鬼", "main");
            _myMenu.AddLabel(" Vladimir The Troll " + Program.Version);
            _myMenu.AddLabel(" Made by MeLoDag | CH汉化");
        }

        private static void DrawMeNuPage()
        {
            DrawMeNu = _myMenu.AddSubMenu("线圈", "Draw");
            DrawMeNu.AddGroupLabel("线圈设置:");
            DrawMeNu.Add("nodraw",
                new CheckBox("屏蔽线圈", false));
          DrawMeNu.AddSeparator();
            DrawMeNu.Add("draw.Q",
                new CheckBox("显示 Q"));
            DrawMeNu.Add("draw.W",
                new CheckBox("显示 W"));
            DrawMeNu.Add("draw.E",
                new CheckBox("显示 E"));
            DrawMeNu.Add("draw.R",
                new CheckBox("显示 R"));
          }

        private static void ComboMenuPage()
        {
            ComboMenu = _myMenu.AddSubMenu("连招", "Combo");
            ComboMenu.AddGroupLabel("连招设置:");
            ComboMenu.Add("useQCombo", new CheckBox("使用 Q "));
            ComboMenu.Add("useECombo", new CheckBox("使用 E "));
            ComboMenu.Add("useWCombo", new CheckBox("使用 W "));
            ComboMenu.Add("useWcostumHP", new Slider("使用 W 如果HP%低于", 70, 0, 100));
            ComboMenu.Add("useRCombo", new CheckBox("使用 R "));
            ComboMenu.Add("Rcount", new Slider("使用 R 当可命中 X 敌人数量 ", 2, 1, 5));
        }


        private static void FarmMeNuPage()
        {
            FarmMeNu = _myMenu.AddSubMenu("农兵设置", "FarmSettings");
            FarmMeNu.AddGroupLabel("清线");
            FarmMeNu.Add("qFarmAlways", new CheckBox("总是使用Q"));
            FarmMeNu.Add("qFarm", new CheckBox("使用 Q 尾兵[全局设定]"));
            FarmMeNu.AddLabel("清野");
            FarmMeNu.Add("useQJungle", new CheckBox("使用 Q"));
        }

        private static void HarassMeNuPage()
        {
            HarassMeNu = _myMenu.AddSubMenu("骚扰", "Harass");
            HarassMeNu.AddGroupLabel("骚扰设置");
            HarassMeNu.Add("useQHarass", new CheckBox("使用 Q"));
            HarassMeNu.AddLabel("自动骚扰设置");
            HarassMeNu.Add("useQAuto", new CheckBox("使用 Q"));
            HarassMeNu.AddSeparator();
            HarassMeNu.AddGroupLabel("抢头设置:");
            HarassMeNu.Add("ksQ",
                new CheckBox("使用 Q", false));
        }

        private static void ActivatorPage()
        {
            Activator = _myMenu.AddSubMenu("活化剂", "Items");
            Activator.AddLabel("中亚设置");
            Activator.Add("Zhonyas", new CheckBox("使用中亚"));
            Activator.Add("ZhonyasHp", new Slider("血量低于 X % 使用中亚", 20, 0, 100));
            Activator.AddSeparator();
            Activator.AddGroupLabel("药水");
            Activator.Add("spells.Potions.Check",
                new CheckBox("使用药水"));
            Activator.Add("spells.Potions.HP",
                new Slider("最低血量% 使用药水", 60, 1));
            Activator.Add("spells.Potions.Mana",
                new Slider("最低蓝量% 使用药水", 60, 1));
            Activator.AddSeparator();
            Activator.AddGroupLabel("召唤师技能设置:");
            Activator.AddGroupLabel("治疗设置:");
            Activator.Add("spells.Heal.Hp",
                new Slider("低于HP%时 使用治疗", 30, 1));
            Activator.AddGroupLabel("点燃设在:");
            Activator.Add("spells.Ignite.Focus",
                new Slider("敌人血量低于 X % 时 使用点燃", 10, 1));
        }

        private static void MiscMeNuPage()
        {
            MiscMeNu = _myMenu.AddSubMenu("杂项菜单", "othermenu");
            MiscMeNu.AddGroupLabel("换肤");
            MiscMeNu.Add("checkSkin",
                new CheckBox("开启换肤:", false));
            MiscMeNu.Add("skin.Id",
                new Slider("皮肤ID", 5, 0, 10));
        }

        public static bool Nodraw()
        {
            return DrawMeNu["nodraw"].Cast<CheckBox>().CurrentValue;
        }
        
        public static bool DrawingsQ()
        {
            return DrawMeNu["draw.Q"].Cast<CheckBox>().CurrentValue;
        }

        public static bool DrawingsW()
        {
            return DrawMeNu["draw.W"].Cast<CheckBox>().CurrentValue;
        }

        public static bool DrawingsE()
        {
            return DrawMeNu["draw.E"].Cast<CheckBox>().CurrentValue;
        }

        public static bool DrawingsR()
        {
            return DrawMeNu["draw.R"].Cast<CheckBox>().CurrentValue;
        }

        public static bool DrawingsT()
        {
            return DrawMeNu["draw.T"].Cast<CheckBox>().CurrentValue;
        }
        
      public static bool SpellsPotionsCheck()
        {
            return Activator["spells.Potions.Check"].Cast<CheckBox>().CurrentValue;
        }

        public static float SpellsPotionsHp()
        {
            return Activator["spells.Potions.HP"].Cast<Slider>().CurrentValue;
        }

        public static float SpellsPotionsM()
        {
            return Activator["spells.Potions.Mana"].Cast<Slider>().CurrentValue;
        }

        public static float SpellsHealHp()
        {
            return Activator["spells.Heal.HP"].Cast<Slider>().CurrentValue;
        }

        public static float SpellsIgniteFocus()
        {
            return Activator["spells.Ignite.Focus"].Cast<Slider>().CurrentValue;
        }
        
        public static int SkinId()
        {
            return MiscMeNu["skin.Id"].Cast<Slider>().CurrentValue;
        }

      
        public static bool SkinChanger()
        {
            return MiscMeNu["SkinChanger"].Cast<CheckBox>().CurrentValue;
        }

        public static bool CheckSkin()
        {
            return MiscMeNu["checkSkin"].Cast<CheckBox>().CurrentValue;
        }
     }
}