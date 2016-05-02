namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class DrMundo : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public DrMundo()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 1000, SkillShotType.Linear, 50, 2000, 60);
            this.W = new Spell.Active(SpellSlot.W, 162);
            this.E = new Spell.Active(SpellSlot.E);
            this.R = new Spell.Active(SpellSlot.R);
        }
    }
}