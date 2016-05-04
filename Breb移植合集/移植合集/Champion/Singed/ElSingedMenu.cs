using System;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace ElSinged
{
    public class ElSingedMenu
    {
        public static Menu Menu, cMenu, hMenu, lcMenu, miscMenu;

        public static void Initialize()
        {
            Menu = MainMenu.AddMenu("ElSinged", "menu");

            cMenu = Menu.AddSubMenu("Combo", "Combo");
            cMenu.Add("ElSinged.Combo.Q", new CheckBox("Use Q"));
            cMenu.Add("ElSinged.Combo.W", new CheckBox("Use W"));
            cMenu.Add("ElSinged.Combo.E", new CheckBox("Use E"));
            cMenu.Add("ElSinged.Combo.R", new CheckBox("Use R"));
            cMenu.Add("ElSinged.Combo.R.Count", new Slider("Use R enemies >= ", 2, 1, 5));
            cMenu.Add("ElSinged.Combo.Ignite", new CheckBox("Use Ignite"));

            hMenu = Menu.AddSubMenu("Harass", "Harass");
            hMenu.Add("ElSinged.Harass.Q", new CheckBox("Use Q"));
            hMenu.Add("ElSinged.Harass.W", new CheckBox("Use W"));
            hMenu.Add("ElSinged.Harass.E", new CheckBox("Use E"));

            lcMenu = Menu.AddSubMenu("Laneclear", "Laneclear");
            lcMenu.Add("ElSinged.Laneclear.Q", new CheckBox("Use Q"));
            lcMenu.Add("ElSinged.Laneclear.E", new CheckBox("Use E"));

            miscMenu = Menu.AddSubMenu("Drawings", "Misc");
            miscMenu.Add("ElSinged.Draw.off", new CheckBox("Turn drawings off", false));
            miscMenu.Add("ElSinged.Draw.Q", new CheckBox("Draw Q")); //.SetValue(new Circle()));
            miscMenu.Add("ElSinged.Draw.W", new CheckBox("Draw W")); //.SetValue(new Circle()));
            miscMenu.Add("ElSinged.Draw.E", new CheckBox("Draw E")); //.SetValue(new Circle()));
            miscMenu.Add("DontOffQ", new CheckBox("Do not turn off Q", false));
            miscMenu.Add("ElSinged.Misc.QRange", new Slider("Q Search Range", 1000, 1000, 6000)); //.SetValue(new Circle()));

            Console.WriteLine("Menu Loaded");
        }
    }
}