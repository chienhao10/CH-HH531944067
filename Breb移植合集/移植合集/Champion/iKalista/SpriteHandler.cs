// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpriteHandler.cs" company="LeagueSharp">
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
// <summary>
//   Sprite Handler
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace IKalista
{
    using System.Linq;


    using PortAIO.Properties;
    using EloBuddy;
    using EloBuddy.SDK;
    using SharpDX;
    using LeagueSharp.Common;

    /// <summary>
    ///     Sprite Handler
    /// </summary>
    internal class SpriteHandler
    {
        #region Properties

        /// <summary>
        /// Gets the draw position.
        /// </summary>
        private static Vector2 DrawPosition
        {
            get
            {
                return Target != null
                           ? new Vector2(
                        Drawing.WorldToScreen(Target.Position).X - Target.BoundingRadius * 2 +
                        Target.BoundingRadius / 1.5f,
                        Drawing.WorldToScreen(Target.Position).Y - Target.BoundingRadius * 2) : Vector2.Zero;
            }
        }

        /// <summary>
        /// Gets a value indicating whether draw sprite.
        /// </summary>
        private static bool DrawSprite
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the target.
        /// </summary>
        private static AIHeroClient Target
        {
            get
            {
                return
                    ObjectManager.Get<AIHeroClient>()
                        .OrderBy(x => x.Health)
                        .FirstOrDefault(x => x.IsValidTarget(1000f) && x.HasBuff("KalistaCoopStrikeProtect"));
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Loads the sprite.
        /// </summary>
        public static void LoadSprite()
        {
            new Render.Sprite(Resources.ScopeSprite, new Vector2())
            {
                PositionUpdate = () => DrawPosition,
                Scale = new Vector2(1f, 1f),
                VisibleCondition = sender => DrawSprite
            }.Add();
        }

        #endregion
    }
}