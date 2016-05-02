namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Azir : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Azir()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 825, SkillShotType.Linear, 250, 1000, 70);
            this.W = new Spell.Skillshot(SpellSlot.W, 450, SkillShotType.Circular);
            this.E = new Spell.Skillshot(SpellSlot.E, 1200, SkillShotType.Linear, 250, 1600, 100);
            this.R = new Spell.Skillshot(SpellSlot.R, 300, SkillShotType.Linear, 500, 1000, 532);
            this.RisCc = true;
            this.EisDash = true;
        }
    }
}