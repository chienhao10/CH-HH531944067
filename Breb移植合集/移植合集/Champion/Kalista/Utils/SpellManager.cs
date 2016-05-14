using System.Collections.Generic;
using EloBuddy;
using LeagueSharp.Common;

namespace iKalistaReborn.Utils
{
    /// <summary>
    ///     The Spell Manager
    /// </summary>
    internal class SpellManager
    {

        /// <summary>
        ///     The dictionary to call the Spell slot and the Spell Class
        /// </summary>
        public static Dictionary<SpellSlot, Spell> Spell = new Dictionary<SpellSlot, Spell>
        {
            {SpellSlot.Q, new Spell(SpellSlot.Q, 1130)},
            {SpellSlot.W, new Spell(SpellSlot.W, 5200)},
            {SpellSlot.E, new Spell(SpellSlot.E, 950)},
            {SpellSlot.R, new Spell(SpellSlot.R, 1200)}
        };

        /// <summary>
        ///     Initializes static members of the <see cref="SpellManager" /> class.
        /// </summary>
        static SpellManager()
        {
            // Spell initialization
            Spell[SpellSlot.Q].SetSkillshot(0.25f, 40f, 1200f, true, SkillshotType.SkillshotLine);
            Spell[SpellSlot.R].SetSkillshot(0.50f, 1500f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }
    }
}