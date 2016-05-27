namespace ElUtilitySuite.Items.DefensiveItems
{
    using System;
    using System.Linq;

    using ElUtilitySuite.Vendor.SFX;

    using LeagueSharp.Common;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy;

    internal class Locket : Item
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public Locket()
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
                return ItemId.Locket_of_the_Iron_Solari;
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
                return "Locket of the Iron Solari";
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
            this.Menu.Add("UseLocketCombo", new CheckBox("Activate"));
            this.Menu.Add("ModeLOCKET", new ComboBox("Activation mode: ", 1, "Use always", "Use in combo"));
            this.Menu.Add("locket-min-health", new Slider("Health percentage", 50, 1));
            this.Menu.Add("locket-min-damage", new Slider("Incoming damage percentage", 50, 1));
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
                if (!getCheckBoxItem(this.Menu, "UseLocketCombo") || !Items.HasItem((int)this.Id) || !Items.CanUseItem((int)this.Id))
                {
                    return;
                }

                if (getBoxItem(this.Menu, "ModeLOCKET") == 1 && !this.ComboModeActive)
                {
                    return;
                }

                foreach (var ally in HeroManager.Allies.Where(a => a.LSIsValidTarget(600f, false) && !a.LSIsRecalling()))
                {
                    var enemies = ally.LSCountEnemiesInRange(600f);
                    var totalDamage = IncomingDamageManager.GetDamage(ally) * 1.1f;

                    if (ally.HealthPercent <= getSliderItem(this.Menu, "locket-min-health")
                        && enemies >= 1)
                    {
                        if ((int)(totalDamage / ally.Health)
                            > getSliderItem(this.Menu, "locket-min-damage")
                            || ally.HealthPercent < getSliderItem(this.Menu, "locket-min-health"))
                        {
                            Items.UseItem((int)this.Id, ally);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("[ELUTILITYSUITE - LOCKET] Used for: {0} - health percentage: {1}%", ally.ChampionName, (int)ally.HealthPercent);
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    }
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