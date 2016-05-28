using EloBuddy;
using LeagueSharp;
using LeagueSharp.SDK;

namespace ExorSDK.Champions.Caitlyn
{
    /// <summary>
    ///     The methods class.
    /// </summary>
    internal class Methods
    {
        /// <summary>
        ///     Initializes the methods.
        /// </summary>
        public static void Initialize()
        {
            Game.OnUpdate += Caitlyn.OnUpdate;
            Events.OnGapCloser += Caitlyn.OnGapCloser;
        }
    }
}