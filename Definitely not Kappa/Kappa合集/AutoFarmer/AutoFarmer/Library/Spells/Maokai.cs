namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Maokai : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Maokai()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 600, SkillShotType.Linear, 500, 1200, 110);
            this.W = new Spell.Targeted(SpellSlot.W, 525);
            this.E = new Spell.Skillshot(SpellSlot.E, 1075, SkillShotType.Circular, 1000, 1500, 225);
            this.R = new Spell.Active(SpellSlot.R, 475);
        }
    }
}