namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Blitzcrank : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Blitzcrank()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 980, SkillShotType.Linear, 250, 1800, 70);
            this.W = new Spell.Active(SpellSlot.W, 0);
            this.E = new Spell.Active(SpellSlot.E, 150);
            this.R = new Spell.Active(SpellSlot.R, 550);
        }
    }
}