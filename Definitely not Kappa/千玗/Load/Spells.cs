namespace KappaKindred
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    internal class Spells
    {
        public static Spell.Skillshot Q { get; private set; }

        public static Spell.Active W { get; private set; }

        public static Spell.Targeted E { get; private set; }

        public static Spell.Active R { get; private set; }

        public static void Load()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 750, SkillShotType.Linear, 250, 1000, 250);
            W = new Spell.Active(SpellSlot.W, 800);
            E = new Spell.Targeted(SpellSlot.E, 450);
            R = new Spell.Active(SpellSlot.R, 500);
        }
    }
}