using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace KA_Ezreal
{
    public static class SpellManager
    {
        public static Spell.Skillshot Q { get; private set; }
        public static Spell.Skillshot W { get; private set; }
        public static Spell.Skillshot E { get; private set; }
        public static Spell.Skillshot R { get; private set; }

        static SpellManager()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 1020, SkillShotType.Linear, 350, 2000, 65);
            W = new Spell.Skillshot(SpellSlot.W, 920, SkillShotType.Linear, 350, 1550, 80)
            {
                AllowedCollisionCount = int.MaxValue
            };
            E = new Spell.Skillshot(SpellSlot.E, 470, SkillShotType.Circular, 450, 450, 10)
            {
                AllowedCollisionCount = int.MaxValue
            };
            R = new Spell.Skillshot(SpellSlot.R, 2000, SkillShotType.Linear, 1000, 2000, 150)
            {
                AllowedCollisionCount = int.MaxValue
            };
        }

        public static void Initialize()
        {
        }
    }
}
