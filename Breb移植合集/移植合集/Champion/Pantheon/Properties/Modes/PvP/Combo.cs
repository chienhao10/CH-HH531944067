using System;
using EloBuddy;
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
        public static void Combo(EventArgs args)
        {
            if (!Targets.Target.IsValidTarget() ||
                Bools.IsSpellShielded(Targets.Target))
            {
                return;
            }

            if (!Bools.HasSheenBuff() ||
                !Targets.Target.IsValidTarget(Variables.AARange))
            {
                /// <summary>
                ///     The Combo Q Logic.
                /// </summary>
                if (Variables.Q.IsReady() &&
                    Targets.Target.IsValidTarget(Variables.Q.Range) &&
                    !ObjectManager.Player.HasBuff("pantheonesound") &&
                    !ObjectManager.Player.HasBuff("pantheonpassiveshield") &&
                    Variables.getCheckBoxItem(Variables.QMenu, "qspell.combo"))
                {
                    Variables.Q.CastOnUnit(Targets.Target);
                }

                /// <summary>
                ///     The Combo W Logic.
                /// </summary>
                if (Variables.W.IsReady() &&
                    !Bools.IsImmobile(Targets.Target) &&
                    Targets.Target.IsValidTarget(Variables.W.Range) &&
                    !ObjectManager.Player.HasBuff("pantheonesound") &&
                    Variables.getCheckBoxItem(Variables.WMenu, "wspell.combo"))
                {
                    if (!Targets.Target.IsValidTarget(Variables.AARange) ||
                        !ObjectManager.Player.HasBuff("pantheonpassiveshield") &&
                        ObjectManager.Player.GetBuffCount("pantheonpassivecounter") < 3)
                    {
                        Variables.W.CastOnUnit(Targets.Target);
                    }
                }

                /// <summary>
                ///     The Combo E Logic.
                /// </summary>
                if (Variables.E.IsReady() && !ObjectManager.Player.HasBuff("pantheonpassiveshield") && ObjectManager.Player.CountEnemiesInRange(Variables.E.Range) > 0 && Variables.getCheckBoxItem(Variables.EMenu, "espell.combo"))
                {
                    Variables.E.StartCharging(Targets.Target.Position);
                }
            }
        }
    }
}