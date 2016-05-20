using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

namespace Jayce
{
    class MenuConfig : Helper
    {

        public static Menu Config;

        public static void AddBool(Menu m, string display, string unique, bool def = true)
        {
            m.Add(unique, new CheckBox(display, def));
        }

        public static void AddKeyBind(Menu m, string display, string unique, uint key, KeyBind.BindTypes k, bool def = false)
        {
            m.Add(unique, new KeyBind(display, def, k, key));
        }

        public static void AddValue(Menu m, string display, string unique, int def = 0, int start = 0, int stop = 100)
        {
            m.Add(unique, new Slider(display, def, start, stop));
        }

        public static Menu combo, harass, laneclear, drawings, misc;

        public static void OnLoad()
        {
            Config = MainMenu.AddMenu(Menuname, Menuname);

            AddKeyBind(Config, "Manual E->Q", "manualeq", 'A', KeyBind.BindTypes.HoldActive);
            AddKeyBind(Config, "R Spam", "flee", 'T', KeyBind.BindTypes.PressToggle);
            AddBool(Config, "Disable Orbwalker", "disorb", false);

            combo = Config.AddSubMenu("Combo Settings", "Combo Settings");
            combo.AddGroupLabel("Melee Settings");
            AddBool(combo, "Use [Q]", "useqcm");
            AddBool(combo, "Use [W]", "usewcm");
            AddBool(combo, "Use [E]", "useecm");
            AddBool(combo, "Smart [E]", "useecme");
            combo.AddSeparator();
            combo.AddGroupLabel("Ranged Settings");
            AddBool(combo, "Use [Q]", "useqcr");
            AddBool(combo, "Use [W]", "usewcr");
            AddBool(combo, "Use [E]", "useecr");
            combo.AddSeparator();
            AddBool(combo, "Auto Change Forms ([R])", "usercf");

            harass = Config.AddSubMenu("Harass Settings", "harass Settings");
            harass.AddGroupLabel("Melee Settings");
            AddBool(harass, "Use [Q]", "useqhm");
            harass.AddSeparator();
            harass.AddGroupLabel("Ranged Settings");
            AddBool(harass, "Use [Q]", "useqhr");
            AddBool(harass, "Use [W]", "usewhr");

            laneclear = Config.AddSubMenu("Lane Clear Settings", "Lane Clear Settings");
            AddValue(laneclear, "Minimum minions hit For W/Q", "minhitwq", 2, 0, 10);
            AddValue(laneclear, "Minimum Mana", "minmana", 30);
            laneclear.AddSeparator();
            laneclear.AddGroupLabel("Melee Settings");
            AddBool(laneclear, "Use [Q]", "useqlm");
            AddBool(laneclear, "Use [W]", "usewlm");
            AddBool(laneclear, "Use [E]", "useelm");
            laneclear.AddSeparator();
            laneclear.AddGroupLabel("Ranged Settings");
            AddBool(laneclear, "Use [Q]", "useqlr");
            AddBool(laneclear, "Use [W]", "usewlr");

            drawings = Config.AddSubMenu("Drawings", "Drawings");
            AddBool(drawings, "Draw [Q]", "drawq");
            AddBool(drawings, "Draw [E]", "drawe");
            AddBool(drawings, "Draw Timers", "drawtimers");

            misc = Config.AddSubMenu("Misc Settings", "Misc Settings");
            AddBool(misc, "Auto E On Interruptable", "autoeint");
            AddBool(misc, "Auto E On Dash", "autoedash");
            AddBool(misc, "Auto E On Gap Closers", "autoegap");
        }
    }
}
