namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Corki : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Corki()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 825, SkillShotType.Circular, 300, 1000, 250);
            this.W = new Spell.Skillshot(SpellSlot.W, 600, SkillShotType.Linear);
            //W2 = new Spell.Skillshot(SpellSlot.W, 1800, SkillShotType.Linear);
            this.E = new Spell.Active(SpellSlot.E, 600);
            this.R = new Spell.Skillshot(SpellSlot.R, 1300, SkillShotType.Linear, 200, 1950, 40);
        }
    }
}