namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Rumble : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Rumble()
        {
            this.Q = new Spell.Active(SpellSlot.Q, 600);
            this.W = new Spell.Active(SpellSlot.W);
            this.E = new Spell.Skillshot(SpellSlot.E, 840, SkillShotType.Linear, 250, 2000, 70);
            this.R = new Spell.Skillshot(SpellSlot.R, 1700, SkillShotType.Linear, 400, 2500, 120);
        }
    }
}