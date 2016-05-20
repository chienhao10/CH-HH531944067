using System;
using System.Linq;
using EloBuddy;
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
            ///     The Clear R Logic.
            /// </summary>
            if (Variables.R.IsReady() &&
                Variables.E.IsReady() &&
                Variables.getCheckBoxItem(Variables.RMenu, "rspell.farm"))
            {
                /// <summary>
                ///     The LaneClear R Logic.
                /// </summary>
                if (Targets.Minions.Any())
                {
                    if (Targets.Minions.Count() >= 3)
                    {
                        Variables.R.Cast();
                    }
                }

                /// <summary>
                ///     The JungleClear R Logic.
                /// </summary>
                else if (Targets.JungleMinions.Any())
                {
                    Variables.R.Cast();
                }
            }

            /// <summary>
            ///     The Clear Q Logic.
            /// </summary>
            if (Variables.Q.IsReady() &&
                GameObjects.Player.ManaPercent > ManaManager.NeededQMana &&
                Variables.getCheckBoxItem(Variables.QMenu, "qspell.farm"))
            {
                /// <summary>
                ///     The LaneClear Q Logic.
                /// </summary>
                foreach (var minion in Targets.Minions.Where(m => m.Health < Variables.Q.GetDamage(m)))
                {
                    if (!Variables.Q.GetPrediction(minion).CollisionObjects.Any(c => c.IsMinion))
                    {
                        Variables.Q.Cast(Variables.Q.GetPrediction(minion).UnitPosition);
                        return;
                    }
                }

                /// <summary>
                ///     The JungleClear Q Logic.
                /// </summary>
                if (Targets.JungleMinions.Any())
                {
                    Variables.Q.Cast(Targets.JungleMinions[0].ServerPosition);
                }
            }

            /// <summary>
            ///     The Clear W Logic.
            /// </summary>
            if (Variables.W.IsReady() &&
                GameObjects.Player.ManaPercent > ManaManager.NeededWMana &&
                Variables.getCheckBoxItem(Variables.WMenu, "wspell.farm"))
            {
                /// <summary>
                ///     The LaneClear W Logic.
                /// </summary>
                if (Targets.Minions.Any())
                {
                    Variables.W.CastOnUnit(Targets.Minions[0]);
                }

                /// <summary>
                ///     The JungleClear W Logic.
                /// </summary>
                if (Targets.JungleMinions.Any())
                {
                    Variables.W.CastOnUnit(Targets.JungleMinions[0]);
                }
            }

            /// <summary>
            ///     The Clear E Logic.
            /// </summary>
            if (Variables.E.IsReady() &&
                GameObjects.Player.ManaPercent > ManaManager.NeededEMana &&
                Variables.getCheckBoxItem(Variables.EMenu, "espell.farm"))
            {
                /// <summary>
                ///     The LaneClear E Logic.
                /// </summary>
                if (Targets.Minions.Any())
                {
                    if (Targets.Minions.Count(m => m.LSDistance(Targets.Minions[0]) < 200f) >= 3)
                    {
                        Variables.E.CastOnUnit(Targets.Minions[0]);
                    }
                }

                /// <summary>
                ///     The JungleClear W Logic.
                /// </summary>
                else if (Targets.JungleMinions.Any())
                {
                    Variables.E.CastOnUnit(Targets.JungleMinions[0]);
                }
            }
        }
    }
}