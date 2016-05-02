using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace KA_Lux
{
    public static class SpellManager
    {
        public static Spell.Skillshot Q { get; private set; }
        public static Spell.Skillshot W { get; private set; }
        public static Spell.Skillshot E { get; private set; }
        public static Spell.Skillshot R { get; private set; }

        static SpellManager()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 1300, SkillShotType.Linear, 250, 1200, 70)
            {
                AllowedCollisionCount = 1
            };
            W = new Spell.Skillshot(SpellSlot.W, 1075, SkillShotType.Linear, 0, 1400, 85)
            {
                AllowedCollisionCount = int.MaxValue 
            };
            E = new Spell.Skillshot(SpellSlot.E, 1100, SkillShotType.Circular, 250, 1400, 335)
            {
                AllowedCollisionCount = int.MaxValue 
            };
            R = new Spell.Skillshot(SpellSlot.R, 3300, SkillShotType.Circular, 1000, int.MaxValue, 110)
            {
                AllowedCollisionCount = int.MaxValue 
            };
        }

        public static void Initialize()
        {
        }
    }
}
