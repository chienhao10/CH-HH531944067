using System;
using ExorSDK.Utilities;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;

namespace ExorSDK.Champions.Nunu
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
            ///     The E Combo Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.E.Range) &&
                Vars.getCheckBoxItem(Vars.EMenu, "combo"))
            {
                Vars.E.CastOnUnit(Targets.Target);
            }
        }
    }
}