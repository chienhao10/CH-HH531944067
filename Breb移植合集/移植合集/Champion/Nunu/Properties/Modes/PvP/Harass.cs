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
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Harass(EventArgs args)
        {
            if (!Targets.Target.IsValidTarget() ||
                Bools.IsSpellShielded(Targets.Target))
            {
                return;
            }

            /// <summary>
            ///     The E Harass Logic.
            /// </summary>
            if (Variables.E.IsReady() &&
                Targets.Target.IsValidTarget(Variables.E.Range) &&
                (ObjectManager.Player.ManaPercent > ManaManager.NeededEMana ||
                 ObjectManager.Player.Buffs.Any(b => b.Name.Equals("visionary"))) &&
                Variables.getCheckBoxItem(Variables.EMenu, "espell.harass"))
            {
                Variables.E.CastOnUnit(Targets.Target);
            }
        }
    }
}