#region

using EloBuddy.SDK.Menu;
using EloBuddy;
using LeagueSharp.SDK.Core.UI;
using EloBuddy.SDK.Menu.Values;

#endregion

namespace Spirit_Karma.Menus
{
    internal class MenuConfig
    {
        public static Menu menu, comboMenu, harassMenu, laneclearMenu, itemMenu, drawMenu, trinketMenu;


        public static void Load()
        {
            menu = MainMenu.AddMenu("Spirit Karma", "karma");

            comboMenu = menu.AddSubMenu("Combo", "ComboMenu");
            comboMenu.Add("MantraMode", new ComboBox("R Priority", 0, "Q", "W", "E", "Auto"));
            comboMenu.AddSeparator();
            comboMenu.Add("Mantra", new KeyBind("Change Prio Keybind", false, KeyBind.BindTypes.HoldActive, 'G'));

            harassMenu = menu.AddSubMenu("Harass", "HarassMenu");
            harassMenu.Add("HarassR", new CheckBox("Use R"));
            harassMenu.Add("HarassQ1", new CheckBox("Use Q"));
            harassMenu.Add("HarassW1", new CheckBox("Use W", false));
            harassMenu.Add("HarassE1", new CheckBox("Use E", false));
            harassMenu.Add("HarassQ", new Slider("Q Mana", 70, 0, 100));
            harassMenu.Add("HarassW", new Slider("W Mana", 70, 0, 100));
            harassMenu.Add("HarassE", new Slider("E Mana", 70, 0, 100));

            laneclearMenu = menu.AddSubMenu("Lane", "LaneMenu");
            laneclearMenu.Add("LaneR", new CheckBox("Use R", false));
            laneclearMenu.Add("LaneQ1", new CheckBox("Use Q"));
            laneclearMenu.Add("LaneE1", new CheckBox("Use E", false));
            laneclearMenu.Add("LaneQ", new Slider("Q Mana", 70, 0, 100));
            laneclearMenu.Add("LaneE", new Slider("E Mana", 70, 0, 100));

            itemMenu = menu.AddSubMenu("Items", "ItemsMenu");
            itemMenu.Add("UseItems", new CheckBox("Use Items"));
            itemMenu.Add("ItemLocket", new CheckBox("Locket of the Iron Solari"));
            itemMenu.Add("ItemProtoBelt", new CheckBox("ProtoBelt"));
            itemMenu.Add("ItemFrostQueen", new CheckBox("Frost Queen's Claim"));

            drawMenu = menu.AddSubMenu("Draw", "DrawMenu");
            drawMenu.Add("UseDrawings", new CheckBox("Enable Drawings"));
            drawMenu.Add("Dind", new CheckBox("Damage Indicator (Fps Heavy)"));
            drawMenu.Add("QRange", new CheckBox("Engage Range (Q)"));
            drawMenu.Add("MantraDraw", new CheckBox("Draw Selected Prio"));

            trinketMenu = menu.AddSubMenu("Trinket", "TrinketMenu");
            trinketMenu.Add("Trinket", new CheckBox("Auto Buy Advanced Trinket"));
            trinketMenu.Add("TrinketList", new ComboBox("Choose Trinket", 0, "Oracle Alternation", "Farsight Alternation"));


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
    }
}
