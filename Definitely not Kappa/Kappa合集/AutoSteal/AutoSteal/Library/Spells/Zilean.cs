namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Zilean : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Zilean()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Circular, 300, 2000, 150);
            this.W = new Spell.Active(SpellSlot.W, 700);
            this.E = new Spell.Targeted(SpellSlot.E, 1000);
            this.R = new Spell.Targeted(SpellSlot.R, 410);
        }
    }
}