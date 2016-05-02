namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Galio : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Galio()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 940, SkillShotType.Circular, 250, 1300, 120);
            this.W = new Spell.Targeted(SpellSlot.W, 830);
            this.E = new Spell.Skillshot(SpellSlot.E, 1180, SkillShotType.Linear, 250, 1200, 140);
            this.R = new Spell.Active(SpellSlot.R, 560);
        }
    }
}