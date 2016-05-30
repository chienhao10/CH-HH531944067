using EloBuddy;
using ExorSDK.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;

namespace ExorSDK.Champions.Nautilus
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
            Vars.Q = new Spell(SpellSlot.Q, 950f);
            Vars.W = new Spell(SpellSlot.W, GameObjects.Player.BoundingRadius * 2 + 175f);
            Vars.E = new Spell(SpellSlot.E, 600f);
            Vars.R = new Spell(SpellSlot.R, 825f);

            Vars.Q.SetSkillshot(0.25f, 90f, 2000f, true, SkillshotType.SkillshotLine);
        }
    }
}