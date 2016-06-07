using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy.SDK;

namespace NabbActivator
{
    /// <summary>
    ///     The targets class.
    /// </summary>
    internal class Targets
    {
        /// <summary>
        ///     The main enemy target.
        /// </summary>
        public static AIHeroClient Target => TargetSelector.GetTarget(1200f, DamageType.Magical);

        /// <summary>
        ///     The jungle minion targets.
        /// </summary>
        /// <summary>
        ///     The minions target.
        /// </summary>
        public static List<Obj_AI_Minion> Minions
            =>
                GameObjects.AllyMinions.Where(
                    m =>
                        m.IsMinion() &&
                        m.LSIsValidTarget(750f, false)).ToList();

        /// <summary>
        ///     The jungle minion targets.
        /// </summary>
        public static List<Obj_AI_Minion> JungleMinions
            =>
                GameObjects.Jungle.Where(
                    m =>
                        m.LSIsValidTarget(500f) &&
                        !GameObjects.JungleSmall.Contains(m)).ToList();
    }
}