namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Malzahar : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Malzahar()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Linear, 500, int.MaxValue, 100)
                         { MinimumHitChance = HitChance.High };
            this.W = new Spell.Skillshot(SpellSlot.W, 800, SkillShotType.Circular, 500, int.MaxValue, 250);
            this.E = new Spell.Targeted(SpellSlot.E, 650);
            this.R = new Spell.Targeted(SpellSlot.R, 700);
        }
    }
}