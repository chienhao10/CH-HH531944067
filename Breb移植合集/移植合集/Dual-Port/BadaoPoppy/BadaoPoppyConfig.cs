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

namespace BadaoKingdom.BadaoChampion.BadaoPoppy
{
    public static class BadaoPoppyConfig
    {
        public static Menu config, Combo, Auto;
        public static void BadaoActivate()
        {
            // spells init
            BadaoMainVariables.Q = new Spell(SpellSlot.Q, 430);
            BadaoMainVariables.Q.SetSkillshot(0.5f, 100, float.MaxValue, false, SkillshotType.SkillshotLine);
            BadaoMainVariables.W = new Spell(SpellSlot.W, 375);
            BadaoMainVariables.E = new Spell(SpellSlot.E, 525);
            BadaoMainVariables.E.SetTargetted(0, float.MaxValue);
            BadaoMainVariables.R = new Spell(SpellSlot.R, 1200);
            BadaoMainVariables.R.SetCharged("PoppyR", "PoppyR", 495, 1200, 1.5f);

            // main menu
            config = MainMenu.AddMenu("BadaoKingdom " + ObjectManager.Player.ChampionName, ObjectManager.Player.ChampionName);

            // Combo
            Combo = config.AddSubMenu("Combo", "Combo");
            BadaoPoppyVariables.ComboQ = Combo.Add("ComboQ", new CheckBox("Q")).CurrentValue;
            BadaoPoppyVariables.ComboW = Combo.Add("ComboW", new CheckBox("W to gapclose")).CurrentValue;
            BadaoPoppyVariables.ComboE = Combo.Add("ComboE", new CheckBox("E to gapclose")).CurrentValue;
            foreach (var hero in HeroManager.Enemies)
            {
                Combo.Add("ComboE" + hero.NetworkId, new CheckBox("E stun " + hero.ChampionName + " (" + hero.Name + ")"));
            }
            BadaoPoppyVariables.ComboRKillable = Combo.Add("ComboRKillable", new CheckBox("R Killable")).CurrentValue;

            // Harass
            Menu Harass = config.AddSubMenu("Harass", "Harass");
            BadaoPoppyVariables.HarassQ = Harass.Add("HarassQ", new CheckBox("Q")).CurrentValue;

            //JungleClear
            Menu Jungle = config.AddSubMenu("Jungle", "Jungle");
            BadaoPoppyVariables.JungleQ = Jungle.Add("JungleQ", new CheckBox("Q")).CurrentValue;
            BadaoPoppyVariables.JungleE = Jungle.Add("JungleE", new CheckBox("E")).CurrentValue;
            BadaoPoppyVariables.JungleMana = Jungle.Add("JungleMana", new Slider("Mana Limit", 40, 0, 100)).CurrentValue;

            // Assassinate
            Menu Assassinate = config.AddSubMenu("Assassinate", "Assassinate");
            BadaoPoppyVariables.AssassinateKey = Assassinate.Add("AssassinateKey", new KeyBind("Active", false, KeyBind.BindTypes.HoldActive, 'T')).CurrentValue;
            Assassinate.AddLabel("Select a target to use this mode");

            // Auto
            Auto = config.AddSubMenu("Auto", "Auto");
            foreach (var hero in HeroManager.Enemies)
            {
                Auto.Add("AutoAntiDash" + hero.NetworkId, new CheckBox("W anti dash " + hero.ChampionName + " (" + hero.Name + ")"));
            }
            BadaoPoppyVariables.AutoEInterrupt = Auto.Add("AutoEInterrupt", new CheckBox("E interrupt", false)).CurrentValue;
            BadaoPoppyVariables.AutoRInterrupt = Auto.Add("AutoRInterrupt", new CheckBox("R interrupt", false)).CurrentValue;
            BadaoPoppyVariables.AutoRKS = Auto.Add("AutoRKS", new CheckBox("R KS")).CurrentValue;
            BadaoPoppyVariables.AutoR3Target = Auto.Add("AutoR3Target", new CheckBox("R hits 3 target")).CurrentValue;
        }
    }
}
