namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Chogath : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Chogath()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 950, SkillShotType.Circular, 750, int.MaxValue, 175);
            this.W = new Spell.Skillshot(SpellSlot.W, 575, SkillShotType.Cone, 250, 1750, 100);
            this.E = new Spell.Active(SpellSlot.E);
            this.R = new Spell.Targeted(SpellSlot.R, 500);
        }
    }
}