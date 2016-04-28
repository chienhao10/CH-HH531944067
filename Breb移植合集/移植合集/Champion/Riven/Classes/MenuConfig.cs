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

            emoteMenu = Config.AddSubMenu("Animation", "Animation");
            emoteMenu.Add("qReset", new CheckBox("Fast & Legit Q"));
            emoteMenu.Add("Qstrange", new CheckBox("Animation (Troll!) | Enables Below", false));
            emoteMenu.Add("animLaugh", new CheckBox("Laugh", false));
            emoteMenu.Add("animTaunt", new CheckBox("Taunt", false));
            emoteMenu.Add("animTalk", new CheckBox("Joke", false));
            emoteMenu.Add("animDance", new CheckBox("Dance", false));

            combo = Config.AddSubMenu("Combo", "Combo");
            combo.Add("KAPPA", new CheckBox("Force R OFF Will use R when killable"));
            combo.Add("AlwaysR", new KeyBind("Force R", false, KeyBind.BindTypes.PressToggle, 'G'));
            combo.Add("AlwaysF", new KeyBind("Force Flash", false, KeyBind.BindTypes.PressToggle, 'L'));

            lane = Config.AddSubMenu("Lane", "Lane");
            lane.Add("FastC", new CheckBox("Fast Laneclear", false));
            lane.Add("LaneQ", new CheckBox("Use Q"));
            lane.Add("LaneW", new CheckBox("Use W"));
            lane.Add("LaneE", new CheckBox("Use E"));

            jngl = Config.AddSubMenu("Jungle", "Jungle");
            jngl.Add("JungleQ", new CheckBox("Use Q"));
            jngl.Add("JungleW", new CheckBox("Use W"));
            jngl.Add("JungleE", new CheckBox("Use E"));

            misc = Config.AddSubMenu("Misc", "Misc");
            misc.Add("KeepQ", new CheckBox("Keep Q Alive"));
            misc.Add("QD", new Slider("Ping Delay", 56, 20, 300));
            misc.Add("QLD", new Slider("Spell Delay", 56, 20, 300));
            misc.Add("fastHKey", new KeyBind("Fast Harass", false, KeyBind.BindTypes.HoldActive, 'A'));
            misc.Add("burstKey", new KeyBind("Burst", false, KeyBind.BindTypes.HoldActive, 'T'));

            draw = Config.AddSubMenu("Draw", "Draw");
            draw.Add("FleeSpot", new CheckBox("Draw Flee Spots"));
            draw.Add("Dind", new CheckBox("Damage Indicator"));
            draw.Add("DrawForceFlash", new CheckBox("Flash Status"));
            draw.Add("DrawAlwaysR", new CheckBox("R Status"));
            draw.Add("DrawTimer1", new CheckBox("Draw Q Expiry Time", false));
            draw.Add("DrawTimer2", new CheckBox("Draw R Expiry Time", false));
            draw.Add("DrawCB", new CheckBox("Combo Engage", false));
            draw.Add("DrawBT", new CheckBox("Burst Engage", false));
            draw.Add("DrawFH", new CheckBox("FastHarass Engage", false));
            draw.Add("DrawHS", new CheckBox("Harass Engage", false));
        }
    }
}