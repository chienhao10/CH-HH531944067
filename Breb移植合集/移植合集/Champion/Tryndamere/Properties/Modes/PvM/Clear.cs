using System;
using System.Linq;
using EloBuddy;
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
        public static void Clear(EventArgs args)
        {
            if (Bools.HasSheenBuff())
            {
                return;
            }

            /// <summary>
            ///     The Clear E Logics.
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
                    if (Targets.Minions.Count() >= 3)
                    {
                        Variables.E.Cast(Targets.Minions[0].Position);
                    }
                }

                /// <summary>
                ///     The JungleClear E Logic.
                /// </summary>
                if (Targets.JungleMinions.Any())
                {
                    Variables.E.Cast(Targets.JungleMinions[0].Position);
                }
            }
        }
    }
}