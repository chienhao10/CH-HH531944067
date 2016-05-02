namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Nasus : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Nasus()
        {
            this.Q = new Spell.Active(SpellSlot.Q, 150);
            this.W = new Spell.Targeted(SpellSlot.W, 600);
            this.E = new Spell.Skillshot(SpellSlot.E, 650, SkillShotType.Circular, 250, 190, int.MaxValue);
            this.R = new Spell.Active(SpellSlot.R);
        }
    }
}