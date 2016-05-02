namespace KappaKindred
{
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    internal class Menu
    {
        private static EloBuddy.SDK.Menu.Menu menuIni;

        public static EloBuddy.SDK.Menu.Menu ComboMenu { get; private set; }

        public static EloBuddy.SDK.Menu.Menu DrawMenu { get; private set; }

        public static EloBuddy.SDK.Menu.Menu HarassMenu { get; private set; }

        public static EloBuddy.SDK.Menu.Menu JungleMenu { get; private set; }

        public static EloBuddy.SDK.Menu.Menu LaneMenu { get; private set; }

        public static EloBuddy.SDK.Menu.Menu ManaMenu { get; private set; }

        public static EloBuddy.SDK.Menu.Menu FleeMenu { get; private set; }

        public static EloBuddy.SDK.Menu.Menu UltMenu { get; private set; }

        public static void Load()
        {
            menuIni = MainMenu.AddMenu("Kindred", "Kindred");
            menuIni.AddGroupLabel("Welcome to the Worst Kindred addon!");

            UltMenu = menuIni.AddSubMenu("Ultimate");
            UltMenu.AddGroupLabel("Ultimate Settings");
            UltMenu.Add("Rally", new CheckBox("R Save Ally / Self"));
            UltMenu.Add("Rallyh", new Slider("R Ally Health %", 20, 0, 100));
            UltMenu.AddGroupLabel("Don't Use Ult On: ");
            foreach (var ally in ObjectManager.Get<AIHeroClient>())
            {
                CheckBox cb = new CheckBox(ally.BaseSkinName) { CurrentValue = false };
                if (ally.Team == ObjectManager.Player.Team)
                {
                    UltMenu.Add("DontUlt" + ally.BaseSkinName, cb);
                }
            }

            ComboMenu = menuIni.AddSubMenu("Combo");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.Add("Q", new CheckBox("Use Q"));
            ComboMenu.Add("W", new CheckBox("Use W"));
            ComboMenu.Add("E", new CheckBox("Use E"));
            ComboMenu.AddGroupLabel("Extra Settings");
            ComboMenu.Add("Qmode", new ComboBox("Q Mode", 0, "To Target", "To Mouse"));
            ComboMenu.Add("QW", new CheckBox("Only Q When W is active", false));
            ComboMenu.Add("QAA", new CheckBox("Dont Q When target is in AA Range", false));
            ComboMenu.Add("Emark", new CheckBox("Focus target with E mark"));
            ComboMenu.Add("Pmark", new CheckBox("Focus target with Passive mark"));
            ComboMenu.Add("Pspells", new CheckBox("Dont attack targets in ult under 15% hp", false));

            HarassMenu = menuIni.AddSubMenu("Harass");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.Add("Q", new CheckBox("Use Q"));
            HarassMenu.Add("W", new CheckBox("Use W", false));
            HarassMenu.Add("E", new CheckBox("Use E"));

            LaneMenu = menuIni.AddSubMenu("Lane Clear");
            LaneMenu.AddGroupLabel("LaneClear Settings");
            LaneMenu.Add("Q", new CheckBox("Use Q"));
            LaneMenu.Add("W", new CheckBox("Use W", false));
            LaneMenu.Add("E", new CheckBox("Use E", false));

            JungleMenu = menuIni.AddSubMenu("Jungle Clear");
            JungleMenu.AddGroupLabel("JungleClear Settings");
            JungleMenu.Add("Q", new CheckBox("Use Q"));
            JungleMenu.Add("W", new CheckBox("Use W", false));
            JungleMenu.Add("E", new CheckBox("Use E", false));

            FleeMenu = menuIni.AddSubMenu("Flee");
            FleeMenu.AddGroupLabel("Flee Settings");
            FleeMenu.Add("Q", new CheckBox("Use Q"));
            FleeMenu.Add("Qgap", new CheckBox("Use Q On GapClosers"));

            ManaMenu = menuIni.AddSubMenu("Mana Manager");
            ManaMenu.AddGroupLabel("Harass");
            ManaMenu.Add("harassmana", new Slider("Harass Mana %", 75, 0, 100));
            ManaMenu.AddGroupLabel("Lane Clear");
            ManaMenu.Add("lanemana", new Slider("Lane Clear Mana %", 60, 0, 100));

            DrawMenu = menuIni.AddSubMenu("Drawings");
            DrawMenu.AddGroupLabel("Drawing Settings");
            DrawMenu.Add("Q", new CheckBox("Draw Q"));
            DrawMenu.Add("W", new CheckBox("Draw W"));
            DrawMenu.Add("E", new CheckBox("Draw E"));
            DrawMenu.Add("R", new CheckBox("Draw R"));
            DrawMenu.Add("debug", new CheckBox("debug", false));
        }
    }
}