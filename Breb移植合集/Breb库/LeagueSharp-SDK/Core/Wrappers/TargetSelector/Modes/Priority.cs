// <copyright file="Priority.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK.Modes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;
using EloBuddy;
    using LeagueSharp.SDK.Core.Utils;

    /// <summary>
    ///     The priority Mode.
    /// </summary>
    [ResourceImport]
    public class Priority : ITargetSelectorMode
    {
        #region Constants

        /// <summary>
        ///     The maximum priority
        /// </summary>
        private const int MaxPriority = 5;

        /// <summary>
        ///     The minimum priority
        /// </summary>
        private const int MinPriority = 1;

        #endregion

        #region Static Fields

        /// <summary>
        ///     The priority categories
        /// </summary>
        public static IReadOnlyList<PriorityCategory> PriorityCategories => PriorityCategoriesList;

        [ResourceImport("Data.Priority.json")]
        private static List<PriorityCategory> PriorityCategoriesList = new List<PriorityCategory>();

        #endregion

        #region Fields

        #endregion

        #region Public Properties

        /// <inheritdoc />
        public string DisplayName => "Priorities";

        /// <inheritdoc />
        public string Name => "priorities";

        #endregion

        #region Public Methods and Operators

        /// <inheritdoc />
        /// <summary>
        ///     Gets the default priority.
        /// </summary>
        /// <param name="hero">
        ///     The hero.
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public int GetDefaultPriority(AIHeroClient hero)
        {
            return PriorityCategories.FirstOrDefault(i => i.Champions.Contains(hero.ChampionName))?.Value ?? MinPriority;
        }

        /// <summary>
        ///     Gets the priority.
        /// </summary>
        /// <param name="hero">
        ///     The hero.
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public int GetPriority(AIHeroClient hero)
        {
            return MinPriority;
        }

        /// <inheritdoc />
        public List<AIHeroClient> OrderChampions(List<AIHeroClient> heroes)
        {
            return heroes.OrderByDescending(this.GetPriority).ToList();
        }
        #endregion
    }
}