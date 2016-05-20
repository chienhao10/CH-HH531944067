using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy;

namespace FastTrundle
{
    public class FastTrundleMenu
    {
        #region Data

        public static Menu Menu;

        #endregion

        #region Methods

        public static Menu
            comboMenu,
            harassMenu,
            lastHitMenu,
            laneClearMenu,
            jungleClearMenu,
            itemMenu,
            miscMenu;

        public static void Initialize()
        {
            Menu = MainMenu.AddMenu("FastTrundle", "menu");

            comboMenu = Menu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("FastTrundle.Combo.Q", new CheckBox("Use Q"));
            comboMenu.Add("FastTrundle.Combo.W", new CheckBox("Use W"));
            comboMenu.Add("FastTrundle.Combo.E", new CheckBox("Use E"));
            comboMenu.Add("FastTrundle.Combo.R", new CheckBox("Use R", false));
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                comboMenu.Add("FastTrundle.R.On" + hero.CharData.BaseSkinName, new CheckBox("R : " + hero.CharData.BaseSkinName));
            }
            comboMenu.Add("FastTrundle.Combo.Ignite", new CheckBox("Use Ignite"));

            harassMenu = Menu.AddSubMenu("Harass", "Harass");
            harassMenu.Add("FastTrundle.Harass.Q", new CheckBox("Use Q"));
            harassMenu.Add("FastTrundle.Harass.W", new CheckBox("Use W"));
            harassMenu.Add("FastTrundle.Harass.E", new CheckBox("Use E", false));
            harassMenu.Add("FastTrundle.Harass.Mana", new Slider("Minimum mana", 25));

            lastHitMenu = Menu.AddSubMenu("Last Hit", "Lasthit");
            lastHitMenu.Add("FastTrundle.LastHit.Q", new CheckBox("Use Q"));
            lastHitMenu.Add("FastTrundle.LastHit.Mana", new Slider("Minimum mana", 25));

            laneClearMenu = Menu.AddSubMenu("Lane Clear", "Laneclear");
            laneClearMenu.Add("FastTrundle.LaneClear.Q", new CheckBox("Use Q"));
            laneClearMenu.Add("FastTrundle.LaneClear.Q.Lasthit", new CheckBox("Only lasthit with Q", false));
            laneClearMenu.Add("FastTrundle.LaneClear.W", new CheckBox("Use W"));
            laneClearMenu.Add("FastTrundle.LaneClear.Mana", new Slider("Minimum mana", 25));

            jungleClearMenu = Menu.AddSubMenu("Jungle Clear", "Jungleclear");
            jungleClearMenu.Add("FastTrundle.JungleClear.Q", new CheckBox("Use Q"));
            jungleClearMenu.Add("FastTrundle.JungleClear.W", new CheckBox("Use W"));
            jungleClearMenu.Add("FastTrundle.JungleClear.Mana", new Slider("Minimum mana", 25));


            itemMenu = Menu.AddSubMenu("Items", "Items");
            itemMenu.Add("FastTrundle.Items.Hydra", new CheckBox("Use Tiamat / Ravenous Hydra"));
            itemMenu.Add("FastTrundle.Items.Titanic", new CheckBox("Use Titanic Hydra"));
            itemMenu.Add("FastTrundle.Items.Youmuu", new CheckBox("Use Youmuu's Ghostblade"));
            itemMenu.Add("FastTrundle.Items.Blade", new CheckBox("Use Cutlass / BOTRK"));
            itemMenu.Add("FastTrundle.Items.Blade.MyHP", new Slider("When my HP % <", 50));

            miscMenu = Menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("FastTrundle.Draw.off", new CheckBox("Turn drawings off", false));
            miscMenu.Add("FastTrundle.Draw.Q", new CheckBox("Draw Q", false));
            miscMenu.Add("FastTrundle.Draw.W", new CheckBox("Draw W, false"));
            miscMenu.Add("FastTrundle.Draw.E", new CheckBox("Draw E"));
            miscMenu.Add("FastTrundle.Draw.R", new CheckBox("Draw R", false));
            miscMenu.Add("FastTrundle.Draw.Pillar", new CheckBox("Draw Pillar", false));
            miscMenu.Add("FastTrundle.Antigapcloser", new CheckBox("Antigapcloser", false));
            miscMenu.Add("FastTrundle.Interrupter", new CheckBox("Interrupter"));
        }
        #endregion
    }
}