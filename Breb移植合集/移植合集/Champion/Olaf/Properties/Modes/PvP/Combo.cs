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
        public static void Combo(EventArgs args)
        {
            if (Bools.HasSheenBuff() ||
                !Targets.Target.IsValidTarget() ||
                Bools.IsSpellShielded(Targets.Target))
            {
                return;
            }

            /// <summary>
            ///     The Combo W Logic.
            /// </summary>
            if (Variables.W.IsReady() && ObjectManager.Player.CountEnemiesInRange(Variables.AARange + 125) > 0 && Variables.getCheckBoxItem(Variables.WMenu, "wspell.combo"))
            {
                Variables.W.Cast();
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Variables.Q.IsReady() && !Targets.Target.HasBuffOfType(BuffType.Slow) && Targets.Target.IsValidTarget(Variables.Q.Range) && Variables.getCheckBoxItem(Variables.QMenu, "qspell.combo"))
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