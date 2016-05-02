namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;

    public class Nunu : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Nunu()
        {
            this.Q = new Spell.Targeted(SpellSlot.Q, 350);
            this.W = new Spell.Targeted(SpellSlot.W, 700);
            this.E = new Spell.Targeted(SpellSlot.E, 550);
            this.R = new Spell.Active(SpellSlot.R, 650);
        }
    }
}