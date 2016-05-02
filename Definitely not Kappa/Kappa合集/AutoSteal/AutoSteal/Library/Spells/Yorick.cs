namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Yorick : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Yorick()
        {
            this.Q = new Spell.Active(SpellSlot.Q, 125);
            this.W = new Spell.Skillshot(SpellSlot.W, 585, SkillShotType.Circular, 250, int.MaxValue, 200);
            this.E = new Spell.Targeted(SpellSlot.E, 540);
            this.R = new Spell.Targeted(SpellSlot.R, 835);
        }
    }
}