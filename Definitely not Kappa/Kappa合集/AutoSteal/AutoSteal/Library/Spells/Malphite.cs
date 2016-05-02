namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Malphite : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Malphite()
        {
            this.Q = new Spell.Targeted(SpellSlot.Q, 625);
            this.W = new Spell.Active(SpellSlot.W);
            this.E = new Spell.Active(SpellSlot.E, 400);
            this.R = new Spell.Skillshot(SpellSlot.R, 1000, SkillShotType.Circular, 250, 700, 270);
        }
    }
}