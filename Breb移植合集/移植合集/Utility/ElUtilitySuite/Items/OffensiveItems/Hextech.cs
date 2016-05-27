namespace ElUtilitySuite.Items.OffensiveItems
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    internal class Hextech : Item
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
                return ItemId.Hextech_Gunblade;
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
                return "Hextech Gunblade";
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
            this.Menu.Add("UseHextechCombo", new CheckBox("连招使用"));
            this.Menu.Add("HextechEnemyHp", new Slider("敌人血量 %", 70));
            this.Menu.AddSeparator();
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            return getCheckBoxItem(this.Menu, "UseHextechCombo") && this.ComboModeActive
                   && HeroManager.Enemies.Any(
                       x =>
                       x.HealthPercent < getSliderItem(this.Menu, "HextechEnemyHp")
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
                    x.HealthPercent < getSliderItem(this.Menu, "HextechEnemyHp")
                    && x.LSDistance(this.Player) < 700 && !x.IsDead && !x.IsZombie));
        }

        #endregion
    }
}