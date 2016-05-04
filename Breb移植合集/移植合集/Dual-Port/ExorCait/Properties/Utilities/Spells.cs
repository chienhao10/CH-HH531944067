using LeagueSharp;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Caitlyn
{
    using EloBuddy;
    using ExorAIO.Utilities;

    /// <summary>
    ///     The settings class.
    /// </summary>
    class Spells
    {
        /// <summary>
        ///     Initializes the spells.
        /// </summary>
        public static void Initialize()
        {
            Variables.Q = new Spell(SpellSlot.Q, 1250f);
            Variables.Q2 = new Spell(SpellSlot.Q, 1250f);
            Variables.W = new Spell(SpellSlot.W, 800f);
            Variables.E = new Spell(SpellSlot.E, 750f);
            Variables.R = new Spell(SpellSlot.R, 1500f + (500f * ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level));

            Variables.Q.SetSkillshot(0.65f, 40f, 2200f, false, SkillshotType.SkillshotLine);
            Variables.Q2.SetSkillshot(0.65f, Variables.Q.Width*2, 2200f, false, SkillshotType.SkillshotLine);
            Variables.E.SetSkillshot(0.25f, 70f, 1600f, true, SkillshotType.SkillshotLine);
        }
    }
}
