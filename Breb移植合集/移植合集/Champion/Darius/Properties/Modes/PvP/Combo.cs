using System;
using EloBuddy.SDK;
using ExorAIO.Utilities;
using LeagueSharp.Common;
using EloBuddy;

namespace ExorAIO.Champions.Darius
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
                Bools.IsSpellShielded(Targets.Target))
            {
                return;
            }

            /// <summary>
            /// The E Combo Logic.
            /// </summary>
            if (Variables.E.IsReady() &&
                Targets.Target.IsValidTarget(Variables.E.Range) &&
                !Targets.Target.IsValidTarget(Variables.AARange) &&
                Variables.getCheckBoxItem(Variables.EMenu, "espell.combo"))
            {
                Variables.E.Cast(Targets.Target);
            }

            if (Variables.Q.IsCharging)
            {
                Orbwalker.OrbwalkTo(Game.CursorPos);
            }

            /// <summary>
            /// The Q Combo Logic.
            /// </summary>
            if (Variables.Q.IsReady() && Targets.Target.IsValidTarget(Variables.Q.Range) && Variables.getCheckBoxItem(Variables.QMenu, "qspell.combo"))
            {
                Variables.Q.StartCharging();
            }
        }
    }
}