namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Yasuo : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Yasuo()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 475, SkillShotType.Linear);
            this.W = new Spell.Skillshot(SpellSlot.W, 400, SkillShotType.Linear);
            this.E = new Spell.Targeted(SpellSlot.E, 475);
            this.R = new Spell.Active(SpellSlot.R, 1200);
        }
    }
}