namespace ElUtilitySuite.Items.OffensiveItems
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy;
    class FrostQueen : Item
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
                return ItemId.Frost_Queens_Claim;
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
                return "Frost Queen's Claim";
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
            this.Menu.Add("UseFrostQueenCombo", new CheckBox("连招使用"));
            this.Menu.Add("FrostQueenEnemyHp", new Slider("当敌人血量 % 时使用", 70));
            this.Menu.Add("FrostQueenMyHp", new Slider("我血量 % 时使用", 100));
            this.Menu.AddSeparator();
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            return getCheckBoxItem(this.Menu, "UseFrostQueenCombo") && this.ComboModeActive
                   && (HeroManager.Enemies.Any(
                       x =>
                       x.HealthPercent < getSliderItem(this.Menu, "FrostQueenEnemyHp")
                       && x.LSDistance(this.Player) < 1500)
                       || this.Player.HealthPercent < getSliderItem(this.Menu, "FrostQueenMyHp"));
        }

        /// <summary>
        ///     Uses the item.
        /// </summary>
        public override void UseItem()
        {
            Items.UseItem((int)this.Id);
        }

        #endregion
    }
}