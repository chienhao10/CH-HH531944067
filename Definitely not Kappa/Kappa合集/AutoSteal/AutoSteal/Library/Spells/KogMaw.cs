namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class KogMaw : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public KogMaw()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 980, SkillShotType.Linear, 250, 2000, 50)
                         { AllowedCollisionCount = int.MaxValue };
            this.W = new Spell.Active(SpellSlot.W, 700);
            this.E = new Spell.Skillshot(SpellSlot.E, 1000, SkillShotType.Linear, 250, 1530, 60);
            this.R = new Spell.Skillshot(SpellSlot.R, 1200, SkillShotType.Circular, 250, 1200, 30);
        }
    }
}