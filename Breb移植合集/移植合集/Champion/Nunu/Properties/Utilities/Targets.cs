using System.Collections.Generic;
using System.Linq;
using ExorSDK.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;
using EloBuddy.SDK;

namespace ExorSDK.Champions.Nunu
{
    /// <summary>
    ///     The targets class.
    /// </summary>
    internal class Targets
    {
        /// <summary>
        ///     The main hero target.
        /// </summary>
        public static AIHeroClient Target => TargetSelector.GetTarget(Vars.R.Range, DamageType.Magical);

        /// <summary>
        ///     The minions target.
        /// </summary>
        public static List<Obj_AI_Minion> Minions
            =>
                GameObjects.EnemyMinions.Where(
                    m =>
                        m.IsMinion() &&
                        m.LSIsValidTarget(Vars.E.Range)).ToList();

        /// <summary>
        ///     The jungle minion targets.
        /// </summary>
        public static List<Obj_AI_Minion> JungleMinions
            =>
                GameObjects.Jungle.Where(
                    m =>
                        m.LSIsValidTarget(Vars.E.Range) &&
                        !GameObjects.JungleSmall.Contains(m)).ToList();
    }
}