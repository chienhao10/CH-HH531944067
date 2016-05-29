/*
 Copyright 2015 - 2015 SPrediction
 StasisPrediction.cs is part of SPrediction
 
 SPrediction is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.
 
 SPrediction is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 GNU General Public License for more details.
 
 You should have received a copy of the GNU General Public License
 along with SPrediction. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EloBuddy;
//using EloBuddy.SDK;
using LeagueSharp.Common;
using Spell = LeagueSharp.Common.Spell;

namespace SPrediction
{
    /// <summary>
    ///     StasisPrediction class
    /// </summary>
    public static class StasisPrediction
    {
        private static List<Tuple<string, int>> s_StasisBuffs;
        private static List<Spell> s_RegisteredSpells;
        private static List<Stasis> s_DetectedStasises;

        public static EventHandler<Result> OnGuaranteedHit;

        /// <summary>
        ///     Initializes stasis prediction
        /// </summary>
        public static void Initialize()
        {
            s_StasisBuffs = new List<Tuple<string, int>>
            {
                new Tuple<string, int>("bardrstasis", 2500),
                new Tuple<string, int>("lissandrarself", 1500),
                new Tuple<string, int>("zhonya", 2500),
                new Tuple<string, int>("chronorevive", 2000),
                new Tuple<string, int>("aatroxpassivedeath", 3000),
                new Tuple<string, int>("rebirth", 6000)
            };

            s_RegisteredSpells = new List<Spell>();
            s_DetectedStasises = new List<Stasis>();

            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffAdd;
        }

        /// <summary>
        ///     OnUpdate event
        /// </summary>
        /// <param name="args">The args.</param>
        private static void Game_OnUpdate(EventArgs args)
        {
            s_DetectedStasises.RemoveAll(p => Utils.TickCount - p.StartTick > p.Duration + 500);
            foreach (var stasis in s_DetectedStasises)
            {
                if (!stasis.Processed)
                {
                    foreach (var spell in s_RegisteredSpells)
                        stasis.Process(spell);
                }
            }
        }

        /// <summary>
        ///     Registers the spell for stasis calculations
        /// </summary>
        /// <param name="s">The spell.</param>
        public static void RegisterSpell(Spell s)
        {
            if (!s_RegisteredSpells.Contains(s))
                s_RegisteredSpells.Add(s);
        }

        /// <summary>
        ///     Unregisters spell
        /// </summary>
        /// <param name="s"></param>
        public static void UnregisterSpell(Spell s)
        {
            if (s_RegisteredSpells.Contains(s))
                s_RegisteredSpells.Remove(s);
        }

        /// <summary>
        ///     OnBuffAdd Event
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        private static void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (sender.IsChampion() && sender.IsEnemy)
            {
                var stasis = s_StasisBuffs.FirstOrDefault(p => args.Buff.Name.Contains(p.Item1));
                if (stasis != null)
                    s_DetectedStasises.Add(new Stasis
                    {
                        Unit = sender,
                        StartTick = Utils.TickCount,
                        Duration = stasis.Item2,
                        Name = stasis.Item1,
                        Processed = false
                    });
            }
        }

        /// <summary>
        ///     Stasis prediction result
        /// </summary>
        public class Result : EventArgs
        {
            /// <summary>
            ///     The prediction result.
            /// </summary>
            public Prediction.Result Prediction;

            /// <summary>
            ///     The spell.
            /// </summary>
            public Spell Spell;
        }

        /// <summary>
        ///     Stasis class
        /// </summary>
        internal class Stasis
        {
            /// <summary>
            ///     The duration of stasis
            /// </summary>
            internal int Duration;

            /// <summary>
            ///     The name of stasis
            /// </summary>
            internal string Name;

            /// <summary>
            /// </summary>
            internal bool Processed;

            /// <summary>
            ///     The start tick of stasis
            /// </summary>
            internal int StartTick;

            /// <summary>
            ///     The Unit
            /// </summary>
            internal Obj_AI_Base Unit;

            /// <summary>
            ///     Stasis calculations
            /// </summary>
            /// <param name="spell">The spell.</param>
            internal void Process(Spell spell)
            {
                var arrivalT =
                    Prediction.GetArrivalTime(spell.From.LSDistance(Unit.ServerPosition), spell.Delay, spell.Speed)*1000f;
                if (Utils.TickCount - StartTick >= Duration - arrivalT)
                {
                    var pred = new Prediction.Result
                    {
                        Input = new Prediction.Input(Unit, spell.Delay, spell.Speed, spell.Width, spell.Range,
                            spell.Collision, spell.Type, spell.From, spell.RangeCheckFrom),
                        Unit = Unit,
                        CastPosition = Unit.ServerPosition.LSTo2D()
                    };
                    pred.UnitPosition = pred.CastPosition;
                    pred.HitChance = HitChance.VeryHigh;
                    pred.Lock(false);

                    var result = new Result
                    {
                        Spell = spell,
                        Prediction = pred
                    };

                    if (OnGuaranteedHit != null && pred.HitChance != HitChance.Collision &&
                        pred.HitChance != HitChance.OutOfRange)
                        OnGuaranteedHit(MethodBase.GetCurrentMethod().DeclaringType, result);

                    Processed = true;
                }
            }
        }
    }
}