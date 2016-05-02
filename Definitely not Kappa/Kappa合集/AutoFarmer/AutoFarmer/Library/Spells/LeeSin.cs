namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class LeeSin : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public LeeSin()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 1100, SkillShotType.Linear, 250, 1800, 60)
                         { AllowedCollisionCount = 0 };
            //Q2 = new Spell.Active(SpellSlot.Q, 1300);
            this.W = new Spell.Skillshot(SpellSlot.W, 1200, SkillShotType.Linear, 50, 1500, 100);
            //W2 = new Spell.Active(SpellSlot.W, 700);
            this.E = new Spell.Skillshot(SpellSlot.E, 350, SkillShotType.Linear, 250, 2500, 100);
            //E2 = new Spell.Skillshot(SpellSlot.E, 675, SkillShotType.Linear, 250, 2500, 100)
            this.R = new Spell.Targeted(SpellSlot.R, 375);
        }
    }
}