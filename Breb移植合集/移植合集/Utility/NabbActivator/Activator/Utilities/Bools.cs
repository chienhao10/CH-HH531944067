using System.Linq;
using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;

namespace NabbActivator
{
    /// <summary>
    ///     The bools.
    /// </summary>
    internal class Bools
    {
        /// <summary>
        ///     Defines whether a Health Potion is running.
        /// </summary>
        public static bool IsHealthPotRunning()
        {
            return GameObjects.Player.HasBuff("ItemCrystalFlask") ||
                   GameObjects.Player.HasBuff("RegenerationPotion") ||
                   GameObjects.Player.HasBuff("ItemMiniRegenPotion") ||
                   GameObjects.Player.HasBuff("ItemDarkCrystalFlask") ||
                   GameObjects.Player.HasBuff("ItemCrystalFlaskJungle");
        }

        /// <summary>
        ///     Defines whether a Mana Potion is running.
        /// </summary>
        public static bool IsManaPotRunning()
        {
            return GameObjects.Player.HasBuff("ItemDarkCrystalFlask") ||
                   GameObjects.Player.HasBuff("ItemCrystalFlaskJungle");
        }

        /// <summary>
        ///     Gets a value indicating whether the target has protection or not.
        /// </summary>
        /// <summary>
        ///     Gets a value indicating whether a determined root is worth cleansing.
        /// </summary>
        public static bool IsValidSnare()
        {
            return GameObjects.Player.Buffs.Any(
                b =>
                    b.Type == BuffType.Snare &&
                    !Vars.InvalidSnareCasters.Contains((b.Caster as AIHeroClient).ChampionName));
        }

        /// <summary>
        ///     Gets a value indicating whether a determined Stun is worth cleansing.
        /// </summary>
        public static bool IsValidStun()
        {
            return GameObjects.Player.Buffs.Any(
                b =>
                    b.Type == BuffType.Stun &&
                    !Vars.InvalidStunCasters.Contains((b.Caster as AIHeroClient).ChampionName));
        }

        /// <summary>
        ///     Gets a value indicating whether BuffType is worth cleansing.
        /// </summary>
        public static bool ShouldCleanse(AIHeroClient target)
        {
            return !Invulnerable.Check(target) &&
                GameObjects.EnemyHeroes.Any(t => t.LSIsValidTarget(1500f)) &&
                (
                   IsValidStun() ||
                   IsValidSnare() ||
                   target.HasBuffOfType(BuffType.Flee) ||
                   target.HasBuffOfType(BuffType.Charm) ||
                   target.HasBuffOfType(BuffType.Taunt) ||
                   target.HasBuffOfType(BuffType.Polymorph) ||
                   GameObjects.Player.HasBuff("ThreshQ")
                );
        }

        /// <summary>
        ///     Defines whether the player should use a cleanser.
        /// </summary>
        public static bool ShouldUseCleanser()
        {
            return !Invulnerable.Check(GameObjects.Player) &&
                GameObjects.EnemyHeroes.Any(t => t.LSIsValidTarget(1500f)) &&
                (
                   GameObjects.Player.HasBuffOfType(BuffType.Suppression) ||
                   GameObjects.Player.HasBuff("ThreshQ") /* ||
                   GameObjects.Player.HasBuff("zedrdeathmark") ||
                   GameObjects.Player.HasBuff("summonerexhaust") ||
                   GameObjects.Player.HasBuff("fizzmarinerdoombomb") ||
                   GameObjects.Player.HasBuff("vladimirhemoplague") ||
                   GameObjects.Player.HasBuff("mordekaiserchildrenofthegrave")
                   */
                );
        }
    }
}