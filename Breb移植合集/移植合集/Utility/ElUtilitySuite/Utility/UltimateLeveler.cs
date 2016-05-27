namespace ElUtilitySuite.Utility
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy;/// <summary>
                   /// Automatically levels R.
                   /// </summary>
    internal class UltimateLeveler : IPlugin
    {
        #region Static Fields

        /// <summary>
        ///     A collection of champions that should not be auto leveled.
        /// </summary>
        public static string[] BlacklistedChampions = { "Elise", "Nidalee", "Udyr" };

        /// <summary>
        /// The random
        /// </summary>
        private static Random random;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        private Menu Menu { get; set; }

        #endregion

        #region Public Methods and Operators

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

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        public void CreateMenu(Menu rootMenu)
        {
            this.Menu = rootMenu.AddSubMenu("自动加点", "UltLeveler");
            this.Menu.Add("AutoLevelR", new CheckBox("自动加点大招"));

            random = new Random(Environment.TickCount);
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            if (
                !BlacklistedChampions.Any(
                    x => x.Equals(ObjectManager.Player.ChampionName, StringComparison.InvariantCultureIgnoreCase)))
            {
                Obj_AI_Base.OnLevelUp += this.ObjAiBaseOnOnLevelUp;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when an <see cref="Obj_AI_Base" /> levels up.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ObjAiBaseOnOnLevelUp(Obj_AI_Base sender, EventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (getCheckBoxItem(this.Menu, "AutoLevelR"))
            {
                LeagueSharp.Common.Utility.DelayAction.Add(random.Next(100, 1000), () => sender.Spellbook.LevelSpell(SpellSlot.R));
            }
        }

        #endregion
    }
}