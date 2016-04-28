using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

namespace SCommon.TS
{
    internal static class ConfigMenu
    {
        private static Menu s_Config;

        /// <summary>
        ///     Gets or sets focus selected target value
        /// </summary>
        public static bool FocusSelected
        {
            get { return getCheckBoxItem("TargetSelector.Root.blFocusSelected"); }
        }

        /// <summary>
        ///     Gets extra focus range
        /// </summary>
        public static int FocusExtraRange
        {
            get { return getSliderItem("TargetSelector.Root.iFocusSelectedExtraRange"); }
        }

        /// <summary>
        ///     Gets or sets only attack selected value
        /// </summary>
        public static bool OnlyAttackSelected
        {
            get { return getCheckBoxItem("TargetSelector.Root.blOnlyAttackSelected"); }
        }

        /// <summary>
        ///     Gets or sets selected target color value
        /// </summary>
        public static bool SelectedTargetColor
        {
            get { return getCheckBoxItem("TargetSelector.Root.SelectedTargetColor"); }
        }

        /// <summary>
        ///     Gets targetting mode
        /// </summary>
        public static int TargettingMode
        {
            get { return getSliderItem("TargetSelector.Root.iTargettingMode"); }
        }

        public static bool getCheckBoxItem(string item)
        {
            return s_Config[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return s_Config[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(string item)
        {
            return s_Config[item].Cast<KeyBind>().CurrentValue;
        }

        public static void Create(Menu menuToAttach)
        {
            s_Config = MainMenu.AddMenu("Target Selector", "TargetSelector.Root");
            s_Config.Add("TargetSelector.Root.blFocusSelected", new CheckBox("Focus Selected Target"));
            s_Config.Add("TargetSelector.Root.iFocusSelectedExtraRange",
                new Slider("Extra Focus Selected Range", 0, 0, 250));
            s_Config.Add("TargetSelector.Root.blOnlyAttackSelected", new CheckBox("Only Attack Selected Target", false));
            s_Config.Add("TargetSelector.Root.SelectedTargetColor", new CheckBox("Selected Target Color", false));

            s_Config.AddGroupLabel("Champion Priority");
            foreach (var enemy in HeroManager.Enemies)
            {
                s_Config.Add(string.Format("TargetSelector.Priority.{0}", enemy.ChampionName),
                    new Slider(enemy.ChampionName, 1, 1, 5));
            }
            s_Config.AddLabel("0 : Auto");
            s_Config.AddLabel("1 : Low HP");
            s_Config.AddLabel("2 : Most AD");
            s_Config.AddLabel("3 : Most AP");
            s_Config.AddLabel("4 : Closest");
            s_Config.AddLabel("5 : Near Mouse");
            s_Config.AddLabel("6 : Less Attack");
            s_Config.AddLabel("7 : Less Cast");
            s_Config.Add("TargetSelector.Root.iTargettingMode", new Slider("Targetting Mode", 0, 0, 7));
        }

        /// <summary>
        ///     Gets priority of given enemy
        /// </summary>
        /// <param name="enemy">Enemy</param>
        /// <returns>Given enemy's priority which set by user</returns>
        public static int GetChampionPriority(AIHeroClient enemy)
        {
            if (enemy == null)
                return 1;
            try
            {
                return getSliderItem(string.Format("TargetSelector.Priority.{0}", enemy.ChampionName));
            }
            catch
            {
                return 1;
            }
        }
    }
}