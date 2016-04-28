/*
 Copyright 2015 - 2015 SPrediction
 ConfigMenu.cs is part of SPrediction
 
 SPrediction is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.
 
 SPrediction is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 GNU General Public License for more details.
 
 You should have received a copy of the GNU General Public License
 along with SPrediction. If not, see <http://www.gnu.org/licenses/>.
*/

using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace SPrediction
{
    /// <summary>
    ///     SPrediction Config Menu class
    /// </summary>
    public static class ConfigMenu
    {
        #region Private Properties

        private static Menu s_Menu;

        #endregion

        #region Initalizer Method

        public static bool getCheckBoxItem(string item)
        {
            return s_Menu[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return s_Menu[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(string item)
        {
            return s_Menu[item].Cast<KeyBind>().CurrentValue;
        }

        /// <summary>
        ///     Creates the sprediciton menu
        /// </summary>
        public static void Initialize(string prefMenuName = "SPRED")
        {
            s_Menu = MainMenu.AddMenu("SPrediction", prefMenuName);
            s_Menu.Add("PREDICTONLIST", new Slider("Pred. Method (0 : Common/EB Pred)", 0, 0, 0));
            s_Menu.Add("SPREDWINDUP", new CheckBox("Check for target AA Windup", false));
            s_Menu.Add("SPREDMAXRANGEIGNORE", new Slider("Max Range Dodge Ignore (%)", 50));
            s_Menu.Add("SPREDREACTIONDELAY", new Slider("Ignore Rection Delay", 0, 0, 200));
            s_Menu.Add("SPREDDELAY", new Slider("Spell Delay", 0, 0, 200));
            s_Menu.Add("SPREDHC", new KeyBind("Count HitChance", false, KeyBind.BindTypes.HoldActive, 32));
            s_Menu.Add("SPREDDRAWINGX", new Slider("Drawing Pos X", Drawing.Width - 200, 0, Drawing.Width));
            s_Menu.Add("SPREDDRAWINGY", new Slider("Drawing Pos Y", 0, 0, Drawing.Height));
            s_Menu.Add("SPREDDRAWINGS", new CheckBox("Enable Drawings", false));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets selected prediction for spell extensions
        /// </summary>
        public static int SelectedPrediction
        {
            get { return getSliderItem("PREDICTONLIST"); }
        }

        /// <summary>
        ///     Gets or sets Check AA WindUp value
        /// </summary>
        public static bool CheckAAWindUp
        {
            get { return getCheckBoxItem("SPREDWINDUP"); }
        }

        /// <summary>
        ///     Gets or sets max range ignore value
        /// </summary>
        public static int MaxRangeIgnore
        {
            get { return getSliderItem("SPREDMAXRANGEIGNORE"); }
        }

        /// <summary>
        ///     Gets or sets ignore reaction delay value
        /// </summary>
        public static int IgnoreReactionDelay
        {
            get { return getSliderItem("SPREDREACTIONDELAY"); }
        }

        /// <summary>
        ///     Gets or sets spell delay value
        /// </summary>
        public static int SpellDelay
        {
            get { return getSliderItem("SPREDDELAY"); }
        }

        /// <summary>
        ///     Gets count hitchance key is enabled
        /// </summary>
        public static bool CountHitChance
        {
            get { return getKeyBindItem("SPREDHC"); }
        }

        /// <summary>
        ///     Gets or sets drawing x pos for hitchance drawings
        /// </summary>
        public static int HitChanceDrawingX
        {
            get { return getSliderItem("SPREDDRAWINGX"); }
        }

        /// <summary>
        ///     Gets or sets drawing y pos for hitchance drawings
        /// </summary>
        public static int HitChanceDrawingY
        {
            get { return getSliderItem("SPREDDRAWINGY"); }
        }

        /// <summary>
        ///     Gets or sets drawings are enabled
        /// </summary>
        public static bool EnableDrawings
        {
            get { return getCheckBoxItem("SPREDDRAWINGS"); }
        }

        #endregion
    }
}