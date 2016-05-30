using System;
using ExorSDK.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;

namespace ExorSDK.Champions.Darius
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
            Vars.Q = new Spell(SpellSlot.Q, 425f);
            Vars.W = new Spell(SpellSlot.W, Vars.AARange + 25f);
            Vars.E = new Spell(SpellSlot.E, 500f);
            Vars.R = new Spell(SpellSlot.R, 460f);

            Vars.E.SetSkillshot(0.25f, (float) (80f * Math.PI / 180), float.MaxValue, false, SkillshotType.SkillshotCone);
        }
    }
}