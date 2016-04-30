using System;
using System.Linq;
using EloBuddy.SDK;
using ExorAIO.Utilities;
using LeagueSharp.Common;
using LeagueSharp.SDK.Core.Utils;

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
        public static void Killsteal(EventArgs args)
        {

            /// <summary>
            ///     The KillSteal Q Logic.
            /// </summary>
            if (Variables.Q.IsReady() &&
                Variables.getCheckBoxItem(Variables.QMenu, "qspell.ks"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(t => !Invulnerable.Check(t) && t.Health < Variables.Q.GetDamage(t) && t.IsValidTarget(Variables.Q.Range - 100f)))
                {
                    if (!Variables.Q.GetPrediction(Targets.Target).CollisionObjects.Any(c => c.IsMinion))
                    {
                        Variables.Q.Cast(Variables.Q.GetPrediction(target).UnitPosition);
                        return;
                    }
                }
            }

            /// <summary>
            ///     The KillSteal W Logic.
            /// </summary>
            if (Variables.W.IsReady() &&
                Variables.getCheckBoxItem(Variables.WMenu, "wspell.ks"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        !Invulnerable.Check(t) &&
                        t.IsValidTarget(Variables.W.Range) &&
                        t.Health < Variables.W.GetDamage(t)))
                {
                    Variables.W.CastOnUnit(target);
                    return;
                }
            }

            /// <summary>
            ///     The KillSteal E Logic.
            /// </summary>
            if (Variables.E.IsReady() &&
                Variables.getCheckBoxItem(Variables.EMenu, "espell.ks"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        !Invulnerable.Check(t) &&
                        t.IsValidTarget(Variables.E.Range) &&
                        t.Health < Variables.E.GetDamage(t)))
                {
                    Variables.E.CastOnUnit(target);
                }
            }
        }
    }
}