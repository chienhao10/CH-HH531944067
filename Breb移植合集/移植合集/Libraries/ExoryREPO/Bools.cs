using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;

namespace ExorAIO.Utilities
{
    /// <summary>
    ///     The Bools class.
    /// </summary>
    internal class Bools
    {

        /// <summary>
        ///     Gets a value indicating whether the target has protection or not.
        /// </summary>
        public static bool HasAnyImmunity(AIHeroClient unit, bool includeSpellShields = false)
            =>
                unit.IsInvulnerable ||
                unit.HasBuffOfType(BuffType.SpellImmunity) ||
                (includeSpellShields && unit.HasBuffOfType(BuffType.SpellShield));

        /// <summary>
        ///     Gets a value indicating whether the target has protection or not.
        /// </summary>
        public static bool IsSpellShielded(AIHeroClient unit)
        {
            return unit.IsInvulnerable ||
                   unit.HasBuffOfType(BuffType.SpellShield) ||
                   unit.HasBuffOfType(BuffType.SpellImmunity) ||
                   Utils.TickCount - unit.LastCastedSpellT() < 300 &&
                   (
                       unit.LastCastedSpellName().Equals("SivirE", StringComparison.InvariantCultureIgnoreCase) ||
                       unit.LastCastedSpellName().Equals("BlackShield", StringComparison.InvariantCultureIgnoreCase) ||
                       unit.LastCastedSpellName().Equals("NocturneShit", StringComparison.InvariantCultureIgnoreCase)
                       );
        }

        /// <summary>
        ///     Gets a value indicating whether the player has a sheen-like buff.
        /// </summary>
        public static bool HasSheenBuff()
        {
            return ObjectManager.Player.HasBuff("Sheen") ||
                   ObjectManager.Player.HasBuff("LichBane") ||
                   ObjectManager.Player.HasBuff("ItemFrozenFist");
        }

        /// <summary>
        ///     Gets a value indicating whether a determined champion can move or not.
        /// </summary>
        public static bool IsImmobile(AIHeroClient target)
        {
            return
                target.HasBuff("zhonyasringshield") ||
                target.IsChannelingImportantSpell() ||
                target.HasBuffOfType(BuffType.Stun) ||
                target.HasBuffOfType(BuffType.Flee) ||
                target.HasBuffOfType(BuffType.Snare) ||
                target.HasBuffOfType(BuffType.Taunt) ||
                target.HasBuffOfType(BuffType.Charm) ||
                target.HasBuffOfType(BuffType.Knockup) ||
                target.HasBuffOfType(BuffType.Suppression);
        }

        /// <summary>
        ///     Gets a value indicating whether a determined champion has a stackable item.
        /// </summary>
        public static bool HasTear(AIHeroClient target)
        {
            return target.InventoryItems.Any(item =>
                item.Id.Equals(ItemId.Tear_of_the_Goddess) ||
                item.Id.Equals(ItemId.Archangels_Staff) ||
                item.Id.Equals(ItemId.Manamune) ||
                item.Id.Equals(ItemId.Tear_of_the_Goddess_Crystal_Scar) ||
                item.Id.Equals(ItemId.Archangels_Staff_Crystal_Scar) ||
                item.Id.Equals(ItemId.Manamune_Crystal_Scar));
        }

        /// <summary>
        ///     Gets a value indicating whether a determined root is worth cleansing.
        /// </summary>
        public static bool IsValidSnare()
        {
            return ObjectManager.Player.Buffs.Any(
                b =>
                    b.Type == BuffType.Snare &&
                    !Variables.InvalidSnareCasters.Contains(((AIHeroClient) b.Caster).ChampionName));
        }

        /// <summary>
        ///     Gets a value indicating whether a determined Stun is worth cleansing.
        /// </summary>
        public static bool IsValidStun()
        {
            return ObjectManager.Player.Buffs.Any(
                b =>
                    b.Type == BuffType.Stun &&
                    !Variables.InvalidStunCasters.Contains(((AIHeroClient) b.Caster).ChampionName));
        }

        /// <summary>
        ///     Gets a value indicating whether BuffType is worth cleansing.
        /// </summary>
        public static bool ShouldCleanse(AIHeroClient target)
        {
            return
                !IsSpellShielded(target) &&
                IsValidStun() ||
                IsValidSnare() ||
                target.HasBuffOfType(BuffType.Flee) ||
                target.HasBuffOfType(BuffType.Charm) ||
                target.HasBuffOfType(BuffType.Taunt) ||
                target.HasBuffOfType(BuffType.Knockup) ||
                target.HasBuffOfType(BuffType.Knockback) ||
                target.HasBuffOfType(BuffType.Polymorph) ||
                target.HasBuffOfType(BuffType.Suppression);
        }

        /// <summary>
        ///     Defines whether the player has a deadly mark.
        /// </summary>
        public static bool HasDeadlyMark()
        {
            return
                !IsSpellShielded(ObjectManager.Player) &&
                ObjectManager.Player.HasBuff("SummonerExhaust") ||
                ObjectManager.Player.HasBuff("FizzMarinerDoom") ||
                ObjectManager.Player.HasBuff("ZedUltTargetMark") ||
                ObjectManager.Player.HasBuff("VladimirHemoplague") ||
                ObjectManager.Player.HasBuff("MordekaiserChildrenOfTheGrave");
        }

        /// <summary>
        ///     Returns true if the target is a perfectly valid rend target.
        /// </summary>
        public static bool IsPerfectRendTarget(Obj_AI_Base target)
        {
            if (target.HasBuff("KalistaExpungeMarker") &&
                target.IsValidTarget(Variables.E.Range))
            {
                if (target.IsValid<Obj_AI_Minion>())
                {
                    return true;
                }
                if (target.IsValid<AIHeroClient>())
                {
                    return !IsSpellShielded((AIHeroClient) target);
                }
            }

            return false;
        }
    }
}