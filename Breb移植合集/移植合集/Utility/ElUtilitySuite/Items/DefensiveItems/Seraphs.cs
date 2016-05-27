namespace ElUtilitySuite.Items.DefensiveItems
{
    using System;
    using System.Linq;

    using ElUtilitySuite.Vendor.SFX;

    using EloBuddy;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;
    using EloBuddy.SDK.Menu.Values;

    internal class Seraphs : Item
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public Seraphs()
        {
            IncomingDamageManager.RemoveDelay = 500;
            IncomingDamageManager.Skillshots = true;
            Game.OnUpdate += this.Game_OnUpdate;
        }

        #endregion

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
                return (ItemId)3040;
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
                return "Seraph's embrace";
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// 
        public override void CreateMenu()
        {
            this.Menu.AddGroupLabel(Name);
            this.Menu.Add("UseSeraphsCombo", new CheckBox("Activated"));
            this.Menu.Add("ModeSERAPH", new ComboBox("Activation mode: ", 1, "Use always", "Use in combo"));
            this.Menu.Add("seraphs-min-health", new Slider("Health percentage", 20, 1));
            this.Menu.Add("seraphs-min-damage", new Slider("Incoming damage percentage", 20, 1));
            this.Menu.AddSeparator();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when the game updates
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Game_OnUpdate(EventArgs args)
        {
            try
            {
                if (!ItemData.Seraphs_Embrace.GetItem().IsOwned() || !getCheckBoxItem(this.Menu, "UseSeraphsCombo"))
                {
                    return;
                }

                if (getBoxItem(this.Menu, "Mode-seraphs") == 1 && !this.ComboModeActive)
                {
                    return;
                }

                var enemies = this.Player.LSCountEnemiesInRange(800);
                var totalDamage = IncomingDamageManager.GetDamage(this.Player) * 1.1f;

                if (this.Player.HealthPercent <= getSliderItem(this.Menu, "seraphs-min-health") && enemies >= 1)
                {
                    if ((int)(totalDamage / this.Player.Health)
                        > getSliderItem(this.Menu, "seraphs-min-damage")
                        || this.Player.HealthPercent < getSliderItem(this.Menu, "seraphs-min-health"))
                    {
                        Items.UseItem((int)this.Id, this.Player);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("[ELUTILITYSUITE - SERAPHS] Used for: {0} - health percentage: {1}%", this.Player.ChampionName, (int)this.Player.HealthPercent);
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(@"An error occurred: '{0}'", e);
            }
        }

        #endregion
    }
}