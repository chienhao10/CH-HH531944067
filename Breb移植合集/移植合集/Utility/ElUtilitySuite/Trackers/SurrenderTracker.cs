namespace ElUtilitySuite.Trackers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using SharpDX;
    using LeagueSharp.Common;
    using SharpDX.Direct3D9;

    // Creds on KappaUtility

    internal class Surrender : IPlugin
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        public Menu Menu { get; set; }

        #endregion

        public bool getCheckBoxItem(string item)
        {
            return Menu[item].Cast<CheckBox>().CurrentValue;
        }

        public int getSliderItem(string item)
        {
            return Menu[item].Cast<Slider>().CurrentValue;
        }

        #region Properties

        /// <summary>
        ///     Gets or sets the font.
        /// </summary>
        /// <value>
        ///     The font.
        /// </value>
        private static Font Font { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>

        public void CreateMenu(Menu rootMenu)
        {
            var surrenderMenu = rootMenu.AddSubMenu("Surrender Tracker", "surrender");
            surrenderMenu.Add("Trackally", new CheckBox("Ally"));
            surrenderMenu.Add("Trackenemy", new CheckBox("Enemies"));

            this.Menu = surrenderMenu;
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            Obj_AI_Base.OnSurrender += Obj_AI_Base_OnSurrender;
        }

        #endregion

        #region Methods

        private void Obj_AI_Base_OnSurrender(Obj_AI_Base sender, Obj_AI_BaseSurrenderVoteEventArgs args)
        {
            if (sender == null)
            {
                return;
            }

            if ((sender.IsAlly || sender.IsMe) && getCheckBoxItem("Trackally"))
            {
                if (args.Type == SurrenderVoteType.Yes)
                {
                    Chat.Print("[Ally] " + sender.BaseSkinName + " voted yes on surrender.", System.Drawing.Color.Green);
                }

                if (args.Type == SurrenderVoteType.No)
                {
                    Chat.Print("[Ally] " + sender.BaseSkinName + " voted no on surrender", System.Drawing.Color.Green);
                }
            }

            if (sender.IsEnemy && getCheckBoxItem("Trackenemy"))
            {
                if (args.Type == SurrenderVoteType.Yes)
                {
                    Chat.Print("[Enemy] " + sender.BaseSkinName + " voted yes on surrender", System.Drawing.Color.Red);
                }

                if (args.Type == SurrenderVoteType.No)
                {
                    Chat.Print("[Enemy] " + sender.BaseSkinName + " voted no on surrender", System.Drawing.Color.Red);
                }
            }
        }

        #endregion
    }
}