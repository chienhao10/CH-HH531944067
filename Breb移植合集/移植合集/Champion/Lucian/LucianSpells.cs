using EloBuddy;
using LeagueSharp.Common;

namespace LCS_Lucian
{
    internal class LucianSpells
    {
        public static Spell Q, Q2, W, E, R;

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 675);
            Q2 = new Spell(SpellSlot.Q, 1200);
            W = new Spell(SpellSlot.W, 1000);
            E = new Spell(SpellSlot.E, 425);
            R = new Spell(SpellSlot.R, 1400);

            Q.SetTargetted(0.25f, float.MaxValue);
            Q2.SetSkillshot(0.55f, 50f, float.MaxValue, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.4f, 150f, 1600, true, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 1f, float.MaxValue, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.2f, 110f, 2500, true, SkillshotType.SkillshotLine);
        }
    }
}