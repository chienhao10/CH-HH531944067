namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Leona : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Leona()
        {
            this.Q = new Spell.Active(SpellSlot.Q);
            this.W = new Spell.Active(SpellSlot.W, 275);
            this.E = new Spell.Skillshot(SpellSlot.E, 875, SkillShotType.Linear, 250, 2000, 70);
            this.R = new Spell.Skillshot(SpellSlot.R, 1200, SkillShotType.Circular, 1000, int.MaxValue, 250);
        }
    }
}