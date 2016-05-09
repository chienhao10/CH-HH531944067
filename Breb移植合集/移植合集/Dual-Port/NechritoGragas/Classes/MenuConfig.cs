using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

namespace Nechrito_Gragas
{
    class MenuConfig
    {
        public static Menu Config, combo, Lane, misc, draw;

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        public static string menuName = "Nechrito Gragas";
        public static void LoadMenu()
        {
            Config = MainMenu.AddMenu(menuName, menuName);

            //COMBOS ETC HERE
            combo = Config.AddSubMenu("Combo", "Combo");
            combo.Add("OnlyR", new CheckBox("Only R Selected Target"));
            combo.Add("ComboR", new CheckBox("Use R"));

            Lane = Config.AddSubMenu("Lane", "Lane");
            Lane.Add("LaneQ", new CheckBox("Use Q"));
            Lane.Add("LaneW", new CheckBox("Use W"));
            Lane.Add("LaneE", new CheckBox("Use E"));

            misc = Config.AddSubMenu("Misc", "Misc");
            misc.Add("SmiteJngl", new CheckBox("Auto Smite"));

            draw = Config.AddSubMenu("Draw", "Draw");
            draw.Add("dind", new CheckBox("Damage Indicator"));
        }
        public static bool OnlyR => getCheckBoxItem(combo, "OnlyR");
        public static bool ComboR => getCheckBoxItem(combo, "ComboR");
        public static bool LaneQ => getCheckBoxItem(Lane, "LaneQ");
        public static bool LaneW => getCheckBoxItem(Lane, "LaneW");
        public static bool LaneE => getCheckBoxItem(Lane, "LaneE");
        public static bool dind => getCheckBoxItem(draw, "dind");
    }
}
