namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Lulu : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Lulu()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 950, SkillShotType.Linear, 250, 1450, 60);
            this.W = new Spell.Targeted(SpellSlot.W, 650);
            this.E = new Spell.Targeted(SpellSlot.E, 650);
            this.R = new Spell.Targeted(SpellSlot.R, 900);
        }
    }
}