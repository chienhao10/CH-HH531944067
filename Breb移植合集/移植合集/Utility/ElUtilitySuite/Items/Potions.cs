namespace ElUtilitySuite.Items
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;
    using EloBuddy.SDK.Menu;
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    internal class Potions : IPlugin
    {
        #region Delegates

        /// <summary>
        ///     Gets an health item
        /// </summary>
        /// <returns></returns>
        private delegate Items.Item GetHealthItemDelegate();

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
                return getSliderItem(this.Menu, "Potions.Player.Health");
            }
        }

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

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            var potionsMenu = rootMenu.AddSubMenu("药水", "Potions");
            {
            potionsMenu.Add("Potions.Activated", new CheckBox("使用药水"));
            potionsMenu.Add("Potions.Health", new CheckBox("红药"));
            potionsMenu.Add("Potions.Biscuit", new CheckBox("饼干"));
            potionsMenu.Add("Potions.RefillablePotion", new CheckBox("可充药水"));
            potionsMenu.Add("Potions.HuntersPotion", new CheckBox("猎人药水"));
            potionsMenu.Add("Potions.CorruptingPotion", new CheckBox("腐蚀药水"));
            potionsMenu.Add("Potions.Player.Health", new Slider("血量百分比", 20));

            }

            this.Menu = potionsMenu;
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            this.Items = new List<HealthItem>
                             {
                                 new HealthItem { GetItem = () => ItemData.Health_Potion.GetItem(), BuffName = "RegenerationPotion" },
                                 new HealthItem { GetItem = () => ItemData.Total_Biscuit_of_Rejuvenation2.GetItem(), BuffName = "ItemMiniRegenPotion"},
                                 new HealthItem { GetItem = () => ItemData.Refillable_Potion.GetItem(), BuffName = "ItemCrystalFlask" }, 
                                 new HealthItem { GetItem = () => ItemData.Hunters_Potion.GetItem(), BuffName = "ItemCrystalFlaskJungle"},
                                 new HealthItem { GetItem = () => ItemData.Corrupting_Potion.GetItem(), BuffName = "ItemDarkCrystalFlask"}
                             };

            Game.OnUpdate += this.OnUpdate;
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
            return
                this.Items.Any(
                    potion => this.Player.Buffs.Any(
                        b => b.Name.Equals(potion.BuffName, StringComparison.OrdinalIgnoreCase)));
        }

        private void OnUpdate(EventArgs args)
        {
            try
            {
                if (!getCheckBoxItem(this.Menu, "Potions.Activated") || this.Player.IsDead || this.Player.InFountain() || this.Player.Buffs.Any(
                        b => b.Name.ToLower().Contains("Recall") || b.Name.ToLower().Contains("Teleport")))
                {
                    return;
                }

                if (this.Player.HealthPercent < this.PlayerHp)
                {
                    if (this.IsBuffActive())
                    {
                        return;
                    }

                    var item = this.Items.Select(x => x.Item).FirstOrDefault(x => x.IsReady() && x.IsOwned());

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
            public GetHealthItemDelegate GetItem { get; set; }

            /// <summary>
            ///     Gets the buffname
            /// </summary>
            /// <value>
            ///     The buffname
            /// </value>
            public String BuffName { get; set; }

            /// <summary>
            ///     Gets the item.
            /// </summary>
            /// <value>
            ///     The item.
            /// </value>
            public Items.Item Item
            {
                get
                {
                    return this.GetItem();
                }
            }

            #endregion
        }
    }
}