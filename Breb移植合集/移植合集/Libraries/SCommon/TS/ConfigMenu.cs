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

        public static void Create()
        {
            s_Config = MainMenu.AddMenu("目标选择器", "TargetSelector.Root");
            s_Config.Add("TargetSelector.Root.blFocusSelected", new CheckBox("集火选择的目标"));
            s_Config.Add("TargetSelector.Root.iFocusSelectedExtraRange",
                new Slider("额外集火选择的目标范围", 0, 0, 250));
            s_Config.Add("TargetSelector.Root.blOnlyAttackSelected", new CheckBox("只攻击选择的", false));
            s_Config.Add("TargetSelector.Root.SelectedTargetColor", new CheckBox("选择颜色", false));

            s_Config.AddGroupLabel("英雄优先顺序");
            foreach (var enemy in HeroManager.Enemies)
            {
                s_Config.Add(string.Format("TargetSelector.Priority.{0}", enemy.ChampionName),
                    new Slider(enemy.ChampionName, 1, 1, 5));
            }
            s_Config.AddLabel("0 : 自动");
            s_Config.AddLabel("1 : 低血量");
            s_Config.AddLabel("2 : 最高 AD");
            s_Config.AddLabel("3 : 最高 AP");
            s_Config.AddLabel("4 : 最近");
            s_Config.AddLabel("5 : 鼠标附近");
            s_Config.AddLabel("6 : 最少攻击次数");
            s_Config.AddLabel("7 : 最少施法次数");
            s_Config.Add("TargetSelector.Root.iTargettingMode", new Slider("目标选择模式", 0, 0, 7));
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