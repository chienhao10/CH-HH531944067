using System;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace ElVladimirReborn
{
    public class ElVladimirMenu
    {
        #region Static Fields

        public static Menu Menu, comboMenu, harassMenu, clearMenu, settingsMenu, miscMenu;

        #endregion

        #region Public Methods and Operators

        public static void Initialize()
        {
            Menu = MainMenu.AddMenu("ElVladimir:Reborn", "menu");

            comboMenu = Menu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("ElVladimir.Combo.Q", new CheckBox("Use Q"));
            comboMenu.Add("ElVladimir.Combo.W", new CheckBox("Use W", false));
            comboMenu.Add("ElVladimir.Combo.E", new CheckBox("Use E"));
            comboMenu.Add("ElVladimir.Combo.R", new CheckBox("Use R"));
            comboMenu.Add("ElVladimir.Combo.SmartUlt", new CheckBox("Use Smartult"));
            comboMenu.Add("ElVladimir.Combo.Count.R", new Slider("R when >= target", 2, 2, 5));
            comboMenu.Add("ElVladimir.Combo.R.Killable", new CheckBox("Use R only when killable"));
            comboMenu.Add("ElVladimir.Combo.Ignite", new CheckBox("Use ignite"));

            harassMenu = Menu.AddSubMenu("Harass", "Harass");
            harassMenu.Add("ElVladimir.Harass.Q", new CheckBox("Use Q"));
            harassMenu.Add("ElVladimir.Harass.E", new CheckBox("Use E"));

            clearMenu = Menu.AddSubMenu("Waveclear", "Waveclear");
            clearMenu.Add("ElVladimir.WaveClear.Q", new CheckBox("Use Q"));
            clearMenu.Add("ElVladimir.WaveClear.E", new CheckBox("Use E"));
            clearMenu.Add("ElVladimir.JungleClear.Q", new CheckBox("Use Q in jungle"));
            clearMenu.Add("ElVladimir.JungleClear.E", new CheckBox("Use E in jungle"));
            clearMenu.Add("ElVladimir.WaveClear.Health.E", new Slider("Minimum health for E", 20));

            miscMenu = Menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("ElVladimir.Draw.off", new CheckBox("Turn drawings off", false));
            miscMenu.Add("ElVladimir.Draw.Q", new CheckBox("Draw Q"));
            miscMenu.Add("ElVladimir.Draw.W", new CheckBox("Draw W"));
            miscMenu.Add("ElVladimir.Draw.E", new CheckBox("Draw E"));
            miscMenu.Add("ElVladimir.Draw.R", new CheckBox("Draw R"));
            miscMenu.Add("ElVladimir.Draw.Text", new CheckBox("Draw Text"));

            Console.WriteLine("Menu Loaded");
        }

        #endregion
    }
}