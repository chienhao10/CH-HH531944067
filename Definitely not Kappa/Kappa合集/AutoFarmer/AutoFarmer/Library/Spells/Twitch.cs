namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Twitch : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Twitch()
        {
            this.Q = new Spell.Active(SpellSlot.Q);
            this.W = new Spell.Skillshot(SpellSlot.W, 925, SkillShotType.Circular, 250, 1400, 275)
                         { AllowedCollisionCount = int.MaxValue };
            this.E = new Spell.Active(SpellSlot.E, 1200);
            this.R = new Spell.Active(SpellSlot.R, 900);
            //R2 = new Spell.Skillshot(SpellSlot.R, 1200, SkillShotType.Linear, 0, 3000, 100)
        }
    }
}