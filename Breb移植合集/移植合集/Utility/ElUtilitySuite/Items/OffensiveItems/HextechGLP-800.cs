namespace ElUtilitySuite.Items.OffensiveItems
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    class HextechGLP_800 : Item
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
                return (ItemId)3030;
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
                return "Hextech GLP 800";
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
            this.Menu.Add("UseHextech800Combo", new CheckBox("连招使用"));
            this.Menu.Add("Hextech800EnemyHp", new Slider("敌人血量 % 时使用", 70));
            this.Menu.AddSeparator();
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            return getCheckBoxItem(this.Menu, "UseHextech800Combo") && this.ComboModeActive
                   && HeroManager.Enemies.Any(
                       x =>
                       x.HealthPercent < getSliderItem(this.Menu, "Hextech800EnemyHp")
                       && x.LSDistance(this.Player) < 700 && !x.IsDead && !x.IsZombie);
        }

        /// <summary>
        ///     Uses the item.
        /// </summary>
        public override void UseItem()
        {
            var objAiHero = HeroManager.Enemies.FirstOrDefault(
                x => x.HealthPercent < getSliderItem(this.Menu, "Hextech800EnemyHp")
                && x.LSDistance(this.Player) < 500 && !x.IsDead && !x.IsZombie);

            if (objAiHero != null)
            {
                Items.UseItem((int)this.Id, objAiHero.ServerPosition);
            }
        }

        #endregion
    }
}
