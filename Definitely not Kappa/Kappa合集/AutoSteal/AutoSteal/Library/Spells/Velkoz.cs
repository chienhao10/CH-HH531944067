namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Velkoz : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Velkoz()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 1050, SkillShotType.Linear, 250, 1300, 50)
                         { MinimumHitChance = HitChance.High, AllowedCollisionCount = 0 };
            this.W = new Spell.Skillshot(SpellSlot.W, 1050, SkillShotType.Linear, 250, 1700, 80)
                         { MinimumHitChance = HitChance.High, AllowedCollisionCount = int.MaxValue };
            this.E = new Spell.Skillshot(SpellSlot.E, 850, SkillShotType.Circular, 500, 1500, 120)
                         { MinimumHitChance = HitChance.High, AllowedCollisionCount = int.MaxValue };
            this.R = new Spell.Skillshot(SpellSlot.R, 1550, SkillShotType.Linear)
                         { MinimumHitChance = HitChance.High, AllowedCollisionCount = int.MaxValue };
        }
    }
}