namespace ElUtilitySuite.Items.DefensiveItems
{
    using System.Linq;
    using System.Runtime.CompilerServices;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    internal class Talisman : Item
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
                return ItemId.Talisman_of_Ascension;
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
                return "Talisman of Ascension";
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
            this.Menu.Add("UseTalismanCombo", new CheckBox("Activated"));
            this.Menu.Add("ModeTALIS", new ComboBox("Activation mode: ", 1, "Use always", "Use in combo"));
            this.Menu.Add("TalismanEnemyHp", new Slider("Use on Enemy Hp %", 70));
            this.Menu.Add("TalismanMyHp", new Slider("Use on My Hp %", 50));
            this.Menu.AddSeparator();
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            return getCheckBoxItem(this.Menu, "UseTalismanCombo")
                  && (HeroManager.Enemies.Any(
                      x =>
                      x.HealthPercent < getSliderItem(this.Menu, "TalismanEnemyHp")
                      && x.LSDistance(this.Player) < 550)
                      || this.Player.HealthPercent < getSliderItem(this.Menu, "TalismanMyHp"));
        }

        /// <summary>
        ///     Uses the item.
        /// </summary>
        public override void UseItem()
        {
            if (getBoxItem(this.Menu, "ModeTALIS") == 1 && !this.ComboModeActive)
            {
                return;
            }

            Items.UseItem((int)this.Id);
        }

        #endregion
    }
}
