namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;

    public class Warwick : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Warwick()
        {
            this.Q = new Spell.Targeted(SpellSlot.Q, 400);
            this.W = new Spell.Active(SpellSlot.W, 1250);
            this.E = new Spell.Active(SpellSlot.E, 1500);
            this.R = new Spell.Targeted(SpellSlot.R, 700);
        }
    }
}