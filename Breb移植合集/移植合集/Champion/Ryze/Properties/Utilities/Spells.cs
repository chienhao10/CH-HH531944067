using EloBuddy;
using ExorAIO.Utilities;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Ryze
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
            Variables.Q = new Spell(SpellSlot.Q, 900f);
            Variables.W = new Spell(SpellSlot.W, 600f);
            Variables.E = new Spell(SpellSlot.E, 600f);
            Variables.R = new Spell(SpellSlot.R);

            Variables.Q.SetSkillshot(0.25f, 55f, 1400f, true, SkillshotType.SkillshotLine);
        }
    }
}