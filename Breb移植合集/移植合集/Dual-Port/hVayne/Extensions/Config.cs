using LeagueSharp.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;

namespace hVayne.Extensions
{
    class Config
    {
        private static Menu Menu;

        public static Menu comboMenu, harassMenu, itemMenu, miscMenu, condemMenu, jungleMenu, hvayneModes;


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

        public static int PushDistance;
        public static int CondemnMethod;
        public static int MethodQ;
        public static int ComboMethod;
        public static int HarassMethod;

        public static void ExecuteMenu()
        {

            Menu = MainMenu.AddMenu("hVayne [SDK]", "hVayne [SDK]");

            hvayneModes = Menu.AddSubMenu("hVayne Modes", "genel.seperator1");
            hvayneModes.Add("condemn.push.distance", new Slider("Condemn Push Distance", 410, 350, 420));
            hvayneModes.Add("condemn.style", new ComboBox("Condemn Mode :", 0, "Shine", "Azuna", "360"));
            hvayneModes.Add("q.style", new ComboBox("(Q) Mode :", 0, "Cursor Position", "Safe Position"));
            hvayneModes.Add("combo.type", new ComboBox("Combo Mode :", 0, "Normal", "Burst"));
            hvayneModes.Add("harass.type", new ComboBox("Harass Mode :", 0, "2W + Q", "2W + E"));

            comboMenu = Menu.AddSubMenu("Combo Settings", "combo.settings");
            comboMenu.Add("combo.q", new CheckBox("Use (Q)"));
            comboMenu.Add("combo.e", new CheckBox("Use (E)"));
            comboMenu.Add("combo.r", new CheckBox("Use (R)"));
            comboMenu.Add("combo.r.count", new Slider("Min. Enemy Count (R)", 3, 1, 5));

            harassMenu = Menu.AddSubMenu("Harass Settings", "harass.settings");
            harassMenu.Add("harass.q", new CheckBox("Use (Q)"));
            harassMenu.Add("harass.e", new CheckBox("Use (E)"));
            harassMenu.Add("harass.mana", new Slider("Min. Mana", 50, 1, 99));

            jungleMenu = Menu.AddSubMenu("Jungle Settings", "jungle.settings");
            jungleMenu.Add("jungle.q", new CheckBox("Use (Q)"));
            jungleMenu.Add("jungle.e", new CheckBox("Use (E)"));
            jungleMenu.Add("jungle.mana", new Slider("Min. Mana", 50, 1, 99));


            condemMenu = Menu.AddSubMenu("Condemn Settings", "condemn.settings");
            foreach (var enemy in GameObjects.EnemyHeroes)
            {
                condemMenu.Add("condemn." + enemy.NetworkId, new CheckBox("(Codenmn): " + enemy.ChampionName));
            }


            miscMenu = Menu.AddSubMenu("Miscellaneous", "misc.settings");
            miscMenu.Add("interrupter.e", new CheckBox("Interrupter (E)"));
            miscMenu.AddLabel("Scrying Orb Settings");
            miscMenu.Add("auto.orb.buy", new CheckBox("Auto Orb. Buy"));
            miscMenu.Add("orb.level", new Slider("Orb. Level", 9, 9, 18));

            itemMenu = Menu.AddSubMenu("Activator Settings", "activator.settings");
            itemMenu.Add("use.qss", new CheckBox("Use QSS"));
            itemMenu.AddLabel("Quicksilver Sash Debuffs");
            itemMenu.Add("use.charm", new CheckBox("Charm"));
            itemMenu.Add("use.snare", new CheckBox("Snare"));
            itemMenu.Add("use.polymorph", new CheckBox("Polymorph"));
            itemMenu.Add("use.stun", new CheckBox("Stun"));
            itemMenu.Add("use.suppression", new CheckBox("Suppression"));
            itemMenu.Add("use.taunt", new CheckBox("Taunt"));
            itemMenu.AddLabel("BOTRK Settings");
            itemMenu.Add("use.botrk", new CheckBox("Use BOTRK"));
            itemMenu.Add("botrk.vayne.hp", new Slider("Min. Vayne HP", 20, 1, 99));
            itemMenu.Add("botrk.enemy.hp", new Slider("Min. Enemy", 20, 1, 99));
            itemMenu.AddLabel("Youmuu Settings");
            itemMenu.Add("use.youmuu", new CheckBox("Use Youmuu"));

            PushDistance = getSliderItem(hvayneModes, "condemn.push.distance");
            CondemnMethod = getBoxItem(hvayneModes, "condemn.style");
            MethodQ = getBoxItem(hvayneModes, "q.style");
            ComboMethod = getBoxItem(hvayneModes, "combo.type");
            HarassMethod = getBoxItem(hvayneModes, "harass.type");

        }
    }
}