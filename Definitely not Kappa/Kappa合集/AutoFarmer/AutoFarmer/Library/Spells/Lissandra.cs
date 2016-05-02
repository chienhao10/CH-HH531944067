namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Lissandra : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Lissandra()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 725, SkillShotType.Linear);
            //Q1 = new Spell.Skillshot(SpellSlot.Q, 825, SkillShotType.Linear);
            this.W = new Spell.Active(SpellSlot.W, 450);
            this.E = new Spell.Skillshot(SpellSlot.E, 1050, SkillShotType.Linear);
            //E1 = new Spell.Active(SpellSlot.E);
            this.R = new Spell.Targeted(SpellSlot.R, 550);
        }
    }
}