#region

using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

#endregion

namespace SimplisticTemplate.Champion.Fizz.Utils
{
    internal static class GameMenu
    {
        private static Menu _menu;

        public static Menu ComboMenu,
            HarassMenu,
            MiscMenu,
            DrawMenu;

        private static AIHeroClient Me
        {
            get { return ObjectManager.Player; }
        }

        public static void Initialize()
        {
            _menu = MainMenu.AddMenu("Simplistic " + Me.ChampionName, Me.ChampionName.ToLower());
            _menu.AddLabel("Simplistic Fizz");
            _menu.AddLabel("by nonm");

            ComboMenu = _menu.AddSubMenu("连招", "combo");
            ComboMenu.AddLabel("连招设置");
            ComboMenu.Add("qrcombo", new KeyBind("Q - R 连招", false, KeyBind.BindTypes.HoldActive, 'A'));
            ComboMenu.Add("useQ", new CheckBox("使用 Q"));
            ComboMenu.Add("useW", new CheckBox("使用 W"));
            ComboMenu.Add("useE", new CheckBox("使用 E"));
            ComboMenu.Add("useR", new CheckBox("使用 R"));
            ComboMenu.Add("useEGap", new CheckBox("Use E to Gapclose and then Q if killable?"));
            ComboMenu.Add("useRGap", new CheckBox("Use R and then E for Gapclose if killable?"));

            HarassMenu = _menu.AddSubMenu("骚扰", "harass");
            HarassMenu.AddLabel("骚扰设置");
            HarassMenu.Add("useQ", new CheckBox("使用 Q"));
            HarassMenu.Add("useW", new CheckBox("使用 W"));
            HarassMenu.Add("useE", new CheckBox("使用 E"));
            HarassMenu.AddSeparator();
            HarassMenu.AddLabel("E 模式: (1) 回之前位置 (2) 至敌人");
            HarassMenu.Add("useEMode", new Slider("E 模式", 0, 0, 1));

            MiscMenu = _menu.AddSubMenu("杂项", "misc");
            MiscMenu.AddLabel("杂项设置");
            MiscMenu.AddLabel("使用 W : (1) 之前 Q (2) 接触到敌人时");
            MiscMenu.Add("useWMode", new Slider("使用 W", 0, 0, 1));
            MiscMenu.AddSeparator();
            MiscMenu.Add("useETower", new CheckBox("使用 E 躲避塔的攻击"));

            DrawMenu = _menu.AddSubMenu("线圈", "drawings");
            DrawMenu.AddLabel("线圈设置");
            DrawMenu.Add("disable", new CheckBox("屏蔽所有线圈", false));
            DrawMenu.Add("drawDamage", new CheckBox("显示伤害"));
            DrawMenu.Add("drawQ", new CheckBox("显示 Q 范围"));
            DrawMenu.Add("drawW", new CheckBox("显示 W 范围"));
            DrawMenu.Add("drawE", new CheckBox("显示 E 范围"));
            DrawMenu.Add("drawR", new CheckBox("显示 R 范围"));
            DrawMenu.Add("drawRPred", new CheckBox("显示 R 范围"));
        }
    }
}