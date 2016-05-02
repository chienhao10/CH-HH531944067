namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Graves : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Graves()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 950, SkillShotType.Linear, 0, 3000, 40)
                         { AllowedCollisionCount = 0 };
            this.W = new Spell.Skillshot(SpellSlot.W, 950, SkillShotType.Circular, 500, 1500, 120);
            this.E = new Spell.Skillshot(SpellSlot.E, 425, SkillShotType.Linear, 500, 0, 50);
            this.R = new Spell.Skillshot(SpellSlot.R, 1000, SkillShotType.Linear, 500, 2100, 100);
        }
    }
}