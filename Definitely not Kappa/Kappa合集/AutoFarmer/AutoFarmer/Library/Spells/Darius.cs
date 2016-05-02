namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Darius : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Darius()
        {
            this.Q = new Spell.Active(SpellSlot.Q, 400);
            this.W = new Spell.Active(SpellSlot.W, 145);
            this.E = new Spell.Skillshot(SpellSlot.E, 540, SkillShotType.Cone, 250, 100, 120);
            this.R = new Spell.Targeted(SpellSlot.R, 460);
        }
    }
}