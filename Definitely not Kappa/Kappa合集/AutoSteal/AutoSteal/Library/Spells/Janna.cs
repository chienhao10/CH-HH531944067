namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Janna : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Janna()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 1100, SkillShotType.Linear, 300);
            this.W = new Spell.Targeted(SpellSlot.W, 600);
            this.E = new Spell.Targeted(SpellSlot.E, 800);
            this.R = new Spell.Active(SpellSlot.R, 725);
        }
    }
}