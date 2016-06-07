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
            this.Menu.Add("UseLocketCombo", new CheckBox("开启鸟盾"));
            this.Menu.Add("ModeLOCKET", new ComboBox("模式: ", 1, "总是使用", "连招使用"));;
            this.Menu.Add("locket-min-health", new Slider("最低血量使用 %", 50, 1));
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
                if (!getCheckBoxItem(this.Menu, "UseLocketCombo") || !EloBuddy.SDK.Item.HasItem((int)this.Id) || !EloBuddy.SDK.Item.CanUseItem((int)this.Id))
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

                    if (ally.HealthPercent <= getSliderItem(this.Menu, "locket-min-health")
                        && enemies >= 1)
                    {
                        if (ally.HealthPercent < getSliderItem(this.Menu, "locket-min-health"))
                        {
                            EloBuddy.SDK.Item.UseItem((int)this.Id, ally);
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