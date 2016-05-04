using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace NechritoRiven
{
    internal class MenuConfig
    {
        public static Menu Config, combo, emoteMenu, lane, misc, draw, jngl;

        public static string menuName = "Nechrito Riven";

        public static bool AlwaysF
        {
            get { return getKeyBindItem(combo, "AlwaysF"); }
        }

        public static bool burstKey
        {
            get { return getKeyBindItem(misc, "burstKey"); }
        }

        public static bool fastHKey
        {
            get { return getKeyBindItem(misc, "fastHKey"); }
        }

        public static bool ForceFlash
        {
            get { return getCheckBoxItem(draw, "DrawForceFlash"); }
        }

        public static bool QReset
        {
            get { return getCheckBoxItem(emoteMenu, "qReset"); }
        }

        public static bool Dind
        {
            get { return getCheckBoxItem(draw, "Dind"); }
        }

        public static bool DrawCb
        {
            get { return getCheckBoxItem(draw, "DrawCB"); }
        }

        public static bool AnimLaugh
        {
            get { return getCheckBoxItem(emoteMenu, "animLaugh"); }
        }

        public static bool AnimTaunt
        {
            get { return getCheckBoxItem(emoteMenu, "animTaunt"); }
        }

        public static bool AnimDance
        {
            get { return getCheckBoxItem(emoteMenu, "animDance"); }
        }

        public static bool AnimTalk
        {
            get { return getCheckBoxItem(emoteMenu, "animTalk"); }
        }

        public static bool DrawAlwaysR
        {
            get { return getCheckBoxItem(draw, "DrawAlwaysR"); }
        }

        public static bool KeepQ
        {
            get { return getCheckBoxItem(misc, "KeepQ"); }
        }

        public static bool DrawFh
        {
            get { return getCheckBoxItem(draw, "DrawFH"); }
        }

        public static bool DrawTimer1
        {
            get { return getCheckBoxItem(draw, "DrawTimer1"); }
        }

        public static bool DrawTimer2
        {
            get { return getCheckBoxItem(draw, "DrawTimer2"); }
        }

        public static bool DrawHs
        {
            get { return getCheckBoxItem(draw, "DrawHS"); }
        }

        public static bool DrawBt
        {
            get { return getCheckBoxItem(draw, "DrawBT"); }
        }

        public static bool AlwaysR
        {
            get { return getKeyBindItem(combo, "AlwaysR"); }
        }

        public static int Qd
        {
            get { return getSliderItem(misc, "QD"); }
        }

        public static int Qld
        {
            get { return getSliderItem(misc, "QLD"); }
        }

        public static bool LaneW
        {
            get { return getCheckBoxItem(lane, "LaneW"); }
        }

        public static bool LaneE
        {
            get { return getCheckBoxItem(lane, "LaneE"); }
        }

        public static bool Qstrange
        {
            get { return getCheckBoxItem(emoteMenu, "Qstrange"); }
        }

        public static bool LaneQ
        {
            get { return getCheckBoxItem(lane, "LaneQ"); }
        }

        public static bool FastC => getCheckBoxItem(lane, "FastC");
        public static bool FleeSpot => getCheckBoxItem(draw, "FleeSpot");
        //public static bool WallFlee => Config.Item("WallFlee").GetValue<bool>();
        public static bool jnglQ => getCheckBoxItem(jngl, "JungleQ");
        public static bool jnglW => getCheckBoxItem(jngl, "JungleW");
        public static bool jnglE => getCheckBoxItem(jngl, "JungleE");

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

        public static void LoadMenu()
        {
            Config = MainMenu.AddMenu(menuName, menuName);

            emoteMenu = Config.AddSubMenu("动作", "Animation");
            emoteMenu.Add("qReset", new CheckBox("快速  & 人性化 Q"));
            emoteMenu.Add("Qstrange", new CheckBox("开启动作 (乱玩!) | 开启已下动作", false));
            emoteMenu.Add("animLaugh", new CheckBox("笑", false));
            emoteMenu.Add("animTaunt", new CheckBox("嘲讽", false));
            emoteMenu.Add("animTalk", new CheckBox("说笑话", false));
            emoteMenu.Add("animDance", new CheckBox("跳舞", false));

            combo = Config.AddSubMenu("连招", "Combo");
            combo.Add("KAPPA", new CheckBox("关闭 强制R 将只会在可击杀时才会使用"));
            combo.Add("AlwaysR", new KeyBind("强制 R", false, KeyBind.BindTypes.PressToggle, 'G'));
            combo.Add("AlwaysF", new KeyBind("强制 闪现", false, KeyBind.BindTypes.PressToggle, 'L'));

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
            misc.Add("QD", new Slider("Ping 延迟", 56, 20, 300));
            misc.Add("QLD", new Slider("技能 延迟", 56, 20, 300));
            misc.Add("fastHKey", new KeyBind("快速骚扰按键", false, KeyBind.BindTypes.HoldActive, 'A'));
            misc.Add("burstKey", new KeyBind("爆发按键", false, KeyBind.BindTypes.HoldActive, 'T'));

            draw = Config.AddSubMenu("线圈", "Draw");
            draw.Add("FleeSpot", new CheckBox("显示可逃跑位置"));
            draw.Add("Dind", new CheckBox("伤害指示器"));
            draw.Add("DrawForceFlash", new CheckBox("闪现状态"));
            draw.Add("DrawAlwaysR", new CheckBox("R 状态"));
            draw.Add("DrawTimer1", new CheckBox("显示 Q 结束时间", false));
            draw.Add("DrawTimer2", new CheckBox("显示 R 结束时间", false));
            draw.Add("DrawCB", new CheckBox("显示 连招 进攻", false));
            draw.Add("DrawBT", new CheckBox("显示 爆发 进攻", false));
            draw.Add("DrawFH", new CheckBox("显示 快速骚扰 进攻", false));
            draw.Add("DrawHS", new CheckBox("显示 骚扰 进攻", false));
        }
    }
}