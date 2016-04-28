using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace ElZilean
{
    public class ZileanMenu
    {
        #region Static Fields

        public static Menu Menu;

        #endregion

        #region Public Methods and Operators

        public static Menu comboMenu, harassMenu, clearMenu, castUltMenu, miscMenu;

        public static void Initialize()
        {
            Menu = MainMenu.AddMenu("ElZilean", "menu");

            comboMenu = Menu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("ElZilean.Combo.Q", new CheckBox("Use Q"));
            comboMenu.Add("ElZilean.Combo.E", new CheckBox("Use E"));
            comboMenu.Add("ElZilean.Combo.W", new CheckBox("Use W to reset Q when target is marked"));
            comboMenu.Add("ElZilean.Combo.Ignite", new CheckBox("Use Ignite"));

            harassMenu = Menu.AddSubMenu("Harass", "Harass");
            harassMenu.Add("ElZilean.Harass.Q", new CheckBox("Use Q"));
            harassMenu.Add("ElZilean.Harass.E", new CheckBox("Use E"));
            harassMenu.Add("ElZilean.hitChance", new ComboBox("Hitchance", 3, "Low", "Medium", "High", "Very High"));
            harassMenu.AddSeparator();
            harassMenu.AddGroupLabel("Auto Harass");
            harassMenu.Add("ElZilean.AutoHarass",
                new KeyBind("[Toggle] Auto harass", false, KeyBind.BindTypes.PressToggle, 'U'));
            harassMenu.Add("ElZilean.UseQAutoHarass", new CheckBox("Use Q"));
            harassMenu.Add("ElZilean.UseEAutoHarass", new CheckBox("Use E", false));
            harassMenu.Add("ElZilean.harass.mana", new Slider("Min % mana for autoharass", 55, 1));

            clearMenu = Menu.AddSubMenu("Laneclear", "LC");
            clearMenu.Add("ElZilean.Clear.Q", new CheckBox("Use Q"));
            clearMenu.Add("ElZilean.Clear.W", new CheckBox("Use W to reset bomb"));

            castUltMenu = Menu.AddSubMenu("Ult settings", "ElZilean.Ally.Ult");
            castUltMenu.Add("ElZilean.useult", new CheckBox("Use ult on ally"));
            castUltMenu.Add("ElZilean.Ally.HP", new Slider("Ally Health %", 25, 1));
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsAlly && !hero.IsMe))
            {
                castUltMenu.Add("ElZilean.Cast.Ult.Ally" + hero.CharData.BaseSkinName,
                    new CheckBox("Ult : " + hero.CharData.BaseSkinName));
            }
            castUltMenu.Add("ElZilean.R", new CheckBox("Cast R"));
            castUltMenu.Add("ElZilean.HP", new Slider("Self Health %", 25, 1));

            miscMenu = Menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("AA.Block", new CheckBox("Don't use AA before attack"));
            miscMenu.Add("ElZilean.Draw.off", new CheckBox("[Drawing] Drawings off", false));
            miscMenu.Add("ElZilean.Draw.Q", new CheckBox("Draw Q"));
            miscMenu.Add("ElZilean.Draw.W", new CheckBox("Draw W"));
            miscMenu.Add("ElZilean.Draw.E", new CheckBox("Draw E"));
            miscMenu.Add("ElZilean.Draw.R", new CheckBox("Draw R"));
            miscMenu.Add("ElZilean.SupportMode", new CheckBox("Support mode", false));
            miscMenu.Add("FleeActive", new KeyBind("Flee", false, KeyBind.BindTypes.HoldActive, 'A'));

            Console.WriteLine("Menu Loaded");
        }

        #endregion
    }
}