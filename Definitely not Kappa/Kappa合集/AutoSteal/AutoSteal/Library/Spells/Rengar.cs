namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Rengar : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Rengar()
        {
            this.Q = new Spell.Active(SpellSlot.Q);
            this.W = new Spell.Skillshot(SpellSlot.W, 500, SkillShotType.Circular, 250, 2000, 100);
            this.E = new Spell.Skillshot(SpellSlot.E, 1000, SkillShotType.Linear, 250, 1500, 140);
            this.R = new Spell.Active(SpellSlot.R);
        }
    }
}