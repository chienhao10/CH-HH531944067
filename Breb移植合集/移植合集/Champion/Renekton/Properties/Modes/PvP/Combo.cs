using System;
using EloBuddy;
using EloBuddy.SDK;
using ExorAIO.Utilities;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Renekton
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
            ///     The Q Combo Logic.
            /// </summary>
            if (Variables.Q.IsReady() &&
                !Variables.W.IsReady() &&
                Targets.Target.IsValidTarget(Variables.Q.Range) &&
                !ObjectManager.Player.HasBuff("RenektonPreExecute") &&
                Variables.getCheckBoxItem(Variables.QMenu, "qspell.combo"))
            {
                Variables.Q.Cast();
            }

            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Variables.E.IsReady() &&
                Targets.Target.IsValidTarget(Variables.E.Range) &&
                !ObjectManager.Player.HasBuff("renektonsliceanddicedelay") &&
                (!Targets.Target.UnderTurret() || Targets.Target.HealthPercent < 10) &&
                Variables.getCheckBoxItem(Variables.EMenu, "espell.combo"))
            {
                Variables.E.Cast(Targets.Target.Position);
            }
        }
    }
}