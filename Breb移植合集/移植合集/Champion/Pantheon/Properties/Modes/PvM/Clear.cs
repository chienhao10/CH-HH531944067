using System;
using System.Linq;
using ExorSDK.Utilities;
using LeagueSharp.SDK;

namespace ExorSDK.Champions.Pantheon
{
    /// <summary>
    ///     The logics class.
    /// </summary>
    internal partial class Logics
    {
        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Clear(EventArgs args)
        {
            if (Bools.HasSheenBuff())
            {
                return;
            }

            /// <summary>
            ///     The Q JungleClear Logics.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Targets.JungleMinions.Any() &&
                !GameObjects.Player.HasBuff("pantheonpassiveshield") &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "jungleclear")) &&
                Vars.getSliderItem(Vars.QMenu, "jungleclear") != 101)
            {
                Vars.Q.CastOnUnit(Targets.JungleMinions[0]);
            }

            /// <summary>
            ///     The E Clear Logics.
            /// </summary>
            if (Vars.E.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.E.Slot, Vars.getSliderItem(Vars.EMenu, "clear")) &&
                Vars.getSliderItem(Vars.EMenu, "clear") != 101)
            {
                /// <summary>
                ///     The LaneClear E Logic.
                /// </summary>
                if (Targets.Minions.Any())
                {
                    Vars.E.Cast(Targets.Minions[0].ServerPosition);
                }

                /// <summary>
                ///     The JungleClear E Logic.
                /// </summary>
                else if (Targets.JungleMinions.Any() &&
                    !GameObjects.Player.HasBuff("pantheonpassiveshield"))
                {
                    Vars.E.Cast(Targets.JungleMinions[0].ServerPosition);
                }
            }
        }
    }
}