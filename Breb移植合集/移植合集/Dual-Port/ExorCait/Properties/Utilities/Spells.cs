using EloBuddy;
using ExorSDK.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;

namespace ExorSDK.Champions.Caitlyn
{
    /// <summary>
    ///     The settings class.
    /// </summary>
    internal class Spells
    {
        /// <summary>
        ///     Initializes the spells.
        /// </summary>
        public static void Initialize()
        {
            Vars.Q = new Spell(SpellSlot.Q, 1250f);
            Vars.Q2 = new Spell(SpellSlot.Q, 1250f);
            Vars.W = new Spell(SpellSlot.W, 800f);
            Vars.E = new Spell(SpellSlot.E, 750f);
            Vars.R = new Spell(SpellSlot.R, 1500f + 500f * GameObjects.Player.Spellbook.GetSpell(SpellSlot.R).Level);

            Vars.Q.SetSkillshot(0.65f, 40f, 2200f, false, SkillshotType.SkillshotLine);
            Vars.Q2.SetSkillshot(0.65f, Vars.Q.Width * 2, 2200f, false, SkillshotType.SkillshotLine);
            Vars.E.SetSkillshot(0.25f, 70f, 1600f, true, SkillshotType.SkillshotLine);
        }
    }
}