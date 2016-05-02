namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Tristana : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Tristana()
        {
            this.Q = new Spell.Active(SpellSlot.Q, 550);
            this.W = new Spell.Skillshot(SpellSlot.W, 900, SkillShotType.Circular, 450, int.MaxValue, 180);
            this.E = new Spell.Targeted(SpellSlot.E, 550);
            this.R = new Spell.Targeted(SpellSlot.R, 550);
        }
    }
}