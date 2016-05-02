namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;

    public class Vi : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Vi()
        {
            this.Q = new Spell.Chargeable(SpellSlot.Q, 250, 875, 1250, 0, 1400, 55);
            this.W = new Spell.Active(SpellSlot.W);
            this.E = new Spell.Active(SpellSlot.E, 600);
            //E2 = new Spell.Skillshot(SpellSlot.E, 600, SkillShotType.Cone);
            this.R = new Spell.Targeted(SpellSlot.R, 800);
        }
    }
}