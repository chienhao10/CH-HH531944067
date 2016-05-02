namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;

    public class MasterYi : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public MasterYi()
        {
            this.Q = new Spell.Targeted(SpellSlot.Q, 625);
            this.W = new Spell.Active(SpellSlot.W);
            this.E = new Spell.Active(SpellSlot.E);
            this.R = new Spell.Active(SpellSlot.R);
        }
    }
}