namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Soraka : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Soraka()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 950, SkillShotType.Circular, (int)0.283f, 1100, (int)210f);
            this.W = new Spell.Targeted(SpellSlot.W, 550);
            this.E = new Spell.Skillshot(SpellSlot.E, 925, SkillShotType.Circular, (int)0.5f, 1750, (int)70f);
            this.R = new Spell.Active(SpellSlot.R);
        }
    }
}