// <copyright file="OrbwalkerSelector.cs" company="LeagueSharp">
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using EloBuddy;
    using LeagueSharp.SDK.Core.Utils;

    /// <summary>
    ///     The target selecting system for <c>Orbwalker</c>.
    /// </summary>
    internal class OrbwalkerSelector
    {
        #region Constants

        /// <summary>
        ///     The lane clear wait time.
        /// </summary>
        private const float LaneClearWaitTime = 2f;

        #endregion

        #region Fields

        /// <summary>
        ///     The clones
        /// </summary>
        private readonly string[] clones = { "shaco", "monkeyking", "leblanc" };

        /// <summary>
        ///     The ignored minions
        /// </summary>
        private readonly string[] ignoreMinions = { "jarvanivstandard" };

        /// <summary>
        ///     The <see cref="Orbwalker" /> class.
        /// </summary>
        //private readonly Orbwalker orbwalker;

        /// <summary>
        ///     The special minions
        /// </summary>
        private readonly string[] specialMinions =
            {
                "zyrathornplant", "zyragraspingplant", "heimertyellow",
                "heimertblue", "malzaharvoidling", "yorickdecayedghoul",
                "yorickravenousghoul", "yorickspectralghoul", "shacobox",
                "annietibbers", "teemomushroom", "elisespiderling"
            };

        #endregion

        #region Constructors and Destructors

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the forced target.
        /// </summary>
        public AttackableUnit ForceTarget { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the last minion used for lane clear.
        /// </summary>
        private Obj_AI_Base LaneClearMinion { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the enemy minions.
        /// </summary>
        /// <param name="range">
        ///     The range.
        /// </param>
        /// <returns>
        ///     The <see cref="List{T}" /> of <see cref="Obj_AI_Minion" />.
        /// </returns>
        public List<Obj_AI_Minion> GetEnemyMinions(float range = 0)
        {
            return
                GameObjects.EnemyMinions.Where(
                    m => this.IsValidUnit(m, range) && !this.ignoreMinions.Any(b => b.Equals(m.CharData.BaseSkinName)))
                    .ToList();
        }

        /// <summary>
        ///     Gets the target.
        /// </summary>
        /// <param name="mode">
        ///     The mode.
        /// </param>
        /// <returns>
        ///     Returns the filtered target.
        /// </returns>
        /// <summary>
        ///     Indicates whether the depended process should wait before executing.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>

        /// <summary>
        ///     Determines if the orbwalker should wait before attacking a minion under turret.
        /// </summary>
        /// <param name="noneKillableMinion">
        ///     The non killable minion.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>

        #endregion

        #region Methods

        /// <summary>
        ///     Orders the enemy minions.
        /// </summary>
        /// <param name="minions">
        ///     The minions.
        /// </param>
        /// <returns>
        ///     The <see cref="List{T}" /> of <see cref="Obj_AI_Minion" />.
        /// </returns>
        private static List<Obj_AI_Minion> OrderEnemyMinions(IEnumerable<Obj_AI_Minion> minions)
        {
            return
                minions?.OrderByDescending(minion => minion.GetMinionType().HasFlag(MinionTypes.Siege))
                    .ThenBy(minion => minion.GetMinionType().HasFlag(MinionTypes.Super))
                    .ThenBy(minion => minion.Health)
                    .ThenByDescending(minion => minion.MaxHealth)
                    .ToList();
        }

        /// <summary>
        ///     Returns possible minions based on settings.
        /// </summary>
        /// <param name="mode">
        ///     The requested mode
        /// </param>
        /// <returns>
        ///     The <see cref="List{Obj_AI_Minion}" />.
        /// </returns>

        /// <summary>
        ///     Determines whether the unit is valid.
        /// </summary>
        /// <param name="unit">
        ///     The unit.
        /// </param>
        /// <param name="range">
        ///     The range.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private bool IsValidUnit(AttackableUnit unit, float range = 0f)
        {
            var minion = unit as Obj_AI_Minion;
            return unit.IsValidTarget(range > 0 ? range : unit.GetRealAutoAttackRange())
                   && (minion == null || minion.IsHPBarRendered);
        }
        

        #endregion
    }
}