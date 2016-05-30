namespace iTwitch
{
    using System;
    using System.Linq;

    using EloBuddy;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;

    class CustomDamageIndicator
    {
        #region Constants

        private const int BarWidth = 104;

        private const int LineThickness = 9;

        #endregion

        #region Static Fields

        private static readonly Vector2 BarOffset = new Vector2(10, 25);

        private static LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnitDelegate _damageToUnit;

        private static Color drawingColor;

        #endregion

        #region Public Properties

        public static Color DrawingColor
        {
            get
            {
                return drawingColor;
            }

            set
            {
                drawingColor = Color.FromArgb(170, value);
            }
        }

        public static bool Enabled { get; set; }

        public static bool EnabledJ { get; set; }

        #endregion

        #region Public Methods and Operators

        public static void Initialize(LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnitDelegate damageToUnit)
        {
            // Apply needed field delegate for damage calculation
            _damageToUnit = damageToUnit;
            DrawingColor = Color.Green;
            Enabled = true;

            // Register event handlers
            Drawing.OnDraw += Drawing_OnDraw;
        }

        #endregion

        #region Methods

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Enabled)
            {
                foreach (var unit in ObjectManager.Get<AIHeroClient>().Where(u => u.LSIsValidTarget() && u.IsHPBarRendered)
                    )
                {
                    // Get damage to unit
                    var damage = _damageToUnit(unit);

                    // Continue on 0 damage
                    if (damage <= 0) continue;

                    // Get remaining HP after damage applied in percent and the current percent of health
                    var damagePercentage = ((unit.Health - damage) > 0 ? (unit.Health - damage) : 0) / unit.MaxHealth;
                    var currentHealthPercentage = unit.Health / unit.MaxHealth;

                    // Calculate start and end point of the bar indicator
                    var startPoint = new Vector2(
                        (int)(unit.HPBarPosition.X + BarOffset.X + damagePercentage * BarWidth),
                        (int)(unit.HPBarPosition.Y + BarOffset.Y) - 5);
                    var endPoint =
                        new Vector2(
                            (int)(unit.HPBarPosition.X + BarOffset.X + currentHealthPercentage * BarWidth) + 1,
                            (int)(unit.HPBarPosition.Y + BarOffset.Y) - 5);

                    // Draw the line
                    Drawing.DrawLine(startPoint, endPoint, LineThickness, DrawingColor);
                }
            }
        }

        #endregion
    }
}