namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Anivia : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Anivia()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 1000, SkillShotType.Linear, 250, 850, 100);
            this.W = new Spell.Skillshot(SpellSlot.W, 800, SkillShotType.Circular, 0, int.MaxValue, 20);
            this.E = new Spell.Targeted(SpellSlot.E, 650);
            this.R = new Spell.Skillshot(SpellSlot.R, 600, SkillShotType.Circular, 0, int.MaxValue, 200);
            this.QisCc = true;
        }
    }
}