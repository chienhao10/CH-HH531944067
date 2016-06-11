using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Marksman.Champions;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Marksman.Common
{
    internal static class CommonSettings
    {
        public static Menu MenuLocal { get; private set; }
        private static Menu MenuCastSettings { get; set; }
        private static Menu MenuHitchanceSettings { get; set; }
        public static void Init(Menu nParentMenu)
        {
            MenuCastSettings = nParentMenu.AddSubMenu("Spell Cast:", "MenuSettings.CastDelay");
            {
                string[] strQ = new string[1000/250];
                for (float i = 250; i <= 1000; i += 250)
                {
                    strQ[(int) (i/250 - 1)] = (i/1000) + " sec. ";
                }
                MenuCastSettings.Add("Settings.SpellCast.VisibleDelay", new ComboBox("Cast Delay: Instatly Visible Enemy", 2, strQ));
                MenuCastSettings.Add("Settings.SpellCast.Default", new CheckBox("Load Recommended Settings"))
                    .OnValueChange += (sender, args) =>
                    {
                        if (args.NewValue)
                        {
                            LoadDefaultCastDelaySettings();
                        }
                    };
            }

            MenuHitchanceSettings = nParentMenu.AddSubMenu("Hitchance:", "MenuSettings.Hitchance");
            {
                string[] nHitchanceList = new[] { "Medium", "High", "VeryHigh" }; 

                MenuHitchanceSettings.Add("MenuSettings.Hitchance.Q", new ComboBox("Q Hitchance:", 1, nHitchanceList));
                MenuHitchanceSettings.Add("MenuSettings.Hitchance.W", new ComboBox("W Hitchance:", 1, nHitchanceList));
                MenuHitchanceSettings.Add("MenuSettings.Hitchance.E", new ComboBox("E Hitchance:", 1, nHitchanceList));
                MenuHitchanceSettings.Add("MenuSettings.Hitchance.R", new ComboBox("R Hitchance:", 1, nHitchanceList));
            }
        }

        static void LoadDefaultCastDelaySettings()
        {
            string[] strQ = new string[1000 / 250];
            //for (var i = 250; i <= 1000; i += 250)
            //{
            //    str[i / 250 - 1] = i + " ms. ";
            //}
            for (float i = 250; i <= 1000; i += 250)
            {
                strQ[(int)(i / 250 - 1)] = (i / 100) + " sec. ";
            }
            MenuCastSettings["Settings.SpellCast.VisibleDelay"].Cast<ComboBox>().CurrentValue = 2;
        }

        public static HitChance GetHitchance(this Spell nSpell)
        {
            HitChance[] hitChances = new[] { HitChance.Medium, HitChance.High, HitChance.VeryHigh};
            return hitChances[MenuHitchanceSettings["MenuSettings.Hitchance." + nSpell.Slot].Cast<ComboBox>().CurrentValue];
        }
    }
}
