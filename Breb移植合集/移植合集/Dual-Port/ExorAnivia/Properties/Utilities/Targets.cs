using System.Collections.Generic;
using System.Linq;
using ExorSDK.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;
using EloBuddy.SDK;

namespace ExorSDK.Champions.Anivia
{
    /// <summary>
    ///     The targets class.
    /// </summary>
    internal class Targets
    {
        /// <summary>
        ///     The main hero target.
        /// </summary>
        public static AIHeroClient Target => TargetSelector.GetTarget(Vars.Q.Range, DamageType.Magical);

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
                        m.LSIsValidTarget(Vars.Q.Range) &&
                        !GameObjects.JungleSmall.Contains(m)).ToList();

        /// <summary>
        ///     The minions hit by the Q missile.
        /// </summary>
        public static List<Obj_AI_Minion> QMinions
            =>
                Minions.Concat(JungleMinions)
                    .Where(m => m.Distance(Anivia.QMissile.Position) < Vars.Q.Width*2 -10f)
                    .ToList();

        /// <summary>
        ///     The minions hit by the R missile.
        /// </summary>
        public static List<Obj_AI_Minion> RMinions
            =>
                Minions.Concat(JungleMinions)
                    .Where(m => m.Distance(Anivia.RMissile.Position) < Vars.R.Width+250f)
                    .ToList();
    }
}