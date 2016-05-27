using EloBuddy;
using LeagueSharp;

namespace ExorSDK.Champions.Sivir
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
            Game.OnUpdate += Sivir.OnUpdate;
            Obj_AI_Base.OnSpellCast += Sivir.OnDoCast;
            Obj_AI_Base.OnProcessSpellCast += Sivir.OnProcessSpellCast;
        }
    }
}