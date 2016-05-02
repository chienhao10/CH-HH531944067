namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Talon : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Talon()
        {
            this.Q = new Spell.Active(SpellSlot.Q);
            this.W = new Spell.Skillshot(SpellSlot.W, 600, SkillShotType.Cone, 1, 2300, 75)
                         { AllowedCollisionCount = int.MaxValue };
            this.E = new Spell.Targeted(SpellSlot.E, 700);
            this.R = new Spell.Active(SpellSlot.R);
        }
    }
}