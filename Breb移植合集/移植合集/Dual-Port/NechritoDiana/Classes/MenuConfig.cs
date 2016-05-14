using ClipperLib;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using EloBuddy;
using Font = SharpDX.Direct3D9.Font;
using LeagueSharp.Common.Data;
using LeagueSharp.Common;
using SharpDX.Direct3D9;
using SharpDX;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Security.AccessControl;
using System;
using System.Speech.Synthesis;
namespace Nechrito_Diana
{
    class MenuConfig
    {
        public static Menu Config, combo,lane, jungle, killsteal, misc, draw, flee;
        public static string menuName = "Nechrito 皎月";
        public static void LoadMenu()
        {
            Config = MainMenu.AddMenu(menuName, menuName);
            
            combo = Config.AddSubMenu("连招", "Combo");
            combo.Add("ComboR", new CheckBox("Q 命中目标才使用 R", false));
            combo.Add("ComboE", new CheckBox("智能 E", false));
            combo.Add("Misaya", new CheckBox("强制 使用若风连招", false));
            combo.Add("AutoSmite", new CheckBox("使用惩戒"));
            
            lane = Config.AddSubMenu("清线", "Lane");
            lane.Add("LaneQ", new CheckBox("使用 Q", false));
            lane.Add("LaneW", new CheckBox("使用 W"));

            jungle = Config.AddSubMenu("清野", "Jungle");
            jungle.Add("jnglQR", new CheckBox("QR 接近"));
            jungle.Add("jnglE", new Slider("清野 E 蓝量", 15, 0, 50));
            jungle.Add("jnglW", new CheckBox("使用 W"));

            killsteal = Config.AddSubMenu("抢头", "Killsteal");
            killsteal.Add("ksQ", new CheckBox("抢头 Q"));
            killsteal.Add("ksR", new CheckBox("抢头 R"));
            killsteal.Add("ignite", new CheckBox("自动 点燃"));
            killsteal.Add("ksSmite", new CheckBox("自动 惩戒"));

            misc = Config.AddSubMenu("杂项", "Misc");
            misc.Add("Gapcloser", new CheckBox("接近/防突进"));
            misc.Add("Interrupt", new CheckBox("技能打断"));

            draw = Config.AddSubMenu("线圈", "Draw");
            draw.Add("EngageDraw", new CheckBox("显示进攻距离"));
            draw.Add("EscapeSpot", new CheckBox("显示可逃跑位置"));

            flee = Config.AddSubMenu("逃跑", "Flee");
            flee.Add("FleeMouse", new KeyBind("逃跑按键", false, KeyBind.BindTypes.HoldActive, 'A'));

            SPrediction.Prediction.Initialize(Config);
        }

        public static bool ComboR => getCheckBoxItem(combo, "ComboR");
        public static bool ComboE => getCheckBoxItem(combo, "ComboE");
        public static bool EscapeSpot => getCheckBoxItem(draw, "EscapeSpot");
        public static bool EngageDraw => getCheckBoxItem(draw, "EngageDraw");
        public static bool Misaya => getCheckBoxItem(combo, "Misaya");
        public static bool LaneQ => getCheckBoxItem(lane, "LaneQ");
        public static bool LaneW => getCheckBoxItem(lane, "LaneW");
        public static bool ksSmite => getCheckBoxItem(killsteal, "ksSmite");
        public static bool ksQ => getCheckBoxItem(killsteal, "ksQ");
        public static bool ksR => getCheckBoxItem(killsteal, "ksR");
        public static bool Interrupt => getCheckBoxItem(misc, "Interrupt");
        public static bool Gapcloser => getCheckBoxItem(misc, "Gapcloser");
        public static bool jnglW => getCheckBoxItem(jungle, "jnglW");
        public static bool jnglQR => getCheckBoxItem(jungle, "jnglQR");
        public static bool ignite => getCheckBoxItem(killsteal, "ignite");
        public static bool AutoSmite => getCheckBoxItem(combo, "AutoSmite");
        public static bool FleeMouse => getKeyBindItem(flee, "FleeMouse");
        public static int jnglE => getSliderItem(jungle, "jnglE");

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
    }
}
