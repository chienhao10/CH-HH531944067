using System;
using EloBuddy;
using EloBuddy.SDK;
using ExorAIO.Utilities;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Tryndamere
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
            ///     The W Combo Logic.
            /// </summary>
            if (Variables.W.IsReady() &&
                !Targets.Target.IsFacing(ObjectManager.Player) &&
                Targets.Target.IsValidTarget(Variables.W.Range) &&
                Variables.getCheckBoxItem(Variables.WMenu, "wspell.combo"))
            {
                Variables.W.Cast();
            }

            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Variables.E.IsReady() &&
                Targets.Target.IsValidTarget(Variables.E.Range) &&
                !Targets.Target.IsValidTarget(Variables.AARange) &&
                Variables.getCheckBoxItem(Variables.EMenu, "espell.combo"))
            {
                Variables.E.Cast(Targets.Target.Position);
            }
        }
    }
}