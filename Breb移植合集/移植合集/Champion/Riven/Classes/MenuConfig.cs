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

namespace NechritoRiven
{
    class MenuConfig
    {
        public static Menu Config, animation, combo, lane, jngl, misc, draw, flee;

        public static string menuName = "Nechrito Riven";

        public static void LoadMenu()
        {
            Config = MainMenu.AddMenu(menuName, menuName);

            animation = Config.AddSubMenu("动作", "Animation");
            animation.Add("qReset", new CheckBox("快速  & 人性化 Q"));
            animation.AddSeparator();
            animation.Add("Qstrange", new CheckBox("开启动作", false));
            animation.Add("animLaugh", new CheckBox("笑", false));
            animation.Add("animTaunt", new CheckBox("嘲讽", false));
            animation.Add("animTalk", new CheckBox("说笑话", false));
            animation.Add("animDance", new CheckBox("跳舞", false));

            combo = Config.AddSubMenu("连招", "Combo");
            combo.Add("ignite", new CheckBox("自动 点燃"));
            combo.Add("Burst", new KeyBind("强制 爆发 按键", false, KeyBind.BindTypes.PressToggle, 'T'));
            combo.Add("AlwaysR", new KeyBind("强制 R 按键", false, KeyBind.BindTypes.PressToggle, 'G'));
            combo.Add("AlwaysF", new KeyBind("总是 闪现 按键", false, KeyBind.BindTypes.PressToggle, 'L'));

            lane = Config.AddSubMenu("清线", "Lane");
            lane.Add("FastC", new CheckBox("快速清线", false));
            lane.Add("LaneQ", new CheckBox("使用 Q"));
            lane.Add("LaneW", new CheckBox("使用 W"));
            lane.Add("LaneE", new CheckBox("使用 E"));

            jngl = Config.AddSubMenu("清野", "Jungle");
            jngl.Add("JungleQ", new CheckBox("使用 Q"));
            jngl.Add("JungleW", new CheckBox("使用 W"));
            jngl.Add("JungleE", new CheckBox("使用 E"));

            misc = Config.AddSubMenu("杂项", "Misc");
            misc.Add("KeepQ", new CheckBox("保持 Q 状态"));
            misc.Add("FHarass", new KeyBind("快速骚扰按键", false, KeyBind.BindTypes.PressToggle, 'J'));
            misc.Add("QD", new Slider("Ping 延迟", 56, 20, 300));
            misc.Add("QLD", new Slider("技能 延迟", 56, 20, 300));

            draw = Config.AddSubMenu("线圈", "Draw");
            draw.Add("FleeSpot", new CheckBox("显示可逃跑位置"));
            draw.Add("Dind", new CheckBox("伤害指示器"));
            draw.Add("DrawForceFlash", new CheckBox("闪现状态"));
            draw.Add("DrawAlwaysR", new CheckBox("R 状态"));
            draw.Add("DrawTimer1", new CheckBox("显示 Q 结束时间", false));
            draw.Add("DrawTimer2", new CheckBox("显示 R 结束时间", false));
            draw.Add("DrawCB", new CheckBox("显示 连招 进攻", false));
            draw.Add("DrawBT", new CheckBox("爆发 进攻", false));
            draw.Add("DrawFH", new CheckBox("显示 快速骚扰 进攻", false));
            draw.Add("DrawHS", new CheckBox("显示 骚扰 进攻", false));

            flee = Config.AddSubMenu("逃跑", "Flee");
            flee.Add("WallFlee", new CheckBox("逃跑时跳墙"));
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

        public static bool fastHar => getKeyBindItem(misc, "FHarass");
        public static bool burst => getKeyBindItem(combo, "Burst");
        public static bool FastC => getCheckBoxItem(lane, "FastC");
        public static bool FleeSpot => getCheckBoxItem(draw, "FleeSpot");
        public static bool WallFlee => getCheckBoxItem(flee, "WallFlee");
        public static bool jnglQ => getCheckBoxItem(jngl, "JungleQ");
        public static bool jnglW => getCheckBoxItem(jngl, "JungleW");
        public static bool jnglE => getCheckBoxItem(jngl, "JungleE");
        public static bool AlwaysF => getKeyBindItem(combo, "AlwaysF");
        public static bool ignite => getCheckBoxItem(combo, "ignite");
        public static bool ForceFlash => getCheckBoxItem(draw, "DrawForceFlash");
        public static bool QReset => getCheckBoxItem(animation, "qReset");
        public static bool Dind => getCheckBoxItem(draw, "Dind");
        public static bool DrawCb => getCheckBoxItem(draw, "DrawCB");
        public static bool AnimLaugh => getCheckBoxItem(animation, "animLaugh");
        public static bool AnimTaunt => getCheckBoxItem(animation, "animTaunt");
        public static bool AnimDance => getCheckBoxItem(animation, "animDance");
        public static bool AnimTalk => getCheckBoxItem(animation, "animTalk");
        public static bool DrawAlwaysR => getCheckBoxItem(draw, "DrawAlwaysR");
        public static bool KeepQ => getCheckBoxItem(misc, "KeepQ");
        public static bool DrawFh => getCheckBoxItem(draw, "DrawFH");
        public static bool DrawTimer1 => getCheckBoxItem(draw, "DrawTimer1");
        public static bool DrawTimer2 => getCheckBoxItem(draw, "DrawTimer2");
        public static bool DrawHs => getCheckBoxItem(draw, "DrawHS");
        public static bool DrawBt => getCheckBoxItem(draw, "DrawBT");
        public static bool AlwaysR => getKeyBindItem(combo, "AlwaysR");
        public static int Qd => getSliderItem(misc, "QD");
        public static int Qld => getSliderItem(misc, "QLD");
        public static bool LaneW => getCheckBoxItem(lane, "LaneW");
        public static bool LaneE => getCheckBoxItem(lane, "LaneE");
        public static bool Qstrange => getCheckBoxItem(animation, "Qstrange");
        public static bool LaneQ => getCheckBoxItem(lane, "LaneQ");
    }
}