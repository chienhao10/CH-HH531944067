// <copyright file="TargetSelectorMode.cs" company="LeagueSharp">
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
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using LeagueSharp.SDK.Core.Utils;
    using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;
using EloBuddy;
    /// <summary>
    ///     The mode menu for the TargetSelector
    /// </summary>
    public class TargetSelectorMode
    {
        #region Fields


        private readonly List<ITargetSelectorMode> pEntries = new List<ITargetSelectorMode>();

        private ITargetSelectorMode current;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TargetSelectorMode" /> class.
        /// </summary>
        /// <param name="menu">
        ///     The menu.
        /// </param>


        #endregion

        #region Delegates

        /// <summary>
        ///     The<see cref="OnChange" /> event delegate.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        public delegate void OnChangeDelegate(object sender, ITargetSelectorMode e);

        #endregion

        #region Public Events

        /// <summary>
        ///     Occurs when the mode is changed.
        /// </summary>
        public event OnChangeDelegate OnChange;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the current.
        /// </summary>

        /// <summary>
        ///     Gets the entries.
        /// </summary>
        public ReadOnlyCollection<ITargetSelectorMode> Entries => this.pEntries.AsReadOnly();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Deregisters the specified Mode.
        /// </summary>
        /// <param name="mode">
        ///     The Mode.
        /// </param>

        /// <summary>
        /// <summary>
        ///     Overwrites the specified old Mode.
        /// </summary>
        /// <param name="oldMode">
        ///     The old Mode.
        /// </param>
        /// <param name="newMode">

        /// <summary>
        ///     Registers the specified Mode.
        /// </summary>
        /// <param name="mode">
        ///     The Mo

        #endregion

        #region Methods

        /// <summary>
        ///     Ges the index of the mode by selected.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <returns>
        ///     The <see cref="ITargetSelectorMode" />.
        /// </returns>
        private ITargetSelectorMode GeModeBySelectedIndex(int index)
        {
            return index < this.pEntries.Count && index >= 0 ? this.pEntries[index] : null;
        }

        /// <summar

        #endregion
    }
}