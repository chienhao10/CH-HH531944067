using System;
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
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Automatic(EventArgs args)
        {
            if (ObjectManager.Player.IsRecalling())
            {
                return;
            }

            /// <summary>
            ///     The Stacking Logics.
            /// </summary>
            if (Variables.Q.IsReady() &&
                ObjectManager.Player.ManaPercent > ManaManager.NeededTearMana)
            {
                /// <summary>
                ///     The Tear Stacking Logic.
                /// </summary>
                if (Bools.HasTear(ObjectManager.Player) &&
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) &&
                    ObjectManager.Player.CountEnemiesInRange(1500) == 0 &&
                    Variables.getCheckBoxItem(Variables.MiscMenu, "misc.tear"))
                {
                    Variables.Q.Cast(Game.CursorPos);
                }

                /// <summary>
                ///     The Passive Stacking Logic.
                /// </summary>
                else if (ObjectManager.Player.GetBuffCount("RyzePassiveStack") < 3 &&
                         Variables.getCheckBoxItem(Variables.MiscMenu, "misc.manager"))
                {
                    Variables.Q.Cast(Game.CursorPos);
                }
            }
        }
    }
}