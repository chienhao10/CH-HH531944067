namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Kindred : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Kindred()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 1125, SkillShotType.Linear);
            this.W = new Spell.Active(SpellSlot.W);
            this.E = new Spell.Targeted(SpellSlot.E, 500);
            this.R = new Spell.Targeted(SpellSlot.R, 500);
        }
    }
}