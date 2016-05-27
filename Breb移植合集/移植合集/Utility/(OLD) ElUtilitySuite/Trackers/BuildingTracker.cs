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


    internal class BuildingTracker : IPlugin
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

        public static bool getCheckBoxItem(string item)
        {
            return buildingMenu[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return buildingMenu[item].Cast<Slider>().CurrentValue;
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
        public static Menu rootMenu = ElUtilitySuite.Entry.menu;
        public static Menu buildingMenu;
        public void CreateMenu(Menu rootMenu)
        {
            buildingMenu = rootMenu.AddSubMenu("Turrets/Inhib tracker", "healthbuilding");
            buildingMenu.Add("DrawHealth", new CheckBox("Activated"));
            buildingMenu.Add("DrawTurrets", new CheckBox("Turrets"));
            buildingMenu.Add("DrawInhibs", new CheckBox("Inhibitors"));
            buildingMenu.Add("FontSize", new Slider("Font size", 13, 13, 30));

        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            Font = new Font(
                Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Tahoma",
                    Height = getSliderItem("FontSize"),
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.Default
                });

            Drawing.OnEndScene += this.Drawing_OnEndScene;
            Drawing.OnPreReset += args => { Font.OnLostDevice(); };
            Drawing.OnPostReset += args => { Font.OnResetDevice(); };
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fired when the scene is completely rendered.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Drawing_OnEndScene(EventArgs args)
        {
            try
            {
                if (!getCheckBoxItem("DrawHealth"))
                {
                    return;
                }

                if (getCheckBoxItem("DrawTurrets"))
                {
                    foreach (var turret in ObjectManager.Get<Obj_AI_Turret>().Where(x => x != null && x.IsValid && !x.IsDead & x.HealthPercent <= 99))
                    {
                        var turretPosition = Drawing.WorldToMinimap(turret.Position);
                        var healthPercent = string.Format("{0}%", (int)turret.HealthPercent);

                        Font.DrawText(
                            null,
                            healthPercent,
                            (int)
                            (turretPosition.X - Font.MeasureText(null, healthPercent, FontDrawFlags.Center).Width / 2f),
                            (int)
                            (turretPosition.Y - Font.MeasureText(null, healthPercent, FontDrawFlags.Center).Height / 2f),
                            new ColorBGRA(255, 255, 255, 255));
                    }
                }

                if (getCheckBoxItem("DrawInhibs"))
                {
                    foreach (var inhibitor in ObjectManager.Get<Obj_BarracksDampener>().Where(x => x.IsValid && !x.IsDead && x.Health > 1 && x.HealthPercent <= 99))
                    {
                        var turretPosition = Drawing.WorldToMinimap(inhibitor.Position);
                        var healthPercent = string.Format("{0}%", (int)inhibitor.HealthPercent);

                        Font.DrawText(
                            null,
                            healthPercent,
                            (int)
                            (turretPosition.X - Font.MeasureText(null, healthPercent, FontDrawFlags.Center).Width / 2f),
                            (int)
                            (turretPosition.Y - Font.MeasureText(null, healthPercent, FontDrawFlags.Center).Height / 2f),
                            new ColorBGRA(255, 255, 255, 255));
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