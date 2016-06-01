using EloBuddy;
using LeagueSharp;

namespace ExorSDK.Champions.Olaf
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
            Game.OnUpdate += Olaf.OnUpdate;
            Obj_AI_Base.OnSpellCast += Olaf.OnDoCast;
        }
    }
}