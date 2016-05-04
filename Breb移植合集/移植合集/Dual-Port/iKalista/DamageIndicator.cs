// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DamageIndicator.cs" company="LeagueSharp">
//   Copyright (C) 2015 LeagueSharp
//   
//             This program is free software: you can redistribute it and/or modify
//             it under the terms of the GNU General Public License as published by
//             the Free Software Foundation, either version 3 of the License, or
//             (at your option) any later version.
//   
//             This program is distributed in the hope that it will be useful,
//             but WITHOUT ANY WARRANTY; without even the implied warranty of
//             MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//             GNU General Public License for more details.
//   
//             You should have received a copy of the GNU General Public License
//             along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace IKalista
{
    using System;
    using System.Linq;
    using EloBuddy;
    using EloBuddy.SDK;
    using SharpDX;
    using Color = System.Drawing.Color;
    using LeagueSharp.Common;

    /// <summary>
    ///     TODO The custom damage indicator.
    /// </summary>
    public class CustomDamageIndicator
    {
        #region Constants

        /// <summary>
        ///     TODO The ba r_ width.
        /// </summary>
        private const int BarWidth = 104;

        /// <summary>
        ///     TODO The lin e_ thickness.
        /// </summary>
        private const int LineThickness = 9;

        #endregion

        #region Static Fields

        /// <summary>
        ///     TODO The bar offset.
        /// </summary>
        private static readonly Vector2 BarOffset = new Vector2(10, 25);

        /// <summary>
        ///     TODO The damage to unit.
        /// </summary>
        private static LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnitDelegate damageToUnit;

        /// <summary>
        ///     TODO The _drawing color.
        /// </summary>
        private static Color drawingColor;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the drawing color.
        /// </summary>
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

        /// <summary>
        ///     Gets or sets a value indicating whether enabled.
        /// </summary>
        public static bool Enabled { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// TODO The initialize.
        /// </summary>
        /// <param name="damageToUnit">
        /// TODO The damage to unit.
        /// </param>
        public static void Initialize(LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnitDelegate damageToUnit)
        {
            // Apply needed field delegate for damage calculation
            CustomDamageIndicator.damageToUnit = damageToUnit;
            DrawingColor = Color.LawnGreen;
            Enabled = true;

            // Register event handlers
            Drawing.OnDraw += Drawing_OnDraw;
        }

        #endregion

        #region Methods

        /// <summary>
        /// TODO The drawing_ on draw.
        /// </summary>
        /// <param name="args">
        /// TODO The args.
        /// </param>
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!Enabled)
            {
                return;
            }

            foreach (var unit in HeroManager.Enemies.Where(u => u.IsValidTarget() && u.IsHPBarRendered))
            {
                // Get damage to unit
                var damage = damageToUnit(unit);

                // Continue on 0 damage
                if (damage <= 0)
                {
                    continue;
                }

                // Get remaining HP after damage applied in percent and the current percent of health
                var damagePercentage = ((unit.Health - damage) > 0 ? (unit.Health - damage) : 0) / unit.MaxHealth;
                var currentHealthPercentage = unit.Health / unit.MaxHealth;

                // Calculate start and end point of the bar indicator
                var startPoint = new Vector2(
                    (int)(unit.HPBarPosition.X + BarOffset.X + (damagePercentage * BarWidth)), 
                    (int)(unit.HPBarPosition.Y + BarOffset.Y) - 5);
                var endPoint =
                    new Vector2(
                        (int)(unit.HPBarPosition.X + BarOffset.X + (currentHealthPercentage * BarWidth) + 1), 
                        (int)(unit.HPBarPosition.Y + BarOffset.Y) - 5);

                // Draw the line
                Drawing.DrawLine(startPoint, endPoint, LineThickness, DrawingColor);
            }
        }

        #endregion
    }
}