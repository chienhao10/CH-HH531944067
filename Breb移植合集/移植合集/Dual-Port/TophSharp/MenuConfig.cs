using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace TophSharp
{
    internal class MenuConfig
    {
        private static Menu Config;
        public static Menu comboMenu, harassMenu, ksMenu, clearMenu, lasthitMenu, drawMenu, ahMenu;


        public static void MenuLoaded()
        {
            Config = MainMenu.AddMenu("Toph Sharp", "TophSharp");

            comboMenu = Config.AddSubMenu("Combo", "Combo");
            comboMenu.Add("useq", new CheckBox("Use Q"));
            comboMenu.Add("usew", new CheckBox("Use W"));
            comboMenu.Add("usee", new CheckBox("Use E"));
            comboMenu.Add("useignite", new CheckBox("Ignite Usage"));

            harassMenu = Config.AddSubMenu("Mixed", "Mixed");
            harassMenu.Add("useqh", new CheckBox("Use Q"));
            harassMenu.Add("usewh", new CheckBox("Use W"));


            ahMenu = Config.AddSubMenu("Auto Harass", "AutoHarass");
            ahMenu.Add("onofftoggle", new KeyBind("Toggle", false, KeyBind.BindTypes.PressToggle, 'T'));
            ahMenu.Add("useqha", new CheckBox("Use Q"));
            ahMenu.Add("usewha", new CheckBox("Use W"));

            clearMenu = Config.AddSubMenu("Lane Clear", "LaneClear");
            clearMenu.Add("minmana", new Slider("Min Mana%", 30, 0, 100));
            clearMenu.Add("qlasthitlane", new CheckBox("Last Hit With Q In Lane Clear"));
            clearMenu.Add("wlasthitlane", new CheckBox("Last Hit With W In Lane Clear"));
            clearMenu.Add("qlaneclear", new CheckBox("Use Q Always"));
            clearMenu.Add("wlaneclear", new CheckBox("Use W Always"));
            clearMenu.Add("wlaneclearmin", new Slider("Min Minions To [W]", 3, 1, 20));


            lasthitMenu = Config.AddSubMenu("Last Hit", "LastHit");
            lasthitMenu.Add("minmanal", new Slider("Min Mana%", 30, 0, 100));
            lasthitMenu.Add("qlasthit", new CheckBox("Last Hit With Q"));
            lasthitMenu.Add("wlasthit", new CheckBox("Last Hit With W"));


            ksMenu = Config.AddSubMenu("Kill Steal", "KillSteal");
            ksMenu.Add("useqks", new CheckBox("Use Q"));
            ksMenu.Add("usewks", new CheckBox("Use W"));
            ksMenu.Add("useeks", new CheckBox("Use E"));

            drawMenu = Config.AddSubMenu("Drawing", "Drawing");
            drawMenu.Add("drawq", new CheckBox("Draw Q"));
            drawMenu.Add("draww", new CheckBox("Draw W"));
            drawMenu.Add("drawe", new CheckBox("Draw E"));

        }
    }
}
