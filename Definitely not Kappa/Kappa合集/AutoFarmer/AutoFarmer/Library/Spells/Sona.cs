namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Sona : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Sona()
        {
            this.Q = new Spell.Active(SpellSlot.Q, 850);
            this.W = new Spell.Active(SpellSlot.W, 1000);
            this.E = new Spell.Active(SpellSlot.E, 350);
            this.R = new Spell.Skillshot(SpellSlot.R, 1000, SkillShotType.Circular, 250, 2400, 140);
        }
    }
}