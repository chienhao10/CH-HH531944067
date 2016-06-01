using System;
using System.Linq;
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
        public static void Harass(EventArgs args)
        {
            if (!Targets.Target.LSIsValidTarget() ||
                Invulnerable.Check(Targets.Target))
            {
                return;
            }

            /// <summary>
            ///     The E Harass Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.E.Range) &&
                Vars.getSliderItem(Vars.EMenu, "harass") != 101)
            {
                if (GameObjects.Player.ManaPercent <
                        ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.EMenu, "harass")) &&
                    !GameObjects.Player.Buffs.Any(b => b.Name.Equals("visionary")))
                {
                    return;
                }

                Vars.E.CastOnUnit(Targets.Target);
            }
        }
    }
}