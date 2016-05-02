namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Sivir : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Sivir()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 1245, SkillShotType.Linear, (int)0.25, 1030, 90);
            this.W = new Spell.Active(SpellSlot.W);
            this.E = new Spell.Active(SpellSlot.E);
            this.R = new Spell.Active(SpellSlot.R, 1000);
        }
    }
}