namespace ElUtilitySuite.Items.DefensiveItems
{
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Randuins : Item
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public override ItemId Id
        {
            get
            {
                return ItemId.Randuins_Omen;
            }
        }

        /// <summary>
        ///     Gets or sets the name of the item.
        /// </summary>
        /// <value>
        ///     The name of the item.
        /// </value>
        public override string Name
        {
            get
            {
                return "Randuin's Omen";
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        public override void CreateMenu()
        {
            this.Menu.AddGroupLabel(Name);
            this.Menu.Add("UseRanduinsCombo", new CheckBox("开启兰盾"));
            this.Menu.Add("ModeRANDUIN", new ComboBox("模式: ", 1, "总是使用", "连招使用"));
            this.Menu.Add("RanduinsCount", new Slider("命中敌人数量", 3, 1, 5));
            this.Menu.AddSeparator();
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            return getCheckBoxItem(this.Menu, "UseRanduinsCombo") && this.Player.LSCountEnemiesInRange(500f) >= getSliderItem(this.Menu, "RanduinsCount");
        }

        /// <summary>
        ///     Uses the item.
        /// </summary>
        public override void UseItem()
        {
            if (getBoxItem(this.Menu, "ModeRANDUIN") == 1 && !this.ComboModeActive)
            {
                return;
            }

            if (EloBuddy.SDK.Item.HasItem(this.Id) && EloBuddy.SDK.Item.CanUseItem(this.Id))
                EloBuddy.SDK.Item.UseItem((int)this.Id);
        }

        #endregion
    }
}