using LeagueSharp;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Caitlyn
{
    using System.Collections.Generic;
    using ExorAIO.Utilities;
    using EloBuddy;
    using EloBuddy.SDK;    /// <summary>
                           ///     The targets class.
                           /// </summary>
    class Targets
    {
        /// <summary>
        ///     The main hero target.
        /// </summary>
        public static AIHeroClient Target
        =>
            TargetSelector.GetTarget(
                Variables.Q.Range,
                DamageType.Physical);

        /// <summary>
        ///     The minions target.
        /// </summary>
        public static List<Obj_AI_Base> Minions
        =>
            MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                Variables.Q.Range,
                MinionTypes.All,
                MinionTeam.NotAlly,
                MinionOrderTypes.None);

        /// <summary>
        ///     The jungle minion targets.
        /// </summary>
        public static List<Obj_AI_Base> JungleMinions
        => 
            MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                Variables.Q.Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
    }
}
