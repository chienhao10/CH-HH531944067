namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;

    public class Volibear : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Volibear()
        {
            this.Q = new Spell.Active(SpellSlot.Q, 750);
            this.W = new Spell.Targeted(SpellSlot.W, 395);
            this.E = new Spell.Active(SpellSlot.E, 415);
            this.R = new Spell.Active(SpellSlot.R);
        }
    }
}