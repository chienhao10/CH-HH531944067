using System;
using EloBuddy.SDK;
using ExorAIO.Utilities;
using LeagueSharp.Common;

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
        public static void Combo(EventArgs args)
        {
            if (!Targets.Target.IsValidTarget() || Bools.IsSpellShielded(Targets.Target))
            {
                return;
            }

            if (Bools.HasSheenBuff())
            {
                if (Targets.Target.IsValidTarget(Variables.AARange))
                {
                    return;
                }
            }

            /// <summary>
            ///     The R Combo Logic.
            /// </summary>
            if (Variables.R.IsReady() && GameObjects.Player.ManaPercent > 20 && Variables.getCheckBoxItem(Variables.RMenu, "rspell.combo"))
            {
                if (!GameObjects.Player.HasBuff("RyzePassiveCharged") && GameObjects.Player.GetBuffCount("RyzePassiveStack") == 0)
                {
                    return;
                }

                if (!Variables.Q.IsReady() &&
                    GameObjects.Player.GetBuffCount("RyzePassiveStack") == 3)
                {
                    Variables.R.Cast();
                }
                else if (GameObjects.Player.GetBuffCount("RyzePassiveStack") < 3)
                {
                    Variables.R.Cast();
                }
            }

            /// <summary>
            ///     The W Combo Logic.
            /// </summary>
            if (Variables.W.IsReady() &&
                Targets.Target.IsValidTarget(Variables.W.Range) &&
                Variables.getCheckBoxItem(Variables.WMenu, "wspell.combo"))
            {
                if (!Variables.Q.IsReady() &&
                    GameObjects.Player.GetBuffCount("RyzePassiveStack") == 1)
                {
                    return;
                }

                if (GameObjects.Player.HasBuff("RyzePassiveCharged") ||
                    GameObjects.Player.GetBuffCount("RyzePassiveStack") != 0)
                {
                    Variables.W.CastOnUnit(Targets.Target);
                }
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Variables.Q.IsReady() &&
                Targets.Target.IsValidTarget(Variables.Q.Range - 50f) &&
                Variables.getCheckBoxItem(Variables.QMenu, "qspell.combo"))
            {
                Variables.Q.Cast(Targets.Target);
            }

            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Variables.E.IsReady() &&
                Targets.Target.IsValidTarget(Variables.E.Range) &&
                Variables.getCheckBoxItem(Variables.EMenu, "espell.combo"))
            {
                if (!Variables.Q.IsReady() &&
                    GameObjects.Player.GetBuffCount("RyzePassiveStack") == 1)
                {
                    return;
                }

                if (GameObjects.Player.HasBuff("RyzePassiveCharged") ||
                    GameObjects.Player.GetBuffCount("RyzePassiveStack") != 0)
                {
                    Variables.E.CastOnUnit(Targets.Target);
                }
            }
        }
    }
}