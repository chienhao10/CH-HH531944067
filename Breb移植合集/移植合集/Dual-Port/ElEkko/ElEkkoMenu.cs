using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
namespace ElEkko
{
    public class ElEkkoMenu
    {
        public static Menu Config, Combo, Harass, Clear, JungleClear, KillSteal, Misc;

        public static void Initialize()
        {
            Config = MainMenu.AddMenu("ElEkko", "ElEkko");

            Combo = Config.AddSubMenu("Combo", "Combo");
            Combo.Add("ElEkko.Combo.Q", new CheckBox("Use Q"));
            Combo.Add("ElEkko.Combo.Auto.Q", new CheckBox("Use Q when 2 stacks"));
            Combo.Add("ElEkko.Combo.W", new CheckBox("Use W"));
            Combo.Add("ElEkko.Combo.W.Stuned", new CheckBox("Use W on stunned targets"));
            Combo.Add("ElEkko.Combo.W.Count", new Slider("Minimum targets for W >=",3,1,5));
            Combo.Add("ElEkko.Combo.E", new CheckBox("Use E"));
            Combo.Add("ElEkko.Combo.R", new CheckBox("Use R"));
            Combo.Add("ElEkko.Combo.R.Kill", new CheckBox("Use R when target can be killed"));
            Combo.Add("ElEkko.Combo.R.HP", new Slider("Use R when HP >=",25,0,100));
            Combo.Add("ElEkko.Combo.R.Enemies", new Slider("Use R on enemies >=",3,1,5));
            Combo.Add("ElEkko.Combo.Ignite", new CheckBox("Use ignite"));

            Harass = Config.AddSubMenu("Harass", "Harass");
            Harass.Add("ElEkko.Harass.Q", new CheckBox("Use Q"));
            Harass.Add("ElEkko.Harass.E", new CheckBox("Use E"));
            Harass.Add("ElEkko.Harass.Q.Mana", new Slider("Minimum mana",55,0,100));
            Harass.Add("ElEkko.AutoHarass.Q", new CheckBox("Auto harass",false));

            Clear = Config.AddSubMenu("Clear", "Laneclear");
            Clear.Add("ElEkko.LaneClear.Q", new CheckBox("Use Q"));
            Clear.Add("ElEkko.LaneClear.Minions", new Slider("Use Q when minions >=",3,1,5));
            Clear.Add("ElEkko.LaneClear.mana", new Slider("Minimum mana",55,0,100));

            JungleClear = Config.AddSubMenu("JungleClear","JungleClear");
            JungleClear.Add("ElEkko.JungleClear.Q",new CheckBox("Use Q"));
            JungleClear.Add("ElEkko.JungleClear.W", new CheckBox("Use W"));
            JungleClear.Add("ElEkko.JungleClear.Minions", new Slider("Use Q when minions >=",1,1,5));
            JungleClear.Add("ElEkko.JungleClear.mana", new Slider("Minimum mana", 55, 0, 100));

            KillSteal = Config.AddSubMenu("Killsteal", "Killsteal");
            KillSteal.Add("ElEkko.Killsteal.Active", new CheckBox("Use Killsteal"));
            KillSteal.Add("ElEkko.Killsteal.Ignite", new CheckBox("Use ignite"));
            KillSteal.Add("ElEkko.Killsteal.Q", new CheckBox("Use Q"));
            KillSteal.Add("ElEkko.Killsteal.R", new CheckBox("Use R"));

            Misc = Config.AddSubMenu("Misc", "Misc && Draws");
            Misc.Add("ElEkko.R.text", new CheckBox("Display how many people in R"));
            Misc.Add("ElEkko.Draw.off", new CheckBox("Turn drawings off",false));
            Misc.Add("ElEkko.Draw.Q", new CheckBox("Draw Q"));
            Misc.Add("ElEkko.Draw.W", new CheckBox("Draw E"));
            Misc.Add("ElEkko.Draw.R", new CheckBox("Draw R"));
            Misc.Add("ElEkko.DrawComboDamage", new CheckBox("Draw combo damage"));

            DrawDamage.DamageToUnit = ElEkko.GetComboDamage;

        }
    }
}
