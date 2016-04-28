using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

namespace YasuoPro
{
    internal static class YasuoMenu
    {
        internal static Menu Config;
        internal static Yasuo Yas;

        public static Menu ComboA, HarassA, KillstealA, FarmingA, WaveclearA, MiscA, DrawingsA, Flee;

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

        public static void Init(Yasuo yas)
        {
            Yas = yas;

            Config = MainMenu.AddMenu("YasuoPro", "YasuoPro");

            ComboA = Config.AddSubMenu("Combo");
            Combo.Attach(ComboA);

            HarassA = Config.AddSubMenu("Harass");
            Harass.Attach(HarassA);

            KillstealA = Config.AddSubMenu("Killsteal");
            Killsteal.Attach(KillstealA);

            FarmingA = Config.AddSubMenu("LastHitting");
            Farm.Attach(FarmingA);

            WaveclearA = Config.AddSubMenu("Waveclear");
            Waveclear.Attach(WaveclearA);

            MiscA = Config.AddSubMenu("Misc");
            Misc.Attach(MiscA);

            DrawingsA = Config.AddSubMenu("Drawings");
            Drawings.Attach(DrawingsA);

            Flee = Config.AddSubMenu("Flee Settings", "Flee");
            Flee.Add("Flee.Mode", new ComboBox("Flee Mode", 0, "To Nexus", "To Allies", "To Cursor"));
            Flee.Add("Flee.StackQ", new CheckBox("Stack Q during Flee"));
            Flee.Add("Flee.UseQ2", new CheckBox("Use Tornado", false));
        }

        internal static uint KeyCode(string s)
        {
            return s.ToCharArray()[0];
        }

        private struct Combo
        {
            internal static void Attach(Menu menu)
            {
                menu.AddGroupLabel("Items");
                menu.Add("Items.Enabled", new CheckBox("Use Items"));
                menu.Add("Items.UseTIA", new CheckBox("Use Tiamat"));
                menu.Add("Items.UseHDR", new CheckBox("Use Hydra"));
                menu.Add("Items.UseBRK", new CheckBox("Use BORK"));
                menu.Add("Items.UseBLG", new CheckBox("Use Bilgewater"));
                menu.Add("Items.UseYMU", new CheckBox("Use Youmu"));
                menu.AddSeparator();

                menu.AddGroupLabel("Combo");
                menu.Add("Combo.UseQ", new CheckBox("Use Q"));
                menu.Add("Combo.UseQ2", new CheckBox("Use Q2"));
                menu.Add("Combo.StackQ", new CheckBox("Stack Q if not in Range"));
                menu.Add("Combo.UseW", new CheckBox("Use W"));
                menu.Add("Combo.UseE", new CheckBox("Use E"));
                menu.Add("Combo.ETower", new CheckBox("Use E under Tower", false));
                menu.Add("Combo.EAdvanced", new CheckBox("Predict E position with Waypoints"));
                menu.Add("Combo.NoQ2Dash", new CheckBox("Dont Q2 while dashing", false));
                menu.AddSeparator();

                menu.AddGroupLabel("Ult Settings");
                foreach (var hero in HeroManager.Enemies)
                {
                    menu.Add("ult" + hero.ChampionName, new CheckBox("Ult " + hero.ChampionName));
                }
                menu.Add("Combo.UltMode",
                    new ComboBox("Ult Prioritization", 0, "Lowest Health", "TS Priority", "Most enemies"));
                menu.Add("Combo.knockupremainingpct", new Slider("Knockup Remaining % for Ult", 95, 40));
                menu.Add("Combo.UseR", new CheckBox("Use R"));
                menu.Add("Combo.UltTower", new CheckBox("Ult under Tower", false));
                menu.Add("Combo.UltOnlyKillable", new CheckBox("Ult only Killable", false));
                menu.Add("Combo.RPriority", new CheckBox("Ult if priority 5 target is knocked up"));
                menu.Add("Combo.RMinHit", new Slider("Min Enemies for Ult", 1, 1, 5));
                menu.Add("Combo.OnlyifMin", new CheckBox("Only Ult if minimum enemies met", false));
                menu.Add("Combo.MinHealthUlt", new Slider("Minimum health to Ult %"));
                menu.AddSeparator();

                menu.Add("Combo.UseIgnite", new CheckBox("Use Ignite"));
            }
        }

        private struct Harass
        {
            internal static void Attach(Menu menu)
            {
                menu.Add("Harass.KB", new KeyBind("Harass Key", false, KeyBind.BindTypes.PressToggle, 'H'));
                menu.Add("Harass.InMixed", new CheckBox("Harass in Mixed Mode", false));
                menu.Add("Harass.UseQ", new CheckBox("Use Q"));
                menu.Add("Harass.UseQ2", new CheckBox("Use Q2"));
                menu.Add("Harass.UseE", new CheckBox("Use E", false));
                menu.Add("Harass.UseEMinion", new CheckBox("Use E Minions", false));
            }
        }

