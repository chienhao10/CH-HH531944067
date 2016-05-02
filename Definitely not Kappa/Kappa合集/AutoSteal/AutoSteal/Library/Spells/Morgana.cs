namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Morgana : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Morgana()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 1200, SkillShotType.Linear, 250, 1200, 80);
            this.W = new Spell.Skillshot(SpellSlot.W, 900, SkillShotType.Circular, 250, 2200, 400);
            this.E = new Spell.Targeted(SpellSlot.E, 750);
            this.R = new Spell.Active(SpellSlot.R, 600);
        }
    }
}