namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Shen : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Shen()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 2000, SkillShotType.Linear, 500, 2500, 150);
            this.W = new Spell.Active(SpellSlot.W);
            this.E = new Spell.Skillshot(SpellSlot.E, 610, SkillShotType.Linear, 500, 1600, 50);
            this.R = new Spell.Targeted(SpellSlot.R, 31000);
        }
    }
}