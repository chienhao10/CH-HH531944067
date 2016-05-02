namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Sejuani : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Sejuani()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 650, SkillShotType.Linear, 0, 1600, 70);
            this.W = new Spell.Active(SpellSlot.W, 350);
            this.E = new Spell.Active(SpellSlot.E, 1000);
            this.R = new Spell.Skillshot(SpellSlot.R, 1175, SkillShotType.Linear, 250, 1600, 110);
        }
    }
}