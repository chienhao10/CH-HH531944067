namespace Genesis.Library.Spells
{
    using System;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class FiddleSticks : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public FiddleSticks()
        {
            this.Q = new Spell.Targeted(SpellSlot.Q, 575);
            this.W = new Spell.Targeted(SpellSlot.W, 575);
            this.E = new Spell.Targeted(SpellSlot.E, 750);
            this.R = new Spell.Skillshot(SpellSlot.R, 800, SkillShotType.Circular, 1750, Int32.MaxValue, 600);
        }
    }
}