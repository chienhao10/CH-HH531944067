namespace ElUtilitySuite.Trackers
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;
    using SharpDX.Direct3D9;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy;
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
            var buildingMenu = rootMenu.AddSubMenu("防御塔/水晶 记录器", "healthbuilding");
            {
                buildingMenu.Add("DrawHealth", new CheckBox("启用"));
                buildingMenu.Add("DrawPercent", new CheckBox("显示百分比"));
                buildingMenu.Add("DrawTurrets", new CheckBox("防御塔"));
                buildingMenu.Add("DrawInhibs", new CheckBox("水晶"));
                buildingMenu.Add("Turret.FontSize", new Slider("塔字体大小", 13, 13, 30));
            }

            this.Menu = buildingMenu;
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
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            Font = new Font(
                Drawing.Direct3DDevice,
                new FontDescription
                    {
                        FaceName = "Tahoma", Height = getSliderItem(this.Menu, "Turret.FontSize"),
                        OutputPrecision = FontPrecision.Default, Quality = FontQuality.Default
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
                if (!getCheckBoxItem(this.Menu, "DrawHealth"))
                {
                    return;
                }

                var percent = getCheckBoxItem(this.Menu, "DrawPercent");

                if (getCheckBoxItem(this.Menu, "DrawTurrets"))
                {
                    foreach (var turret in
                        ObjectManager.Get<Obj_AI_Turret>()
                            .Where(x => x != null && x.IsValid && !x.IsDead & x.HealthPercent <= 75))
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

                if (getCheckBoxItem(this.Menu, "DrawInhibs"))
                {
                    foreach (var inhibitor in
                        ObjectManager.Get<Obj_BarracksDampener>()
                            .Where(x => x.IsValid && !x.IsDead && x.Health > 1 && x.HealthPercent <= 75))
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
                Console.WriteLine(@"An error occurred: '{0}'", e);
            }
        }

        #endregion
    }
}