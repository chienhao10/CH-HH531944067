namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Kennen : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Kennen()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 1000, SkillShotType.Linear, 125, 1700, 50);
            this.W = new Spell.Active(SpellSlot.W, 900);
            this.E = new Spell.Active(SpellSlot.E, 500); //Kappa ;)
            this.R = new Spell.Active(SpellSlot.R, 500);
        }
    }
}