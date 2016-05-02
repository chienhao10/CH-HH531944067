namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Teemo : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Teemo()
        {
            this.Q = new Spell.Targeted(SpellSlot.Q, 680);
            this.W = new Spell.Active(SpellSlot.W);
            this.E = new Spell.Active(SpellSlot.E);
            this.R = new Spell.Skillshot(SpellSlot.R, 300, SkillShotType.Circular, 500, 1000, 120);
        }
    }
}