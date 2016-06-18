using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace WuAIO.Extensions
{
    public static class SpellsExtensions
    {
        public static bool HitChanceCast(this Spell.Skillshot spell, Obj_AI_Base target, float chance = 85)
        {
            var pred = spell.GetPrediction(target);

            if (pred.HitChancePercent >= chance)
                if (spell.Cast(pred.CastPosition))
                    return true;

            return false;
        }

        public static bool HitChanceCast(this Spell.Skillshot spell, Obj_AI_Base target, HitChance chance)
        {
            var pred = spell.GetPrediction(target);

            if (pred.HitChance >= chance)
                if (spell.Cast(pred.CastPosition))
                    return true;

            return false;
        }
    }
}
