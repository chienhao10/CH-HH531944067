using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;
using ExorAIO.Utilities;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Ryze
{
    /// <summary>
    ///     The targets class.
    /// </summary>
    internal class Targets
    {
        /// <summary>
        ///     The main hero target.
        /// </summary>
        public static AIHeroClient Target
        {
            get { return TargetSelector.GetTarget(Variables.Q.Range, DamageType.Magical); }
        }

        /// <summary>
        ///     The minions target.
        /// </summary>
        public static List<Obj_AI_Base> Minions
        {
            get
            {
                return
                    MinionManager.GetMinions(
                        ObjectManager.Player.ServerPosition,
                        Variables.Q.Range,
                        MinionTypes.All,
                        MinionTeam.NotAlly,
                        MinionOrderTypes.None);
            }
        }

        /// <summary>
        ///     The jungle minion targets.
        /// </summary>
        public static List<Obj_AI_Base> JungleMinions
        {
            get
            {
                return MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition,
                    Variables.Q.Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);
            }
        }
    }
}