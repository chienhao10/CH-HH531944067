using System;
using System.Linq;
using EloBuddy.SDK;
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
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Killsteal(EventArgs args)
        {
            /// <summary>
            ///     The KillSteal W Logic.
            /// </summary>
            if (Variables.W.IsReady() &&
                Variables.getCheckBoxItem(Variables.WMenu, "wspell.ks"))
            {
                foreach (var target in
                    HeroManager.Enemies.Where(
                        t =>
                            !Bools.IsSpellShielded(t) &&
                            t.IsValidTarget(Variables.W.Range) &&
                            !t.IsValidTarget(Variables.AARange) &&
                            t.Health < Variables.W.GetDamage(t) + Variables.Q.GetDamage(t)))
                {
                    Variables.W.CastOnUnit(target);
                    return;
                }
            }

            /// <summary>
            ///     The KillSteal Q Logic.
            /// </summary>
            if (Variables.Q.IsReady() &&
                Variables.getCheckBoxItem(Variables.QMenu, "qspell.ks"))
            {
                foreach (var target in
                    HeroManager.Enemies.Where(
                        t =>
                            !Bools.IsSpellShielded(t) &&
                            t.IsValidTarget(Variables.Q.Range) &&
                            !t.IsValidTarget(Variables.AARange) &&
                            t.Health < Variables.Q.GetDamage(t)))
                {
                    Variables.Q.CastOnUnit(target);
                }
            }
        }
    }
}