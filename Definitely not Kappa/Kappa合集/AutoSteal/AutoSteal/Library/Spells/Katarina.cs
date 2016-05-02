namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;

    public class Katarina : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Katarina()
        {
            this.Q = new Spell.Targeted(SpellSlot.Q, 675);
            this.W = new Spell.Active(SpellSlot.W, 375);
            this.E = new Spell.Targeted(SpellSlot.E, 700);
            this.R = new Spell.Active(SpellSlot.R, 550);
        }
    }
}