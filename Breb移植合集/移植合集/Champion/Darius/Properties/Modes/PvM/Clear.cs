using System;
using System.Linq;
using EloBuddy;
using ExorAIO.Utilities;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Darius
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
            if (Variables.Q.IsReady() && ObjectManager.Player.ManaPercent > ManaManager.NeededQMana &&
                (Targets.Minions.Count >= 3 || Targets.JungleMinions.Any()) &&
                Variables.getCheckBoxItem(Variables.QMenu, "qspell.farm"))
            {
                Variables.Q.Cast();
            }
        }
    }
}