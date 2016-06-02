using System;
using ExorSDK.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;

namespace ExorSDK.Champions.Pantheon
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
            Vars.Q = new Spell(SpellSlot.Q, 600f);
            Vars.W = new Spell(SpellSlot.W, 600f);
            Vars.E = new Spell(SpellSlot.E, GameObjects.Player.BoundingRadius + 600f);

            Vars.E.SetSkillshot(0f, (float) (35f * Math.PI / 180), float.MaxValue, false, SkillshotType.SkillshotCone);
        }
    }
}