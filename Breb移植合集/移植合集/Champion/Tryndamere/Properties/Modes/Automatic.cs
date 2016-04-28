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
        public static void Automatic(EventArgs args)
        {
            /// <summary>
            ///     The Lifesaver R Logic.
            /// </summary>
            if (Variables.R.IsReady() &&
                !Variables.Q.IsReady() &&
                ObjectManager.Player.CountEnemiesInRange(700f) > 0 &&
                HealthPrediction.GetHealthPrediction(ObjectManager.Player, (int) (250 + Game.Ping/2f)) <=
                ObjectManager.Player.MaxHealth/4 &&
                Variables.getCheckBoxItem(Variables.RMenu, "rspell.lifesaver"))
            {
                Variables.R.Cast();
            }

            if (ObjectManager.Player.IsRecalling())
            {
                return;
            }

            /// <summary>
            ///     The Automatic Q Logic.
            /// </summary>
            if (Variables.Q.IsReady() &&
                ObjectManager.Player.ManaPercent >= 75 &&
                HealthPrediction.GetHealthPrediction(ObjectManager.Player, (int) (250 + Game.Ping/2f)) <=
                ObjectManager.Player.MaxHealth/2 &&
                Variables.getCheckBoxItem(Variables.QMenu, "qspell.auto"))
            {
                Variables.Q.Cast();
            }
        }
    }
}