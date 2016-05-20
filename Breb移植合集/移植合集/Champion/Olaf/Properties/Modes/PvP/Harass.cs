using System;
using EloBuddy;
using EloBuddy.SDK;
using ExorAIO.Utilities;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Olaf
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
            ///     The Q Harass Logic.
            /// </summary>
            if (Variables.Q.IsReady() && Targets.Target.IsValidTarget(Variables.Q.Range) && ObjectManager.Player.ManaPercent > ManaManager.NeededQMana && Variables.getCheckBoxItem(Variables.QMenu, "qspell.harass"))
            {
                var castPosition = Targets.Target.Position.LSExtend(ObjectManager.Player.Position, -120);
                var castPosition2 = Targets.Target.Position.LSExtend(ObjectManager.Player.Position, -90);

                if (ObjectManager.Player.LSDistance(Targets.Target.ServerPosition) >= 300)
                {
                    Variables.Q.Cast(castPosition);
                }
                else
                {
                    Variables.Q.Cast(castPosition2);
                }
            }
        }
    }
}