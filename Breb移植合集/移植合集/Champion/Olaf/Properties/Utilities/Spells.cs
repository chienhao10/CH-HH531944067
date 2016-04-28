using EloBuddy;
using ExorAIO.Utilities;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Olaf
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
            Variables.Q = new Spell(SpellSlot.Q, 950f);
            Variables.W = new Spell(SpellSlot.W);
            Variables.E = new Spell(SpellSlot.E, 325f);
            Variables.R = new Spell(SpellSlot.R);

            Variables.Q.SetSkillshot(0.25f, 105f, 1600f, false, SkillshotType.SkillshotLine);
        }
    }
}