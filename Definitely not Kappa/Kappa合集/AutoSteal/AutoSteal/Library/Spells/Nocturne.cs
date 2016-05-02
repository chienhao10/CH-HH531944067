namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Nocturne : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Nocturne()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 1125, SkillShotType.Linear);
            this.W = new Spell.Active(SpellSlot.W);
            this.E = new Spell.Targeted(SpellSlot.E, 425);
            this.R = new Spell.Active(SpellSlot.R, 2500);
            // R1 = new Spell.Targeted(SpellSlot.R, R.Range);
        }
    }
}