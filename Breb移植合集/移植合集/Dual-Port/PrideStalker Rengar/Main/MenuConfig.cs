using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;


namespace PrideStalker_Rengar.Main
{
    public class MenuConfig
    {
        private static Menu Menu { get; set; }
        public static Menu comboMenu, miscMenu, drawMenu;

        public static void Load()
        {

            Menu = MainMenu.AddMenu("PrideStalker Rengar", "pridestalker");

            comboMenu = Menu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("ComboMode", new ComboBox("Combo Mode", 0, "Gank", "Triple Q", "Ap Combo", "OneShot"));
            comboMenu.Add("IgnoreE", new CheckBox("E To Cursor In TripleQ/Ap Combo/OneShot", false));
            comboMenu.AddLabel("In TripleQ/Ap Combo/OneShot");
            comboMenu.AddSeparator();
            comboMenu.Add("TripleQAAReset", new CheckBox("AA Q Reset In Triple Q", true));

            miscMenu = Menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("KillStealSummoner", new CheckBox("KillSteal Summoner Q", true));
            miscMenu.Add("UseItem", new CheckBox("Use Items", true));
            miscMenu.Add("StackLastHit", new CheckBox("Stack In Lasthit", true));
            miscMenu.Add("ChangeComboMode", new KeyBind("Change ComboMode", false, KeyBind.BindTypes.HoldActive, 'L'));
            miscMenu.Add("Passive", new KeyBind("Passive", false, KeyBind.BindTypes.PressToggle, 'G'));

            drawMenu = Menu.AddSubMenu("Draw", "Draw");
            drawMenu.Add("DrawCombo", new CheckBox("Draw ComboMode", true));
            drawMenu.Add("DrawAnim", new CheckBox("Draw Animation", true));
            drawMenu.Add("DrawHelp", new CheckBox("Draw Tips", true));
            drawMenu.Add("dind", new CheckBox("Damage Indicator", true));
            drawMenu.Add("EngageDraw", new CheckBox("Draw Engage", true));

        }

        public static int ComboMode { get { return miscMenu["ComboMode"].Cast<ComboBox>().CurrentValue; } }
        public static bool Passive { get { return miscMenu["Passive"].Cast<KeyBind>().CurrentValue; } }
        public static bool ChangeComboMode { get { return miscMenu["ChangeComboMode"].Cast<KeyBind>().CurrentValue; } }
        public static bool TripleQAAReset { get { return comboMenu["TripleQAAReset"].Cast<CheckBox>().CurrentValue; } }
        public static bool StackLastHit { get { return miscMenu["StackLastHit"].Cast<CheckBox>().CurrentValue; } }
        public static bool IgnoreE { get { return comboMenu["IgnoreE"].Cast<CheckBox>().CurrentValue; } }
        public static bool KillStealSummoner { get { return miscMenu["KillStealSummoner"].Cast<CheckBox>().CurrentValue; } }
        public static bool UseItem { get { return miscMenu["UseItem"].Cast<CheckBox>().CurrentValue; } }
        public static bool DrawCombo { get { return drawMenu["DrawCombo"].Cast<CheckBox>().CurrentValue; } }
        public static bool dind { get { return drawMenu["dind"].Cast<CheckBox>().CurrentValue; } }
        public static bool DrawAnim { get { return drawMenu["DrawAnim"].Cast<CheckBox>().CurrentValue; } }
        public static bool DrawHelp { get { return drawMenu["DrawHelp"].Cast<CheckBox>().CurrentValue; } }
        public static bool EngageDraw { get { return drawMenu["EngageDraw"].Cast<CheckBox>().CurrentValue; } }

    }
}
