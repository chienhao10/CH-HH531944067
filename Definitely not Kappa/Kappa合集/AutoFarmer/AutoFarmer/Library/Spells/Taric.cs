namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;

    public class Taric : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Taric()
        {
            this.Q = new Spell.Targeted(SpellSlot.Q, 750);
            this.W = new Spell.Active(SpellSlot.W, 400);
            this.E = new Spell.Targeted(SpellSlot.E, 625);
            this.R = new Spell.Active(SpellSlot.R, 400);
        }
    }
}