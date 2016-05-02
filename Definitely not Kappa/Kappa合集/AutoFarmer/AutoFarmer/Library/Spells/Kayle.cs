namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Kayle : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Kayle()
        {
            this.Q = new Spell.Targeted(SpellSlot.Q, 650);
            this.W = new Spell.Targeted(SpellSlot.W, 900);
            this.E = new Spell.Skillshot(SpellSlot.E, 650, SkillShotType.Circular, 1, 50, 400);
            this.R = new Spell.Targeted(SpellSlot.R, 900);
        }
    }
}