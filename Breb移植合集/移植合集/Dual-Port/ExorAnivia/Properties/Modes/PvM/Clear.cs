using EloBuddy;
using LeagueSharp.Common;
using EloBuddy.SDK;

namespace ExorAIO.Champions.Anivia
{
    using System;
    using System.Linq;
    using ExorAIO.Utilities;
    using Geometry = LeagueSharp.Common.Geometry;

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
            ///     The R Clear Logic.
            /// </summary>
            if (Variables.R.IsReady() &&
                ObjectManager.Player.ManaPercent > ManaManager.NeededRMana &&
                ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).ToggleState == 1 &&
                Variables.getCheckBoxItem(Variables.RMenu, "rspell.farm"))
            {
                /// <summary>
                ///     The R LaneClear Logic.
                /// </summary>
                if (Variables.R.GetCircularFarmLocation(Targets.Minions, Variables.R.Width).MinionsHit >= 3)
                {
                    Variables.R.Cast(Variables.R.GetCircularFarmLocation(Targets.Minions, Variables.R.Width).Position);
                }

                /// <summary>
                ///     The R JungleClear Logic.
                /// </summary>
                else if (Targets.JungleMinions.Any())
                {
                    Variables.R.Cast((Targets.JungleMinions[0]).Position);
                }
                return;
            }

            /// <summary>
            ///     The Q Clear Logic.
            /// </summary>
            if (Variables.Q.IsReady() &&
                ObjectManager.Player.ManaPercent > ManaManager.NeededQMana &&
                ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).ToggleState == 1 &&
               Variables.getCheckBoxItem(Variables.QMenu, "qspell.farm"))
            {
                /// <summary>
                ///     The Q JungleClear Logic.
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
                            Variables.Q.Cast(Variables.Q.GetLineFarmLocation(Targets.Minions, Variables.Q.Width).Position);
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
                            Variables.Q.Cast(Variables.Q.GetLineFarmLocation(Targets.Minions, Variables.Q.Width).Position);
                        }
                    }
                }
            }
        }
    }
}
