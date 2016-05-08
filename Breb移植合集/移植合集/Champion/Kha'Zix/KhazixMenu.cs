using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp;
using LeagueSharp.Common;

namespace SephKhazix
{
    class KhazixMenu
    {
        public static Menu menu, harass, combo, farm, ks, safety, djump, draw, debug;

        public KhazixMenu()
        {
            menu = MainMenu.AddMenu("Seph 螳螂", "SephKhazix");

            //Harass
            harass = menu.AddSubMenu("骚扰");
            harass.Add("UseQHarass", new CheckBox("使用 Q"));
            harass.Add("UseWHarass", new CheckBox("使用 W"));
            harass.Add("Harass.AutoWI", new CheckBox("自动-W 定身单位"));
            harass.Add("Harass.AutoWD", new CheckBox("自动 W"));
            harass.Add("Harass.Key", new KeyBind("骚扰按键开关", false, KeyBind.BindTypes.PressToggle, 'H'));
            harass.Add("Harass.InMixed", new CheckBox("混合模式下骚扰", false));
            harass.Add("Harass.WHitchance", new ComboBox("W 命中率", 1, HitChance.Low.ToString(), HitChance.Medium.ToString(), HitChance.High.ToString()));


            //Combo
            combo = menu.AddSubMenu("连招");
            combo.Add("UseQCombo", new CheckBox("使用 Q"));
            combo.Add("UseWCombo", new CheckBox("使用 W"));
            combo.Add("WHitchance", new ComboBox("W 命中率", 1, HitChance.Low.ToString(), HitChance.Medium.ToString(), HitChance.High.ToString()));
            combo.Add("UseECombo", new CheckBox("使用 E"));
            combo.Add("UseEGapclose", new CheckBox("使用 E 接近目标 Q"));
            combo.Add("UseEGapcloseW", new CheckBox("使用 E 接近目标 W"));
            combo.Add("UseRGapcloseW", new CheckBox("超出距离后再使用 R"));
            combo.Add("UseRCombo", new CheckBox("使用 R"));
            combo.Add("UseItems", new CheckBox("使用 物品"));

            //Farm
            farm = menu.AddSubMenu("农兵");
            farm.Add("UseQFarm", new CheckBox("使用 Q"));
            farm.Add("UseEFarm", new CheckBox("使用 E"));
            farm.Add("UseWFarm", new CheckBox("使用 W"));
            farm.Add("Farm.WHealth", new Slider("Health % to use W", 80, 0, 100));
            farm.Add("UseItemsFarm", new CheckBox("使用 Items"));

            //Kill Steal
            ks = menu.AddSubMenu("抢头");
            ks.Add("Kson", new CheckBox("开启抢头"));
            ks.Add("UseQKs", new CheckBox("使用 Q"));
            ks.Add("UseWKs", new CheckBox("使用 W"));
            ks.Add("UseEKs", new CheckBox("使用 E"));
            ks.Add("Ksbypass", new CheckBox("无视安全检查，E抢头", false));
            ks.Add("UseEQKs", new CheckBox("使用 EQ 抢头"));
            ks.Add("UseEWKs", new CheckBox("使用 EW 抢头"));
            ks.Add("UseTiamatKs", new CheckBox("使用 物品"));
            ks.Add("Edelay", new Slider("E 延迟 (毫秒)", 0, 0, 300));

            // safety
            safety = menu.AddSubMenu("安全检查");
            safety.Add("Safety.Enabled", new CheckBox("开启安全检查"));
            safety.Add("Safety.Override", new KeyBind("强制开启安全检查按键", false, KeyBind.BindTypes.HoldActive, 'T'));
            safety.Add("Safety.autoescape", new CheckBox("低血量时 E 跳出战斗"));
            safety.Add("Safety.CountCheck", new CheckBox("最低友军比例跳跃至敌方"));
            safety.Add("Safety.Ratio", new Slider("友军:敌方 比例 (/5)", 1, 0, 5));
            safety.Add("Safety.TowerJump", new CheckBox("防止越塔"));
            safety.Add("Safety.MinHealth", new Slider("血量 %", 15, 0, 100));
            safety.Add("Safety.noaainult", new CheckBox("隐身时不普攻", false));

            //Double Jump
            djump = menu.AddSubMenu("双跳");
            djump.Add("djumpenabled", new CheckBox("开启"));
            djump.Add("JEDelay", new Slider("延迟跳跃", 250, 250, 500));
            djump.Add("jumpmode", new ComboBox("跳跃模式", 0, "预设 (自己泉水方向)", "自订 - 以下设置"));
            djump.Add("save", new CheckBox("保留双跳进", false));
            djump.Add("noauto", new CheckBox("等待Q冷却结束", false));
            djump.Add("jcursor", new CheckBox("第一跳跃至鼠标（脚本逻辑）"));
            djump.Add("secondjump", new CheckBox("进行第二次跳跃"));
            djump.Add("jcursor2", new CheckBox("第二跳跃至鼠标（脚本逻辑）"));
            djump.Add("jumpdrawings", new CheckBox("开启跳跃线圈"));


            //Drawings
            draw = menu.AddSubMenu("线圈");
            draw.Add("Drawings.Disable", new CheckBox("屏蔽线圈", true));
            draw.Add("DrawQ", new CheckBox("显示 Q"));//, 0, System.Drawing.Color.White);
            draw.Add("DrawW", new CheckBox("显示 W"));//, 0, System.Drawing.Color.Red);
            draw.Add("DrawE", new CheckBox("显示 E"));//, 0, System.Drawing.Color.Green);

            //Debug
            debug = menu.AddSubMenu("调试");
            debug.Add("Debugon", new CheckBox("挑起调试"));
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

        internal HitChance GetHitChance(string search)
        {
            var hitchance = getBoxItem(combo, "WHitchance");
            switch (hitchance)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
            }
            return HitChance.Medium;
        }
    }
}
