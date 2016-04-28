using System;
using EloBuddy;
using ExorAIO.Utilities;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Pantheon
{
    /// <summary>
    ///     The spells class.
    /// </summary>
    internal class Spells
    {
        /// <summary>
        ///     Sets the spells.
        /// </summary>
        public static void Initialize()
        {
            Variables.Q = new Spell(SpellSlot.Q, 600f);
            Variables.W = new Spell(SpellSlot.W, 600f);
            Variables.E = new Spell(SpellSlot.E, ObjectManager.Player.BoundingRadius + 600f);

            Variables.E.SetSkillshot(0f, (float) (35f*Math.PI/180), float.MaxValue, false, SkillshotType.SkillshotCone);
        }
    }
}