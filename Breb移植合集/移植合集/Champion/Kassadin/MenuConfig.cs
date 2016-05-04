using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Kassawin
{
    internal class MenuConfig : Helper
    {
        public static void OnLoad()
        {
            Config = MainMenu.AddMenu(Menuname, Menuname);

            comboMenu = Config.AddSubMenu("连招", "Combo Settings");
            comboMenu.Add("useq", new CheckBox("使用 [Q]"));
            comboMenu.Add("usew", new CheckBox("使用 [W]"));
            comboMenu.Add("usee", new CheckBox("使用 [E]"));
            comboMenu.Add("user", new CheckBox("使用 [R]"));
            comboMenu.Add("usert", new CheckBox("塔下不 [R]"));
            comboMenu.Add("useignite", new CheckBox("使用 [点燃]"));
            comboMenu.Add("rcount", new Slider("只使用 [R] 当有 X 层叠加", 1, 1, 5));
            comboMenu.Add("rhp", new Slider("只使用 [R] 当 HP% 高于", 15));

            harassMenu = Config.AddSubMenu("骚扰", "Harass Settings");
            harassMenu.Add("useqharass", new CheckBox("使用 [Q]"));
            harassMenu.Add("useeharass", new CheckBox("使用 [E]"));
            harassMenu.Add("harassmana", new Slider("最低蓝量", 30));

            farmMenu = Config.AddSubMenu("农兵", "Farm Settings");
            farmMenu.AddGroupLabel("清线");
            farmMenu.Add("minmanalaneclear", new Slider("最低蓝量", 30));
            farmMenu.Add("useql", new CheckBox("使用 [Q]"));
            farmMenu.Add("usewl", new CheckBox("使用 [W]"));
            farmMenu.Add("useel", new CheckBox("使用 [E]"));
            farmMenu.Add("userl", new CheckBox("使用 [R]"));
            farmMenu.Add("useels", new Slider("最少小兵 E", 3, 1, 10));
            farmMenu.Add("userls", new Slider("最少小兵 R", 3, 1, 10));
            farmMenu.Add("rcountl", new Slider("只使用 [R] 当有 X 层叠加", 1, 1, 5));
            farmMenu.AddSeparator();
            farmMenu.AddGroupLabel("清野");
            farmMenu.Add("minmanajungleclear", new Slider("最低蓝量", 30));
            farmMenu.Add("useqj", new CheckBox("使用 [Q]"));
            farmMenu.Add("usewj", new CheckBox("使用 [W]"));
            farmMenu.Add("useej", new CheckBox("使用 [E]"));
            farmMenu.Add("userj", new CheckBox("使用 [R]"));
            farmMenu.Add("rcountj", new Slider("只使用 [R] 当有 X 层叠加", 1, 1, 5));
            farmMenu.AddSeparator();
            farmMenu.AddGroupLabel("尾兵");
            farmMenu.Add("minmanalasthit", new Slider("最低蓝量", 30));
            farmMenu.Add("useqlh", new CheckBox("使用 [Q]"));
            farmMenu.AddSeparator();

            ksMenu = Config.AddSubMenu("抢头", "Kill Steal Settings");
            ksMenu.Add("ks", new CheckBox("开启 [KS]"));
            ksMenu.Add("qks", new CheckBox("使用 [Q]"));
            ksMenu.Add("eks", new CheckBox("使用 [E]"));
            ksMenu.Add("rks", new CheckBox("使用 [R]"));
            ksMenu.Add("rgks", new CheckBox("使用 [R]进行接近"));

            drawMenu = Config.AddSubMenu("线圈", "Drawings");
            drawMenu.Add("enabledraw", new CheckBox("开启线圈"));
            drawMenu.Add("drawq", new CheckBox("显示 [Q] 范围"));
            drawMenu.Add("drawe", new CheckBox("显示 [E] 范围"));
            drawMenu.Add("drawr", new CheckBox("显示 [R] 范围"));
            drawMenu.Add("drawcount", new CheckBox("显示 [R] 层数"));
            drawMenu.Add("drawdamage", new CheckBox("显示 伤害"));

            miscMenu = Config.AddSubMenu("杂项", "Misc");
            miscMenu.Add("fleemode", new KeyBind("逃跑模式", false, KeyBind.BindTypes.HoldActive, 'A'));
            miscMenu.Add("userflee", new CheckBox("开启 [R] 逃跑模式"));
        }
    }
}