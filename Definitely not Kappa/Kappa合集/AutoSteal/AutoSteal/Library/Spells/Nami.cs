namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Nami : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Nami()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 875, SkillShotType.Circular, 1, int.MaxValue, 150);
            this.W = new Spell.Targeted(SpellSlot.W, 725);
            this.E = new Spell.Targeted(SpellSlot.E, 800);
            this.R = new Spell.Skillshot(SpellSlot.R, 2750, SkillShotType.Linear, 250, 500, 160);
        }
    }
}