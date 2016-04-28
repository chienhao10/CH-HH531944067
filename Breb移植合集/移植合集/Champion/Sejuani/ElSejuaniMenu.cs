using System;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace ElSejuani
{
    public class ElSejuaniMenu
    {
        public static Menu Menu, cMenu, hMenu, lMenu, interuptMenu, miscMenu;

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        public static void Initialize()
        {
            Menu = MainMenu.AddMenu("ElSejuani", "sejjjj");

            cMenu = Menu.AddSubMenu("Combo", "Combo");
            cMenu.Add("ElSejuani.Combo.Q", new CheckBox("Use Q"));
            cMenu.Add("ElSejuani.Combo.W", new CheckBox("Use W"));
            cMenu.Add("ElSejuani.Combo.E", new CheckBox("Use E"));
            cMenu.Add("ElSejuani.Combo.R", new CheckBox("Use R"));
            cMenu.Add("ElSejuani.Combo.Ignite", new CheckBox("Use Ignite"));
            cMenu.Add("ElSejuani.Combo.R.Count", new Slider("Minimum targets for R >=", 2, 1, 5));
            cMenu.Add("ElSejuani.Combo.E.Count", new Slider("Minimum targets for E >=", 1, 1, 5));
            cMenu.Add("ElSejuani.Combo.Semi.R", new KeyBind("Semi-manual R", false, KeyBind.BindTypes.HoldActive, 'T'));

            hMenu = Menu.AddSubMenu("Harass", "Harass");
            hMenu.Add("ElSejuani.Harass.Q", new CheckBox("Use Q"));
            hMenu.Add("ElSejuani.Harass.W", new CheckBox("Use Q"));
            hMenu.Add("ElSejuani.Harass.E", new CheckBox("Use E"));
            hMenu.Add("ElSejuani.harass.mana", new Slider("Minimum mana for harass >=", 55, 1));

            lMenu = Menu.AddSubMenu("Clear", "Clear");
            lMenu.Add("ElSejuani.Clear.Q", new CheckBox("Use Q"));
            lMenu.Add("ElSejuani.Clear.W", new CheckBox("Use W"));
            lMenu.Add("ElSejuani.Clear.E", new CheckBox("Use E"));
            lMenu.Add("ElSejuani.Clear.Q.Count", new Slider("Minimum targets for Q >=", 1, 1, 5));
            lMenu.Add("minmanaclear", new Slider("Minimum mana to clear >=", 55, 1));

            interuptMenu = Menu.AddSubMenu("Interupt settings", "interuptsettings");
            interuptMenu.Add("ElSejuani.Interupt.Q", new CheckBox("Use Q"));
            interuptMenu.Add("ElSejuani.Interupt.R", new CheckBox("Use R", false));

            miscMenu = Menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("ElSejuani.Draw.off", new CheckBox("Turn drawings off", false));
            miscMenu.Add("ElSejuani.Draw.Q", new CheckBox("Draw Q"));
            miscMenu.Add("ElSejuani.Draw.W", new CheckBox("Draw W"));
            miscMenu.Add("ElSejuani.Draw.E", new CheckBox("Draw E"));
            miscMenu.Add("ElSejuani.Draw.R", new CheckBox("Draw R"));
            miscMenu.Add("ElSejuani.hitChance", new ComboBox("Hitchance Q", 3, "Low", "Medium", "High", "Very High"));

            Console.WriteLine("Menu Loaded");
        }
    }
}