using System;
using EloBuddy.SDK;
using ExorAIO.Utilities;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Ryze
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
            if (Bools.HasSheenBuff() || !Targets.Target.IsValidTarget() || Bools.IsSpellShielded(Targets.Target))
            {
                return;
            }

            /// <summary>
            ///     The R Combo Logic.
            /// </summary>
            if (Variables.R.IsReady() && Bools.IsImmobile(Targets.Target) && Variables.getCheckBoxItem(Variables.RMenu, "rspell.combo"))
            {
                Variables.R.Cast();
            }

            /// <summary>
            ///     The W Combo Logic.
            /// </summary>
            if (Variables.W.IsReady() && !Bools.IsImmobile(Targets.Target) & Targets.Target.IsValidTarget(Variables.W.Range) && Variables.getCheckBoxItem(Variables.WMenu, "wspell.combo"))
            {
                Variables.W.CastOnUnit(Targets.Target);
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Variables.Q.IsReady() &&
                Targets.Target.IsValidTarget(Variables.Q.Range) &&
                Variables.getCheckBoxItem(Variables.QMenu, "qspell.combo"))
            {
                Variables.Q.Cast(Targets.Target);
            }

            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Variables.E.IsReady() && Targets.Target.IsValidTarget(Variables.E.Range) && Variables.getCheckBoxItem(Variables.EMenu, "espell.combo"))
            {
                Variables.E.CastOnUnit(Targets.Target);
            }
        }
    }
}