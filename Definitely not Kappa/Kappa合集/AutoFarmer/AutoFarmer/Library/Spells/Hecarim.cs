namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Hecarim : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Hecarim()
        {
            this.Q = new Spell.Active(SpellSlot.Q, 350);
            this.W = new Spell.Active(SpellSlot.W, 525);
            this.E = new Spell.Active(SpellSlot.E, 450);
            this.R = new Spell.Skillshot(SpellSlot.R, 1000, SkillShotType.Linear, 250, 800, 200);
        }
    }
}