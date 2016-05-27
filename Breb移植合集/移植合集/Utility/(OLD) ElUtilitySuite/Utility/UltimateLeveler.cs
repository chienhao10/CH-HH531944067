namespace ElUtilitySuite.Utility
{
    using System;
    using System.Linq;
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    /// <summary>
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

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// 
        public static bool getCheckBoxItem(string item)
        {
            return levelMenu[item].Cast<CheckBox>().CurrentValue;
        }

        public static Menu rootMenu = ElUtilitySuite.Entry.menu;
        public static Menu levelMenu;
        public void CreateMenu(Menu rootMenu)
        {
            levelMenu = rootMenu.AddSubMenu("Ultimate Leveler", "UltLeveler");
            levelMenu.Add("AutoLevelR", new CheckBox("Automaticly Level Up Ultimate"));

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

            if (getCheckBoxItem("AutoLevelR"))
            {
                LeagueSharp.Common.Utility.DelayAction.Add(random.Next(100, 1000), () => sender.Spellbook.LevelSpell(SpellSlot.R));
            }
        }

        #endregion
    }
}