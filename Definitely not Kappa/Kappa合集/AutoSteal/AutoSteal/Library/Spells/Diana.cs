namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Diana : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Diana()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 830, SkillShotType.Cone, 500, 1600, 195);
            this.W = new Spell.Active(SpellSlot.W, 350);
            this.E = new Spell.Active(SpellSlot.E, 200);
            this.R = new Spell.Targeted(SpellSlot.R, 825);
        }
    }
}