using EloBuddy;
using LeagueSharp;
using LeagueSharp.Common;

namespace TreeLib.Extensions
{
    public static class SpellExtensions
    {
        public static SpellDataInst[] GetMainSpells(this Spellbook spellbook)
        {
            return new[]
            {
                spellbook.GetSpell(SpellSlot.Q), spellbook.GetSpell(SpellSlot.W), spellbook.GetSpell(SpellSlot.E),
                spellbook.GetSpell(SpellSlot.R)
            };
        }

        public static SpellDataInst[] GetSummonerSpells(this Spellbook spellbook)
        {
            return new[] { spellbook.GetSpell(SpellSlot.Summoner1), spellbook.GetSpell(SpellSlot.Summoner2) };
        }

        public static bool IsSkillShot(this SpellDataTargetType type)
        {
            return type.Equals(SpellDataTargetType.Location) || type.Equals(SpellDataTargetType.Location2) ||
                   type.Equals(SpellDataTargetType.LocationVector);
        }

        public static bool IsTargeted(this SpellDataTargetType type)
        {
            return type.Equals(SpellDataTargetType.Unit) || type.Equals(SpellDataTargetType.SelfAndUnit);
        }

        public static bool LastCastedDelay(this Spell spell, int delay)
        {
            var casted = ObjectManager.Player.LastCastedspell();
            return casted != null && casted.Name == spell.Instance.Name && Utils.TickCount - casted.Tick < delay;
        }
    }
}