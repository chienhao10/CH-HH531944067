namespace ElUtilitySuite.Items.OffensiveItems
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK;
    internal class Cutlass : ElUtilitySuite.Items.Item
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
                return ItemId.Bilgewater_Cutlass;
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
                return "Bilgewater Cutlass";
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
            this.Menu.Add("UseCutlassCombo", new CheckBox("连招使用"));
            this.Menu.Add("CutlassMyHp", new Slider("自身生命 %", 100));
            this.Menu.AddSeparator();
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            return getCheckBoxItem(this.Menu, "UseCutlassCombo") && this.ComboModeActive && this.Player.HealthPercent < getSliderItem(this.Menu, "CutlassMyHp");
        }

        /// <summary>
        ///     Uses the item.
        /// </summary>
        public override void UseItem()
        {
            if (EloBuddy.SDK.Item.HasItem(this.Id) && EloBuddy.SDK.Item.CanUseItem(this.Id))
                Item.UseItem((int)this.Id, TargetSelector.GetTarget(550, DamageType.Physical));
        }

        #endregion
    }
}
