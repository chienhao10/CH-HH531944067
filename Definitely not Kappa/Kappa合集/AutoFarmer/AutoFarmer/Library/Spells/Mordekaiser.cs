namespace Genesis.Library.Spells
{
    using System;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Mordekaiser : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Mordekaiser()
        {
            this.Q = new Spell.Active(SpellSlot.Q);
            this.W = new Spell.Targeted(SpellSlot.W, 1000);
            this.E = new Spell.Skillshot(
                SpellSlot.E,
                670,
                SkillShotType.Cone,
                (int)0.25f,
                2000,
                12 * 2 * (int)Math.PI / 180);
            this.R = new Spell.Targeted(SpellSlot.R, 1500);
        }
    }
}