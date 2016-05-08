using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp;
using LeagueSharp.Common;

namespace Nechrito_Twitch
{
    class MenuConfig
    {
        public static Menu Config, combo, harass, lane, draw;
        public static string menuName = "Nechrito 老鼠";

        public static void LoadMenu()
        {
            Config = MainMenu.AddMenu(menuName, menuName);

            combo = Config.AddSubMenu("连招", "Combo");
            combo.Add("UseW", new CheckBox("使用 W"));
            combo.Add("KsE", new CheckBox("抢头 E"));

            harass = Config.AddSubMenu("骚扰", "Harass");
            harass.Add("harassW", new CheckBox("使用 W"));
            harass.Add("ESlider", new Slider("超出普攻距离使用 E 层数", 0, 0, 6));

            lane = Config.AddSubMenu("清线", "Lane");
            lane.Add("laneW", new CheckBox("使用 W"));

            draw = Config.AddSubMenu("线圈", "Draw");
            draw.Add("dind", new CheckBox("伤害指示器"));
        }

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

        // Menu Items
        public static bool UseW => getCheckBoxItem(combo, "UseW");
        public static bool KsE => getCheckBoxItem(combo, "KsE");
        public static bool laneW => getCheckBoxItem(lane, "laneW");
        public static bool harassW => getCheckBoxItem(harass, "harassW");
        public static bool dind => getCheckBoxItem(draw, "dind");
        public static int ESlider => getSliderItem(harass, "ESlider");
    }
}
