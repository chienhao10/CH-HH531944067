using System;
using EloBuddy;
using ExorAIO.Utilities;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Darius
{
    /// <summary>
    ///     The spell class.
    /// </summary>
    internal class Spells
    {
        /// <summary>
        ///     Sets the spells.
        /// </summary>
        public static void Initialize()
        {
            Variables.Q = new Spell(SpellSlot.Q, 425f);
            Variables.W = new Spell(SpellSlot.W, 200f);
            Variables.E = new Spell(SpellSlot.E, 500f);
            Variables.R = new Spell(SpellSlot.R, 460f);

            Variables.E.SetSkillshot(0.25f, (float) (80f*Math.PI/180), float.MaxValue, false,
                SkillshotType.SkillshotCone);
            Variables.R.SetTargetted(0.5f, float.MaxValue);
        }
    }
}