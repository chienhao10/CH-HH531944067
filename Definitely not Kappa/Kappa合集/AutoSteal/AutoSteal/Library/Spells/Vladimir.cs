namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Vladimir : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Vladimir()
        {
            this.Q = new Spell.Targeted(SpellSlot.Q, 600);
            this.W = new Spell.Active(SpellSlot.W, 150);
            this.E = new Spell.Active(SpellSlot.E, 600);
            this.R = new Spell.Skillshot(SpellSlot.R, 750, SkillShotType.Circular, 250, int.MaxValue, 170);
        }
    }
}