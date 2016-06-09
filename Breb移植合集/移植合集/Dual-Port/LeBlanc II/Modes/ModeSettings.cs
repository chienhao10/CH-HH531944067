using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Leblanc.Champion;
using Leblanc.Common;
using Color = SharpDX.Color;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Leblanc.Modes
{
    internal class ModeSettings
    {
        public static Menu MenuLocal { get; private set; }
        public static Menu MenuSettingE { get; private set; }
        public static Menu MenuFlame { get; private set; }
        public static Menu MenuSpellR { get; private set; }

        public static int MaxERange => MenuSettingE["Settings.E.MaxRange"].Cast<Slider>().CurrentValue;
        public static int EHitchance => MenuSettingE["Settings.E.Hitchance"].Cast<ComboBox>().CurrentValue;
        public static void Init(Menu MenuParent)
        {
            MenuLocal = MenuParent.AddSubMenu("Humanizer Spell Cast", "SettingsSpellCast");
            {
                string[] strQ = new string[1000 / 250];
                for (float i = 250; i <= 1000; i += 250)
                {
                    strQ[(int) (i / 250 - 1)] = (i / 1000) + " sec. ";
                }
                MenuLocal.Add("Settings.SpellCast.VisibleDelay", new ComboBox("Cast Delay: Instatly Visible Enemy", 2, strQ));
                MenuLocal.Add("Settings.SpellCast.Default", new CheckBox("Load Recommended Settings"))
                    .OnValueChange += (sender, args) =>
                                {
                                    if (args.NewValue)
                                    {
                                        LoadDefaultSettingsQ();
                                    }
                                };
            }

            
            MenuSettingE = MenuParent.AddSubMenu("E Settings:", "MenuSettings.E");
            int eRange = (int)PlayerSpells.E.Range;
            MenuSettingE.Add("Settings.E.MaxRange", new Slider("E: Max. Rage [Default: 800]", eRange - 20, eRange / 2, eRange + 50));
            MenuSettingE.Add("Settings.E.Hitchance", new ComboBox("E:", 2, "Hitchance = Very High", "Hitchance >= High", "Hitchance >= Medium", "Hitchance >= Low"));

            MenuFlame = MenuParent.AddSubMenu("Flame", "Flame");
            MenuFlame.Add("Flame.Laugh", new ComboBox("After Kill:", 5, "Off", "Joke", "Taunt", "Laugh", "Mastery Badge", "Random"));
        }

        static void LoadDefaultSettingsQ()
        {
            string[] strQ = new string[1000 / 250];
            for (float i = 250; i <= 1000; i += 250)
            {
                strQ[(int)(i / 250 - 1)] = (i / 100) + " sec. ";
            }
            MenuLocal["Settings.SpellCast.VisibleDelay"].Cast<ComboBox>().CurrentValue = 2;
        }
    }
}
