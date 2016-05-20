using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace ElTristana
{
    public class MenuInit
    {
        public static Menu Menu_, comboMenu, suicideMenu, harassMenu, laneClearMenu, jungleClearMenu, miscMenu, killstealMenu;

        public static void Initialize()
        {
            Menu_ = MainMenu.AddMenu("ElTristana", "menu");

            comboMenu = Menu_.AddSubMenu("Combo", "Combo");
            comboMenu.Add("ElTristana.Combo.Q", new CheckBox("Use Q"));
            comboMenu.Add("ElTristana.Combo.E", new CheckBox("Use E"));
            comboMenu.Add("ElTristana.Combo.R", new CheckBox("Use R"));
            comboMenu.Add("ElTristana.Combo.OnlyQ", new CheckBox("Only Q if target has E"));
            comboMenu.Add("ElTristana.Combo.Focus.E", new CheckBox("Focus E target"));
            comboMenu.Add("ElTristana.Combo.Always.RE", new CheckBox("Use E + R finisher"));
            comboMenu.Add("ElTristana.Combo.E.Mana", new Slider("Minimum mana for E", 25));
            comboMenu.AddSeparator();
            comboMenu.Add("ElTristana.Combo.E.KeyBind", new KeyBind("Semi-Manual E", false, KeyBind.BindTypes.HoldActive, 'E'));
            comboMenu.AddSeparator();
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
                comboMenu.Add("ElTristana.E.On" + hero.NetworkId, new CheckBox("E? : " + hero.ChampionName));

            suicideMenu = Menu_.AddSubMenu("W settings", "Suicide menu");
            suicideMenu.Add("ElTristana.W", new CheckBox("Use this special feature", false));
            suicideMenu.Add("ElTristana.W.Jump.kill", new CheckBox("Only jump when killable", false));
            suicideMenu.Add("ElTristana.W.Jump.tower", new CheckBox("Check under tower"));
            suicideMenu.Add("ElTristana.W.Jump", new CheckBox("W to enemy with 4 stacks"));
            suicideMenu.Add("ElTristana.W.Enemies", new Slider("Only jump when enemies in range", 1, 1, 5));
            suicideMenu.Add("ElTristana.W.Enemies.Range", new Slider("Enemies in range distance check", 1500, 800, 2000));

            harassMenu = Menu_.AddSubMenu("Harass", "Harass");
            harassMenu.Add("ElTristana.Harass.Q", new CheckBox("Use Q"));
            harassMenu.Add("ElTristana.Harass.E", new CheckBox("Use E"));
            harassMenu.Add("ElTristana.Harass.QE", new CheckBox("Use Q only with E"));
            harassMenu.Add("ElTristana.Harass.E.Mana", new Slider("Minimum mana for E", 25));
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
                harassMenu.Add("ElTristana.E.On.Harass" + hero.NetworkId, new CheckBox("E? : " + hero.CharData.BaseSkinName));

            laneClearMenu = Menu_.AddSubMenu("Laneclear", "Laneclear");
            laneClearMenu.Add("ElTristana.LaneClear.Q", new CheckBox("Use Q"));
            laneClearMenu.Add("ElTristana.LaneClear.E", new CheckBox("Use E"));
            laneClearMenu.Add("ElTristana.LaneClear.Tower", new CheckBox("Use E on tower"));
            laneClearMenu.Add("ElTristana.LaneClear.E.Mana", new Slider("Minimum mana for E", 25));

            jungleClearMenu = Menu_.AddSubMenu("Jungleclear", "Jungleclear");
            jungleClearMenu.Add("ElTristana.JungleClear.Q", new CheckBox("Use Q"));
            jungleClearMenu.Add("ElTristana.JungleClear.E", new CheckBox("Use E"));
            jungleClearMenu.Add("ElTristana.JungleClear.E.Mana", new Slider("Minimum mana for E", 25));

            killstealMenu = Menu_.AddSubMenu("Killsteal", "Killsteal");
            killstealMenu.Add("ElTristana.killsteal.Active", new CheckBox("Activate killsteal"));
            killstealMenu.Add("ElTristana.Killsteal.R", new CheckBox("Use R"));

            miscMenu = Menu_.AddSubMenu("Misc", "Misc");
            miscMenu.Add("ElTristana.Draw.off", new CheckBox("Turn drawings off"));
            miscMenu.Add("ElTristana.DrawStacks", new CheckBox("Draw E stacks"));
            miscMenu.Add("ElTristana.Draw.Q", new CheckBox("Draw Q"));
            miscMenu.Add("ElTristana.Draw.E", new CheckBox("Draw E"));
            miscMenu.Add("ElTristana.Draw.R", new CheckBox("Draw R"));
            miscMenu.Add("ElTristana.Antigapcloser", new CheckBox("Antigapcloser", false));
            miscMenu.Add("ElTristana.Interrupter", new CheckBox("Interrupter", false));

            Console.WriteLine("Menu Loaded");
        }
    }
}