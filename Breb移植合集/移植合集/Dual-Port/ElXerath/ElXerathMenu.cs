namespace ElXerath
{
    using System;

    using LeagueSharp.Common;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    public class ElXerathMenu
    {
        #region Static Fields

        public static Menu Menu, cMenu, rMenu, hMenu, miscMenu, lMenu;

        #endregion

        #region Public Methods and Operators

        public static void Initialize()
        {
            Menu = MainMenu.AddMenu("El泽拉斯", "menu");

            cMenu = Menu.AddSubMenu("Combo", "Combo");
            cMenu.Add("ElXerath.Combo.Q", new CheckBox("使用 Q"));
            cMenu.Add("ElXerath.Combo.W", new CheckBox("使用 W"));
            cMenu.Add("ElXerath.Combo.E", new CheckBox("使用 E"));

            rMenu = Menu.AddSubMenu("大招", "Ult");
            rMenu.Add("ElXerath.R.AutoUseR", new CheckBox("自动使用 R"));
            rMenu.Add("ElXerath.R.Mode", new ComboBox("模式 ", 0, "正常", "自定义延迟", "发射按键", "自定义命中率", "鼠标附近"));
            rMenu.Add("ElXerath.R.OnTap", new KeyBind("发射按键", false, KeyBind.BindTypes.HoldActive, 'T'));
            rMenu.Add("ElXerath.R.Block", new CheckBox("R 屏蔽移动"));
            rMenu.AddGroupLabel("自定义延迟");
            for (var i = 1; i <= 5; i++)
            {
                rMenu.Add("Delay" + i, new Slider("延迟" + i, 0, 0, 1500));
            }
            rMenu.Add("ElXerath.R.Radius", new Slider("目标半径", 700, 300, 1500));


            hMenu = Menu.AddSubMenu("骚扰", "Harass");
            hMenu.Add("ElXerath.Harass.Q", new CheckBox("使用 Q"));
            hMenu.Add("ElXerath.Harass.W", new CheckBox("使用 W"));
            hMenu.Add("ElXerath.AutoHarass", new KeyBind("[开关] 自动骚扰", false, KeyBind.BindTypes.PressToggle, 'U'));
            hMenu.Add("ElXerath.UseQAutoHarass", new CheckBox("使用 Q"));
            hMenu.Add("ElXerath.UseWAutoHarass", new CheckBox("使用 W"));
            hMenu.Add("ElXerath.harass.mana", new Slider("自动骚扰蓝量", 55));

            lMenu = Menu.AddSubMenu("推线", "LaneClear");
            lMenu.Add("ElXerath.clear.Q", new CheckBox("清线 Q"));
            lMenu.Add("ElXerath.clear.W", new CheckBox("清线 W"));
            lMenu.Add("ElXerath.jclear.Q", new CheckBox("清野 Q"));
            lMenu.Add("ElXerath.jclear.W", new CheckBox("清野 W"));
            lMenu.Add("ElXerath.jclear.E", new CheckBox("清野 E"));
            lMenu.Add("minmanaclear", new Slider("自动骚扰蓝量", 55));

            //ElXerath.Misc
            miscMenu = Menu.AddSubMenu("杂项", "Misc");
            miscMenu.Add("ElXerath.Draw.off", new CheckBox("屏蔽线圈", false));
            miscMenu.Add("ElXerath.Draw.Q", new CheckBox("显示 Q"));
            miscMenu.Add("ElXerath.Draw.W", new CheckBox("显示 W"));
            miscMenu.Add("ElXerath.Draw.E", new CheckBox("显示 E"));
            miscMenu.Add("ElXerath.Draw.R", new CheckBox("显示 R"));
            miscMenu.Add("ElXerath.Draw.Text", new CheckBox("显示 文字"));
            miscMenu.Add("ElXerath.Draw.RON", new CheckBox("显示 R 目标半径"));
            miscMenu.Add("ElXerath.Ignite", new CheckBox("使用 点燃"));
            miscMenu.Add("ElXerath.misc.ks", new CheckBox("抢头模式", false));
            miscMenu.Add("ElXerath.misc.Antigapcloser", new CheckBox("防突进"));
            miscMenu.Add("ElXerath.misc.Notifications", new CheckBox("开启 提醒"));
            miscMenu.Add("ElXerath.Misc.E", new KeyBind("施放 E 按键", false, KeyBind.BindTypes.HoldActive, 'H'));
            miscMenu.Add("ElXerath.hitChance", new ComboBox("Q 命中率", 3, "低", "中", "高", "非常高"));

            Console.WriteLine("Menu Loaded");
        }

        #endregion
    }
}