namespace ElUtilitySuite.Utility
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    internal class BlueTrinket : IPlugin
    {
        #region Constants

        /// <summary>
        ///     A collection of champions that should not buy the blue trinket
        /// </summary>
        public static string[] BlacklistedChampions = { "Elise", "Nidalee", "Udyr", "LeeSin", "Monkeyking", "Hecarim", "Nautilus", "Thresh", "Bard", "Soraka", "Nami", "Rengar", "Zilean", "Fiddlesticks", "Blitzcrank", "Braum", "Katarina", "Alistar", "Maokai", "MasterYi", "Rammus", "Pantheon", "Evelynn", "Warwick", "Ryze", "Zed", "Fizz", "Taric", "Malphite", "JarvanIV" };

        /// <summary>
        ///     The Check Interval
        /// </summary>
        private const float CheckInterval = 333f;

        #region Static Fields

        /// <summary>
        ///     The random
        /// </summary>
        private static Random random;

        #endregion

        #endregion

        #region Fields

        /// <summary>
        ///     The lastCheck
        /// </summary>
        private float lastCheck = Environment.TickCount;

        #endregion

        #region Public Properties

        /// <summary>
        ///     The Menu
        /// </summary>
        public Menu Menu { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            var autoTrinketMenu = rootMenu.AddSubMenu("Blue trinket", "bluetrinket");
            autoTrinketMenu.Add("AutoTrinket", new CheckBox("Auto buy blue trinket", false));

            this.Menu = autoTrinketMenu;
            random = new Random(Environment.TickCount);
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            try
            {
                if (
               !BlacklistedChampions.Any(
                   x => x.Equals(ObjectManager.Player.ChampionName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    Game.OnUpdate += this.OnUpdate;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void OnUpdate(EventArgs args)
        {
            try
            {
                if (!this.Menu["AutoTrinket"].Cast<CheckBox>().CurrentValue)
                {
                    return;
                }

                if (this.lastCheck + CheckInterval > Environment.TickCount)
                {
                    return;
                }

                this.lastCheck = Environment.TickCount;

                if (this.Player.InShop() || this.Player.IsDead)
                {
                    if (this.Player.Level >= 9)
                    {
                        if (ItemData.Farsight_Alteration.GetItem().IsOwned())
                        {
                            return;
                        }
                        LeagueSharp.Common.Utility.DelayAction.Add(random.Next(100, 1000), () => Shop.BuyItem(ItemId.Farsight_Alteration));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion
    }
}