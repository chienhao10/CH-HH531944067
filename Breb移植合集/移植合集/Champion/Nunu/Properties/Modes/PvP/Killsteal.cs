using System;
using System.Linq;
using EloBuddy.SDK;
using ExorAIO.Utilities;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Nunu
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
            ///     The KillSteal E Logic.
            /// </summary>
            if (Variables.E.IsReady() &&
                Variables.getCheckBoxItem(Variables.EMenu, "espell.ks"))
            {
                foreach (var target in
                    HeroManager.Enemies.Where(
                        t =>
                            !Bools.IsSpellShielded(t) &&
                            t.IsValidTarget(Variables.E.Range) &&
                            t.Health < Variables.E.GetDamage(t)))
                {
                    Variables.E.CastOnUnit(target);
                }
            }
        }
    }
}