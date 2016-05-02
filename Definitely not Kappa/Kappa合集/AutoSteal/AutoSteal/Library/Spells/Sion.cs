namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Sion : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Sion()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 740, SkillShotType.Cone, 250, 100, 500);
            //Q2 = new Spell.Active(SpellSlot.Q, 680);
            this.W = new Spell.Active(SpellSlot.W, 490);
            this.E = new Spell.Skillshot(SpellSlot.E, 755, SkillShotType.Linear);
            this.R = new Spell.Active(SpellSlot.R, 800);
        }
    }
}