using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

namespace Nechrito_Nidalee
{
    class MenuConfig : Core
    {
        private const string MenuName = "Nechrito Nidalee";
        public static Menu Menu { get; set; } = MainMenu.AddMenu(MenuName, MenuName);

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

        public static Menu ks, jngl, heal, misc, draw, flee;

        public static void Load()
        {
            ks = Menu.AddSubMenu("抢头", "KillSteal");
            ks.Add("SpellsKS", new CheckBox("抢头技能"));
            ks.Add("ComboSmite", new CheckBox("惩戒"));
            ks.Add("ComboIgnite", new CheckBox("点燃"));

            jngl = Menu.AddSubMenu("清野", "Jungle");
            jngl.Add("jnglQ", new Slider("使用Q 蓝量 %", 15, 0, 100));
            jngl.Add("jnglHeal", new Slider("自动治疗", 15, 0, 95));

            heal = Menu.AddSubMenu("Heal", "治疗管理器");
            heal.Add("allyHeal", new Slider("治疗友军血量 <= %", 45, 0, 80));
            heal.Add("SelfHeal", new Slider("治疗自己当血量 <= %", 80, 0, 90));
            heal.Add("ManaHeal", new Slider("不使用当蓝量 <= %", 20, 0, 100));

            misc = Menu.AddSubMenu("杂项", "Misc");
            misc.Add("Gapcloser", new CheckBox("防突进"));

            draw = Menu.AddSubMenu("线圈", "Draw");
            draw.Add("dind", new CheckBox("伤害指示器"));
            draw.Add("EngageDraw", new CheckBox("进攻范围"));
            draw.Add("fleeDraw", new CheckBox("显示逃跑点"));

            flee = Menu.AddSubMenu("逃跑", "Flee");
            flee.Add("FleeMouse", new KeyBind("逃跑按键 (测试)", false, KeyBind.BindTypes.HoldActive, 'A'));
        }

        public static bool ComboSmite => getCheckBoxItem(ks, "ComboSmite");
        public static bool ComboIgnite => getCheckBoxItem(ks, "ComboIgnite");
        public static bool dind => getCheckBoxItem(draw, "dind");
        public static bool fleeDraw => getCheckBoxItem(draw, "fleeDraw");
        public static bool SpellsKS => getCheckBoxItem(ks, "SpellsKS");
        public static bool EngageDraw => getCheckBoxItem(draw, "EngageDraw");
        public static bool Gapcloser => getCheckBoxItem(misc, "Gapcloser");

        public static bool FleeMouse => getKeyBindItem(flee, "FleeMouse");

        public static int ManaHeal => getSliderItem(heal, "ManaHeal");
        public static int SelfHeal => getSliderItem(heal, "SelfHeal");
        public static int allyHeal => getSliderItem(heal, "allyHeal");
        public static int jnglQ => getSliderItem(jngl, "jnglQ");
        public static int jnglHeal => getSliderItem(jngl, "jnglHeal");
    }
}
