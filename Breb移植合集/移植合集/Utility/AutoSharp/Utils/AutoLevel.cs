#region LICENSE

/*
 Copyright 2014 - 2015 LeagueSharp
 AutoLevel.cs is part of LeagueSharp.Common.
 
 LeagueSharp.Common is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.
 
 LeagueSharp.Common is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 GNU General Public License for more details.
 
 You should have received a copy of the GNU General Public License
 along with LeagueSharp.Common. If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

#region

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using EloBuddy;

#endregion

namespace AutoSharp.Utils
{
    /// <summary>
    /// Automatically levels skills.
    /// </summary>
    public partial class AutoLevel
    {
        /// <summary>
        /// The order
        /// </summary>
        private static List<int> order = new List<int>();

        /// <summary>
        /// The last leveled
        /// </summary>
        private static float LastLeveled;

        /// <summary>
        /// The next delay
        /// </summary>
        private static float NextDelay;

        /// <summary>
        /// The player
        /// </summary>
        private static readonly AIHeroClient Player = ObjectManager.Player;

        /// <summary>
        /// The random number
        /// </summary>
        private static System.Random RandomNumber;

        /// <summary>
        /// The enabled
        /// </summary>
        private static bool enabled;

        /// <summary>
        /// The initialize
        /// </summary>
        private static bool init;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoLevel"/> class.
        /// </summary>
        /// <param name="levels">The levels.</param>
        public AutoLevel(IEnumerable<int> levels)
        {
            UpdateSequence(levels);
            Init();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private static void Init()
        {
            if (init)
            {
                return;
            }

            init = true;
            RandomNumber = new System.Random(Environment.TickCount);
            Game.OnUpdate += Game_OnGameUpdate;
        }

        /// <summary>
        /// Fired when the game updates.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (!enabled || Player.SpellTrainingPoints < 1 || Environment.TickCount - LastLeveled < NextDelay || Shop.IsOpen)
            {
                return;
            }

            NextDelay = RandomNumber.Next(300, 1200);
            LastLeveled = Environment.TickCount;
            // subtract 1 from spell int cuz in enum q=0 but it looks better to have q=1, w=2 etc.
            var spell = (SpellSlot)(order[ObjectManager.Player.Level-1] - 1);
            if (ObjectManager.Player.Spellbook.GetSpell(spell).Level < 5)
            {
                Player.Spellbook.LevelSpell(spell);
            }
            else
            {
               var list = new List<SpellDataInst> {Player.Spellbook.GetSpell(SpellSlot.Q), Player.Spellbook.GetSpell(SpellSlot.W), Player.Spellbook.GetSpell(SpellSlot.E), Player.Spellbook.GetSpell(SpellSlot.R)};
                var spellWithLowestLevel = list.OrderBy(entry => entry.Level).FirstOrDefault();
                if (spellWithLowestLevel != null)
                {
                Player.Spellbook.LevelSpell(spellWithLowestLevel.Slot);
}
            }
        }

        /// <summary>
        /// Gets the total points.
        /// </summary>
        /// <returns></returns>
        private static int GetTotalPoints()
        {
            var spell = Player.Spellbook;
            var q = spell.GetSpell(SpellSlot.Q).Level;
            var w = spell.GetSpell(SpellSlot.W).Level;
            var e = spell.GetSpell(SpellSlot.E).Level;
            var r = spell.GetSpell(SpellSlot.R).Level;

            return q + w + e + r;
        }

        /// <summary>
        /// Enables this instance.
        /// </summary>
        public void Enable()
        {
            enabled = true;
        }

        /// <summary>
        /// Disables this instance.
        /// </summary>
        public static void Disable()
        {
            enabled = false;
        }

        /// <summary>
        /// Sets if this instance is enabled or not according to the <paramref name="b"/>.
        /// </summary>
        /// <param name="b">if set to <c>true</c> [b].</param>
        public static void Enabled(bool b)
        {
            enabled = b;
        }

        /// <summary>
        /// Updates the sequence.
        /// </summary>
        /// <param name="levels">The levels.</param>
        public static void UpdateSequence(IEnumerable<int> levels)
        {
            Init();
            order.Clear();
            order = levels.ToList();
        }

        /// <summary>
        /// Gets the sequence.
        /// </summary>
        /// <returns></returns>
        public static int[] GetSequence()
        {
            return order.Select(spell => spell).ToArray();
        }

        /// <summary>
        /// Gets the sequence list.
        /// </summary>
        /// <returns></returns>
        public static List<int> GetSequenceList()
        {
            return order;
        }
    }
}
