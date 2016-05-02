namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Olaf : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Olaf()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 1000, SkillShotType.Linear, 250, 1550, 75);
            //Q2 = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Linear, 250, 1550, 75)     
            this.W = new Spell.Active(SpellSlot.W);
            this.E = new Spell.Targeted(SpellSlot.E, 325);
            this.R = new Spell.Active(SpellSlot.R);
        }
    }
}