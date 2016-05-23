// <copyright file="Weight.cs" company="LeagueSharp">
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
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;
    using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using EloBuddy;
    using LeagueSharp.SDK.Core.Utils;


    /// <summary>
    ///     The weight Mode.
    /// </summary>
    public class Weight
    {
        #region Constants

        /// <summary>
        ///     The default percentage const.
        /// </summary>
        private const int DefaultPercentage = 100;

        /// <summary>
        ///     The max percentage const.
        /// </summary>
        private const int MaxPercentage = 200;

        /// <summary>
        ///     The max weight const.
        /// </summary>
        private const int MaxWeight = 20;

        /// <summary>
        ///     THe min percentage const.
        /// </summary>
        private const int MinPercentage = 0;

        /// <summary>
        ///     The min weight const.
        /// </summary>
        private const int MinWeight = 0;

        #endregion

        #region Fields

        /// <summary>
        ///     The weight items.
        /// </summary>
        private readonly List<WeightItemWrapper> pItems = new List<WeightItemWrapper>();
        
        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Weight" /> class.
        /// </summary>
        public Weight()
        {
            var weights =
                Assembly.GetAssembly(typeof(IWeightItem))
                    .GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && typeof(IWeightItem).IsAssignableFrom(t))
                    .ToList();

            foreach (var instance in weights.Select(DynamicInitializer.NewInstance).OfType<IWeightItem>())
            {
                this.pItems.Add(new WeightItemWrapper(instance));
            }

            this.pItems = this.pItems.OrderBy(p => p.DisplayName).ToList();
        }

        #endregion

        #region Public Properties

        /// <inheritdoc />
        public string DisplayName => "Weight";

        /// <summary>
        ///     Gets the items.
        /// </summary>
        public ReadOnlyCollection<WeightItemWrapper> Items => this.pItems.AsReadOnly();

        /// <inheritdoc />
        public string Name => "weight";

        #endregion

        #region Public Methods and Operators

        /// <inheritdoc />


        /// <summary>
        ///     Calculates the weight.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <param name="hero">
        ///     The hero.
        /// </param>
        /// <param name="simulation">Indicates whether to enable simulation.</param>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
        public float Calculate(WeightItemWrapper item, AIHeroClient hero, bool simulation = false)
        {
            var minValue = simulation ? item.SimulationMinValue : item.MinValue;
            var maxValue = simulation ? item.SimulationMaxValue : item.MaxValue;
            if (item.Weight <= MinWeight || maxValue <= 0)
            {
                return MinWeight;
            }

            var minWeight = minValue > 0 ? item.Weight / (maxValue / minValue) : MinWeight;
            var weight = item.Inverted
                             ? item.Weight - (item.Weight * item.GetValue(hero) / maxValue) + minWeight
                             : item.Weight * item.GetValue(hero) / maxValue;
            return float.IsNaN(weight) || float.IsInfinity(weight)
                       ? MinWeight
                       : Math.Min(MaxWeight, Math.Min(item.Weight, Math.Max(MinWeight, Math.Max(weight, minWeight))));
        }
        /// <summary>
        ///     Gets the hero percent.
        /// </summary>
        /// <param name="hero">
        ///     The hero.
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>

        /// <inheritdoc />

        /// <summary>
        ///     Updates the maximum minimum value.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <param name="heroes">
        ///     The heroes.
        /// </param>
        /// <param name="simulation">
        ///     Indicates whether to use simluation.
        /// </param>
        public void UpdateMaxMinValue(WeightItemWrapper item, List<AIHeroClient> heroes, bool simulation = false)
        {
            var min = float.MaxValue;
            var max = float.MinValue;
            foreach (var hero in heroes)
            {
                var value = item.GetValue(hero);
                if (value < min)
                {
                    min = value;
                }

                if (value > max)
                {
                    max = value;
                }
            }

            if (!simulation)
            {
                item.MinValue = Math.Min(max, min);
                item.MaxValue = Math.Max(max, min);
            }
            else
            {
                item.SimulationMinValue = Math.Min(max, min);
                item.SimulationMaxValue = Math.Max(max, min);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Exports the settings.
        /// </summary>
        private void ExportSettings()
        {
            Clipboard.SetText(string.Join("|", this.pItems.Select(i => $"{i.Name}:{i.Weight}").ToArray()));
            Chat.Print("Weights: Exported to clipboard.");
        }

        /// <summary>
        ///     Imports the settings.
        /// </summary>

        /// <summary>
        ///     Resets the sett
        #endregion
    }
}