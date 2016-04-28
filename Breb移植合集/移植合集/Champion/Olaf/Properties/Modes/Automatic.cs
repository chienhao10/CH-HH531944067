using System;
using EloBuddy;
using ExorAIO.Utilities;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Olaf
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
        public static void Automatic(EventArgs args)
        {
            /// <summary>
            ///     The R Automatic Logic.
            /// </summary>
            if (Variables.R.IsReady() &&
                Bools.ShouldCleanse(ObjectManager.Player) &&
                Variables.getCheckBoxItem(Variables.RMenu, "rspell.auto"))
            {
                Variables.R.Cast();
            }
        }
    }
}