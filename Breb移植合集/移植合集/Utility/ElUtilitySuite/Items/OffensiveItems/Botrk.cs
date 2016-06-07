namespace ElUtilitySuite.Items.OffensiveItems
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    internal class Botrk : Item
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
                return ItemId.Blade_of_the_Ruined_King;
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
                return "Blade of the Ruined King";
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
            this.Menu.Add("UseBotrkCombo", new CheckBox("连招使用"));
            this.Menu.Add("BotrkEnemyHp", new Slider("敌人血量 %", 100));
            this.Menu.Add("BotrkMyHp", new Slider("自身血量 %", 100));
            this.Menu.AddSeparator();
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            return getCheckBoxItem(this.Menu, "UseBotrkCombo") && this.ComboModeActive
                   && (HeroManager.Enemies.Any(
                       x =>
                       x.HealthPercent < getSliderItem(this.Menu, "BotrkEnemyHp")
                       && x.LSDistance(this.Player) < 550)
                       || this.Player.HealthPercent < getSliderItem(this.Menu, "BotrkMyHp"));
        }

        /// <summary>
        ///     Uses the item.
        /// </summary>
        public override void UseItem()
        {
            if (EloBuddy.SDK.Item.HasItem(this.Id) && EloBuddy.SDK.Item.CanUseItem(this.Id))
                EloBuddy.SDK.Item.UseItem((int)this.Id, HeroManager.Enemies.FirstOrDefault(x => x.HealthPercent < getSliderItem(this.Menu, "BotrkEnemyHp") && x.LSDistance(this.Player) < 550));
        }

        #endregion
    }
}