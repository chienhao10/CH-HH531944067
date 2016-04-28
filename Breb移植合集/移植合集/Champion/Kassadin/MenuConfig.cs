using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Kassawin
{
    internal class MenuConfig : Helper
    {
        public static void OnLoad()
        {
            Config = MainMenu.AddMenu(Menuname, Menuname);

            comboMenu = Config.AddSubMenu("Combo Settings", "Combo Settings");
            comboMenu.Add("useq", new CheckBox("Use [Q]"));
            comboMenu.Add("usew", new CheckBox("Use [W]"));
            comboMenu.Add("usee", new CheckBox("Use [E]"));
            comboMenu.Add("user", new CheckBox("Use [R]"));
            comboMenu.Add("usert", new CheckBox("Don't [R] Under Turret"));
            comboMenu.Add("useignite", new CheckBox("Use [Ignite]"));
            comboMenu.Add("rcount", new Slider("Only [R] When Stack Below", 1, 1, 5));

            harassMenu = Config.AddSubMenu("Harass Settings", "Harass Settings");
            harassMenu.Add("useqharass", new CheckBox("Use [Q]"));
            harassMenu.Add("useeharass", new CheckBox("Use [E]"));
            harassMenu.Add("harassmana", new Slider("Minimum Mana", 30));

            farmMenu = Config.AddSubMenu("Farm Settings", "Farm Settings");
            farmMenu.AddGroupLabel("Lane Clear");
            farmMenu.Add("minmanalaneclear", new Slider("Minimum Mana", 30));
            farmMenu.Add("useql", new CheckBox("Use [Q]"));
            farmMenu.Add("usewl", new CheckBox("Use [W]"));
            farmMenu.Add("useel", new CheckBox("Use [E]"));
            farmMenu.Add("userl", new CheckBox("Use [R]"));
            farmMenu.Add("useels", new Slider("Minimum Minions For E", 3, 1, 10));
            farmMenu.Add("userls", new Slider("Minimum Minions For R", 3, 1, 10));
            farmMenu.Add("rcountl", new Slider("Only [R] When Stack Below", 1, 1, 5));
            farmMenu.AddSeparator();
            farmMenu.AddGroupLabel("Jungle Clear");
            farmMenu.Add("minmanajungleclear", new Slider("Minimum Mana", 30));
            farmMenu.Add("useqj", new CheckBox("Use [Q]"));
            farmMenu.Add("usewj", new CheckBox("Use [W]"));
            farmMenu.Add("useej", new CheckBox("Use [E]"));
            farmMenu.Add("userj", new CheckBox("Use [R]"));
            farmMenu.Add("rcountj", new Slider("Only [R] When Stack Below", 1, 1, 5));
            farmMenu.AddSeparator();
            farmMenu.AddGroupLabel("Last Hit");
            farmMenu.Add("minmanalasthit", new Slider("Minimum Mana", 30));
            farmMenu.Add("useqlh", new CheckBox("Use [Q]"));
            farmMenu.AddSeparator();

            ksMenu = Config.AddSubMenu("Kill Steal Settings", "Kill Steal Settings");
            ksMenu.Add("ks", new CheckBox("Activate [KS]"));
            ksMenu.Add("qks", new CheckBox("Use [Q]"));
            ksMenu.Add("eks", new CheckBox("Use [E]"));
            ksMenu.Add("rks", new CheckBox("Use [R]"));
            ksMenu.Add("rgks", new CheckBox("Use [R] As Gap Close"));

            drawMenu = Config.AddSubMenu("Drawings", "Drawings");
            drawMenu.Add("enabledraw", new CheckBox("Enable Drawings"));
            drawMenu.Add("drawq", new CheckBox("Draw [Q] Range"));
            drawMenu.Add("drawe", new CheckBox("Draw [E] Range"));
            drawMenu.Add("drawr", new CheckBox("Draw [R] Range"));
            drawMenu.Add("drawqkill", new CheckBox("Draw Killable Minions with [Q]"));
            drawMenu.Add("drawcount", new CheckBox("Draw [R] Count"));
            drawMenu.Add("drawdamage", new CheckBox("Draw Damage"));

            miscMenu = Config.AddSubMenu("Misc", "Misc");
            miscMenu.Add("fleemode", new KeyBind("Flee Mode", false, KeyBind.BindTypes.HoldActive, 'A'));
            miscMenu.Add("userflee", new CheckBox("Use [R] Flee Mode"));
        }
    }
}