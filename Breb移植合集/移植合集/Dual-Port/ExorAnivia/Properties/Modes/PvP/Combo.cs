using EloBuddy;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Anivia
{
    using System;
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
        public static void Combo(EventArgs args)
        {
            if (Bools.HasSheenBuff() ||
                !Targets.Target.IsValidTarget() ||
                Bools.IsSpellShielded(Targets.Target))
            {
                return;
            }

            /// <summary>
            ///     The Wall Logic.
            /// </summary>
            if (Variables.W.IsReady() &&
                ObjectManager.Player.ManaPercent > 30 &&
                Targets.Target.IsValidTarget(Variables.W.Range) &&
               Variables.getCheckBoxItem(Variables.WMenu, "wspell.combo") &&
                Variables.getCheckBoxItem(Variables.WhiteListMenu, "wspell.whitelist." + Targets.Target.NetworkId))
            {
                Variables.W.Cast(ObjectManager.Player.ServerPosition.LSExtend(Targets.Target.ServerPosition, ObjectManager.Player.LSDistance(Targets.Target) + 20f));
            }

            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Variables.E.IsReady() &&
                Targets.Target.HasBuff("chilled") &&
                Targets.Target.IsValidTarget(Variables.R.Range) &&
               Variables.getCheckBoxItem(Variables.EMenu, "espell.combo"))
            {
                Variables.E.CastOnUnit(Targets.Target);
            }

            /// <summary>
            ///     The R Combo Logic.
            /// </summary>
            if (Variables.R.IsReady() &&
                Targets.Target.IsValidTarget(Variables.R.Range) &&
                ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).ToggleState == 1 &&
               Variables.getCheckBoxItem(Variables.RMenu, "rspell.combo"))
            {
                Variables.R.Cast(Variables.R.GetPrediction(Targets.Target).CastPosition);
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Variables.Q.IsReady() &&
                Targets.Target.IsValidTarget(Variables.Q.Range) &&
                ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).ToggleState == 1 &&
               Variables.getCheckBoxItem(Variables.QMenu, "qspell.combo"))
            {
                Variables.Q.Cast(Variables.Q.GetPrediction(Targets.Target).CastPosition);
            }
        }
    }
}
