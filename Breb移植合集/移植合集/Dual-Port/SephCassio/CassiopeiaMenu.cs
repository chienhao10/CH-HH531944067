using System.Collections.Generic;
using EloBuddy;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System.Linq;

namespace SephCassiopeia
{
    class CassiopeiaMenu
    {
        public static Menu Config;
        public static Menu Combo, KillSteal, Harass, Farm, Waveclear, Interrupter, hc, misc, Drawings;
        public static Menu CreateMenu()
        {

            Config = MainMenu.AddMenu("SephCassio", "Cassiopeia");

            Combo = Config.AddSubMenu("Combo", "Combo");
            Combo.Add("Disableautoifspellsready", new CheckBox("Disable autos only when spells up"));
            Combo.Add("Combo.Useauto", new CheckBox("Use auto attacks"));
            Combo.Add("Combo.UseQ", new CheckBox("Use Q"));
            Combo.Add("Combo.UseW", new CheckBox("Use W"));
            Combo.Add("Combo.UseE", new CheckBox("Use E"));
            Combo.Add("Combo.useepoison", new CheckBox("Use E only if poison"));
            Combo.Add("Combo.edelay", new Slider("Edelay", 0, 0, 1000));
            Combo.Add("Combo.UseR", new CheckBox("Use R"));
            Combo.Add("Combo.Rcount", new Slider("Enemies count for Ult", 1, 0, 5));
            Combo.Add("Combo.UseRNF", new CheckBox("Use R even if targets not facing", false));
            Combo.Add("Combo.Rcountnf", new Slider("Enemies count if not facing", 3, 0, 5));


            KillSteal = Config.AddSubMenu("Killsteal", "Killsteal");
            KillSteal.Add("Killsteal", new CheckBox("KillSteal", true));
            KillSteal.Add("Killsteal.UseQ", new CheckBox("Use Q", true));
            KillSteal.Add("Killsteal.UseW", new CheckBox("Use W", true));
            KillSteal.Add("Killsteal.UseE", new CheckBox("Use E", true));
            KillSteal.Add("Killsteal.UseR", new CheckBox("Use R", false));
            KillSteal.Add("Killsteal.UseIgnite", new CheckBox("Use Ignite", true));

            Harass = Config.AddSubMenu("Harass", "Harass");
            Harass.Add("Keys.HarassT", new KeyBind("Harass Toggle", false, KeyBind.BindTypes.PressToggle, 'H'));
            Harass.Add("Harass.InMixed", new CheckBox("Harass in Mixed Mode", true));
            Harass.Add("Harass.UseQ", new CheckBox("Use Q", true));
            Harass.Add("Harass.UseW", new CheckBox("Use W", true));
            Harass.Add("Harass.UseE", new CheckBox("Use E", true));
            Harass.Add("Harass.Mana", new Slider("Min mana for harass (%)", 50, 0, 100));

            Farm = Config.AddSubMenu("Farm (LH)", "Farm");
            Farm.Add("Farm.Enable", new CheckBox("Enable abilities for farming", true));
            Farm.Add("Farm.Mana", new Slider("Min mana (%)", 50, 0, 100));
            Farm.Add("Farm.Useauto", new CheckBox("Enable auto attacks", true));
            Farm.Add("Farm.UseQ", new CheckBox("Use Q", true));
            Farm.Add("Farm.UseW", new CheckBox("Use W", true));
            Farm.Add("Farm.UseE", new CheckBox("Use E", true));
            Farm.Add("Farm.useepoison", new CheckBox("Use E only if poisoned", true));


            Waveclear = Config.AddSubMenu("Waveclear", "Waveclear");
            Waveclear.Add("Waveclear.Useauto", new CheckBox("Enable autos", true));
            Waveclear.Add("Waveclear.UseQ", new CheckBox("Use Q", true));
            Waveclear.Add("Waveclear.UseW", new CheckBox("Use W", true));
            Waveclear.Add("Waveclear.UseE", new CheckBox("Use E", true));
            Waveclear.Add("Waveclear.useepoison", new CheckBox("Use E only if poisoned", true));
            Waveclear.Add("Waveclear.useekillable", new CheckBox("Use E only if killable", true));
            Waveclear.Add("Waveclear.UseR", new CheckBox("Use R", false));
            Waveclear.Add("Waveclear.Rcount", new Slider("Minions for R", 10, 0, 20));


            Interrupter = Config.AddSubMenu("Interrupter", "Interrupter +");
            Interrupter.Add("Interrupter.UseR", new CheckBox("Interrupt with R", true));
            Interrupter.Add("Interrupter.AntiGapClose", new CheckBox("AntiGapClosers", true));
            Interrupter.Add("Interrupter.AG.UseR", new CheckBox("AntiGapClose with R", true));

            hc = Config.AddSubMenu("Hitchance Settings", "hc");
            hc.Add("Hitchance.Q", new ComboBox("Q Hit Chance", 2, HitChance.Low.ToString(), HitChance.Medium.ToString(), HitChance.High.ToString(), HitChance.VeryHigh.ToString(), HitChance.Immobile.ToString()));
            hc.Add("Hitchance.W", new ComboBox("W Hit Chance", 2, HitChance.Low.ToString(), HitChance.Medium.ToString(), HitChance.High.ToString(), HitChance.VeryHigh.ToString(), HitChance.Immobile.ToString()));
            hc.Add("Hitchance.R", new ComboBox("R Hit Chance", 3, HitChance.Low.ToString(), HitChance.Medium.ToString(), HitChance.High.ToString(), HitChance.VeryHigh.ToString(), HitChance.Immobile.ToString()));


            misc = Config.AddSubMenu("Misc", "Misc");
            misc.Add("Misc.autoe", new CheckBox("Auto use e when possible", false));

            Drawings = Config.AddSubMenu("Drawings", "Drawing");
            Drawings.Add("Drawing.Disable", new CheckBox("Disable all", false));
            Drawings.Add("Drawing.DrawQ", new CheckBox("Draw Q", true));
            Drawings.Add("Drawing.DrawW", new CheckBox("Draw W", true));
            Drawings.Add("Drawing.DrawE", new CheckBox("Draw E", true));
            Drawings.Add("Drawing.DrawR", new CheckBox("Draw R", true));


            return Config;
        }
    }
}
