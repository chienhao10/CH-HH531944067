namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Syndra : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Syndra()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 800, SkillShotType.Circular, 600, int.MaxValue, 125);
            this.W = new Spell.Skillshot(SpellSlot.W, 950, SkillShotType.Circular, 250, 1600, 140);
            this.E = new Spell.Skillshot(SpellSlot.E, 700, SkillShotType.Cone, 250, 2500, 22);
            this.R = new Spell.Targeted(SpellSlot.R, 675);
            //EQ = new Spell.Skillshot(SpellSlot.Q, 1200, SkillShotType.Linear, 500, 2500, 55);
        }
    }
}