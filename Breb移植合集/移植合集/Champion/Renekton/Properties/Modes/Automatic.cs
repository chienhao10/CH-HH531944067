using System;
using System.Linq;
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
        public static void Automatic(EventArgs args)
        {
            if (ObjectManager.Player.IsRecalling())
            {
                return;
            }

            /// <summary>
            ///     The Automatic Q Logic.
            /// </summary>
            if (Variables.Q.IsReady() &&
                !ObjectManager.Player.UnderTurret() &&
                ObjectManager.Player.ManaPercent >= 50 &&
                Variables.getCheckBoxItem(Variables.QMenu, "qspell.auto"))
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                    ObjectManager.Player.HasBuff("RenektonPreExecute"))
                {
                    return;
                }

                if (HeroManager.Enemies.Any(
                    t =>
                        t.IsValidTarget(Variables.Q.Range) &&
                        (!t.IsValidTarget(Variables.W.Range) || !Variables.W.IsReady())))
                {
                    Variables.Q.Cast();
                }
            }

            /// <summary>
            ///     The Automatic R Logic.
            /// </summary>
            if (Variables.R.IsReady() &&
                ObjectManager.Player.CountEnemiesInRange(700f) > 0 &&
                HealthPrediction.GetHealthPrediction(ObjectManager.Player, (int) (250 + Game.Ping/2f)) <=
                ObjectManager.Player.MaxHealth/4 &&
                Variables.getCheckBoxItem(Variables.RMenu, "rspell.lifesaver"))
            {
                Variables.R.Cast();
            }
        }
    }
}