using System;
using ExorSDK.Utilities;
using LeagueSharp.SDK;
using EloBuddy.SDK;
using EloBuddy;

namespace ExorSDK.Champions.Pantheon
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
            if (GameObjects.Player.LSIsRecalling())
            {
                return;
            }

            Orbwalker.DisableAttacking = ObjectManager.Player.HasBuff("PantheonESound");
            Orbwalker.DisableMovement = ObjectManager.Player.HasBuff("PantheonESound");
        }
    }
}