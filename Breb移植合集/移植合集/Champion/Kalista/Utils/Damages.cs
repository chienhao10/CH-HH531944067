using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;
using EloBuddy.SDK;

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

        private static readonly float[] RawRendDamage = { 20, 30, 40, 50, 60 };
        private static readonly float[] RawRendDamageMultiplier = { 0.6f, 0.6f, 0.6f, 0.6f, 0.6f };
        private static readonly float[] RawRendDamagePerSpear = { 10, 14, 19, 25, 32 };
        private static readonly float[] RawRendDamagePerSpearMultiplier = { 0.2f, 0.225f, 0.25f, 0.275f, 0.3f };

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
        public static float GetRawRendDamage(Obj_AI_Base target)
        {
            var stacks = (target.HasRendBuff() ? target.GetRendBuff().Count : 0) - 1;
            if (stacks > -1)
            {
                var index = SpellManager.Spell[SpellSlot.E].Level - 1;
                return RawRendDamage[index] + stacks * RawRendDamagePerSpear[index] +
                       EloBuddy.Player.Instance.TotalAttackDamage * (RawRendDamageMultiplier[index] + stacks * RawRendDamagePerSpearMultiplier[index]);
            }

            return 0;
        }

        public static float GetActualDamage(Obj_AI_Base target)
        {
            if (!SpellManager.Spell[SpellSlot.E].IsReady() || !target.HasRendBuff()) return 0f;

            var damage = GetRendDamage(target);

            if (target.Name.Contains("Baron"))
            {
                // Buff Name: barontarget or barondebuff
                // Baron's Gaze: Baron Nashor takes 50% reduced damage from champions he's damaged in the last 15 seconds. 
                damage = EloBuddy.Player.Instance.HasBuff("barontarget")
                    ? damage * 0.5f
                    : damage;
            }

            else if (target.Name.Contains("Dragon"))
            {
                // DragonSlayer: Reduces damage dealt by 7% per a stack
                damage = EloBuddy.Player.Instance.HasBuff("s5test_dragonslayerbuff")
                    ? damage * (1 - (.07f * EloBuddy.Player.Instance.GetBuffCount("s5test_dragonslayerbuff")))
                    : damage;
            }

            if (EloBuddy.Player.Instance.HasBuff("summonerexhaust"))
            {
                damage = damage * 0.6f;
            }

            if (target.HasBuff("FerociousHowl"))
            {
                damage = damage * 0.7f;
            }

            return damage;
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
        public static float GetRendDamage(Obj_AI_Base target)
        {
            return EloBuddy.Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, GetRawRendDamage(target)) *
                   (EloBuddy.Player.Instance.HasBuff("summonerexhaust") ? 0.6f : 1);
        }

        #endregion
    }
}