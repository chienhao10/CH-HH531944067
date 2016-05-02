namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Ahri : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Ahri()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Linear, 250, 1750, 100);
            this.W = new Spell.Active(SpellSlot.W, 550);
            this.E = new Spell.Skillshot(SpellSlot.E, 950, SkillShotType.Linear, 250, 1550, 60);
            this.R = new Spell.Active(SpellSlot.R, 600);
            this.Options.Clear();
            this.Options.Add("EisCC", true);
            this.Options.Add("RisDash", true);
        }
    }
}