using EloBuddy;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Pantheon
{
    /// <summary>
    ///     The methods class.
    /// </summary>
    internal class Methods
    {
        /// <summary>
        ///     Sets the methods.
        /// </summary>
        public static void Initialize()
        {
            Game.OnUpdate += Pantheon.OnUpdate;
            Interrupter2.OnInterruptableTarget += Pantheon.OnInterruptableTarget;
        }
    }
}