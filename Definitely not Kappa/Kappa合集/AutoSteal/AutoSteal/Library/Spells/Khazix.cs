namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Khazix : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Khazix()
        {
            this.Q = new Spell.Targeted(SpellSlot.Q, 325);
            this.W = new Spell.Skillshot(SpellSlot.W, 1000, SkillShotType.Linear, 225, 828, 80);
            this.E = new Spell.Skillshot(SpellSlot.E, 600, SkillShotType.Circular, 25, 1000, 100);
            this.R = new Spell.Active(SpellSlot.R);
        }
    }
}