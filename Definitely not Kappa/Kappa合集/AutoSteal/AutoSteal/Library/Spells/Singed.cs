namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Singed : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Singed()
        {
            this.Q = new Spell.Active(SpellSlot.Q);
            this.W = new Spell.Skillshot(SpellSlot.W, 1000, SkillShotType.Circular, 500, 700, 350);
            this.E = new Spell.Targeted(SpellSlot.E, 125);
            this.R = new Spell.Active(SpellSlot.R);
        }
    }
}