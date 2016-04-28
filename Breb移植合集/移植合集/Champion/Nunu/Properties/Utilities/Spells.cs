using EloBuddy;
using ExorAIO.Utilities;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Nunu
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
            Variables.Q = new Spell(SpellSlot.Q, ObjectManager.Player.BoundingRadius + 125f);
            Variables.W = new Spell(SpellSlot.W, 700f);
            Variables.E = new Spell(SpellSlot.E, 550f);
            Variables.R = new Spell(SpellSlot.R, 650f);
        }
    }
}