namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Bard : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Bard()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 860, SkillShotType.Linear, 250, 1600, 65);
            //Q2 = new Spell.Skillshot(SpellSlot.Q, 1310, SkillShotType.Linear, 250, 1600, 65);
            this.W = new Spell.Skillshot(SpellSlot.W, 800, SkillShotType.Circular);
            this.E = new Spell.Skillshot(SpellSlot.E, int.MaxValue, SkillShotType.Linear);
            this.R = new Spell.Skillshot(SpellSlot.R, 3400, SkillShotType.Circular, 250, int.MaxValue, 650);
        }
    }
}