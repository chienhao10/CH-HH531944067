namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Tryndamere : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Tryndamere()
        {
            this.Q = new Spell.Active(SpellSlot.Q);
            this.W = new Spell.Active(SpellSlot.W, 400);
            this.E = new Spell.Skillshot(SpellSlot.E, 660, SkillShotType.Linear, 250, 700, (int)92.5);
            this.R = new Spell.Active(SpellSlot.R);
        }
    }
}