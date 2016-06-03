#region

using EloBuddy;
using LeagueSharp.SDK;
using Spell = LeagueSharp.SDK.Spell;

#endregion

namespace Spirit_Karma.Core
{
    internal class Spells
    {
        public static Spell Q { get; set; }
        public static Spell W { get; set; }
        public static Spell E { get; set; }
        public static Spell R { get; set; }

        public static void Load()
        {
            Q = new Spell(SpellSlot.Q, 1050);
            W = new Spell(SpellSlot.W, 700);
            E = new Spell(SpellSlot.E, 800);
            R = new Spell(SpellSlot.R);
            
            Q.SetSkillshot(0.25f, 60f, 1700f, true, SkillshotType.SkillshotLine);
        }
    }
}
