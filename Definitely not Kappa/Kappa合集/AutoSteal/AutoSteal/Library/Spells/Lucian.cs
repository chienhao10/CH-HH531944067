namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Lucian : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Lucian()
        {
            this.Q = new Spell.Targeted(SpellSlot.Q, 675);
            this.W = new Spell.Skillshot(SpellSlot.W, 1000, SkillShotType.Linear, 250, 1600, 80);
            this.E = new Spell.Skillshot(SpellSlot.E, 475, SkillShotType.Linear);
            this.R = new Spell.Skillshot(SpellSlot.R, 1100, SkillShotType.Linear, 500, 2800, 110);
        }
    }
}