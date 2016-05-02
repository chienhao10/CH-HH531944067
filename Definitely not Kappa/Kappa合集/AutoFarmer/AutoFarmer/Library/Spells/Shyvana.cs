namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Shyvana : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Shyvana()
        {
            this.Q = new Spell.Active(SpellSlot.Q, (uint)Player.Instance.GetAutoAttackRange());
            this.W = new Spell.Active(SpellSlot.W, 425);
            this.E = new Spell.Skillshot(SpellSlot.E, 925, SkillShotType.Linear, 250, 1500, 60);
            this.R = new Spell.Skillshot(SpellSlot.R, 1000, SkillShotType.Circular, 250, 1500, 150);
        }
    }
}