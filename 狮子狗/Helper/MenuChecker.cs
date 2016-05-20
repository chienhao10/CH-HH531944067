using EloBuddy.SDK.Menu.Values;

namespace RengarPro_Revamped.Helper
{
    internal class MenuChecker
    {
        public static int ComboModeSelected
        {
            get { return Menu.ComboM["combo.mode"].Cast<Slider>().CurrentValue; }
        }

        public static bool LaneClearUseQ
        {
            get { return Menu.LaneM["lane.useQ"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool LaneClearUseW
        {
            get { return Menu.LaneM["lane.useW"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool LaneClearUseE
        {
            get { return Menu.LaneM["lane.useE"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool LaneClearSaveStacks
        {
            get { return Menu.LaneM["lane.save"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool JungleClearUseQ
        {
            get { return Menu.JungleM["jungle.useQ"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool JungleClearUseW
        {
            get { return Menu.JungleM["jungle.useW"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool JungleClearUseE
        {
            get { return Menu.JungleM["jungle.useE"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool JungleClearSaveStacks
        {
            get { return Menu.JungleM["jungle.save"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool DrawComboModeActive
        {
            get { return Menu.DrawM["draw.mode"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool DrawSelectedEnemyActive
        {
            get { return Menu.DrawM["draw.selectedenemy"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool AutoYoumuuActive
        {
            get { return Menu.MiscM["misc.autoyoumuu"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool MagnetActive
        {
            get { return Menu.MiscM["misc.magnet"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool ComboSmiteActive
        {
            get { return Menu.MiscM["misc.smite"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool AutoLaughActive
        {
            get { return Menu.MiscM["misc.laugh"].Cast<CheckBox>().CurrentValue; }
        }

        public static int AutoLaughDelayValue
        {
            get { return Menu.MiscM["misc.laugh.delay"].Cast<Slider>().CurrentValue; }
        }

        public static bool AutoHpActive
        {
            get { return Menu.MiscM["misc.autohp"].Cast<CheckBox>().CurrentValue; }
        }

        public static int AutoHpValue
        {
            get { return Menu.MiscM["misc.hp.value"].Cast<Slider>().CurrentValue; }
        }

        public static bool SkinHackActive
        {
            get { return Menu.MiscM["skin.active"].Cast<CheckBox>().CurrentValue; }
        }

        public static int SkinHackValue
        {
            get { return Menu.MiscM["skin.value"].Cast<Slider>().CurrentValue; }
        }

        public static bool UseEoutofQRange
        {
            get { return Menu.ComboM["combo.useEoutofQ"].Cast<CheckBox>().CurrentValue; }
        }
    }
}