using EloBuddy;
using LeagueSharp;
using LeagueSharp.Common;

namespace iKalistaReborn.Utils
{
    /// <summary>
    ///     TODO The damages.
    /// </summary>
    internal static class Damages
    {
        #region Static Fields

        /// <summary>
        ///     TODO The player.
        /// </summary>
        private static readonly AIHeroClient Player = ObjectManager.Player;

        /// <summary>
        ///     TODO The raw rend damage.
        /// </summary>
        private static readonly float[] RawRendDamage = { 20, 30, 40, 50, 60 };

        /// <summary>
        ///     TODO The raw rend damage multiplier.
        /// </summary>
        private static readonly float[] RawRendDamageMultiplier = { 0.6f, 0.6f, 0.6f, 0.6f, 0.6f };

        /// <summary>
        ///     TODO The raw rend damage per spear.
        /// </summary>
        private static readonly float[] RawRendDamagePerSpear = { 10, 14, 19, 25, 32 };

        /// <summary>
        ///     TODO The raw rend damage per spear multiplier.
        /// </summary>
        private static readonly float[] RawRendDamagePerSpearMultiplier = { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     TODO The get raw rend damage.
        /// </summary>
        /// <param name="target">
        ///     TODO The target.
        /// </param>
        /// <param name="customStacks">
        ///     TODO The custom stacks.
        /// </param>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
        public static float GetRawRendDamage(Obj_AI_Base target, int customStacks = -1)
        {
            // Get buff
            var buff = target?.GetRendBuff();

            if (buff == null && customStacks <= -1) return 0;

            if (buff != null)
                return RawRendDamage[SpellManager.Spell[SpellSlot.E].Level - 1]
                       + RawRendDamageMultiplier[SpellManager.Spell[SpellSlot.E].Level - 1]
                       * Player.TotalAttackDamage + // Base damage
                       ((customStacks < 0 ? buff.Count : customStacks) - 1) * // Spear count
                       (RawRendDamagePerSpear[SpellManager.Spell[SpellSlot.E].Level - 1]
                        + RawRendDamagePerSpearMultiplier[SpellManager.Spell[SpellSlot.E].Level - 1]
                        * Player.TotalAttackDamage); // Damage per spear

            return 0;
        }

        /// <summary>
        ///     TODO The get rend damage.
        /// </summary>
        /// <param name="target">
        ///     TODO The target.
        /// </param>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
        public static float GetRendDamage(AIHeroClient target)
        {
            return GetRendDamage(target, -1);
        }

        /// <summary>
        ///     TODO The get rend damage.
        /// </summary>
        /// <param name="target">
        ///     TODO The target.
        /// </param>
        /// <param name="customStacks">
        ///     TODO The custom stacks.
        /// </param>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
        public static float GetRendDamage(Obj_AI_Base target, int customStacks = -1)
        {
            // Calculate the damage and return
            return (float)Player.CalcDamage(target, DamageType.Physical, GetRawRendDamage(target, customStacks));
        }

        /// <summary>
        ///     TODO The total attack damage.
        /// </summary>
        /// <param name="target">
        ///     TODO The target.
        /// </param>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
        public static float TotalAttackDamage(this Obj_AI_Base target)
        {
            return target.BaseAttackDamage + target.FlatPhysicalDamageMod;
        }

        #endregion
    }
}