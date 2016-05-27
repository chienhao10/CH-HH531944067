namespace ElUtilitySuite.Items.DefensiveItems
{
    using System;
    using System.Linq;

    using ElUtilitySuite.Vendor.SFX;

    using EloBuddy;
    using LeagueSharp.Common;
    using EloBuddy.SDK.Menu.Values;

    internal class FaceOfTheMountain : Item
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public FaceOfTheMountain()
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
                return ItemId.Face_of_the_Mountain;
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
                return "Face of the Mountain";
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
            this.Menu.Add("UseFaceCombo", new CheckBox("Activate"));
            this.Menu.Add("ModeFACE", new ComboBox("Activation mode: ", 1, "Use always", "Use in combo"));
            this.Menu.Add("face-min-health", new Slider("Use on Hp %", 50));
            this.Menu.Add("face-min-damage", new Slider("Incoming damage percentage", 50, 1));
            foreach (var x in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly))
            {
                this.Menu.Add("Faceon" + x.ChampionName, new CheckBox("Use for " + x.ChampionName));
            }
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
                if (!Items.HasItem((int)this.Id) || !Items.CanUseItem((int)this.Id)
                    || !getCheckBoxItem(this.Menu, "UseFaceCombo"))
                {
                    return;
                }

                if (getBoxItem(this.Menu, "ModeFACE") == 1 && !this.ComboModeActive)
                {
                    return;
                }

                foreach (var ally in HeroManager.Allies.Where(a => a.LSIsValidTarget(850f, false) && !a.LSIsRecalling()))
                {
                    if (!getCheckBoxItem(this.Menu, string.Format("Faceon{0}", ally.ChampionName)))
                    {
                        return;
                    }

                    var enemies = ally.LSCountEnemiesInRange(800);
                    var totalDamage = IncomingDamageManager.GetDamage(ally) * 1.1f;

                    if (ally.HealthPercent <= getSliderItem(this.Menu, "face-min-health") && enemies >= 1)
                    {
                        if ((int)(totalDamage / ally.Health)
                            > getSliderItem(this.Menu, "face-min-damage")
                            || ally.HealthPercent < getSliderItem(this.Menu, "face-min-health"))
                        {
                            Items.UseItem((int)this.Id, ally);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("[ELUTILITYSUITE - FACE OF THE MOUNTAIN] Used for: {0} - health percentage: {1}%", ally.ChampionName, (int)ally.HealthPercent);
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