using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace BadaoKingdom.BadaoChampion.BadaoGangplank
{
    public static class BadaoGangplankConfig
    {
        public static Menu config;
        public static void BadaoActivate()
        {

            // spells init
            BadaoMainVariables.Q = new Spell(SpellSlot.Q, 625);
            BadaoMainVariables.W = new Spell(SpellSlot.W);
            BadaoMainVariables.E = new Spell(SpellSlot.E,1000);
            BadaoMainVariables.R = new Spell(SpellSlot.R);

            // main menu
            config = MainMenu.AddMenu("BadaoKingdom " + ObjectManager.Player.ChampionName, ObjectManager.Player.ChampionName);

            // Combo
            Menu Combo = config.AddSubMenu("Combo", "Combo");
            BadaoGangplankVariables.ComboE1 = Combo.Add("ComboE1", new CheckBox("Place 1st Barrel")).CurrentValue;


            // Harass
            Menu Harass = config.AddSubMenu("Harass", "Harass");
            BadaoGangplankVariables.HarassQ = Harass.Add("HarassQ", new CheckBox("Q")).CurrentValue;


            // LaneClear
            Menu LaneClear = config.AddSubMenu("LaneClear", "LaneClear");
            BadaoGangplankVariables.LaneQ = LaneClear.Add("LaneQ", new CheckBox("Use Q last hit")).CurrentValue;

            // JungleClear
            Menu JungleClear = config.AddSubMenu("JungleClear", "JungleClear");
            BadaoGangplankVariables.JungleQ = JungleClear.Add("jungleQ", new CheckBox("Use Q last hit")).CurrentValue;

            //Auto
            Menu Auto = config.AddSubMenu("Auto", "Auto");
            BadaoGangplankVariables.AutoWLowHealth = (Auto.Add("AutoWLowHealth", new CheckBox("W when low health"))).CurrentValue;
            BadaoGangplankVariables.AutoWLowHealthValue = Auto.Add("AutoWLowHealthValue", new Slider("% HP to W", 20, 1, 100)).CurrentValue;
            BadaoGangplankVariables.AutoWCC = Auto.Add("AutoWCC", new CheckBox("W anti CC")).CurrentValue;
        }
    }
}
