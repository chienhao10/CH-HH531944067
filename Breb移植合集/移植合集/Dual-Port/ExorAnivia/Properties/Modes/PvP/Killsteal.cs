using EloBuddy;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Anivia
{
    using System;
    using System.Linq;
    using ExorAIO.Utilities;
    using EloBuddy.SDK;

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
            ///     The KillSteal E Logic.
            /// </summary>
            if (Variables.E.IsReady() &&
               Variables.getCheckBoxItem(Variables.EMenu, "espell.ks"))
            {
                foreach (AIHeroClient target in
                    HeroManager.Enemies.Where(
                    t =>
                        !Bools.IsSpellShielded(t) &&
                        t.IsValidTarget(Variables.E.Range) &&
                        t.Health < Variables.E.GetDamage(t)))
                {
                    Variables.E.CastOnUnit(target);
                    return;
                }
            }

            /// <summary>
            ///     The KillSteal Q Logic.
            /// </summary>
            if (Variables.Q.IsReady() &&
                ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).ToggleState == 1 &&
               Variables.getCheckBoxItem(Variables.QMenu, "qspell.ks"))
            {
                foreach (AIHeroClient target in
                    HeroManager.Enemies.Where(
                    t =>
                        !Bools.IsSpellShielded(t) &&
                        t.IsValidTarget(Variables.Q.Range) &&
                        t.Health < Variables.Q.GetDamage(t)))
                {
                    Variables.Q.Cast(Variables.Q.GetPrediction(target).CastPosition);
                }
            }
        }
    }
}
