namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Lux : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Lux()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 1175, SkillShotType.Linear, 250, 1200, 65)
                         { AllowedCollisionCount = 1 };
            this.W = new Spell.Skillshot(SpellSlot.W, 1075, SkillShotType.Linear, 0, 1400, 85);
            this.E = new Spell.Skillshot(SpellSlot.E, 1050, SkillShotType.Circular, 250, 1300, 330);
            this.R = new Spell.Skillshot(SpellSlot.R, 3200, SkillShotType.Circular, 500, int.MaxValue, 160);
        }
    }
}