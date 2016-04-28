using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Slutty_Gnar_Reworked
{
    internal class MenuConfig
    {
        public const string Menuname = "Slutty Gnar";
        public static Menu Config, drawMenu, comboMenu, clearMenu, harassMenu, ksMenu, miscMenu;

        public static void CreateMenu()
        {
            Config = MainMenu.AddMenu(Menuname, Menuname);

            #region Drawings

            drawMenu = Config.AddSubMenu("Drawings", "Drawings");
            drawMenu.Add("Draw", new CheckBox("Display Drawings"));
            drawMenu.Add("qDraw", new CheckBox("Draw Q"));
            drawMenu.Add("wDraw", new CheckBox("Draw W"));
            drawMenu.Add("eDraw", new CheckBox("Draw E"));

            #endregion

            #region Combo Menu

            comboMenu = Config.AddSubMenu("Combo Settings", "combospells");
            comboMenu.AddGroupLabel("Mini Gnar");
            comboMenu.Add("UseQMini", new CheckBox("Use Q"));
            comboMenu.Add("UseQs", new CheckBox("Only Q if Target Has 2 W Stacks"));
            comboMenu.Add("eGap", new CheckBox("Use E to Gapclose When Killable"));
            comboMenu.Add("focust", new CheckBox("Focus Target with 2 W Stacks"));
            comboMenu.AddSeparator();
            comboMenu.AddGroupLabel("Mega Gnar");
            comboMenu.Add("UseQMega", new CheckBox("Use Q"));
            comboMenu.Add("UseEMega", new CheckBox("Use E"));
            comboMenu.Add("UseEMini", new CheckBox("Use E Only When Ready to Transform"));
            comboMenu.Add("UseWMega", new CheckBox("Use W"));
            comboMenu.Add("UseRMega", new CheckBox("Use R"));
            comboMenu.Add("useRSlider", new Slider("Min. Targets to R", 3, 1, 5));

            #endregion

            #region Clear

            clearMenu = Config.AddSubMenu("Clear Settings", "Clear Settings");
            clearMenu.Add("transform", new CheckBox("Use Spells When Ready to Transform"));
            clearMenu.AddSeparator();
            clearMenu.AddGroupLabel("Lane Clear");
            clearMenu.Add("UseQl", new CheckBox("Use Q"));
            clearMenu.Add("UseQlslider", new Slider("Use Q Only if X Minions Hit", 3, 1, 10));
            clearMenu.Add("UseWl", new CheckBox("Use W"));
            clearMenu.Add("UseWlslider", new Slider("Use W Only if X Minions Hit", 3, 1, 10));
            clearMenu.AddSeparator();
            clearMenu.AddGroupLabel("Jungle Clear");
            clearMenu.Add("UseQj", new CheckBox("Use Q"));
            clearMenu.Add("UseWj", new CheckBox("Use W"));

            #endregion

            #region Harass

            harassMenu = Config.AddSubMenu("Harass", "Harras");
            harassMenu.Add("qharras", new CheckBox("Use Q"));
            harassMenu.Add("qharras2", new CheckBox("Only Q if Target Has 2 W Stacks"));
            harassMenu.Add("wharras", new CheckBox("Use R"));
            harassMenu.Add("autoq", new CheckBox("Automatically Use Q", false));

            #endregion

            #region Kill Steal

            ksMenu = Config.AddSubMenu("Killsteal", "Kill Steal");
            ksMenu.Add("qks", new CheckBox("Use Q"));
            ksMenu.Add("rks", new CheckBox("Use R"));
            ksMenu.Add("qeks", new CheckBox("Use E to Gapclose + Q"));

            #endregion

            #region Misc

            miscMenu = Config.AddSubMenu("Misc", "Misc");
            miscMenu.Add("qgap", new CheckBox("Q against Enemy Gapcloser"));
            miscMenu.Add("qwd", new CheckBox("Q/W against Enemy Dash"));
            miscMenu.Add("qwi", new CheckBox("W on Interruptable"));

            #endregion

            #region Flee Key

            miscMenu.Add("fleekey", new KeyBind("Use Flee Mode", false, KeyBind.BindTypes.HoldActive, 65));

            #endregion
        }
    }
}