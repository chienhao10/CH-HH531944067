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

            AddKeyBind(Config, "手动 E->Q按键", "manualeq", 'A', KeyBind.BindTypes.HoldActive);
            AddKeyBind(Config, "R 循环逃跑", "flee", 'T', KeyBind.BindTypes.PressToggle);
            AddBool(Config, "屏蔽走砍", "disorb", false);

            combo = Config.AddSubMenu("连招", "Combo Settings");
            combo.AddGroupLabel("近程模式");
            AddBool(combo, "使用 [Q]", "useqcm");
            AddBool(combo, "使用 [W]", "usewcm");
            AddBool(combo, "使用 [E]", "useecm");
            AddBool(combo, "智能 [E]", "useecme");
            combo.AddSeparator();
            combo.AddGroupLabel("远程模式");
            AddBool(combo, "使用 [Q]", "useqcr");
            AddBool(combo, "使用 [W]", "usewcr");
            AddBool(combo, "使用 [E]", "useecr");
            combo.AddSeparator();
            AddBool(combo, "自动切换模式 ([R])", "usercf");

            harass = Config.AddSubMenu("骚扰", "harass Settings");
            harass.AddGroupLabel("M近程模式");
            AddBool(harass, "使用 [Q]", "useqhm");
            harass.AddSeparator();
            harass.AddGroupLabel("远程模式");
            AddBool(harass, "使用 [Q]", "useqhr");
            AddBool(harass, "使用 [W]", "usewhr");

            laneclear = Config.AddSubMenu("Lane Clear Settings", "Lane Clear Settings");
            AddValue(laneclear, "Minimum minions hit For W/Q", "minhitwq", 2, 0, 10);
            AddValue(laneclear, "Minimum Mana", "minmana", 30);
            laneclear.AddSeparator();
            laneclear.AddGroupLabel("近程模式");
            AddBool(laneclear, "使用 [Q]", "useqlm");
            AddBool(laneclear, "使用 [W]", "usewlm");
            AddBool(laneclear, "使用 [E]", "useelm");
            laneclear.AddSeparator();
            laneclear.AddGroupLabel("远程模式");
            AddBool(laneclear, "使用 [Q]", "useqlr");
            AddBool(laneclear, "使用 [W]", "usewlr");

            drawings = Config.AddSubMenu("线圈", "Drawings");
            AddBool(drawings, "显示 [Q]", "drawq");
            AddBool(drawings, "显示 [E]", "drawe");
            AddBool(drawings, "显示 计时", "drawtimers");

            misc = Config.AddSubMenu("杂项", "Misc Settings");
            AddBool(misc, "自动 E 打断技能", "autoeint");
            AddBool(misc, "自动 E 冲刺敌人", "autoedash");
            AddBool(misc, "自动 E 防突进", "autoegap");
        }
    }
}
