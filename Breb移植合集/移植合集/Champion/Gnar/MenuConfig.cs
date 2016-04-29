using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Slutty_Gnar_Reworked
{
    internal class MenuConfig
    {
        public const string Menuname = "Slutty Gnar";
        public static Menu Config, drawMenu, comboMenu, clearMenu, harassMenu, ksMenu, miscMenu;

        public static void CreateMenu()
        {
            Config = MainMenu.AddMenu(Menuname, Menuname);

            #region Drawings

            drawMenu = Config.AddSubMenu("线圈", "Drawings");
            drawMenu.Add("Draw", new CheckBox("显示线圈"));
            drawMenu.Add("qDraw", new CheckBox("显示 Q"));
            drawMenu.Add("wDraw", new CheckBox("显示 W"));
            drawMenu.Add("eDraw", new CheckBox("显示 E"));

            #endregion

            #region Combo Menu

            comboMenu = Config.AddSubMenu("连招设置", "combospells");
            comboMenu.AddGroupLabel("迷你形态");
            comboMenu.Add("UseQMini", new CheckBox("使用 Q"));
            comboMenu.Add("UseQs", new CheckBox("使用Q (当目标有 2 层 W 时)"));
            comboMenu.Add("eGap", new CheckBox("可击杀时，使用E接近敌方"));
            comboMenu.Add("focust", new CheckBox("集中有 2 层 W 的目标"));
            comboMenu.AddSeparator();
            comboMenu.AddGroupLabel("巨大形态");
            comboMenu.Add("UseQMega", new CheckBox("使用 Q"));
            comboMenu.Add("UseEMega", new CheckBox("使用 E"));
            comboMenu.Add("UseEMini", new CheckBox("准备变身时再使用 E"));
            comboMenu.Add("UseWMega", new CheckBox("使用 W"));
            comboMenu.Add("UseRMega", new CheckBox("使用 R"));
            comboMenu.Add("useRSlider", new Slider("最低目标数量使用 R", 3, 1, 5));

            #endregion

            #region Clear

            clearMenu = Config.AddSubMenu("推线社设置", "Clear Settings");
            clearMenu.Add("transform", new CheckBox("准备变身时使用技能"));
            clearMenu.AddSeparator();
            clearMenu.AddGroupLabel("清线");
            clearMenu.Add("UseQl", new CheckBox("使用 Q"));
            clearMenu.Add("UseQlslider", new Slider("当可命中 X 小兵，才使用 Q", 3, 1, 10));
            clearMenu.Add("UseWl", new CheckBox("使用 W"));
            clearMenu.Add("UseWlslider", new Slider("当可命中 X 小兵，才使用 W", 3, 1, 10));
            clearMenu.AddSeparator();
            clearMenu.AddGroupLabel("清野");
            clearMenu.Add("UseQj", new CheckBox("使用 Q"));
            clearMenu.Add("UseWj", new CheckBox("使用 W"));

            #endregion

            #region Harass

            harassMenu = Config.AddSubMenu("骚扰", "Harras");
            harassMenu.Add("qharras", new CheckBox("使用 Q"));
            harassMenu.Add("qharras2", new CheckBox("使用Q (当目标有 2 层 W 时)"));
            harassMenu.Add("wharras", new CheckBox("使用 R"));
            harassMenu.Add("autoq", new CheckBox("自动使用 Q", false));

            #endregion

            #region Kill Steal

            ksMenu = Config.AddSubMenu("抢头", "Kill Steal");
            ksMenu.Add("qks", new CheckBox("使用 Q"));
            ksMenu.Add("rks", new CheckBox("使用 R"));
            ksMenu.Add("qeks", new CheckBox("使用 E 接近 + Q"));

            #endregion

            #region Misc

            miscMenu = Config.AddSubMenu("杂项", "Misc");
            miscMenu.Add("qgap", new CheckBox("Q 减速突击的敌人"));
            miscMenu.Add("qwd", new CheckBox("Q/W 减速突击的敌人"));
            miscMenu.Add("qwi", new CheckBox("W 技能打断"));

            #endregion

            #region Flee Key

            miscMenu.Add("fleekey", new KeyBind("使用逃跑模式", false, KeyBind.BindTypes.HoldActive, 65));

            #endregion
        }
    }
}