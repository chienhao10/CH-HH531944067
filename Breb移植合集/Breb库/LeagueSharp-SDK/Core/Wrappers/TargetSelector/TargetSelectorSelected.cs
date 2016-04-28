// <copyright file="TargetSelectorSelected.cs" company="LeagueSharp">
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
    using System.Linq;
    using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;
using EloBuddy;

    /// <summary>
    ///     Manages the selection of targets
    /// </summary>
    public class TargetSelectorSelected
    {
        #region Fields

        /// <summary>
        ///     The menu.
        /// </summary>

        /// <summary>
        ///     The focus.
        /// </summary>
        private bool focus = true;

        /// <summary>
        ///     The force.
        /// </summary>
        private bool force;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TargetSelectorSelected" /> class.
        /// </summary>
        /// <param name="menu">
        ///     The menu.
        /// </param>

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the click buffer.
        /// </summary>
        public float ClickBuffer { get; set; } = 100f;

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="TargetSelectorSelected" /> is focus.
        /// </summary>
        public bool Focus
        {
            get
            {
                return this.focus;
            }

            set
            {
                this.focus = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="TargetSelectorSelected" /> is force.
        /// </summary>
        public bool Force
        {
            get
            {
                return this.force;
            }

            set
            {
                this.force = value;
            }
        }

        /// <summary>
        ///     Gets or sets the target.
        /// </summary>
        public AIHeroClient Target { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Raises the <see cref="E:GameWndProc" /> event.
        /// </summary>
        /// <param name="args">
        ///     The <see cref="LeagueSharp.WndEventArgs" /> instance containing the event data.
        /// </param>
        private void OnGameWndProc(WndEventArgs args)
        {
            if (args.Msg != (ulong)WindowsMessages.LBUTTONDOWN)
            {
                return;
            }

            this.Target =
                GameObjects.EnemyHeroes.Where(
                    h => h.IsValidTarget() && h.Distance(Game.CursorPos) < h.BoundingRadius + this.ClickBuffer)
                    .OrderBy(h => h.Distance(Game.CursorPos))
                    .FirstOrDefault();
        }

        #endregion
    }
}