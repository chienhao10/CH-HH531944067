using LeagueSharp;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Caitlyn
{
    using System;
    using System.Linq;
    using ExorAIO.Utilities;
    using Geometry = LeagueSharp.Common.Geometry;
    using EloBuddy;
    /// <summary>
    ///     The logics class.
    /// </summary>
    partial class Logics
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
            ///     The Clear Q Logics.
            /// </summary>
            if (Variables.Q.IsReady() &&
                ObjectManager.Player.ManaPercent > ManaManager.NeededQMana && Menus.getCheckBoxItem(Variables.QMenu, "qspell.farm"))
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
                            t.LSIsValidTarget(Variables.Q2.Range)))
                    {
                        if (Variables.Q2.GetLineFarmLocation(Targets.Minions, Variables.Q2.Width).MinionsHit >= 3 &&
                            new Geometry.Polygon.Rectangle(
                                    ObjectManager.Player.ServerPosition,
                                    ObjectManager.Player.ServerPosition.LSExtend(
                                        Targets.Minions[0].ServerPosition,
                                        Variables.Q2.Range), Variables.Q2.Width).IsInside(Variables.Q2.GetPrediction(HeroManager.Enemies.FirstOrDefault(t => !Bools.IsSpellShielded(t) && t.LSIsValidTarget(Variables.Q2.Range))).CastPosition))
                        {
                            Variables.Q.Cast(Variables.Q2.GetLineFarmLocation(Targets.Minions, Variables.Q2.Width).Position);
                        }
                    }

                    /// <summary>
                    ///     The LaneClear Q Logic.
                    /// </summary>
                    else if (!HeroManager.Enemies.Any(t => !Bools.IsSpellShielded(t) && t.LSIsValidTarget(Variables.Q2.Range + 100f)))
                    {
                        if (Variables.Q2.GetLineFarmLocation(Targets.Minions, Variables.Q2.Width).MinionsHit >= 3)
                        {
                            Variables.Q.Cast(Variables.Q2.GetLineFarmLocation(Targets.Minions, Variables.Q2.Width).Position);
                        }
                    }
                }
            }
        }
    }
}
