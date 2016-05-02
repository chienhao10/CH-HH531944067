namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Pantheon : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Pantheon()
        {
            this.Q = new Spell.Targeted(SpellSlot.Q, 600);
            this.W = new Spell.Targeted(SpellSlot.W, 600);
            this.E = new Spell.Skillshot(SpellSlot.E, 600, SkillShotType.Cone, 250, 2000, 70);
            this.R = new Spell.Skillshot(SpellSlot.R, 2000, SkillShotType.Circular);
        }
    }
}