namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Varus : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Varus()
        {
            //Q2 = new Spell.Skillshot(SpellSlot.Q, 925, EloBuddy.SDK.Enumerations.SkillShotType.Linear, 0, 1900, 100);
            //Q2.AllowedCollisionCount = int.MaxValue;
            this.Q = new Spell.Chargeable(SpellSlot.Q, 925, 1625, 2000, 0, 1900, 100);
            this.E = new Spell.Skillshot(SpellSlot.E, 925, SkillShotType.Circular, 500, int.MaxValue, 750);
            this.R = new Spell.Skillshot(SpellSlot.R, 1075, SkillShotType.Linear, 0, 1200, 120);
        }
    }
}