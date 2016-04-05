using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Ex1L_Riven.Base
{
    internal class MenuDesigner
    {
        public const string MenuName = "Ex1L Riven";

        public static Menu RivenUi;
        public static Menu EmoteUi;
        public static Menu ClearUi;
        public static Menu MiscUi;
        public static Menu LevelUi;

        public static void Initialize()
        {
            RivenUi = MainMenu.AddMenu(MenuName, MenuName.ToLower());
            RivenUi.AddGroupLabel("Welcome to Ex1L Riven");
            RivenUi.AddLabel("Brought to you by Enelx");

            EmoteUi = RivenUi.AddSubMenu("Emote");
            EmoteUi.Add("UseEmote", new CheckBox("Use emote to cancel animation"));
            EmoteUi.AddSeparator();
            EmoteUi.Add("EmoteSelect", new ComboBox("Select emote", new[] {"Laugh", "Dance", "Joke", "Taunt"}));

            ClearUi = RivenUi.AddSubMenu("Clear");
            ClearUi.AddGroupLabel("Jungleclear");
            ClearUi.Add("JungleE", new CheckBox("Use E"));
            ClearUi.Add("JungleW", new CheckBox("Use W"));
            ClearUi.AddSeparator();
            ClearUi.AddGroupLabel("Laneclear");
            ClearUi.Add("LaneE", new CheckBox("Use E"));
            ClearUi.Add("LaneW", new CheckBox("Use W"));
            ClearUi.AddSeparator();
            ClearUi.Add("Wminions", new Slider("Select min. W killable minions", 2, 1, 6));

            MiscUi = RivenUi.AddSubMenu("Misc");
            MiscUi.Add("AutoW", new CheckBox("Use automatic W on X enemies"));
            MiscUi.AddSeparator();
            MiscUi.Add("Wenemies", new Slider("Select min. enemies", 2, 1, 5));
            MiscUi.AddSeparator();
            MiscUi.Add("InterW", new CheckBox("Use W on interruptable spell"));
            MiscUi.Add("GapWE", new CheckBox("Use W - E on enemy gapcloser"));
            MiscUi.Add("UseItems", new CheckBox("Use items"));

            LevelUi = RivenUi.AddSubMenu("Auto Level");
            LevelUi.Add("UseLevel", new CheckBox("Upgrade your spells automatic"));
            LevelUi.AddSeparator();
            LevelUi.Add("SequenceSelect", new ComboBox("Select sequence", new[] {"Q > E > W", "Q > W > E", "E > Q > W"}));
            LevelUi.AddSeparator();
            LevelUi.Add("LevelHumanizer", new Slider("Humanize in miliseconds", 700, 1, 1000));
        }
    }
}