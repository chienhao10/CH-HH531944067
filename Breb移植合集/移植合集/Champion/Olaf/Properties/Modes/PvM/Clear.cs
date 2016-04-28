using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using ExorAIO.Utilities;
using LeagueSharp.Common;
using Geometry = LeagueSharp.Common.Geometry;

namespace ExorAIO.Champions.Olaf
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
            ///     The Q Clear Logics.
            /// </summary>
            if (Variables.Q.IsReady() &&
                ObjectManager.Player.ManaPercent > ManaManager.NeededQMana &&
                Variables.getCheckBoxItem(Variables.QMenu, "qspell.farm"))
            {
                /// <summary>
                ///     The JungleClear Q Logic.
                /// </summary>
                if (Targets.JungleMinions.Any())
                {
                    Variables.Q.Cast(Targets.JungleMinions[0].Position);
                }

                /// <summary>
                ///     The LaneClear Q Logics.
                /// </summary>
                else
                {
                    /// <summary>
                    ///     The Aggressive LaneClear Q Logic.
                    /// </summary>
                    if (HeroManager.Enemies.Any(
                        t =>
                            !Bools.IsSpellShielded(t) &&
                            t.IsValidTarget(Variables.Q.Range)))
                    {
                        if (Variables.Q.GetLineFarmLocation(Targets.Minions, Variables.Q.Width).MinionsHit >= 3 &&
                            new Geometry.Polygon.Rectangle(
                                ObjectManager.Player.ServerPosition,
                                ObjectManager.Player.ServerPosition.LSExtend(
                                    Targets.Minions[0].ServerPosition,
                                    Variables.Q.Range),
                                Variables.Q.Width).IsInside(
                                    Variables.Q.GetPrediction(HeroManager.Enemies.FirstOrDefault(
                                        t =>
                                            !Bools.IsSpellShielded(t) &&
                                            t.IsValidTarget(Variables.Q.Range))).CastPosition))
                        {
                            Variables.Q.Cast(
                                Variables.Q.GetLineFarmLocation(Targets.Minions, Variables.Q.Width).Position);
                        }
                    }

                    /// <summary>
                    ///     The LaneClear Q Logic.
                    /// </summary>
                    else if (!HeroManager.Enemies.Any(
                        t =>
                            !Bools.IsSpellShielded(t) &&
                            t.IsValidTarget(Variables.Q.Range + 100f)))
                    {
                        if (Variables.Q.GetLineFarmLocation(Targets.Minions, Variables.Q.Width).MinionsHit >= 3)
                        {
                            Variables.Q.Cast(
                                Variables.Q.GetLineFarmLocation(Targets.Minions, Variables.Q.Width).Position);
                        }
                    }
                }
            }

            /// <summary>
            ///     The E JungleClear Logics.
            /// </summary>
            if (Variables.E.IsReady() &&
                Targets.JungleMinions.Any() &&
                ObjectManager.Player.ManaPercent > ManaManager.NeededEMana &&
                Variables.getCheckBoxItem(Variables.EMenu, "espell.jgc"))
            {
                Variables.E.Cast(Targets.JungleMinions[0]);
            }
        }
    }
}