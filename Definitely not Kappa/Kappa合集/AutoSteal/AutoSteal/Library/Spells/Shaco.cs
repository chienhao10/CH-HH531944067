namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;

    public class Shaco : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Shaco()
        {
            this.Q = new Spell.Targeted(SpellSlot.Q, 400);
            //Q2 = new Spell.Targeted(SpellSlot.Q, 1100);
            this.W = new Spell.Targeted(SpellSlot.W, 425);
            this.E = new Spell.Targeted(SpellSlot.E, 625);
            this.R = new Spell.Targeted(SpellSlot.R, 200);
        }
    }
}