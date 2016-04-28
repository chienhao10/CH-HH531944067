using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;

namespace ElUtilitySuite.Items
{
    internal class Potions : IPlugin
    {
        #region Delegates

        /// <summary>
        ///     Gets an health item
        /// </summary>
        /// <returns></returns>
        private delegate LeagueSharp.Common.Items.Item GetHealthItemDelegate();

        #endregion

        #region Public Properties

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

        /// <summary>
        ///     Gets or sets the items.
        /// </summary>
        /// <value>
        ///     The items.
        /// </value>
        private List<HealthItem> Items { get; set; }

        /// <summary>
        ///     Gets the set player hp menu value.
        /// </summary>
        /// <value>
        ///     The player hp hp menu value.
        /// </value>
        private int PlayerHp
        {
            get
            {
                return getSliderItem("Potions.Player.Health");
            }
        }

        #endregion

        #region Public Methods and Operators

        public static bool getCheckBoxItem(string item)
        {
            return potionsMenu[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return potionsMenu[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(string item)
        {
            return potionsMenu[item].Cast<KeyBind>().CurrentValue;
        }

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public static Menu rootMenu = Entry.menu;
        public static Menu potionsMenu;
        public void CreateMenu(Menu rootMenu)
        {
            potionsMenu = rootMenu.AddSubMenu("Potions", "Potions");
            potionsMenu.Add("Potions.Activated", new CheckBox("Potions activated"));
            potionsMenu.Add("Potions.Health", new CheckBox("Health potions"));
            potionsMenu.Add("Potions.Biscuit", new CheckBox("Biscuits"));
            potionsMenu.Add("Potions.RefillablePotion", new CheckBox("Refillable Potion"));
            potionsMenu.Add("Potions.HuntersPotion", new CheckBox("Hunters Potion"));
            potionsMenu.Add("Potions.CorruptingPotion", new CheckBox("Corrupting Potion"));
            potionsMenu.AddSeparator();
            potionsMenu.Add("Potions.Player.Health", new Slider("Health percentage", 20));

        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            Items = new List<HealthItem>
                             {
                                 new HealthItem { GetItem = () => ItemData.Health_Potion.GetItem(), BuffName = "RegenerationPotion" },
                                 new HealthItem { GetItem = () => ItemData.Total_Biscuit_of_Rejuvenation2.GetItem(), BuffName = "ItemMiniRegenPotion"},
                                 new HealthItem { GetItem = () => ItemData.Refillable_Potion.GetItem(), BuffName = "ItemCrystalFlask" }, 
                                 new HealthItem { GetItem = () => ItemData.Hunters_Potion.GetItem(), BuffName = "ItemCrystalFlaskJungle"},
                                 new HealthItem { GetItem = () => ItemData.Corrupting_Potion.GetItem(), BuffName = "ItemDarkCrystalFlask"}
                             };

            Game.OnUpdate += OnUpdate;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the player buffs
        /// </summary>
        /// <value>
        ///     The player buffs
        /// </value>
        private bool IsBuffActive()
        {
            return Items.Any(potion => Player.Buffs.Any(b => b.Name.Equals(potion.BuffName, StringComparison.OrdinalIgnoreCase)));
        }

        private void OnUpdate(EventArgs args)
        {
            try
            {
                if (!getCheckBoxItem("Potions.Activated") || Player.IsDead || Player.InFountain() || Player.Buffs.Any(b => b.Name.ToLower().Contains("Recall") || b.Name.ToLower().Contains("Teleport")))
                {
                    return;
                }

                if (Player.HealthPercent < PlayerHp)
                {
                    if (IsBuffActive())
                    {
                        return;
                    }

                    var item = Items.Select(x => x.Item).FirstOrDefault(x => x.IsReady() && x.IsOwned());

                    if (item != null)
                    {
                        item.Cast();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        /// <summary>
        ///     Represents an item that can heal
        /// </summary>
        private class HealthItem
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the get item.
            /// </summary>
            /// <value>
            ///     The get item.
            /// </value>
            public GetHealthItemDelegate GetItem { private get; set; }

            public String BuffName { get; set; }

            /// <summary>
            ///     Gets the item.
            /// </summary>
            /// <value>
            ///     The item.
            /// </value>
            public LeagueSharp.Common.Items.Item Item
            {
                get
                {
                    return GetItem();
                }
            }

            #endregion
        }
    }
}