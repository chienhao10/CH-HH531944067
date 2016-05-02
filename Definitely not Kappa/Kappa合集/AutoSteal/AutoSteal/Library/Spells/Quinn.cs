namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Quinn : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Quinn()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 1025, SkillShotType.Linear, 0, 750, 210);
            this.W = new Spell.Active(SpellSlot.W, 2100);
            this.E = new Spell.Targeted(SpellSlot.E, 675);
            this.R = new Spell.Active(SpellSlot.R);
        }
    }
}