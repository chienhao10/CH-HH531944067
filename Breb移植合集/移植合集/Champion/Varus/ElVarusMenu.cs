using System;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Elvarus
{
    public class ElVarusMenu
    {
        #region Static Fields

        public static Menu Menu, cMenu, hMenu, itemMenu, lMenu, miscMenu;

        #endregion

        #region Public Methods and Operators

        public static void Initialize()
        {
            Menu = MainMenu.AddMenu("ElVarus", "menu");

            cMenu = Menu.AddSubMenu("Combo", "Combo");
            cMenu.Add("ElVarus.Combo.Q", new CheckBox("Use Q"));
            cMenu.Add("ElVarus.combo.always.Q", new CheckBox("always Q", false));
            cMenu.Add("ElVarus.Combo.E", new CheckBox("Use E"));
            cMenu.Add("ElVarus.Combo.R", new CheckBox("Use R"));
            cMenu.Add("ElVarus.Combo.R.Count", new Slider("R when enemies >= ", 1, 1, 5));
            cMenu.Add("ElVarus.Combo.Stack.Count", new Slider("Q when stacks >= ", 3, 1, 3));
            cMenu.Add("ElVarus.SemiR", new KeyBind("Semi-manual cast R key", false, KeyBind.BindTypes.HoldActive, 'T'));

            hMenu = Menu.AddSubMenu("Harass", "Harass");
            hMenu.Add("ElVarus.Harass.Q", new CheckBox("Use Q"));
            hMenu.Add("ElVarus.Harass.E", new CheckBox("Use E"));
            hMenu.Add("minmanaharass", new Slider("Mana needed to clear ", 55));

            itemMenu = Menu.AddSubMenu("Items", "Items");
            itemMenu.Add("ElVarus.Items.Youmuu", new CheckBox("Use Youmuu's Ghostblade"));
            itemMenu.Add("ElVarus.Items.Cutlass", new CheckBox("Use Cutlass"));
            itemMenu.Add("ElVarus.Items.Blade", new CheckBox("Use Blade of the Ruined King"));
            itemMenu.Add("ElVarus.Items.Blade.EnemyEHP", new Slider("Enemy HP Percentage", 80));
            itemMenu.Add("ElVarus.Items.Blade.EnemyMHP", new Slider("My HP Percentage", 80));

            lMenu = Menu.AddSubMenu("Clear", "Clear");
            lMenu.Add("useQFarm", new CheckBox("Use Q"));
            lMenu.Add("ElVarus.Count.Minions", new Slider("Killable minions with Q >=", 2, 1, 5));
            lMenu.Add("useEFarm", new CheckBox("Use E"));
            lMenu.Add("ElVarus.Count.Minions.E", new Slider("Killable minions with E >=", 2, 1, 5));
            lMenu.AddSeparator();
            lMenu.Add("useQFarmJungle", new CheckBox("Use Q in jungle"));
            lMenu.Add("useEFarmJungle", new CheckBox("Use E in jungle"));
            lMenu.Add("minmanaclear", new Slider("Mana needed to clear ", 55));

            miscMenu = Menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("ElVarus.Draw.off", new CheckBox("Turn drawings off"));
            miscMenu.Add("ElVarus.Draw.Q", new CheckBox("Draw Q"));
            miscMenu.Add("ElVarus.Draw.W", new CheckBox("Draw W"));
            miscMenu.Add("ElVarus.Draw.E", new CheckBox("Draw E"));
            miscMenu.Add("ElVarus.KSSS", new CheckBox("Killsteal"));

            Console.WriteLine("Menu Loaded");
        }

        #endregion
    }
}