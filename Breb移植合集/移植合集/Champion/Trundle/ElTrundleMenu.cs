using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace ElTrundle
{
    public class ElTrundleMenu
    {
        #region Static Fields

        public static Menu
            Menu,
            comboMenu,
            harassMenu,
            laneClearMenu,
            jungleClearMenu,
            miscMenu;

        #endregion

        #region Public Methods and Operators

        public static void Initialize()
        {
            Menu = MainMenu.AddMenu("ElTrundle", "menu");

            comboMenu = Menu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("ElTrundle.Combo.Q", new CheckBox("Use Q"));
            comboMenu.Add("ElTrundle.Combo.W", new CheckBox("Use W"));
            comboMenu.Add("ElTrundle.Combo.E", new CheckBox("Use E"));
            comboMenu.Add("ElTrundle.Combo.R", new CheckBox("Use R"));
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                comboMenu.Add("ElTrundle.R.On" + hero.CharData.BaseSkinName, new CheckBox("R On : " + hero.CharData.BaseSkinName));
            }
            comboMenu.Add("ElTrundle.Combo.Ignite", new CheckBox("Use Ignite"));

            harassMenu = Menu.AddSubMenu("Harass", "Harass");
            harassMenu.Add("ElTrundle.Harass.Q", new CheckBox("Use Q"));
            harassMenu.Add("ElTrundle.Harass.W", new CheckBox("Use W"));
            harassMenu.Add("ElTrundle.Harass.E", new CheckBox("Use E", false));
            harassMenu.Add("ElTrundle.Harass.Mana", new Slider("Minimum mana", 25, 1));

            laneClearMenu = Menu.AddSubMenu("Laneclear", "Laneclear");
            laneClearMenu.Add("ElTrundle.LaneClear.Q", new CheckBox("Use Q"));
            laneClearMenu.Add("ElTrundle.LaneClear.Q.Lasthit", new CheckBox("Only lasthit with Q", false));
            laneClearMenu.Add("ElTrundle.LaneClear.W", new CheckBox("Use W"));
            laneClearMenu.Add("ElTrundle.LaneClear.Mana", new Slider("Minimum mana", 25, 1));

            jungleClearMenu = Menu.AddSubMenu("Jungleclear", "Jungleclear");
            jungleClearMenu.Add("ElTrundle.JungleClear.Q", new CheckBox("Use Q"));
            jungleClearMenu.Add("ElTrundle.JungleClear.W", new CheckBox("Use W"));
            jungleClearMenu.Add("ElTrundle.JungleClear.Mana", new Slider("Minimum mana", 25, 1));

            miscMenu = Menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("ElTrundle.Draw.off", new CheckBox("Turn drawings off"));
            miscMenu.Add("ElTrundle.Draw.Q", new CheckBox("Draw Q"));
            miscMenu.Add("ElTrundle.Draw.W", new CheckBox("Draw W"));
            miscMenu.Add("ElTrundle.Draw.E", new CheckBox("Draw E"));
            miscMenu.Add("ElTrundle.Draw.R", new CheckBox("Draw R"));
            miscMenu.Add("ElTrundle.Antigapcloser", new CheckBox("Antigapcloser", false));
            miscMenu.Add("ElTrundle.Interrupter", new CheckBox("Interrupter", false));

            Console.WriteLine("Menu Loaded");
        }

        #endregion
    }
}