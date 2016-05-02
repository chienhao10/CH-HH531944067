namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Swain : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Swain()
        {
            this.Q = new Spell.Targeted(SpellSlot.Q, 625);
            this.W = new Spell.Skillshot(SpellSlot.W, 820, SkillShotType.Circular, 500, 1250, 275);
            this.E = new Spell.Targeted(SpellSlot.E, 625);
            this.R = new Spell.Active(SpellSlot.R);
        }
    }
}