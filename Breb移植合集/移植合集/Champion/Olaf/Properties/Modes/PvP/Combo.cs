using System;
using ExorSDK.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;

namespace ExorSDK.Champions.Olaf
{
    /// <summary>
    ///     The logics class.
    /// </summary>
    internal partial class Logics
    {
        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Combo(EventArgs args)
        {
            if (Bools.HasSheenBuff() ||
                !Targets.Target.LSIsValidTarget() ||
                Invulnerable.Check(Targets.Target))
            {
                return;
            }

            /// <summary>
            ///     The Combo W Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.AARange) &&
                Vars.getCheckBoxItem(Vars.WMenu, "combo"))
            {
                Vars.W.Cast();
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                !Targets.Target.HasBuffOfType(BuffType.Slow) &&
                Targets.Target.LSIsValidTarget(Vars.Q.Range) &&
                Vars.getCheckBoxItem(Vars.QMenu, "combo"))
            {
                Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target)
                    .UnitPosition.LSExtend(GameObjects.Player.ServerPosition, -100f));
            }
        }
    }
}