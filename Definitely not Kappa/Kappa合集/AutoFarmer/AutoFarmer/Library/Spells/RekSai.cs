namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;

    public class RekSai : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public RekSai()
        {
            this.Q = new Spell.Active(SpellSlot.Q, 325);
            this.W = new Spell.Active(SpellSlot.W);
            this.E = new Spell.Targeted(SpellSlot.E, 250);
            this.R = new Spell.Targeted(SpellSlot.R, 0);
        }
    }
}