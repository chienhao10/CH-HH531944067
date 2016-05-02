namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class JarvanIv : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public JarvanIv()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 830, SkillShotType.Linear);
            this.W = new Spell.Active(SpellSlot.W, 520);
            this.E = new Spell.Skillshot(SpellSlot.E, 860, SkillShotType.Circular);
            this.R = new Spell.Targeted(SpellSlot.R, 650);
        }
    }
}