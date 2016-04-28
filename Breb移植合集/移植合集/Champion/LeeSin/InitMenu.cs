using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace ElLeeSin
{
    public class InitMenu
    {
        #region Static Fields

        public static Menu Menu,
            comboMenu,
            harassMenu,
            waveclearMenu,
            kickMenu,
            insecMenu,
            wardjumpMenu,
            miscMenu,
            drawMenu;

        #endregion

        #region Public Methods and Operators

        public static void Initialize()
        {
            Menu = MainMenu.AddMenu("ElLeeSin", "LeeSin");

            comboMenu = Menu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("ElLeeSin.Combo.Q", new CheckBox("Use Q"));
            comboMenu.Add("ElLeeSin.Combo.Q2", new CheckBox("Use Q2"));
            comboMenu.Add("ElLeeSin.Combo.W2", new CheckBox("Use W"));
            comboMenu.Add("ElLeeSin.Combo.E", new CheckBox("Use E"));
            comboMenu.Add("ElLeeSin.Combo.R", new CheckBox("Use R"));
            comboMenu.Add("ElLeeSin.Combo.PassiveStacks", new Slider("Min Stacks", 1, 1, 2));
            comboMenu.Add("ElLeeSin.Combo.W", new CheckBox("Wardjump in combo", false));
            comboMenu.Add("ElLeeSin.Combo.Mode.WW", new CheckBox("Out of AA range", false));
            comboMenu.Add("ElLeeSin.Combo.KS.R", new CheckBox("KS R"));
            comboMenu.Add("starCombo", new KeyBind("Star Combo", false, KeyBind.BindTypes.HoldActive, 'T'));
            comboMenu.Add("ElLeeSin.Combo.AAStacks", new CheckBox("Wait for Passive", false));

            harassMenu = Menu.AddSubMenu("Harass", "Harass");
            harassMenu.Add("ElLeeSin.Harass.Q1", new CheckBox("Use Q"));
            harassMenu.Add("ElLeeSin.Harass.Wardjump", new CheckBox("Use W"));
            harassMenu.Add("ElLeeSin.Harass.E1", new CheckBox("Use E", false));
            harassMenu.Add("ElLeeSin.Harass.PassiveStacks", new Slider("Min Stacks", 1, 1, 2));

            kickMenu = Menu.AddSubMenu("Kick (R)", "Kick");
            kickMenu.Add("ElLeeSin.Combo.New", new CheckBox("Kick multiple targets:"));
            kickMenu.Add("ElLeeSin.Combo.R.Count", new Slider("R target hit count", 3, 2, 4));

            waveclearMenu = Menu.AddSubMenu("Clear", "Clear");
            waveclearMenu.AddGroupLabel("Wave Clear");
            waveclearMenu.Add("ElLeeSin.Lane.Q", new CheckBox("Use Q"));
            waveclearMenu.Add("ElLeeSin.Lane.E", new CheckBox("Use E"));
            waveclearMenu.AddSeparator();
            waveclearMenu.AddGroupLabel("Jungle Clear");

            waveclearMenu.Add("ElLeeSin.Jungle.Q", new CheckBox("Use Q"));
            waveclearMenu.Add("ElLeeSin.Jungle.W", new CheckBox("Use W"));
            waveclearMenu.Add("ElLeeSin.Jungle.E", new CheckBox("Use E"));

            insecMenu = Menu.AddSubMenu("Insec", "Insec");
            insecMenu.Add("InsecEnabled", new KeyBind("Insec key:", false, KeyBind.BindTypes.HoldActive, 'Y'));
            insecMenu.Add("insecMode", new CheckBox("Left click target to Insec"));
            insecMenu.Add("insecOrbwalk", new CheckBox("Orbwalking"));
            insecMenu.Add("ElLeeSin.Flash.Insec", new CheckBox("Flash Insec when no ward", false));
            insecMenu.Add("waitForQBuff", new CheckBox("Wait For Q", false));
            insecMenu.Add("checkOthers1", new CheckBox("Check for units to Insec"));
            insecMenu.Add("clickInsec", new CheckBox("Click Insec"));
            insecMenu.Add("bonusRangeA", new Slider("Ally Bonus Range", 0, 0, 1000));
            insecMenu.Add("bonusRangeT", new Slider("Towers Bonus Range", 0, 0, 1000));
            insecMenu.AddGroupLabel("Insec Mode:");
            insecMenu.Add("ElLeeSin.Insec.Ally", new CheckBox("Insec to allies"));
            insecMenu.Add("ElLeeSin.Insec.Tower", new CheckBox("Insec to tower", false));
            insecMenu.Add("ElLeeSin.Insec.Original.Pos", new CheckBox("Insec to original pos"));
            insecMenu.AddSeparator();
            insecMenu.Add("ElLeeSin.Insec.UseInstaFlash",
                new KeyBind("Flash + R", false, KeyBind.BindTypes.HoldActive, 'G'));

            wardjumpMenu = Menu.AddSubMenu("Wardjump and Escape", "Wardjump");
            wardjumpMenu.Add("ElLeeSin.Escape", new KeyBind("Escape key", false, KeyBind.BindTypes.HoldActive, 'A'));
            wardjumpMenu.Add("escapeMode", new CheckBox("Enable Jungle Escape"));
            wardjumpMenu.Add("ElLeeSin.Wardjump", new KeyBind("Wardjump key", false, KeyBind.BindTypes.HoldActive, 'G'));
            wardjumpMenu.Add("ElLeeSin.Wardjump.MaxRange", new CheckBox("Ward jump on max range", false));
            wardjumpMenu.Add("ElLeeSin.Wardjump.Mouse", new CheckBox("Jump to mouse"));
            wardjumpMenu.Add("ElLeeSin.Wardjump.Minions", new CheckBox("Jump to minions"));
            wardjumpMenu.Add("ElLeeSin.Wardjump.Champions", new CheckBox("Jump to champions"));

            drawMenu = Menu.AddSubMenu("Drawings", "Drawings");
            drawMenu.Add("DrawEnabled", new CheckBox("Draw Enabled"));
            drawMenu.Add("Draw.Insec.Lines", new CheckBox("Draw Insec lines", false));
            drawMenu.Add("ElLeeSin.Draw.Insec.Text", new CheckBox("Draw Insec text", false));
            drawMenu.Add("drawOutLineST", new CheckBox("Draw Outline", false));
            drawMenu.Add("ElLeeSin.Draw.Insec", new CheckBox("Draw Insec", false));
            drawMenu.Add("ElLeeSin.Draw.WJDraw", new CheckBox("Draw WardJump", false));
            drawMenu.Add("ElLeeSin.Draw.Q", new CheckBox("Draw Q", false));
            drawMenu.Add("ElLeeSin.Draw.W", new CheckBox("Draw W", false));
            drawMenu.Add("ElLeeSin.Draw.E", new CheckBox("Draw E", false));
            drawMenu.Add("ElLeeSin.Draw.R", new CheckBox("Draw R", false));
            drawMenu.Add("ElLeeSin.Draw.Escape", new CheckBox("Draw Escape spots", false));
            drawMenu.Add("ElLeeSin.Draw.Q.Width", new CheckBox("Draw Escape Q width", false));

            miscMenu = Menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("ElLeeSin.Ignite.KS", new CheckBox("Use Ignite"));
            miscMenu.Add("ElLeeSin.Smite.KS", new CheckBox("Use Smite"));
            miscMenu.Add("ElLeeSin.Smite.Q", new CheckBox("Smite Q!", false)); //qSmite
        }

        #endregion
    }
}