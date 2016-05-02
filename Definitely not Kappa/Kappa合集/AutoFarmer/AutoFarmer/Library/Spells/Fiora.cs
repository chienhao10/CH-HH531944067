namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Fiora : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Fiora()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 750, SkillShotType.Linear);
            this.W = new Spell.Skillshot(SpellSlot.W, 750, SkillShotType.Linear, 500, 3200, 70);
            this.E = new Spell.Active(SpellSlot.E, 200);
            this.R = new Spell.Targeted(SpellSlot.R, 500);
        }
    }
}