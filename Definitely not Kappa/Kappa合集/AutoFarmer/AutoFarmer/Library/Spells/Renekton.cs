namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Renekton : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Renekton()
        {
            this.Q = new Spell.Active(SpellSlot.Q, 225);
            this.W = new Spell.Active(SpellSlot.W);
            this.E = new Spell.Skillshot(SpellSlot.E, 450, SkillShotType.Linear);
            this.R = new Spell.Active(SpellSlot.R);
        }
    }
}