using System;
using EloBuddy.SDK;
using ExorAIO.Utilities;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Nautilus
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
                !Targets.Target.IsValidTarget() ||
                Bools.IsImmobile(Targets.Target) ||
                Bools.IsSpellShielded(Targets.Target))
            {
                return;
            }

            /// <summary>
            ///     The R Combo Logic.
            /// </summary>
            if (Variables.R.IsReady() && Targets.Target.IsValidTarget(Variables.R.Range) &&
                Variables.getCheckBoxItem(Variables.RMenu, "rspell.combo") &&
                Variables.getCheckBoxItem(Variables.WhiteListMenu,
                    "rspell.whitelist." + Targets.Target.NetworkId))
            {
                Variables.R.CastOnUnit(Targets.Target);
                return;
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Variables.Q.IsReady() && Targets.Target.IsValidTarget(Variables.Q.Range) && Variables.getCheckBoxItem(Variables.QMenu, "qspell.combo"))
            {
                if (Variables.Q.GetPrediction(Targets.Target).CollisionObjects.Count < 0)
                {
                    Variables.Q.Cast(Targets.Target);
                    return;
                }
            }

            if (Bools.HasSheenBuff())
            {
                return;
            }

            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Variables.E.IsReady() &&
                Targets.Target.IsValidTarget(Variables.E.Range) &&
                Variables.getCheckBoxItem(Variables.EMenu, "espell.combo"))
            {
                Variables.E.Cast();
            }
        }
    }
}