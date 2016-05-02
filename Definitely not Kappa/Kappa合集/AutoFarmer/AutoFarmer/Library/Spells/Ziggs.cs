namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Ziggs : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Ziggs()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 850, SkillShotType.Linear, 300, 1700, 130);
            this.W = new Spell.Active(SpellSlot.W, 1000);
            this.E = new Spell.Skillshot(SpellSlot.E, 900, SkillShotType.Linear, 250, 1530, 60);
            this.R = new Spell.Skillshot(SpellSlot.R, 5300, SkillShotType.Circular, 1000, 2800, 500);
        }
    }
}