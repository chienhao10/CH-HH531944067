using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using OlafxQx.Common;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace OlafxQx.Modes
{
    internal class ModeSettings
    {
        public static Menu MenuLocal { get; private set; }
        public static Menu MenuSkins { get; private set; }
        public static Menu MenuSpellE { get; private set; }
        public static Menu MenuFlame { get; private set; }
        public static Menu MenuSpellR { get; private set; }

        public static int QHitchance => MenuLocal["Settings.Q.Hitchance"].Cast<ComboBox>().CurrentValue;
        public static void Init(Menu MenuParent)
        {
            MenuLocal = MenuParent.AddSubMenu("Settings", "Settings");
            string[] strE = new string[1000/250];
            for (var i = 250; i <= 1000; i += 250) { strE[i/250 - 1] = i + " ms."; }
            MenuLocal.Add("Settings.SpellCast.VisibleDelay", new ComboBox("Cast delay for Instanly Visible Enemy:", 0, strE));
            MenuLocal.Add("Settings.Q.Hitchance", new ComboBox("Q Hitchance:", 2, "Hitchance = Very High", "Hitchance >= High", "Hitchance >= Medium", "Hitchance >= Low"));
            MenuLocal.Add("Settings.E.Auto", new ComboBox("E: Auto-Use (If Enemy Hit)", 1, "Off", "On"));

            MenuFlame = MenuParent.AddSubMenu("Flame", "Flame");
            MenuFlame.Add("Flame.Laugh", new ComboBox("After Kill:", 4, "Off", "Joke", "Taunt", "Laugh", "Random"));
            MenuFlame.Add("Flame.Ctrl6", new ComboBox("After Kill: Show Champion Point Icon (Ctrl + 6)", 0, "Off", "On"));
        }
    }
}
