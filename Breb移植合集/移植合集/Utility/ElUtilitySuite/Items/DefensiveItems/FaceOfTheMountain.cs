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
            this.Menu.Add("UseFaceCombo", new CheckBox("开启崇山之盾"));
            this.Menu.Add("ModeFACE", new ComboBox("模式: ", 1, "总是使用", "连招使用"));
            this.Menu.Add("face-min-health", new Slider("最低血量使用 %", 50));
            foreach (var x in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly))
            {
                this.Menu.Add("Faceon" + x.ChampionName, new CheckBox("为以下使用 " + x.ChampionName));
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
                if (!EloBuddy.SDK.Item.HasItem((int)this.Id) || !EloBuddy.SDK.Item.CanUseItem((int)this.Id) || !getCheckBoxItem(this.Menu, "UseFaceCombo"))
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

                    if (ally.HealthPercent <= getSliderItem(this.Menu, "face-min-health") && enemies >= 1)
                    {
                        if (ally.HealthPercent < getSliderItem(this.Menu, "face-min-health"))
                        {
                            EloBuddy.SDK.Item.UseItem((int)this.Id, ally);
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