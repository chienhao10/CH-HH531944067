using System;
using System.Linq;
using EloBuddy;
using ExorAIO.Utilities;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Pantheon
{
    /// <summary>
    ///     The logics class.
    /// </summary>
    internal partial class Logics
    {
        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Clear(EventArgs args)
        {
            if (Bools.HasSheenBuff())
            {
                return;
            }

            /// <summary>
            ///     The Q JungleClear Logics.
            /// </summary>
            if (Variables.Q.IsReady() &&
                Targets.JungleMinions.Any() &&
                !ObjectManager.Player.HasBuff("pantheonpassiveshield") &&
                ObjectManager.Player.ManaPercent > ManaManager.NeededQMana &&
                Variables.getCheckBoxItem(Variables.QMenu, "qspell.jgc"))
            {
                Variables.Q.CastOnUnit(Targets.JungleMinions[0]);
            }

            /// <summary>
            ///     The E Clear Logics.
            /// </summary>
            if (Variables.E.IsReady() &&
                ObjectManager.Player.ManaPercent > ManaManager.NeededEMana &&
                Variables.getCheckBoxItem(Variables.EMenu, "espell.farm"))
            {
                /// <summary>
                ///     The LaneClear E Logic.
                /// </summary>
                if (Targets.Minions.Any())
                {
                    Variables.E.Cast(Targets.Minions[0].Position);
                }

                /// <summary>
                ///     The JungleClear E Logic.
                /// </summary>
                else if (Targets.JungleMinions.Any() &&
                         !ObjectManager.Player.HasBuff("pantheonpassiveshield"))
                {
                    Variables.E.Cast(Targets.JungleMinions[0].Position);
                }
            }
        }
    }
}