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
            ks = Menu.AddSubMenu("KillSteal", "KillSteal");
            ks.Add("SpellsKS", new CheckBox("KS Spells"));
            ks.Add("ComboSmite", new CheckBox("Smite"));
            ks.Add("ComboIgnite", new CheckBox("Ignite"));

            jngl = Menu.AddSubMenu("Jungle", "Jungle");
            jngl.Add("jnglQ", new Slider("Use Javelin Mana %", 15, 0, 100));
            jngl.Add("jnglHeal", new Slider("Auto Heal", 15, 0, 95));

            heal = Menu.AddSubMenu("Heal", "Heal Manager");
            heal.Add("allyHeal", new Slider("Heal Allies Hp <= %", 45, 0, 80));
            heal.Add("SelfHeal", new Slider("Self Heal Hp <= %", 80, 0, 90));
            heal.Add("ManaHeal", new Slider("Mana <= %", 20, 0, 100));

            misc = Menu.AddSubMenu("Misc", "Misc");
            misc.Add("Gapcloser", new CheckBox("Gapcloser"));
            misc.Add("manaW", new Slider("Use W Mana %", 15, 0, 100));


            draw = Menu.AddSubMenu("Draw", "Draw");
            draw.Add("dind", new CheckBox("Draw damage indicator"));
            draw.Add("EngageDraw", new CheckBox("Engage Range"));
            draw.Add("fleeDraw", new CheckBox("Draw Flee Spots"));

            flee = Menu.AddSubMenu("Flee", "Flee");
            flee.Add("FleeMouse", new KeyBind("Flee (BETA)", false, KeyBind.BindTypes.HoldActive, 'A'));
        }

        public static bool ComboSmite => getCheckBoxItem(ks, "ComboSmite");
        public static bool ComboIgnite => getCheckBoxItem(ks, "ComboIgnite");
        public static bool dind => getCheckBoxItem(draw, "dind");
        public static bool fleeDraw => getCheckBoxItem(draw, "fleeDraw");
        public static bool SpellsKS => getCheckBoxItem(ks, "SpellsKS");
        public static bool EngageDraw => getCheckBoxItem(draw, "EngageDraw");
        public static bool Gapcloser => getCheckBoxItem(misc, "Gapcloser");

        public static bool FleeMouse => getKeyBindItem(flee, "FleeMouse");
        public static int manaW => getSliderItem(misc, "manaW");

        public static int ManaHeal => getSliderItem(heal, "ManaHeal");
        public static int SelfHeal => getSliderItem(heal, "SelfHeal");
        public static int allyHeal => getSliderItem(heal, "allyHeal");
        public static int jnglQ => getSliderItem(jngl, "jnglQ");
        public static int jnglHeal => getSliderItem(jngl, "jnglHeal");
    }
}
