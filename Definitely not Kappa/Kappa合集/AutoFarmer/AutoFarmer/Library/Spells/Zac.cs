namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Zac : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Zac()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 550, SkillShotType.Linear, 500, int.MaxValue, 120);
            this.W = new Spell.Active(SpellSlot.W, 350);
            this.E = new Spell.Chargeable(SpellSlot.E, 0, 1750, 1500, 500, 1500, 250);
            this.R = new Spell.Active(SpellSlot.R, 300);
        }
    }
}