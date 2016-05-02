namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Zyra : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Zyra()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 800, SkillShotType.Linear, 250, int.MaxValue, 85);
            this.W = new Spell.Skillshot(SpellSlot.W, 825, SkillShotType.Circular, 250, int.MaxValue, 20);
            this.E = new Spell.Skillshot(SpellSlot.E, 1100, SkillShotType.Linear, 250, 1150, 70);
            this.R = new Spell.Skillshot(SpellSlot.R, 700, SkillShotType.Circular, 250, 1200, 500);

            this.QisCc = true;
            this.EisCc = true;
            this.RisCc = true;
        }
    }
}