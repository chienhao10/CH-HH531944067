using EloBuddy;
using LeagueSharp.Common;

namespace Jhin___The_Virtuoso.Extensions
{
    internal static class Spells
    {
        /// <summary>
        ///     Thats provide Q spell
        /// </summary>
        public static Spell Q;

        /// <summary>
        ///     Thats provide W spell
        /// </summary>
        public static Spell W;

        /// <summary>
        ///     Thats provide E spell
        /// </summary>
        public static Spell E;

        /// <summary>
        ///     Thats provide R spell
        /// </summary>
        public static Spell R;

        /// <summary>
        ///     Initialize all spells
        /// </summary>
        public static void Initialize()
        {
            Q = new Spell(SpellSlot.Q, 550);
            W = new Spell(SpellSlot.W, 2500);
            E = new Spell(SpellSlot.E, 2000);
            R = new Spell(SpellSlot.R, 3500);

            W.SetSkillshot(0.75f, 40, float.MaxValue, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.23f, 120, 1600, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.21f, 80, 5000, false, SkillshotType.SkillshotLine);
        }

        /// <summary>
        ///     Thats provide selected spell damage
        /// </summary>
        /// <param name="spell">Spell</param>
        /// <param name="unit">Target</param>
        /// <returns></returns>
        public static int GetDamage(this Spell spell, AIHeroClient unit)
        {
            return (int) spell.GetDamage(unit);
        }

        /// <summary>
        ///     Thats basically provide combo damage
        /// </summary>
        /// <param name="spell1">Q Spell</param>
        /// <param name="spell2">W Spell</param>
        /// <param name="spell3">E Spell</param>
        /// <param name="spell4">R Spell</param>
        /// <param name="unit">Target</param>
        /// <returns></returns>
        public static float ComboDamage(this Spell spell1, Spell spell2, Spell spell3, Spell spell4, AIHeroClient unit)
        {
            var combodamage = 0f;
            if (spell1.IsReady())
            {
                combodamage += spell1.GetDamage(unit);
            }
            if (spell2.IsReady())
            {
                combodamage += spell2.GetDamage(unit);
            }
            if (spell3.IsReady())
            {
                combodamage += spell2.GetDamage(unit);
            }
            if (spell4.IsReady())
            {
                combodamage += spell2.GetDamage(unit);
            }
            return combodamage;
        }
    }
}