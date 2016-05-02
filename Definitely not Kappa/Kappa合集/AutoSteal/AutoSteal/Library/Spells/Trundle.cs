namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Trundle : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Trundle()
        {
            this.Q = new Spell.Active(SpellSlot.Q);
            this.W = new Spell.Skillshot(SpellSlot.W, 900, SkillShotType.Circular, 0, int.MaxValue, 1000);
            this.E = new Spell.Skillshot(SpellSlot.E, 1000, SkillShotType.Circular, 250, int.MaxValue, 225);
            this.R = new Spell.Targeted(SpellSlot.R, 700);
        }
    }
}