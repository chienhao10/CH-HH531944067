namespace ElUtilitySuite.Items.OffensiveItems
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    class HextechProtobelt_01 //: Item
    {
        /*#region Public Properties

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
                return (ItemId)3145;
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
                return "Hextech Protobelt 01";
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        public override void CreateMenu()
        {
            this.Menu.AddItem(new MenuItem("UseHextech01Combo", "Use on Combo").SetValue(true));
            this.Menu.AddItem(new MenuItem("Hextech01EnemyHp", "Use on Enemy Hp %").SetValue(new Slider(70)));
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            return this.Menu.Item("UseHextech01Combo").IsActive() && this.ComboModeActive
                   && HeroManager.Enemies.Any(
                       x =>
                       x.HealthPercent < this.Menu.Item("Hextech01EnemyHp").GetValue<Slider>().Value
                       && x.LSDistance(this.Player) < 700 && !x.IsDead && !x.IsZombie);
        }

        /// <summary>
        ///     Uses the item.
        /// </summary>
        public override void UseItem()
        {
            Items.UseItem(
                (int)this.Id,
                HeroManager.Enemies.FirstOrDefault(
                    x =>
                    x.HealthPercent < this.Menu.Item("Hextech01EnemyHp").GetValue<Slider>().Value
                    && x.LSDistance(this.Player) < 700 && !x.IsDead && !x.IsZombie));
        }

        #endregion*/
    }
}