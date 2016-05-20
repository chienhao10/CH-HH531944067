using EloBuddy;
using LeagueSharp.Common;
using EloBuddy.SDK;

namespace ExorAIO.Champions.Anivia
{
    using System.Collections.Generic;
    using ExorAIO.Utilities;

    /// <summary>
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
                DamageType.Magical);

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

        /// <summary>
        ///     The minions hit by the Q missile.
        /// </summary>
        public static List<Obj_AI_Base> QMinions
        => 
            MinionManager.GetMinions(
                Anivia.QMissile.Position,
                Variables.Q.Width,
                MinionTypes.All,
                MinionTeam.NotAlly,
                MinionOrderTypes.MaxHealth);

        /// <summary>
        ///     The minions hit by the R missile.
        /// </summary>
        public static List<Obj_AI_Base> RMinions
        => 
            MinionManager.GetMinions(
                Anivia.RMissile.Position,
                Variables.R.Width,
                MinionTypes.All,
                MinionTeam.NotAlly,
                MinionOrderTypes.MaxHealth);
    }
}
