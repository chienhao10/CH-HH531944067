using System.Drawing;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Slutty_ryze
{
    internal class MenuManager
    {
        #region Variable Declaration
        public const string Menuname = "Slutty Ryze";

        public static Menu _config;
        public static Menu passiveMenu, itemMenu, hpMenu, eventMenu, ksMenu, chase, lastMenu, jungleMenu, laneMenu, mixedMenu, combo1Menu, drawMenu, humanizerMenu;
        #endregion
        #region Public Functions
        public static void GetMenu()
        {
            _config = MainMenu.AddMenu(Menuname, Menuname);

            HumanizerMenu();
            DrawingMenu();
            ComboMenu();
            MixedMenu();
            FarmMenu();
            MiscMenu();
            _config.Add("test", new KeyBind("Level 3-5 Oriented Combo", false, KeyBind.BindTypes.HoldActive, 'Z'));
        }
        #endregion
        #region Private Functions

        private static void HumanizerMenu()
        {
            humanizerMenu = _config.AddSubMenu("Humanizer", "Humanizer");

            humanizerMenu.Add("minDelay", new Slider("Minimum Delay for Actions (ms)", 0, 0, 200));
            humanizerMenu.Add("maxDelay", new Slider("Maximum Delay for Actions (ms)", 0, 0, 250));
            humanizerMenu.Add("minCreepHPOffset", new Slider("Minimum HP for a Minion to Have Before CSing Damage >= HP+(%)", 5, 0, 25));
            humanizerMenu.Add("maxCreepHPOffset", new Slider("Maximum HP for a Minion to Have Before CSing Damage >= HP+(%)", 15, 0, 25));
            humanizerMenu.Add("doHuman", new CheckBox("Humanize", false));
        }

        private static void DrawingMenu()
        {
            drawMenu = _config.AddSubMenu("Drawing Settings", "Drawings");
            drawMenu.Add("drawoptions", new ComboBox("Drawing Mode Mode", 0, "Normal Mode", "Colorblind Mode"));
            drawMenu.Add("Draw", new CheckBox("Display Drawings"));
            drawMenu.Add("qDraw", new CheckBox("Draw Q"));
            drawMenu.Add("eDraw", new CheckBox("Draw E"));
            drawMenu.Add("wDraw", new CheckBox("Draw W"));
            drawMenu.Add("stackDraw", new CheckBox("Stack Count"));
            drawMenu.Add("notdraw", new CheckBox("Draw Floating Text"));
            drawMenu.Add("keyBindDisplay", new CheckBox("Display Keybinds"));  
        }

        private static void ComboMenu()
        {
            combo1Menu = _config.AddSubMenu("Combo Settings", "combospells");
            {
                combo1Menu.Add("combooptions", new ComboBox("Combo Mode", 0, "Improved Combo", "New Test Combo"));
                combo1Menu.Add("useQ", new CheckBox("Use Q (Overload)"));
                combo1Menu.Add("useW", new CheckBox("Use W (Rune Prison)"));
                combo1Menu.Add("useE", new CheckBox("Use E (Spell Flux)"));
                combo1Menu.Add("useR", new CheckBox("Use R (Desperate Power)"));
                combo1Menu.Add("useRww", new CheckBox("Only Use R if Target is Rooted"));
                combo1Menu.Add("AAblock", new CheckBox("Block Auto Attack in Combo", false));
                combo1Menu.Add("minaarange", new Slider("Disable AA If Target Distance from target >", 550, 100, 550));
            }
        }

        private static void MixedMenu()
        {
            mixedMenu = _config.AddSubMenu("Mixed Settings", "mixedsettings");
            {
                mixedMenu.Add("mMin", new Slider("Min. Mana for Spells", 40));
                mixedMenu.Add("UseQM", new CheckBox("Use Q"));
                mixedMenu.Add("UseQMl", new CheckBox("Use Q to Last Hit Minions"));
                mixedMenu.Add("UseEM", new CheckBox("Use E", false));
                mixedMenu.Add("UseWM", new CheckBox("Use W", false));
                mixedMenu.Add("UseQauto", new CheckBox("Auto Use Q", false));
            }
        }

        private static void FarmMenu()
        {
            laneMenu = _config.AddSubMenu("Lane Clear", "lanesettings");
            {
                laneMenu.Add("disablelane", new KeyBind("Lane Clear Toggle", false, KeyBind.BindTypes.PressToggle, 'T'));
                laneMenu.Add("useEPL", new Slider("Min. % Mana For Lane Clear", 50));
                laneMenu.Add("passiveproc", new CheckBox("Don't Use Spells if Passive Will Proc"));
                laneMenu.Add("useQlc", new CheckBox("Use Q to Last Hit"));
                laneMenu.Add("useWlc", new CheckBox("Use W to Last Hit", false));
                laneMenu.Add("useElc", new CheckBox("Use E to Last Hit", false));
                laneMenu.Add("useQ2L", new CheckBox("Use Q to Lane Clear"));
                laneMenu.Add("useW2L", new CheckBox("Use W to Lane Clear", false));
                laneMenu.Add("useE2L", new CheckBox("Use E to Lane Clear", false));
                laneMenu.Add("useRl", new CheckBox("Use R to Lane Clear", false));
                laneMenu.Add("rMin", new Slider("Min. Minions to Use R", 3, 1, 20));
            }

            jungleMenu = _config.AddSubMenu("Jungle Settings", "junglesettings");
            {
                jungleMenu.Add("useJM", new Slider("Min. % Mana for Jungle Clear", 50));
                jungleMenu.Add("useQj", new CheckBox("Use Q"));
                jungleMenu.Add("useWj", new CheckBox("Use W"));
                jungleMenu.Add("useEj", new CheckBox("Use E"));
                jungleMenu.Add("useRj", new CheckBox("Use R"));
            }

            lastMenu = _config.AddSubMenu("Last Hit Settings", "lastsettings");
            {
                lastMenu.Add("useQl2h", new CheckBox("Use Q to Last Hit"));
                lastMenu.Add("useWl2h", new CheckBox("Use W to Last Hit", false));
                lastMenu.Add("useEl2h", new CheckBox("Use E to Last Hit", false));
            }
        }

        private static void MiscMenu()
        {
            passiveMenu = _config.AddSubMenu("Auto Passive", "passivesettings");
            {
                passiveMenu.Add("ManapSlider", new Slider("Min. % Mana", 30));
                passiveMenu.Add("autoPassive", new KeyBind("Stack Passive", false, KeyBind.BindTypes.PressToggle, 'Z'));
                passiveMenu.Add("stackSlider", new Slider("Keep Passive Count At", 3, 1, 4));
                passiveMenu.Add("autoPassiveTimer", new Slider("Refresh Passive Every (s)", 5, 1, 10));
            }

            itemMenu = _config.AddSubMenu("Items", "itemsettings");
            {
                itemMenu.Add("tearS", new KeyBind("Auto Stack Tear", false, KeyBind.BindTypes.PressToggle, 'G'));
                itemMenu.Add("tearoptions", new CheckBox("Stack Tear Only at Fountain", false));
                itemMenu.Add("tearSM", new Slider("Min % Mana to Stack Tear", 95));
                itemMenu.Add("staff", new CheckBox("Use Seraph's Embrace"));
                itemMenu.Add("staffhp", new Slider("Seraph's When % HP <", 30));
                itemMenu.Add("muramana", new CheckBox("Use Muramana"));
            }

            hpMenu = _config.AddSubMenu("Auto Potions", "hpsettings");
            {
                hpMenu.Add("autoPO", new CheckBox("Enable Consumable Usage"));
                hpMenu.Add("HP", new CheckBox("Auto Health Potions"));
                hpMenu.Add("HPSlider", new Slider("Min. % Health for Potion", 30));
                hpMenu.Add("MANA", new CheckBox("Auto Mana Potion"));
                hpMenu.Add("MANASlider", new Slider("Min. % Mana for Potion", 30));
                hpMenu.Add("Biscuit", new CheckBox("Auto Biscuit"));
                hpMenu.Add("bSlider", new Slider("Min. % Health for Biscuit", 30));
                hpMenu.Add("flask", new CheckBox("Auto Flask"));
                hpMenu.Add("fSlider", new Slider("Min. % Health for Flask", 30));
            }

            eventMenu = _config.AddSubMenu("Events", "eventssettings");
            {
                eventMenu.Add("useW2I", new CheckBox("Interrupt with W"));
                eventMenu.Add("useQW2D", new CheckBox("W/Q on Dashing"));
                eventMenu.Add("level", new CheckBox("Auto Level-Up"));
                eventMenu.Add("autow", new CheckBox("Auto W Enemy Under Turret"));
            }

            ksMenu = _config.AddSubMenu("Kill Steal", "kssettings");
            {
                ksMenu.Add("KS", new CheckBox("Killsteal"));
                ksMenu.Add("useQ2KS", new CheckBox("Use Q to KS"));
                ksMenu.Add("useW2KS", new CheckBox("Use W to KS"));
                ksMenu.Add("useE2KS", new CheckBox("Use E to KS"));
            }

            chase = _config.AddSubMenu("Chase Target", "Chase Target");
            {
                chase.Add("chase", new KeyBind("Activate Chase", false, KeyBind.BindTypes.HoldActive, 'A'));
                chase.Add("usewchase", new CheckBox("Use W"));
                chase.Add("chaser", new CheckBox("Use [R]", false));
            }
        }
        #endregion
    }

}
