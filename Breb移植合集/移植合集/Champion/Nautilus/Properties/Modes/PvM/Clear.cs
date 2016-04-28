using System;
using System.Linq;
using EloBuddy;
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
            ///     The E Clear Logics.
            /// </summary>
            if (Variables.E.IsReady() && ObjectManager.Player.ManaPercent > ManaManager.NeededEMana &&
                Variables.getCheckBoxItem(Variables.EMenu, "espell.farm"))
            {
                /// <summary>
                ///     The E LaneClear Logic.
                /// </summary>
                if (Targets.Minions.Any())
                {
                    if (Targets.Minions.Count(m => m.IsValidTarget(Variables.E.Range)) >= 3)
                    {
                        Variables.E.Cast();
                    }
                }

                /// <summary>
                ///     The E JungleClear Logic.
                /// </summary>
                else if (Targets.JungleMinions.Any())
                {
                    Variables.E.Cast();
                }
            }
        }
    }
}