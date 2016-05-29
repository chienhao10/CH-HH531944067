using EloBuddy;
using ExorSDK.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;

namespace ExorSDK.Champions.Anivia
{
    /// <summary>
    ///     The spell class.
    /// </summary>
    internal class Spells
    {
        /// <summary>
        ///     Initializes the spells.
        /// </summary>
        public static void Initialize()
        {
            Vars.Q = new Spell(SpellSlot.Q, 1100f);
            Vars.W = new Spell(SpellSlot.W, 1000f);
            Vars.E = new Spell(SpellSlot.E, 600f + GameObjects.Player.BoundingRadius);
            Vars.R = new Spell(SpellSlot.R, 750f);

            Vars.Q.SetSkillshot(0.25f, 75f, 850f, false, SkillshotType.SkillshotLine);
            Vars.W.SetSkillshot(0.25f, 100f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            Vars.R.SetSkillshot(0.25f, 150f, 1600f, false, SkillshotType.SkillshotCircle);
        }
    }
}