namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;

    public class Vayne : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Vayne()
        {
            this.Q = new Spell.Active(SpellSlot.Q, 300);
            //Q2 = new Spell.Skillshot(SpellSlot.Q, 300, SkillShotType.Linear);
            this.W = new Spell.Active(SpellSlot.W);
            this.E = new Spell.Targeted(SpellSlot.E, 590);
            //E2 = new Spell.Skillshot(SpellSlot.E, 590, SkillShotType.Linear, 250, 1250);
            this.R = new Spell.Active(SpellSlot.R);
        }
    }
}