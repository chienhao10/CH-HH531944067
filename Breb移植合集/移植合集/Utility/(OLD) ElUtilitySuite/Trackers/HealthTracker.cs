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

    using Color = System.Drawing.Color;
    using Font = SharpDX.Direct3D9.Font;

    internal class HealthTracker : IPlugin
    {
        #region Fields

        /// <summary>
        ///     The HP bar height
        /// </summary>
        private readonly int BarHeight = 10;

        #endregion

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
            return enemySidebarMenu[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return enemySidebarMenu[item].Cast<Slider>().CurrentValue;
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

        private int HudSpacing
        {
            get
            {
                return getSliderItem("HealthTracker.Spacing");
            }
        }

        /// <summary>
        ///     Gets the right offset of the HUD elements
        /// </summary>
        private int HudOffsetRight
        {
            get
            {
                return getSliderItem("HealthTracker.OffsetRight");
            }
        }

        /// <summary>
        ///     Gets the top offset between the HUD elements
        /// </summary>
        private int HudOffsetTop
        {
            get
            {
                return getSliderItem("HealthTracker.OffsetTop");
            }
        }

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        /// 
        public static Menu rootMenu = ElUtilitySuite.Entry.menu;
        public static Menu enemySidebarMenu;
        public void CreateMenu(Menu rootMenu)
        {
            enemySidebarMenu = rootMenu.AddSubMenu("Enemy healthbars", "healthenemies");
            enemySidebarMenu.Add("DrawHealth_", new CheckBox("Activated"));
            enemySidebarMenu.Add("HealthTracker.OffsetTop", new Slider("Offset Top", 75, 0, 500));
            enemySidebarMenu.Add("HealthTracker.OffsetRight", new Slider("Offset Right", 170, 0, 500));
            enemySidebarMenu.Add("HealthTracker.Spacing", new Slider("Spacing", 10, 0, 30));
            enemySidebarMenu.Add("FontSize", new Slider("Font size", 15, 13, 30));

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
                    Quality = FontQuality.Antialiased
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
            if (!getCheckBoxItem("DrawHealth_"))
            {
                return;
            }

            float i = 0;

            foreach (var hero in HeroManager.Enemies.Where(x => !x.IsDead))
            {
                var champion = hero.ChampionName;
                if (champion.Length > 12)
                {
                    champion = champion.Remove(7) + "...";
                }

                Font.DrawText(
                    null,
                    champion,
                    (int)
                    ((float)(Drawing.Width - HudOffsetRight) - 60f
                     - Font.MeasureText(null, champion, FontDrawFlags.Right).Width / 2f),
                    (int)
                    (HudOffsetTop + i + 4
                     - Font.MeasureText(null, champion, FontDrawFlags.Right).Height / 2f),
                    hero.HealthPercent > 0 ? new ColorBGRA(255, 255, 255, 255) : new ColorBGRA(244, 8, 8, 255));

                this.DrawRect(
                    Drawing.Width - HudOffsetRight,
                    HudOffsetTop + i,
                    100,
                    this.BarHeight,
                    1,
                    Color.FromArgb(255, 51, 55, 51));

                this.DrawRect(
                    Drawing.Width - HudOffsetRight,
                    HudOffsetTop + i,
                    hero.HealthPercent <= 0 ? 100 : (int)(hero.HealthPercent),
                    this.BarHeight,
                    1,
                    hero.HealthPercent < 30 && hero.HealthPercent > 0 ? Color.FromArgb(255, 230, 169, 14) : hero.HealthPercent <= 0 ? Color.FromArgb(255, 206, 20, 30) : Color.FromArgb(255, 29, 201, 38));

                i += 20f + this.HudSpacing;
            }
        }

        /// <summary>
        ///     Draws a rectangle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="thickness"></param>
        /// <param name="color"></param>
        private void DrawRect(float x, float y, int width, float height, float thickness, Color color)
        {
            for (var i = 0; i < height; i++)
            {
                Drawing.DrawLine(x, y + i, x + width, y + i, thickness, color);
            }
        }

        #endregion
    }
}