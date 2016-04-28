using EloBuddy;
using ExorAIO.Utilities;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Renekton
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
            Variables.Q = new Spell(SpellSlot.Q, ObjectManager.Player.BoundingRadius + 225f);
            Variables.W = new Spell(SpellSlot.W, ObjectManager.Player.BoundingRadius + 175f);
            Variables.E = new Spell(SpellSlot.E, 450f);
            Variables.R = new Spell(SpellSlot.R);
        }
    }
}