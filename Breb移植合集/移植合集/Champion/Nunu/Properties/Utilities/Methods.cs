using EloBuddy;
using LeagueSharp;

namespace ExorSDK.Champions.Nunu
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
            Game.OnUpdate += Nunu.OnUpdate;
        }
    }
}