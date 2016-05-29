using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Irelia.Common;
using Color = SharpDX.Color;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Irelia.Modes
{
    internal class ModeSettings
    {
        public static Menu MenuLocal { get; private set; }
        public static Menu MenuSkins { get; private set; }
        public static Menu MenuSettingQ { get; private set; }
        public static Menu MenuSettingE { get; private set; }
        public static Menu MenuFlame { get; private set; }
        public static Menu MenuSpellR { get; private set; }

        public static void Init(Menu MenuParent)
        {
            MenuSettingQ = MenuParent.AddSubMenu("Q:", "SettingsQ");
            {
                string[] strQ = new string[1000 / 250];
                for (var i = 250; i <= 1000; i += 250)
                {
                    strQ[i / 250 - 1] = i + " ms. ";
                }
                MenuSettingQ.Add("Settings.Q.VisibleDelay", new ComboBox("Instatly Visible Enemy Cast Delay:", 2, strQ));
                MenuSettingQ.Add("Settings.Q.CastDelay", new ComboBox("Humanizer Cast Delay [Lane / Combo]", 2, strQ));
            }

            MenuSettingE = MenuParent.AddSubMenu("E:", "SettingsE");
            {
                string[] strE = new string[1000/250];
                for (var i = 250; i <= 1000; i += 250)
                {
                    strE[i/250 - 1] = i + " ms. ";
                }
                MenuSettingE.Add("Settings.E.VisibleDelay", new ComboBox("Instatly Visible Enemy Cast Delay:", 2, strE));
                MenuSettingE.Add("Settings.E.Auto", new ComboBox("Auto-Use (If can stun enemy)", 1, "Off", "On"));
            }

            MenuFlame = MenuParent.AddSubMenu("Flame", "Flame");
            MenuFlame.Add("Flame.Laugh", new ComboBox("After Kill:", 4, "Off", "Joke", "Taunt", "Laugh", "Random"));
            MenuFlame.Add("Flame.Ctrl6", new ComboBox("After Kill: Show Champion Point Icon (Ctrl + 6)", 0, "Off", "On"));
            
            Modes.ModeJump.Init(MenuParent);
        }
    }
}
