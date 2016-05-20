using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using System.Drawing;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace ElRumble
{
    public class ElRumbleMenu
    {
        private static Menu Menu { get; set; }
        public static Menu comboMenu, harassMenu, heatMenu, clearMenu, miscMenu;

        public static void Initialize()
        {

            Menu = MainMenu.AddMenu("ElRumble", "ElRumble");

            comboMenu = Menu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("ElRumble.Combo.Q", new CheckBox("Use Q"));
            comboMenu.Add("ElRumble.Combo.W", new CheckBox("Use W"));
            comboMenu.Add("ElRumble.Combo.E", new CheckBox("Use E"));
            comboMenu.Add("ElRumble.Combo.R", new CheckBox("Use R"));
            comboMenu.Add("ElRumble.Combo.Count.Enemies", new Slider("Enemies in range for R", 2, 1, 5));
            comboMenu.Add("ElRumble.Combo.Ignite", new CheckBox("Use Ignite"));


            harassMenu = Menu.AddSubMenu("Harass", "Harass");
            harassMenu.Add("ElRumble.Harass.Q", new CheckBox("Use Q"));
            harassMenu.Add("ElRumble.Harass.E", new CheckBox("Use E"));


            heatMenu = Menu.AddSubMenu("Heat", "Heat");
            heatMenu.Add("ElRumble.KeepHeat.Activated", new KeyBind("Auto harass", false, KeyBind.BindTypes.PressToggle, 'T'));
            heatMenu.Add("ElRumble.Heat.Q", new CheckBox("Use Q"));
            heatMenu.Add("ElRumble.Heat.W", new CheckBox("Use W"));

            clearMenu = Menu.AddSubMenu("Clear", "Clear");
            clearMenu.Add("ElRumble.LastHit.E", new CheckBox("Lasthit with E"));
            clearMenu.Add("ElRumble.LaneClear.Q", new CheckBox("Use Q LaneClear"));
            clearMenu.Add("ElRumble.LaneClear.E", new CheckBox("Use E LaneClear"));
            clearMenu.Add("ElRumble.JungleClear.Q", new CheckBox("Use Q JungleClear"));
            clearMenu.Add("ElRumble.JungleClear.E", new CheckBox("Use E JungleClear"));

            miscMenu = Menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("ElRumble.Draw.Q", new CheckBox("Draw Q"));
            miscMenu.Add("ElRumble.Draw.E", new CheckBox("Draw E"));
            miscMenu.Add("ElRumble.Draw.R", new CheckBox("Draw R"));
            miscMenu.Add("ElRumble.Misc.R", new KeyBind("Manual R", false, KeyBind.BindTypes.HoldActive, 'L'));
        }
    }
}