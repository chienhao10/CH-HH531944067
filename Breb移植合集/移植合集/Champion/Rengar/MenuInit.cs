using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace ElRengarRevamped
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MenuInit
    {
        #region Static Fields

        public static Menu Menu;

        #endregion

        #region Public Methods and Operators

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

        public static Menu comboMenu, harassMenu, laneClear, jungleClear, healMenu, ksMenu, betaMenu, miscMenu;

        public static void Initialize()
        {
            Menu = MainMenu.AddMenu("ElRengar", "ElRengar");

            comboMenu = Menu.AddSubMenu("Combo", "Modes");
            comboMenu.Add("Combo.Use.Ignite", new CheckBox("Use Ignite"));
            comboMenu.Add("Combo.Use.Q", new CheckBox("Use Q"));
            comboMenu.Add("Combo.Use.W", new CheckBox("Use W"));
            comboMenu.Add("Combo.Use.E", new CheckBox("Use E"));
            comboMenu.Add("Combo.Switch.E", new CheckBox("Switch E prio to Q after E cast"));
            comboMenu.Add("Combo.Prio", new ComboBox("Prioritize", 2, "E", "W", "Q"));
            comboMenu.Add("Combo.Switch", new KeyBind("Switch priority", false, KeyBind.BindTypes.HoldActive, 'L'));
            comboMenu.Add("Combo.Use.QQ", new CheckBox("5 ferocity Q reset"));

            harassMenu = Menu.AddSubMenu("Harass");
            harassMenu.Add("Harass.Use.Q", new CheckBox("Use Q"));
            harassMenu.Add("Harass.Use.W", new CheckBox("Use W"));
            harassMenu.Add("Harass.Use.E", new CheckBox("Use E"));
            harassMenu.Add("Harass.Prio", new ComboBox("Prioritize", 1, "E", "Q"));

            laneClear = Menu.AddSubMenu("Lane Clear", "asd");
            laneClear.Add("Clear.Use.Q", new CheckBox("Use Q"));
            laneClear.Add("Clear.Use.W", new CheckBox("Use W"));
            laneClear.Add("Clear.Use.E", new CheckBox("Use E"));
            laneClear.Add("Clear.Save.Ferocity", new CheckBox("Save ferocity", false));

            jungleClear = Menu.AddSubMenu("Jungle Clear", "asdasdasdasdas");
            jungleClear.Add("Jungle.Use.Q", new CheckBox("Use Q"));
            jungleClear.Add("Jungle.Use.W", new CheckBox("Use W"));
            jungleClear.Add("Jungle.Use.E", new CheckBox("Use E"));
            jungleClear.Add("Jungle.Save.Ferocity", new CheckBox("Save ferocity", false));

            healMenu = Menu.AddSubMenu("Heal", "heal");
            healMenu.Add("Heal.AutoHeal", new CheckBox("Auto heal yourself"));
            healMenu.Add("Heal.HP", new Slider("Self heal at >= ", 25, 1));

            ksMenu = Menu.AddSubMenu("Killsteal", "Killsteal");
            ksMenu.Add("Killsteal.On", new CheckBox("Active"));
            ksMenu.Add("Killsteal.Use.W", new CheckBox("Use W"));

            betaMenu = Menu.AddSubMenu("Beta options", "BetaOptions");
            betaMenu.Add("Beta.Cast.Q1", new CheckBox("Use beta Q"));
            betaMenu.Add("Beta.Cast.Q1.Delay", new Slider("Cast Q delay", 300, 100, 2000));
            betaMenu.Add("Assassin.searchrange", new CheckBox("Assassin search range"));
            betaMenu.Add("Beta.searchrange", new Slider("Search range", 1500, 1000, 2500));
            betaMenu.Add("Beta.searchrange.Q", new Slider("Q cast range", 600, 500, 1500));
            betaMenu.Add("Beta.Search.Range", new CheckBox("Draw search range"));
            betaMenu.Add("Beta.Search.QCastRange", new CheckBox("Draw Q cast range"));

            miscMenu = Menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("Misc.Drawings.Off", new CheckBox("Turn drawings off", false));
            miscMenu.Add("Misc.Drawings.Exclamation", new CheckBox("Draw exclamation mark range"));
            miscMenu.Add("Misc.Drawings.Prioritized", new CheckBox("Draw Prioritized"));
            miscMenu.Add("Misc.Drawings.W", new CheckBox("Draw W"));
            miscMenu.Add("Misc.Drawings.E", new CheckBox("Draw E"));
            miscMenu.Add("Misc.Drawings.Minimap", new CheckBox("Draw R on minimap"));
            miscMenu.Add("Misc.Root", new CheckBox("Auto E on stunned targets1", false));
        }

        #endregion
    }
}