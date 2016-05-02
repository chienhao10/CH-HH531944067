namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Karma : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Karma()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 950, SkillShotType.Linear, 250, 1500, 100);
            this.W = new Spell.Targeted(SpellSlot.W, 675);
            this.E = new Spell.Targeted(SpellSlot.E, 800);
            this.R = new Spell.Active(SpellSlot.R);
        }
    }
}