        private struct Farm
        {
            internal static void Attach(Menu menu)
            {
                menu.Add("Farm.UseQ", new CheckBox("Use Q"));
                menu.Add("Farm.UseQ2", new CheckBox("Use Q - Tornado"));
                menu.Add("Farm.Qcount", new Slider("Minions for Q (Tornado)", 1, 1, 10));
                menu.Add("Farm.UseE", new CheckBox("Use E"));
            }
        }


        private struct Waveclear
        {
            internal static void Attach(Menu menu)
            {
                menu.AddGroupLabel("Items");
                menu.Add("Waveclear.UseItems", new CheckBox("Use Items"));
                menu.Add("Waveclear.MinCountHDR", new Slider("Minion count for Cleave", 2, 1, 10));
                menu.Add("Waveclear.MinCountYOU", new Slider("Minion count for Youmu", 2, 1, 10));
                menu.Add("Waveclear.UseTIA", new CheckBox("Use Tiamat"));
                menu.Add("Waveclear.UseHDR", new CheckBox("Use Hydra"));
                menu.Add("Waveclear.UseYMU", new CheckBox("Use Youmu", false));
                menu.AddSeparator();

                menu.AddGroupLabel("Wave Clear");
                menu.Add("Waveclear.UseQ", new CheckBox("Use Q"));
                menu.Add("Waveclear.UseQ2", new CheckBox("Use Q - Tornado"));
                menu.Add("Waveclear.Qcount", new Slider("Minions for Q (Tornado)", 1, 1, 10));
                menu.Add("Waveclear.UseE", new CheckBox("Use E"));
                menu.Add("Waveclear.ETower", new CheckBox("Use E under Tower", false));
                menu.Add("Waveclear.UseENK", new CheckBox("Use E even if not killable"));
                menu.Add("Waveclear.Smart", new CheckBox("Smart Waveclear"));
            }
        }

        private struct Killsteal
        {
            internal static void Attach(Menu menu)
            {
                menu.Add("Killsteal.Enabled", new CheckBox("KillSteal"));
                menu.Add("Killsteal.UseQ", new CheckBox("Use Q"));
                menu.Add("Killsteal.UseE", new CheckBox("Use E"));
                menu.Add("Killsteal.UseR", new CheckBox("Use R"));
                menu.Add("Killsteal.UseIgnite", new CheckBox("Use Ignite"));
                menu.Add("Killsteal.UseItems", new CheckBox("Use Items"));
            }
        }

        private struct Misc
        {
            internal static void Attach(Menu menu)
            {
                menu.Add("Misc.SafeE", new CheckBox("Safety Check for E"));
                menu.Add("Misc.AutoStackQ", new CheckBox("Auto Stack Q", false));
                menu.Add("Misc.AutoR", new CheckBox("Auto Ultimate"));
                menu.Add("Misc.RMinHit", new Slider("Min Enemies for Autoult", 1, 1, 5));
                menu.Add("Misc.TowerDive", new KeyBind("Tower Dive Key", false, KeyBind.BindTypes.HoldActive, 'T'));
                menu.Add("Hitchance.Q",
                    new ComboBox("Q Hitchance", 2, HitChance.Low.ToString(), HitChance.Medium.ToString(),
                        HitChance.High.ToString(), HitChance.VeryHigh.ToString()));
                menu.Add("Misc.Healthy", new Slider("Healthy Amount HP", 5));
                menu.Add("Misc.AG", new CheckBox("Use Q (Tornado) on Gapcloser"));
                menu.Add("Misc.Interrupter", new CheckBox("Use Q (Tornado) to Interrupt"));
                menu.Add("Misc.Walljump", new CheckBox("Use Walljump", false));
                menu.Add("Misc.Debug", new CheckBox("Debug", false));
            }
        }

        private struct Drawings
        {
            internal static void Attach(Menu menu)
            {
                menu.Add("Drawing.Disable", new CheckBox("Disable Drawings"));
                menu.Add("Drawing.DrawQ", new CheckBox("Draw Q")); // Yas.Qrange, System.Drawing.Color.Red);
                menu.Add("Drawing.DrawE", new CheckBox("Draw E")); // Yas.Erange, System.Drawing.Color.CornflowerBlue);
                menu.Add("Drawing.DrawR", new CheckBox("Draw R")); // Yas.Rrange, System.Drawing.Color.DarkOrange);
                menu.Add("Drawing.SS", new CheckBox("Draw Skillshot Drawings", false));
            }
        }
    }
}