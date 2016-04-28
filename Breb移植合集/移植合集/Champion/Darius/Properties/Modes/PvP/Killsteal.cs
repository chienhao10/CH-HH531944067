using System;
using System.Linq;
using EloBuddy.SDK;
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
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Killsteal(EventArgs args)
        {
            /// <summary>
            ///     The KillSteal R Logic.
            /// </summary>
            if (Variables.R.IsReady() && Variables.getCheckBoxItem(Variables.RMenu, "rspell.ks"))
            {
                foreach (var target in
                    HeroManager.Enemies.Where(
                        t =>
                            !Bools.IsSpellShielded(t) &&
                            t.IsValidTarget(Variables.R.Range) &&
                            t.Health < Damage.GetRDamage(t)))
                {
                    Variables.R.CastOnUnit(target);
                }
            }
        }
    }
}