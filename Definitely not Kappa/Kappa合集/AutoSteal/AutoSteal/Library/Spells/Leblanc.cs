namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Leblanc : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Leblanc()
        {
            this.Q = new Spell.Targeted(SpellSlot.Q, 700);
            this.W = new Spell.Skillshot(SpellSlot.W, 600, SkillShotType.Circular, 250, 1450, 250);
            this.E = new Spell.Skillshot(SpellSlot.E, 950, SkillShotType.Linear, 250, 1550, 55)
                         { AllowedCollisionCount = 0 };
            this.R = new Spell.Targeted(SpellSlot.R, 950);
        }
    }
}