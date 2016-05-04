using LeagueSharp;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Caitlyn
{
    using System;
    using System.Linq;
    using ExorAIO.Utilities;
    using SPrediction;
    using EloBuddy;
    /// <summary>
    ///     The logics class.
    /// </summary>
    partial class Logics
    {
        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        public static void Killsteal(EventArgs args)
        {
            /// <summary>
            ///     The KillSteal Q Logic.
            /// </summary>
            if (Variables.Q.IsReady() &&
                Menus.getCheckBoxItem(Variables.QMenu, "qspell.ks"))
            {
                foreach (AIHeroClient target in
                    HeroManager.Enemies.Where(
                    t =>
                        !Bools.IsSpellShielded(t) &&
                        !t.LSIsValidTarget(Variables.AARange) &&
                        t.LSIsValidTarget(Variables.Q.Range - 150f) &&
                        t.Health < Variables.Q.GetDamage(t) * 0.77f))
                {
                    Variables.Q.Cast(Variables.Q.GetSPrediction(target).CastPosition);
                    return;
                }
            }

            /// <summary>
            ///     The KillSteal R Logic.
            /// </summary>
            if (Variables.R.IsReady() &&
                ObjectManager.Player.LSCountEnemiesInRange(Variables.Q.Range) == 0 &&
                Menus.getCheckBoxItem(Variables.RMenu, "rspell.ks"))
            {
                foreach (AIHeroClient target in
                    HeroManager.Enemies.Where(
                    t =>
                        !Bools.IsSpellShielded(t) &&
                        t.LSIsValidTarget(Variables.R.Range) &&
                        !t.LSIsValidTarget(Variables.AARange) &&
                        t.Health < Variables.R.GetDamage(t)))
                {
                    Variables.R.CastOnUnit(target);
                }
            }
        }
    }
}
