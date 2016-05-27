namespace ElUtilitySuite.Utility
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK;
    internal class AutoLantern : IPlugin
    {
        #region Fields

        public GameObject ThreshLantern;

        #endregion

        #region Public Properties

        public Menu Menu { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the click below hp menu value.
        /// </summary>
        /// <value>
        ///     The lantern below hp menu value.
        /// </value>
        private int ClickBelowHp
        {
            get
            {
                return this.Menu["ThreshLanternHPSlider"].Cast<Slider>().CurrentValue;
            }
        }

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

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            if (EntityManager.Heroes.Allies.Where(x => x.ChampionName == Champion.Thresh.ToString()).FirstOrDefault() == null || Player.ChampionName.Equals("Thresh"))
            {
                return;
            }

            var autoLanternMenu = rootMenu.AddSubMenu("Thresh Lantern", "Threshlantern");
            {
                autoLanternMenu.Add("ThreshLantern", new CheckBox("Auto click Thresh lantern"));
                autoLanternMenu.Add("ThreshLanternHotkey", new KeyBind("Hotkey", false, KeyBind.BindTypes.HoldActive, 'M'));
                autoLanternMenu.Add("ThreshLanternHPSlider", new Slider("Click when HP %", 20));
            }

            this.Menu = autoLanternMenu;
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            if (EntityManager.Heroes.Allies.Where(x => x.ChampionName == Champion.Thresh.ToString()).FirstOrDefault() == null || Player.ChampionName.Equals("Thresh"))
            {
                return;
            }
            try
            {
                Game.OnUpdate += this.OnUpdate;
                GameObject.OnCreate += this.OnCreate;
                GameObject.OnDelete += this.OnDelete;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        #region Methods
        /// <summary>
        ///     Fired when an object is created
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnCreate(GameObject sender, EventArgs args)
        {
            try
            {
                if (!sender.IsValid || !sender.IsAlly || sender.Type != GameObjectType.obj_AI_Minion)
                {
                    return;
                }
                if (sender.Name.Equals("ThreshLantern", StringComparison.OrdinalIgnoreCase))
                {
                    this.ThreshLantern = sender;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        /// <summary>
        ///     Fired when an object is deleted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnDelete(GameObject sender, EventArgs args)
        {
            try
            {
                if (!sender.IsValid || this.ThreshLantern == null)
                {
                    return;
                }

                if (sender.NetworkId == this.ThreshLantern.NetworkId)
                {
                    this.ThreshLantern = null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void OnUpdate(EventArgs args)
        {
            try
            {
                if (this.Player.IsDead || !this.Menu["ThreshLantern"].Cast<CheckBox>().CurrentValue || this.ThreshLantern == null
                    || !this.ThreshLantern.IsValid)
                {
                    return;
                }

                if (this.Player.HealthPercent < this.ClickBelowHp
                    || this.Menu["ThreshLanternHotkey"].Cast<KeyBind>().CurrentValue)
                {
                    if (this.ThreshLantern.Position.LSDistance(this.Player.Position) <= 500)
                    {
                        this.Player.Spellbook.CastSpell((SpellSlot)62, this.ThreshLantern);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion
    }
}