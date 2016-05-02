namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;

    public class Udyr : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Udyr()
        {
            this.Q = new Spell.Active(SpellSlot.Q, 250);
            this.W = new Spell.Active(SpellSlot.W, 250);
            this.E = new Spell.Active(SpellSlot.E, 250);
            this.R = new Spell.Active(SpellSlot.R, 500);
        }
    }
}