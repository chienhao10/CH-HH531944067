namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Ekko : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Ekko()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 750, SkillShotType.Linear, 250, 2200, 60);
            this.W = new Spell.Skillshot(SpellSlot.W, 1620, SkillShotType.Circular, 500, 1000, 500);
            this.E = new Spell.Skillshot(SpellSlot.E, 400, SkillShotType.Linear, 250, int.MaxValue, 1);
            this.R = new Spell.Active(SpellSlot.R, 400);
        }
    }
}