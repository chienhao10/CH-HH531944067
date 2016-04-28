using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using ExorAIO.Utilities;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Nunu
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
            ///     The Q LaneClear Logic.
            /// </summary>
            if (Variables.Q.IsReady() &&
                Targets.Minions.Any() &&
                (ObjectManager.Player.ManaPercent > ManaManager.NeededQMana ||
                 ObjectManager.Player.Buffs.Any(b => b.Name.Equals("visionary"))) &&
                Variables.getCheckBoxItem(Variables.QMenu, "qspell.lc"))
            {
                Variables.Q.CastOnUnit(Targets.Minions.FirstOrDefault(m => m.Health < Variables.Q.GetDamage(m)));
            }

            /// <summary>
            ///     The E Clear Logics.
            /// </summary>
            if (Variables.E.IsReady() &&
                (ObjectManager.Player.ManaPercent > ManaManager.NeededEMana ||
                 ObjectManager.Player.Buffs.Any(b => b.Name.Equals("visionary"))) &&
                Variables.getCheckBoxItem(Variables.EMenu, "espell.farm"))
            {
                /// <summary>
                ///     The E LaneClear Logic.
                /// </summary>
                foreach (Obj_AI_Minion minion in Targets.Minions.Where(
                    m =>
                        m.IsValidTarget(Variables.E.Range) &&
                        m.Health < Variables.E.GetDamage(m)))
                {
                    Variables.E.CastOnUnit(minion);
                }

                /// <summary>
                ///     The E JungleClear Logic.
                /// </summary>
                if (Targets.JungleMinions.Any())
                {
                    Variables.E.CastOnUnit(Targets.JungleMinions[0]);
                }
            }
        }
    }
}