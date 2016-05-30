namespace ElUtilitySuite.Trackers
{
    using System;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Color = System.Drawing.Color;
    using Font = SharpDX.Direct3D9.Font;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy;
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

        #region Properties

        /// <summary>
        ///     Gets or sets the font.
        /// </summary>
        /// <value>
        ///     The font.
        /// </value>
        private static Font Font { get; set; }

        #endregion

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
        ///     Gets the spacing between HUD elements
        /// </summary>
        private int HudSpacing
        {
            get
            {
                return getSliderItem(this.Menu, "HealthTracker.Spacing");
            }
        }

        /// <summary>
        ///     Gets the right offset of the HUD elements
        /// </summary>
        private int HudOffsetRight
        {
            get
            {
                return getSliderItem(this.Menu, "HealthTracker.OffsetRight");
            }
        }

        /// <summary>
        ///     Gets the top offset between the HUD elements
        /// </summary>
        private int HudOffsetTop
        {
            get
            {
                return getSliderItem(this.Menu, "HealthTracker.OffsetTop");
            }
        }

        /// <summary>
        ///     Gets the right offset between text and healthbar
        /// </summary>
        private int HudOffsetText
        {
            get
            {
                return getSliderItem(this.Menu, "HealthTracker.OffsetText");
            }
        }


        

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            var enemySidebarMenu = rootMenu.AddSubMenu("敌方血量记录", "healthenemies");
            {
                enemySidebarMenu.Add("DrawHealth_", new CheckBox("启用"));
                enemySidebarMenu.Add("DrawHealth_percent", new CheckBox("英雄血量 %"));
                enemySidebarMenu.Add("HealthTracker.OffsetText", new Slider("移动文字", 30, 0, 100));
                enemySidebarMenu.Add("HealthTracker.OffsetTop", new Slider("上移动", 75, 0, 1500));
                enemySidebarMenu.Add("HealthTracker.OffsetRight", new Slider("右移动", 170, 0, 1500));
                enemySidebarMenu.Add( "HealthTracker.Spacing", new Slider("间隔", 10, 0, 30));
                enemySidebarMenu.Add("FontSize", new Slider("字体大小", 15, 13, 30));
            }

            this.Menu = enemySidebarMenu;
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
                        FaceName = "Tahoma", Height = getSliderItem(this.Menu, "FontSize"),
                        OutputPrecision = FontPrecision.Default, Quality = FontQuality.Antialiased
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
            if (!getCheckBoxItem(this.Menu, "DrawHealth_"))
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


                var championInfo = getCheckBoxItem(this.Menu, "DrawHealth_percent")
                    ? champion + " (" + (int)hero.HealthPercent + "%)"
                    : champion;

                // Draws the championnames
                Font.DrawText(null, championInfo, (int) ((Drawing.Width - this.HudOffsetRight - this.HudOffsetText - Font.MeasureText(null, championInfo, FontDrawFlags.Left).Width)), (int)(this.HudOffsetTop + i + 4 - Font.MeasureText(null, championInfo, FontDrawFlags.Left).Height / 2f), hero.HealthPercent > 0 ? new ColorBGRA(255, 255, 255, 255) : new ColorBGRA(244, 8, 8, 255));

                // Draws the rectangle
                this.DrawRect(Drawing.Width - this.HudOffsetRight, this.HudOffsetTop + i, 100, this.BarHeight, 1, Color.FromArgb(255, 51, 55, 51));

                // Fils the rectangle
                this.DrawRect(Drawing.Width - this.HudOffsetRight, this.HudOffsetTop + i, hero.HealthPercent <= 0 ? 100 : (int)(hero.HealthPercent), this.BarHeight, 1, hero.HealthPercent < 30 && hero.HealthPercent > 0 ? Color.FromArgb(255, 230, 169, 14) : hero.HealthPercent <= 0 ? Color.FromArgb(255, 206, 20, 30) : Color.FromArgb(255, 29, 201, 38));

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