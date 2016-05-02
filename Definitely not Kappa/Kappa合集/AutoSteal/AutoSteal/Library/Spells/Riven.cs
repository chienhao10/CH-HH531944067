namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Riven : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Riven()
        {
            this.Q = new Spell.Active(SpellSlot.Q);
            this.W = new Spell.Skillshot(SpellSlot.W, 700, SkillShotType.Circular);
            this.E = new Spell.Active(SpellSlot.E, 325);
            this.W = new Spell.Active(SpellSlot.W, (uint)(70 + ObjectManager.Player.BoundingRadius + 120));
        }
    }
}