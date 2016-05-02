namespace Genesis.Library.Spells
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Nautilus : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Nautilus()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 1100, SkillShotType.Linear);
            this.W = new Spell.Active(SpellSlot.W);
            this.E = new Spell.Active(
                SpellSlot.E,
                (uint)ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).SData.CastRange);
            this.R = new Spell.Targeted(
                SpellSlot.R,
                (uint)ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).SData.CastRange);
        }
    }
}