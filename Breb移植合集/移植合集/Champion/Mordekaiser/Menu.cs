using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

namespace Mordekaiser
{
    internal class Menu
    {
        public static EloBuddy.SDK.Menu.Menu Config = Program.Config;

        public static EloBuddy.SDK.Menu.Menu MenuQ;

        public static EloBuddy.SDK.Menu.Menu MenuW;

        public static EloBuddy.SDK.Menu.Menu MenuE;

        public static EloBuddy.SDK.Menu.Menu MenuR;

        public static EloBuddy.SDK.Menu.Menu MenuGhost;

        public static EloBuddy.SDK.Menu.Menu MenuItems;

        public static EloBuddy.SDK.Menu.Menu MenuDrawings;

        public Menu()
        {
            // Q
            MenuQ = Config.AddSubMenu("Q", "Q");
            MenuQ.Add("UseQ.Combo", new CheckBox("Combo"));
            MenuQ.Add("UseQ.Lane", new ComboBox("Lane Clear", 1, "Off", "On", "Only Siege/Super Minion"));
            MenuQ.Add("UseQ.Jungle", new ComboBox("Jungle Clear", 1, "Off", "On", "Only Big Mobs"));
            MenuQ.AddSeparator();
            MenuQ.AddGroupLabel("Min. Heal Settings:");
            MenuQ.Add("UseQ.Lane.MinHeal", new Slider("Lane Clear:", 30));
            MenuQ.Add("UseQ.Jungle.MinHeal", new Slider("Jungle Clear:", 30));

            // W
            MenuW = Config.AddSubMenu("W", "W");
            MenuW.Add("UseW.DamageRadius", new Slider("W Damage Radius Range (Default = 350):", 350, 250, 400));
            MenuW.AddSeparator();
            MenuW.Add("Allies.Active", new CheckBox("Combo"));
            MenuW.Add("Selected" + Utils.Player.Self.ChampionName,
                new ComboBox(Utils.Player.Self.ChampionName + " (Yourself)",
                    Utils.TargetSelector.Ally.GetPriority(Utils.Player.Self.ChampionName), "Don't", "Combo", "Everytime"));
            MenuW.Add("SelectedGhost",
                new ComboBox("Dragon / Ghost Enemy", Utils.TargetSelector.Ally.GetPriority("Dragon"), "Don't", "Combo",
                    "Everytime"));
            foreach (var ally in HeroManager.Allies.Where(a => !a.IsMe))
            {
                MenuW.Add("Selected" + ally.ChampionName,
                    new ComboBox(ally.CharData.BaseSkinName, Utils.TargetSelector.Ally.GetPriority(ally.ChampionName),
                        "Don't", "Combo", "Everytime"));
            }
            MenuW.AddSeparator();
            MenuW.AddGroupLabel("Lane / Jungle Settings:");
            MenuW.Add("UseW.Lane",
                new Slider("Lane Clear : (0 : Off | 1-6 : # of minions | 7 : Auto (Recommended))", 7, 0, 7));
            MenuW.Add("UseW.Jungle", new CheckBox("JungleClear"));
            MenuW.AddSeparator();
            MenuW.AddGroupLabel("Drawings");
            MenuW.Add("DrawW.Search", new CheckBox("W Range")); //.SetValue(new Circle(true, Color.Aqua)));
            MenuW.Add("DrawW.DamageRadius", new CheckBox("W Damage Radius"));
                //.SetValue(new Circle(true, Color.Coral)));

            // E
            MenuE = Config.AddSubMenu("E", "E");
            MenuE.Add("UseE.Combo", new CheckBox("Combo"));
            MenuE.Add("UseE.Harass", new CheckBox("Harass"));
            MenuE.Add("UseE.Lane", new CheckBox("Lane Clear"));
            MenuE.Add("UseE.Jungle", new CheckBox("Jungle Clear"));
            MenuE.AddSeparator();
            MenuE.AddGroupLabel("Toggle Settings:");
            MenuE.Add("UseE.Toggle", new KeyBind("E Toggle:", false, KeyBind.BindTypes.PressToggle, 'T'));
            MenuE.AddSeparator();
            MenuE.AddGroupLabel("Min. Heal Settings:");
            MenuE.Add("UseE.Harass.MinHeal", new Slider("Harass:", 30));
            MenuE.Add("UseE.Lane.MinHeal", new Slider("Lane Clear:", 30));
            MenuE.Add("UseE.Jungle.MinHeal", new Slider("Jungle Clear:", 30));
            MenuE.AddSeparator();
            MenuE.AddGroupLabel("Drawings");
            MenuE.Add("DrawE.Search", new CheckBox("E Range")); //.SetValue(new Circle(true, Color.Aqua)));

            // R
            MenuR = Config.AddSubMenu("R", "R");
            MenuR.Add("UseR.Active", new CheckBox("Use R"));
            foreach (var enemy in HeroManager.Enemies)
            {
                MenuR.Add("Selected" + enemy.ChampionName,
                    new ComboBox(enemy.ChampionName, Utils.TargetSelector.Enemy.GetPriority(enemy.ChampionName),
                        "Don't Use", "Low", "Medium", "High"));
                    //.SetValue(new StringList(new[] { "Don't Use", "Low", "Medium", "High" }, Utils.TargetSelector.Enemy.GetPriority(enemy.ChampionName))));
            }

            MenuR.AddSeparator();
            MenuR.AddGroupLabel("Drawings");
            MenuR.Add("DrawR.Search", new CheckBox("R Skill Range")); //.SetValue(new Circle(true, Color.GreenYellow)));
            MenuR.Add("DrawR.Status.Show", new ComboBox("Targeting Notification:", 0, "Off", "On", "Only High Target"));

            //ghost
            MenuGhost = Config.AddSubMenu("Ghost");
            MenuGhost.AddGroupLabel("What do you want with the Ghost?");
            MenuGhost.Add("Ghost.Use", new ComboBox("Do this:", 1, "Nothing", "Fight w/ Me", "Attack High Prio Targs)"));
            MenuGhost.AddSeparator();
            MenuGhost.AddGroupLabel("Drawings");
            MenuGhost.Add("Ghost.Draw.Position", new CheckBox("Ghost Position"));
                //.SetValue(new Circle(true, Color.DarkRed)));
            MenuGhost.Add("Ghost.Draw.AARange", new CheckBox("Ghost AA Range"));
                //.SetValue(new Circle(true, Color.DarkRed)));
            MenuGhost.Add("Ghost.Draw.ControlRange", new CheckBox("Ghost Control Range"));
                //.SetValue(new Circle(true, Color.WhiteSmoke)));

            //items
            MenuItems = Config.AddSubMenu("Items");
            MenuItems.AddGroupLabel("Use Items on This Mode:");
            MenuItems.Add("Items.Combo", new CheckBox("Combo"));
            MenuItems.Add("Items.Lane", new CheckBox("Lane Clear"));
            MenuItems.Add("Items.Jungle", new CheckBox("Jungle Clear"));

            //draws
            MenuDrawings = Config.AddSubMenu("Other Drawings", "Drawings");
            /* [ Damage After Combo ] */
            MenuDrawings.Add("Draw.Calc.Q", new CheckBox("Q Damage"));
            MenuDrawings.Add("Draw.Calc.W", new CheckBox("W Damage"));
            MenuDrawings.Add("Draw.Calc.E", new CheckBox("E Damage"));
            MenuDrawings.Add("Draw.Calc.R", new CheckBox("R Damage"));
            MenuDrawings.Add("Draw.Calc.I", new CheckBox("Ignite Damage"));
                //.SetFontStyle(FontStyle.Regular, SharpDX.Color.Aqua));
            MenuDrawings.Add("Draw.Calc.T", new CheckBox("Item Damage"));
                //.SetFontStyle(FontStyle.Regular, SharpDX.Color.Aqua));
            if (PlayerSpells.SmiteSlot != SpellSlot.Unknown)
            {
                MenuDrawings.Add("Calc.S", new CheckBox("Smite Damage"));
                    //.SetFontStyle(FontStyle.Regular, SharpDX.Color.Aqua));
            }
        }

        public static bool getCheckBoxItem(EloBuddy.SDK.Menu.Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(EloBuddy.SDK.Menu.Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(EloBuddy.SDK.Menu.Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(EloBuddy.SDK.Menu.Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }
    }
}