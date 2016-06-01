using EloBuddy;
using ExorSDK.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;

namespace ExorSDK.Champions.Olaf
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
            Vars.Q = new Spell(SpellSlot.Q, 950f);
            Vars.W = new Spell(SpellSlot.W);
            Vars.E = new Spell(SpellSlot.E, 325f);
            Vars.R = new Spell(SpellSlot.R);

            Vars.Q.SetSkillshot(0.25f, 105f, 1600f, false, SkillshotType.SkillshotLine);
        }
    }
}