namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Akali : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Akali()
        {
            this.Q = new Spell.Targeted(SpellSlot.Q, 600);
            this.W = new Spell.Skillshot(SpellSlot.W, 700, SkillShotType.Circular);
            this.E = new Spell.Active(SpellSlot.E, 325);
            this.R = new Spell.Targeted(SpellSlot.R, 700);
            this.WisCc = true;
            this.RisDash = true;
        }
    }
}