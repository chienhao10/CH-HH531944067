using EloBuddy;
using LeagueSharp;
using LeagueSharp.SDK;

namespace ExorSDK.Champions.Darius
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
            Game.OnUpdate += Darius.OnUpdate;
            Obj_AI_Base.OnSpellCast += Darius.OnDoCast;
            Events.OnGapCloser += Darius.OnGapCloser;
            Events.OnInterruptableTarget += Darius.OnInterruptableTarget;
        }
    }
}