namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Kalista : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Kalista()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 1150, SkillShotType.Linear, 250, 1200, 40);
            this.W = new Spell.Targeted(SpellSlot.W, 5000);
            this.E = new Spell.Active(SpellSlot.E, 1000);
            this.R = new Spell.Active(SpellSlot.R, 1500); //You are gonna suck until you get logic
        }
    }
}