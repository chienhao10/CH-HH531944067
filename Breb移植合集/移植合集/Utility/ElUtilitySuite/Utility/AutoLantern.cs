namespace ElUtilitySuite.Utility
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
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
                return getSliderItem(this.Menu, "ThreshLanternHPSlider");
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
            var autoLanternMenu = rootMenu.AddSubMenu("锤石灯笼", "Threshlantern");
            {
                autoLanternMenu.Add("ThreshLantern", new CheckBox("自动点击灯笼"));
                autoLanternMenu.Add("ThreshLanternHotkey", new KeyBind("点击灯笼按键", false, KeyBind.BindTypes.HoldActive, 'M'));
                autoLanternMenu.Add("ThreshLanternHPSlider", new Slider("当血量% 时自动点击灯笼", 20));
            }

            this.Menu = autoLanternMenu;
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
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
                if (this.Player.IsDead || !getCheckBoxItem(this.Menu, "ThreshLantern") || this.ThreshLantern == null
                    || !this.ThreshLantern.IsValid)
                {
                    return;
                }

                if (this.Player.HealthPercent < this.ClickBelowHp
                    || getKeyBindItem(this.Menu, "ThreshLanternHotkey"))
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