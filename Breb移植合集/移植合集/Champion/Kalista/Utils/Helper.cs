namespace iKalistaReborn.Utils
{
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;
    using LeagueSharp.Common;

    using SharpDX;

    using Collision = LeagueSharp.Common.Collision;

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
                    .OrderBy(x => x.LSDistance(source))
                    .ToList();
        }

        /// <summary>
        ///     Gets the targets current health including shield damage
        /// </summary>
        /// <param name="target">
        ///     The Target
        /// </param>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
        public static float GetHealthWithShield(this Obj_AI_Base target)
            => target.AttackShield > 0 ? target.Health + target.AttackShield : target.Health + 10;

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
            =>
                target.Buffs.Find(
                    b => b.Caster.IsMe && b.IsValid && b.DisplayName.ToLowerInvariant() == "kalistaexpungemarker");

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

        public static float GetRendDamage(Obj_AI_Base target) => SpellManager.Spell[SpellSlot.E].GetDamage(target);

        /// <summary>
        ///     Checks if a target has the Expunge <see cref="BuffInstance" />
        /// </summary>
        /// <param name="target">
        ///     The Target
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool HasRendBuff(this Obj_AI_Base target) => target?.GetRendBuff() != null;

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

            // Kayle R
            if (target.Buffs.Any(b => b.IsValid && b.DisplayName == "JudicatorIntervention"))
            {
                return true;
            }

            if (target.HasBuff("kindredrnodeathbuff"))
            {
                return true;
            }

            // TODO poppy
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
        public static bool IsMobKillable(this Obj_AI_Base target) => IsRendKillable(target as Obj_AI_Minion);

        /// <summary>
        ///     Checks if the given target is killable
        /// </summary>
        /// <param name="target">
        ///     The Target
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsRendKillable(this Obj_AI_Base target)
        {
            if (target.IsInvulnerable || !target.HasBuff("kalistaexpungemarker"))
            {
                return false;
            }

            double baseDamage = SpellManager.Spell[SpellSlot.E].GetDamage(target);

            // Exhaust
            if (ObjectManager.Player.HasBuff("SummonerExhaust"))
            {
                baseDamage *= 0.6;
            }

            // Urgot P
            if (ObjectManager.Player.HasBuff("urgotentropypassive"))
            {
                baseDamage *= 0.85;
            }

            // Bond Of Stone
            var bondofstoneBuffCount = target.GetBuffCount("MasteryWardenOfTheDawn");
            if (bondofstoneBuffCount > 0)
            {
                baseDamage *= 1 - (0.06 * bondofstoneBuffCount);
            }

            // Phantom Dancer
            var phantomdancerBuff = ObjectManager.Player.GetBuff("itemphantomdancerdebuff");
            if (phantomdancerBuff != null && phantomdancerBuff.Caster == target)
            {
                baseDamage *= 0.88;
            }

            // Alistar R
            if (target.HasBuff("FerociousHowl"))
            {
                baseDamage *= 0.6 - new[] { 0.1, 0.2, 0.3 }[target.Spellbook.GetSpell(SpellSlot.R).Level - 1];
            }

            // Amumu E
            if (target.HasBuff("Tantrum"))
            {
                baseDamage -= new[] { 2, 4, 6, 8, 10 }[target.Spellbook.GetSpell(SpellSlot.E).Level - 1];
            }

            // Braum E
            if (target.HasBuff("BraumShieldRaise"))
            {
                baseDamage *= 1
                              - new[] { 0.3, 0.325, 0.35, 0.375, 0.4 }[target.Spellbook.GetSpell(SpellSlot.E).Level - 1];
            }

            // Galio R
            if (target.HasBuff("GalioIdolOfDurand"))
            {
                baseDamage *= 0.5;
            }

            // Garen W
            if (target.HasBuff("GarenW"))
            {
                baseDamage *= 0.7;
            }

            // Gragas W
            if (target.HasBuff("GragasWSelf"))
            {
                baseDamage *= 1
                              - new[] { 0.1, 0.12, 0.14, 0.16, 0.18 }[target.Spellbook.GetSpell(SpellSlot.W).Level - 1];
            }

            /*
            // Kassadin P
            if (target.HasBuff("VoidStone") && damageType == DamageType.Magical)
            {
                baseDamage *= 0.85;
            }
            */

            // Katarina E
            if (target.HasBuff("KatarinaEReduction"))
            {
                baseDamage *= 0.85;
            }

            // Maokai R
            if (target.HasBuff("MaokaiDrainDefense"))
            {
                baseDamage *= 0.8;
            }

            // MasterYi W
            if (target.HasBuff("Meditate"))
            {
                baseDamage *= 1 - new[] { 0.5, 0.55, 0.6, 0.65, 0.7 }[target.Spellbook.GetSpell(SpellSlot.W).Level - 1];
            }

            // Urgot R
            if (target.HasBuff("urgotswapdef"))
            {
                baseDamage *= 1 - new[] { 0.3, 0.4, 0.5 }[target.Spellbook.GetSpell(SpellSlot.R).Level - 1];
            }

            // Yorick P
            if (target.HasBuff("YorickUnholySymbiosis"))
            {
                baseDamage *= 1
                              - (ObjectManager.Get<Obj_AI_Minion>()
                                     .Count(
                                         g =>
                                         g.Team == target.Team
                                         && (g.Name.Equals("Clyde") || g.Name.Equals("Inky") || g.Name.Equals("Blinky")
                                             || (g.HasBuff("yorickunholysymbiosis")
                                                 && g.GetBuff("yorickunholysymbiosis").Caster == target))) * 0.05);
            }

            if (target is Obj_AI_Minion)
            {
                if (target.Name.Contains("Baron"))
                {
                    baseDamage *= 0.5f;
                }
            }

            return (float)baseDamage > target.GetHealthWithShield();
        }

        #endregion
    }
}