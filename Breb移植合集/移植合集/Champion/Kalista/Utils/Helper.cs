using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using SharpDX;
using Collision = LeagueSharp.Common.Collision;

namespace iKalistaReborn.Utils
{
    /// <summary>
    ///     The Helper class
    /// </summary>
    internal static class Helper
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Gets the list of minions currently between the source and target
        /// </summary>
        /// <param name="source">
        ///     The Source
        /// </param>
        /// <param name="targetPosition">
        ///     The Target Position
        /// </param>
        /// <returns>
        ///     The <see cref="List" />.
        /// </returns>
        public static List<Obj_AI_Base> GetCollisionMinions(AIHeroClient source, Vector3 targetPosition)
        {
            var input = new PredictionInput
            {
                Unit = source,
                Radius = SpellManager.Spell[SpellSlot.Q].Width,
                Delay = SpellManager.Spell[SpellSlot.Q].Delay,
                Speed = SpellManager.Spell[SpellSlot.Q].Speed,
                CollisionObjects = new[] { CollisionableObjects.Minions }
            };

            return
                Collision.GetCollision(new List<Vector3> { targetPosition }, input)
                    .OrderBy(x => x.Distance(source))
                    .ToList();
        }

        /// <summary>
        ///     Gets the rend buff
        /// </summary>
        /// <param name="target">
        ///     The Target
        /// </param>
        /// <returns>
        ///     The <see cref="BuffInstance" />.
        /// </returns>
        public static BuffInstance GetRendBuff(this Obj_AI_Base target)
        {
            return target.Buffs.Find(b => b.Caster.IsMe && b.IsValid() && b.DisplayName == "KalistaExpungeMarker");
        }

        /// <summary>
        ///     Gets the current <see cref="BuffInstance" /> Count of Expunge
        /// </summary>
        /// <param name="target">
        ///     The Target
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public static int GetRendBuffCount(this Obj_AI_Base target)
            => target.Buffs.Count(x => x.Name == "kalistaexpungemarker");

        /// <summary>
        ///     Gets the Rend Damage for each target
        /// </summary>
        /// <param name="target">
        ///     The Target
        /// </param>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
        public static float GetRendDamage(Obj_AI_Base target) => Damages.GetActualDamage(target);

        public static bool HasSpellShield(this AIHeroClient target)
        {
            //Banshee's Veil
            if (target.Buffs.Any(b => b.IsValid() && b.DisplayName == "bansheesveil"))
            {
                return true;
            }

            //Sivir E
            if (target.Buffs.Any(b => b.IsValid() && b.DisplayName == "SivirE"))
            {
                return true;
            }

            //Nocturne W
            if (target.Buffs.Any(b => b.IsValid() && b.DisplayName == "NocturneW"))
            {
                return true;
            }

            //Other spellshields
            return target.HasBuffOfType(BuffType.SpellShield) || target.HasBuffOfType(BuffType.SpellImmunity);
        }

        public static bool IsRendKillable(this Obj_AI_Base target)
        {
            if (target == null 
                || !target.IsValidTarget(SpellManager.Spell[SpellSlot.E].Range + 200)
                || !target.HasRendBuff()
                || target.Health <= 0
                || !SpellManager.Spell[SpellSlot.E].IsReady())
            {
                return false;
            }

            var hero = target as AIHeroClient;
            if (hero != null)
            {
                if (hero.HasUndyingBuff() || hero.HasSpellShield())
                {
                    return false;
                }

                if (hero.ChampionName == "Blitzcrank")
                {
                    if (!hero.HasBuff("BlitzcrankManaBarrierCD") && !hero.HasBuff("ManaBarrier"))
                    {
                        return Damages.GetActualDamage(target) > (target.GetTotalHealth() + (hero.Mana / 2));
                    }

                    if (hero.HasBuff("ManaBarrier") && !(hero.AllShield > 0))
                    {
                        return false;
                    }
                }
            }

            return Damages.GetActualDamage(target) > target.GetTotalHealth();
        }

        public static float GetTotalHealth(this Obj_AI_Base target)
        {
            return target.Health + target.AllShield + target.AttackShield + target.MagicShield + (target.HPRegenRate * 2);
        }

        /// <summary>
        ///     Checks if a target has the Expunge <see cref="BuffInstance" />
        /// </summary>
        /// <param name="target">
        ///     The Target
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool HasRendBuff(this Obj_AI_Base target)
        {
            return target.GetRendBuff() != null;
        }
        /// <summary>
        ///     Checks if the given target has an invulnerable buff
        /// </summary>
        /// <param name="target1">
        ///     The Target
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool HasUndyingBuff(this Obj_AI_Base target1)
        {
            var target = target1 as AIHeroClient;

            if (target == null) return false;
            // Tryndamere R
            if (target.ChampionName == "Tryndamere"
                && target.Buffs.Any(
                    b => b.Caster.NetworkId == target.NetworkId && b.IsValid && b.DisplayName == "Undying Rage"))
            {
                return true;
            }

            // Zilean R
            if (target.Buffs.Any(b => b.IsValid && b.DisplayName == "Chrono Shift"))
            {
                return true;
            }

            if (target.HasBuff("kindredrnodeathbuff"))
            {
                return true;
            }

            // Kayle R
            if (target.Buffs.Any(b => b.IsValid && b.DisplayName == "JudicatorIntervention"))
            {
                return true;
            }

            //TODO poppy

            return false;
        }

        /// <summary>
        ///     TODO The is mob killable.
        /// </summary>
        /// <param name="target">
        ///     TODO The target.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsMobKillable(this Obj_AI_Base target) => IsRendKillable(target);

        #endregion
    }
}