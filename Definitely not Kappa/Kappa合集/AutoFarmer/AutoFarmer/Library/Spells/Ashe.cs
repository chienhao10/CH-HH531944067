namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Ashe : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Ashe()
        {
            this.Q = new Spell.Active(SpellSlot.Q, 600);
            this.W = new Spell.Skillshot(SpellSlot.W, 1200, SkillShotType.Cone);
            this.E = new Spell.Active(SpellSlot.E, 1000);
            this.R = new Spell.Skillshot(SpellSlot.R, 10000, SkillShotType.Linear, 250, 1600, 100);
            this.RisCc = true;
        }
    }
}