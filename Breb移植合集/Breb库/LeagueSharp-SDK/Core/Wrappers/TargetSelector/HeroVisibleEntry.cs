// <copyright file="HeroVisibleEntry.cs" company="LeagueSharp">
//    Copyright (c) 2015 LeagueSharp.
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/
// </copyright>

namespace LeagueSharp.SDK
{
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK;
    using EloBuddy;
    /// <summary>
    ///     The hero visible entry container.
    /// </summary>
    internal class HeroVisibleEntry
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="HeroVisibleEntry" /> class.
        /// </summary>
        /// <param name="hero">
        ///     The hero.
        /// </param>
        public HeroVisibleEntry(AIHeroClient hero)
        {
            this.Hero = hero;
            this.LastVisibleChangeTick = Variables.TickCount;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the hero.
        /// </summary>
        public AIHeroClient Hero { get; }

        /// <summary>
        ///     Gets or sets the last visible change tick.
        /// </summary>
        public int LastVisibleChangeTick { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this hero is visible.
        /// </summary>
        public bool Visible { get; set; }

        #endregion
    }
}