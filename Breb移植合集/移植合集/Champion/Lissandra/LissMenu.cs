using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

namespace SephLissandra
{
    internal class LissMenu
    {
        public static Menu Config,
            comboMenu,
            ksMenu,
            harassMenu,
            lastHitMenu,
            clearMenu,
            interruptMenu,
            blackListMenu,
            miscMenu,
            drawMenu;

        public static void CreateMenu()
        {
            Config = MainMenu.AddMenu("SephLissandra", "Liss");

            comboMenu = Config.AddSubMenu("Combo", "Combo");
            comboMenu.Add("Combo.UseQ", new CheckBox("Use Q"));
            comboMenu.Add("Combo.UseW", new CheckBox("Use W"));
            comboMenu.Add("Combo.UseE", new CheckBox("Use E"));
            comboMenu.Add("Combo.UseE2", new CheckBox("Use E2"));
            comboMenu.Add("Combo.UseR", new CheckBox("Use R"));
            comboMenu.Add("Combo.ecountW", new Slider("Enemies count for 2nd E (W)", 2, 0, 5));
            comboMenu.Add("Combo.ecountR", new Slider("Enemies count for 2nd E (R)", 2, 0, 5));
            comboMenu.Add("Combo.Rcount", new Slider("Enemies count for self Ult", 2, 0, 5));

            ksMenu = Config.AddSubMenu("Killsteal", "Killsteal");
            ksMenu.Add("Killsteal", new CheckBox("KillSteal"));
            ksMenu.Add("Killsteal.UseQ", new CheckBox("Use Q"));
            ksMenu.Add("Killsteal.UseW", new CheckBox("Use W"));
            ksMenu.Add("Killsteal.UseE", new CheckBox("Use E"));
            ksMenu.Add("Killsteal.UseE2", new CheckBox("Use E2"));
            ksMenu.Add("Killsteal.UseR", new CheckBox("Use R"));
            ksMenu.Add("Killsteal.UseIgnite", new CheckBox("Use Ignite"));

            harassMenu = Config.AddSubMenu("Harass", "Harass");
            harassMenu.Add("Keys.HarassT", new KeyBind("Harass Toggle", false, KeyBind.BindTypes.PressToggle, 'H'));
            harassMenu.Add("Harass.UseQ", new CheckBox("Use Q"));
            harassMenu.Add("Harass.UseW", new CheckBox("Use W"));
            harassMenu.Add("Harass.UseE", new CheckBox("Use E"));
            harassMenu.Add("Harass.Mana", new Slider("Min mana for harass (%)", 50));

            lastHitMenu = Config.AddSubMenu("Farm (LH)", "Farm");
            lastHitMenu.Add("Farm.Mana", new Slider("Minimum Mana %", 50));
            lastHitMenu.Add("Farm.UseQ", new CheckBox("Use Q"));
            lastHitMenu.Add("Farm.UseW", new CheckBox("Use W"));
            lastHitMenu.Add("Farm.UseE", new CheckBox("Use E"));

            clearMenu = Config.AddSubMenu("Waveclear", "Waveclear");
            clearMenu.Add("Waveclear.UseQ", new CheckBox("Use Q"));
            clearMenu.Add("Waveclear.UseW", new CheckBox("Use W"));
            clearMenu.Add("Waveclear.Wcount", new Slider("Minions for W", 2, 0, 20));
            clearMenu.Add("Waveclear.UseE", new CheckBox("Use E"));
            clearMenu.Add("Waveclear.UseE2", new CheckBox("Use E2"));

            interruptMenu = Config.AddSubMenu("Interrupter", "Interrupter +");
            interruptMenu.Add("Interrupter", new CheckBox("Interrupt Important Spells"));
            interruptMenu.Add("Interrupter.UseW", new CheckBox("Use W"));
            interruptMenu.Add("Interrupter.UseR", new CheckBox("Use R"));
            interruptMenu.Add("Interrupter.AntiGapClose", new CheckBox("AntiGapClosers"));
            interruptMenu.Add("Interrupter.AG.UseW", new CheckBox("AntiGapClose with W"));
            interruptMenu.Add("Interrupter.AG.UseR", new CheckBox("AntiGapClose with R"));

            blackListMenu = Config.AddSubMenu("Ultimate BlackList", "BlackList");
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(h => h.IsEnemy))
            {
                blackListMenu.Add("Blacklist." + hero.ChampionName, new CheckBox(hero.ChampionName, false));
            }

            miscMenu = Config.AddSubMenu("Misc", "Misc");
            miscMenu.Add("Misc.PrioritizeUnderTurret", new CheckBox("Prioritize Targets Under Turret"));
            miscMenu.Add("Misc.DontETurret", new CheckBox("Don't 2nd E Turret Range"));
            miscMenu.Add("Misc.EMouse", new KeyBind("E to Mouse Key", false, KeyBind.BindTypes.HoldActive, 'G'));
            miscMenu.AddSeparator();
            miscMenu.Add("Hitchance.Q",
                new ComboBox("Q Hit Chance", 1, HitChance.Low.ToString(), HitChance.Medium.ToString(),
                    HitChance.High.ToString()));
            miscMenu.Add("Hitchance.E",
                new ComboBox("E Hit Chance", 1, HitChance.Low.ToString(), HitChance.Medium.ToString(),
                    HitChance.High.ToString()));
            miscMenu.Add("Misc.Debug", new CheckBox("Debug", false));

            drawMenu = Config.AddSubMenu("Drawings", "Drawing");
            drawMenu.Add("Drawing.DrawQ", new CheckBox("Draw Q"));
            drawMenu.Add("Drawing.DrawW", new CheckBox("Draw W"));
            drawMenu.Add("Drawing.DrawE", new CheckBox("Draw E"));
            drawMenu.Add("Drawing.DrawR", new CheckBox("Draw R"));
        }
    }
}