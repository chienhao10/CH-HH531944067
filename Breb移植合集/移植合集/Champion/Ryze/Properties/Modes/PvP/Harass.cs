using System;
using EloBuddy;
using EloBuddy.SDK;
using ExorAIO.Utilities;
using LeagueSharp.Common;
using System.Linq;

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
        public static void Harass(EventArgs args)
        {
            if (!Targets.Target.IsValidTarget() ||
                Bools.IsSpellShielded(Targets.Target))
            {
                return;
            }

            /// <summary>
            ///     The Q Harass Logic.
            /// </summary>
            if (Variables.Q.IsReady() &&
                Targets.Target.LSIsValidTarget(Variables.Q.Range) &&
                GameObjects.Player.ManaPercent > ManaManager.NeededQMana &&
                Variables.getCheckBoxItem(Variables.QMenu, "qspell.harass"))
            {
                if (!Variables.Q.GetPrediction(Targets.Target).CollisionObjects.Any(c => c.IsMinion))
                {
                    Variables.Q.Cast(Targets.Target);
                }
            }
        }
    }
}