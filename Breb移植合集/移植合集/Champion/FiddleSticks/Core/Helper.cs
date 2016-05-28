using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using SebbyLib;

namespace Feedlesticks.Core
{
    internal class Helper
    {
        /// <summary>
        ///     Enemy Immobile
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsEnemyImmobile(AIHeroClient target)
        {
            if (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) ||
                target.HasBuffOfType(BuffType.Knockup) ||
                target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Fear) ||
                target.HasBuffOfType(BuffType.Knockback) ||
                target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression) ||
                target.IsStunned || target.IsChannelingImportantSpell())
            {
                return true;
            }
            return false;
        }
    }
}