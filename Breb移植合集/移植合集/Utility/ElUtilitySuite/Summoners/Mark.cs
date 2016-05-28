namespace ElUtilitySuite.Summoners
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy.SDK.Menu;
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK;
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

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
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
                return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            if (this.Player.GetSpellSlot("summonersnowball") == SpellSlot.Unknown)
            {
                return;
            }


            var snowballMenu = rootMenu.AddSubMenu("大乱斗雪球", "Snowball");
            {
                snowballMenu.Add("Snowball.Activated", new CheckBox("开启丢雪球"));
                snowballMenu.Add("SnowballHotkey", new KeyBind("丢雪球按键", false, KeyBind.BindTypes.HoldActive, 'Z'));
                foreach (var x in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy))
                {
                    snowballMenu.Add("snowballon" + x.ChampionName, new CheckBox("为以下使用 " + x.ChampionName));
                }
            }

            this.Menu = snowballMenu;
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
            if (!getCheckBoxItem(this.Menu, "Snowball.Activated") || !getKeyBindItem(this.Menu, "SnowballHotkey"))
            {
                return;
            }

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && x.LSIsValidTarget(1500f)))
            {
                if (getCheckBoxItem(this.Menu, string.Format("snowballon{0}", enemy.ChampionName)))
                {
                    this.MarkSpell.CastIfHitchanceEquals(enemy, HitChance.High);
                }
            }
        }

        #endregion
    }
}
