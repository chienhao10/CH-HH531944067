namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Kassadin : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Kassadin()
        {
            this.Q = new Spell.Targeted(SpellSlot.Q, 650);
            this.W = new Spell.Active(SpellSlot.W);
            this.E = new Spell.Skillshot(SpellSlot.E, 400, SkillShotType.Cone, (int)0.5f, int.MaxValue, 10);
            this.R = new Spell.Skillshot(SpellSlot.R, 700, SkillShotType.Circular, (int)0.5f, int.MaxValue, 150);
        }
    }
}