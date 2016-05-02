namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class TwistedFate : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public TwistedFate()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 1450, SkillShotType.Linear, 0, 1000, 40);
            this.W = new Spell.Active(SpellSlot.W);
            this.E = new Spell.Active(SpellSlot.E);
            this.R = new Spell.Active(SpellSlot.R, 5500);
        }
    }
}