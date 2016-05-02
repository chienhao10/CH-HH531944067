namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Braum : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Braum()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 1000, SkillShotType.Linear, 250, 1700, 60);
            this.W = new Spell.Targeted(SpellSlot.W, 650);
            this.E = new Spell.Skillshot(SpellSlot.E, 500, SkillShotType.Cone, 250, 2000, 250);
            this.R = new Spell.Skillshot(SpellSlot.R, 1300, SkillShotType.Linear, 250, 1300, 115);
        }
    }
}