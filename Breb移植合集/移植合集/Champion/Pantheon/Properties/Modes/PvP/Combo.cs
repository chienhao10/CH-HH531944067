using System;
using ExorSDK.Utilities;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;

namespace ExorSDK.Champions.Pantheon
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
            if (!Targets.Target.LSIsValidTarget() ||
                Invulnerable.Check(Targets.Target))
            {
                return;
            }

            if (Bools.HasSheenBuff())
            {
                if (Targets.Target.LSIsValidTarget(Vars.AARange))
                {
                    return;
                }
            }

            /// <summary>
            ///     The Combo Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.Q.Range) &&
                !GameObjects.Player.HasBuff("pantheonesound") &&
                !GameObjects.Player.HasBuff("pantheonpassiveshield") &&
                Vars.getCheckBoxItem(Vars.QMenu, "combo"))
            {
                Vars.Q.CastOnUnit(Targets.Target);
            }

            /// <summary>
            ///     The Combo W Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.W.Range) &&
                !GameObjects.Player.HasBuff("pantheonesound") &&
                Vars.getCheckBoxItem(Vars.WMenu, "combo"))
            {
                if (!Targets.Target.LSIsValidTarget(Vars.AARange) ||
                    !GameObjects.Player.HasBuff("pantheonpassiveshield") &&
                    GameObjects.Player.GetBuffCount("pantheonpassivecounter") < 3)
                {
                    Vars.W.CastOnUnit(Targets.Target);
                }
            }

            /// <summary>
            ///     The Combo E Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.E.Range) &&
                !GameObjects.Player.HasBuff("pantheonpassiveshield") &&
                Vars.getCheckBoxItem(Vars.EMenu, "combo"))
            {
                Vars.E.Cast(Targets.Target.ServerPosition);
            }
        }
    }
}