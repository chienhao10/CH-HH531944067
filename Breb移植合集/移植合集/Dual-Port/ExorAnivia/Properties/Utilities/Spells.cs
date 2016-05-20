using LeagueSharp;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Anivia
{
    using System;
    using ExorAIO.Utilities;
    using EloBuddy;

    /// <summary>
    ///     The spell class.
    /// </summary>
    class Spells
    {
        /// <summary>
        ///     Initializes the spells.
        /// </summary>
        public static void Initialize()
        {
            Variables.Q = new Spell(SpellSlot.Q, 1100f);
            Variables.W = new Spell(SpellSlot.W, 1000f);
            Variables.E = new Spell(SpellSlot.E, 650f);
            Variables.R = new Spell(SpellSlot.R, 625f);

            Variables.Q.SetSkillshot(0.25f, 110f, 850f, false, SkillshotType.SkillshotLine);
            Variables.W.SetSkillshot(0f, 100f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            Variables.E.SetTargetted(0.25f, 1200f);
            Variables.R.SetSkillshot(0.25f, 400f, 1600f, false, SkillshotType.SkillshotCircle);
        }
    }
}
