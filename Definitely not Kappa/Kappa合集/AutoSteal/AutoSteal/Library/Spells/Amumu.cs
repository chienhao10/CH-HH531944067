namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Amumu : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Amumu()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 1100, SkillShotType.Linear, 250, 2000, 80);
            this.W = new Spell.Active(SpellSlot.W, 300);
            this.E = new Spell.Active(SpellSlot.E, 350);
            this.R = new Spell.Active(SpellSlot.R, 550);
            this.QisCc = true;
            this.QisDash = true;
            this.RisCc = true;
        }
    }
}