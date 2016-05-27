namespace ElUtilitySuite.Summoners
{
    using System;
    using System.Linq;
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using LeagueSharp.Common;

    public class Mark : IPlugin
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the Mark spell
        /// </summary>
        /// <value>
        ///     The Mark spell.
        /// </value>
        public LeagueSharp.Common.Spell MarkSpell { get; set; }

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        public Menu Menu { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the combo mode is active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if combo mode is active; otherwise, <c>false</c>.
        /// </value>
        public bool ComboModeActive
        {
            get
            {
                return Entry.getComboMenu() || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
            }
        }

        #endregion

        #region Public Methods and Operators

        public static bool getCheckBoxItem(string item)
        {
            return snowballMenu[item].Cast<CheckBox>().CurrentValue;
        }

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        /// 
        public static Menu rootMenu = ElUtilitySuite.Entry.menu;
        public static Menu snowballMenu;
        public void CreateMenu(Menu rootMenu)
        {
            if (this.Player.GetSpellSlot("summonersnowball") == SpellSlot.Unknown)
            {
                return;
            }

            snowballMenu = rootMenu.AddSubMenu("ARAM Snowball", "Snowball");
            snowballMenu.Add("Snowball.Activated", new CheckBox("Snowball activated"));
            foreach (var x in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy))
            {
                snowballMenu.Add("snowballon" + x.ChampionName, new CheckBox("Use for " + x.ChampionName));
            }
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            if (Game.MapId != GameMapId.HowlingAbyss)
            {
                return;
            }

            var markSlot = this.Player.GetSpellSlot("summonersnowball");
            if (markSlot == SpellSlot.Unknown)
            {
                return;
            }

            this.MarkSpell = new LeagueSharp.Common.Spell(markSlot, 1400f);
            this.MarkSpell.SetSkillshot(0f, 60f, 1500f, true, SkillshotType.SkillshotLine);

            Game.OnUpdate += this.GameOnUpdate;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void GameOnUpdate(EventArgs args)
        {
            if (!getCheckBoxItem("Snowball.Activated"))
            {
                return;
            }

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && x.IsValidTarget(1500f)))
            {
                if (getCheckBoxItem(string.Format("snowballon{0}", enemy.ChampionName)) && this.ComboModeActive)
                {
                    this.MarkSpell.CastIfHitchanceEquals(enemy, LeagueSharp.Common.HitChance.High);
                }
            }
        }

        #endregion
    }
}
