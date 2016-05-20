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
            Menu = MainMenu.AddMenu("ElXerath", "menu");

            cMenu = Menu.AddSubMenu("Combo", "Combo");
            cMenu.Add("ElXerath.Combo.Q", new CheckBox("Use Q"));
            cMenu.Add("ElXerath.Combo.W", new CheckBox("Use W"));
            cMenu.Add("ElXerath.Combo.E", new CheckBox("Use E"));

            rMenu = Menu.AddSubMenu("Ult", "Ult");
            rMenu.Add("ElXerath.R.AutoUseR", new CheckBox("Auto use charges"));
            rMenu.Add("ElXerath.R.Mode", new ComboBox("Mode ", 0, "Normal", "Custom delays", "OnTap", "Custom hitchance", "Near mouse"));
            rMenu.Add("ElXerath.R.OnTap", new KeyBind("Ult on tap", false, KeyBind.BindTypes.HoldActive, 'T'));
            rMenu.Add("ElXerath.R.Block", new CheckBox("Block movement"));
            rMenu.AddGroupLabel("Custom delays");
            for (var i = 1; i <= 5; i++)
            {
                rMenu.Add("Delay" + i, new Slider("Delay" + i, 0, 0, 1500));
            }
            rMenu.Add("ElXerath.R.Radius", new Slider("Target radius", 700, 300, 1500));


            hMenu = Menu.AddSubMenu("Harass", "Harass");
            hMenu.Add("ElXerath.Harass.Q", new CheckBox("Use Q"));
            hMenu.Add("ElXerath.Harass.W", new CheckBox("Use W"));
            hMenu.Add("ElXerath.AutoHarass", new KeyBind("[Toggle] Auto harass", false, KeyBind.BindTypes.PressToggle, 'U'));
            hMenu.Add("ElXerath.UseQAutoHarass", new CheckBox("Use Q"));
            hMenu.Add("ElXerath.UseWAutoHarass", new CheckBox("Use W"));
            hMenu.Add("ElXerath.harass.mana", new Slider("Auto harass mana", 55));

            lMenu = Menu.AddSubMenu("Clear", "LaneClear");
            lMenu.Add("ElXerath.clear.Q", new CheckBox("Use Q"));
            lMenu.Add("ElXerath.clear.W", new CheckBox("Use W"));
            lMenu.Add("ElXerath.jclear.Q", new CheckBox("Jungle Use Q"));
            lMenu.Add("ElXerath.jclear.W", new CheckBox("Jungle Use W"));
            lMenu.Add("ElXerath.jclear.E", new CheckBox("Jungle Use E"));
            lMenu.Add("minmanaclear", new Slider("Auto harass mana", 55));

            //ElXerath.Misc
            miscMenu = Menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("ElXerath.Draw.off", new CheckBox("Turn drawings off", false));
            miscMenu.Add("ElXerath.Draw.Q", new CheckBox("Draw Q"));
            miscMenu.Add("ElXerath.Draw.W", new CheckBox("Draw W"));
            miscMenu.Add("ElXerath.Draw.E", new CheckBox("Draw E"));
            miscMenu.Add("ElXerath.Draw.R", new CheckBox("Draw R"));
            miscMenu.Add("ElXerath.Draw.Text", new CheckBox("Draw Text"));
            miscMenu.Add("ElXerath.Draw.RON", new CheckBox("Draw R target radius"));
            miscMenu.Add("ElXerath.Ignite", new CheckBox("Use ignite"));
            miscMenu.Add("ElXerath.misc.ks", new CheckBox("Killsteal mode", false));
            miscMenu.Add("ElXerath.misc.Antigapcloser", new CheckBox("Antigapcloser"));
            miscMenu.Add("ElXerath.misc.Notifications", new CheckBox("Use notifications"));
            miscMenu.Add("ElXerath.Misc.E", new KeyBind("Cast E key", false, KeyBind.BindTypes.HoldActive, 'H'));
            miscMenu.Add("ElXerath.hitChance", new ComboBox("Hitchance Q", 3, "Low", "Medium", "High", "Very High"));

            Console.WriteLine("Menu Loaded");
        }

        #endregion
    }
}