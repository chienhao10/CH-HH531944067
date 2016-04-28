using System;
using System.Linq;
using EloBuddy;
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
            ///     The Clear Q Logic.
            /// </summary>
            if (Variables.Q.IsReady() &&
                Variables.getCheckBoxItem(Variables.QMenu, "qspell.farm"))
            {
                if (Targets.Minions.Any() &&
                    Targets.Minions.Count() >= 3)
                {
                    Variables.Q.Cast();
                }
                else if (Targets.JungleMinions.Any())
                {
                    if (!Variables.W.IsReady() &&
                        !ObjectManager.Player.HasBuff("RenektonPreExecute"))
                    {
                        Variables.Q.Cast();
                    }
                }
            }
        }
    }
}