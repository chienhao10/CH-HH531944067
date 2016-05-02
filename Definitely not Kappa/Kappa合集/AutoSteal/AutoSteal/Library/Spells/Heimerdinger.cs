namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Heimerdinger : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Heimerdinger()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 350, SkillShotType.Linear, (int)0.5f, 1450, (int)40f);
            this.W = new Spell.Skillshot(SpellSlot.W, 1325, SkillShotType.Cone, (int)0.5f, 902, 200);
            this.E = new Spell.Skillshot(SpellSlot.E, 970, SkillShotType.Circular, (int)0.5f, 2500, 120);
            this.R = new Spell.Active(SpellSlot.R, 350);
        }
    }
}