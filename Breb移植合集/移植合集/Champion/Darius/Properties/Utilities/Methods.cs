using EloBuddy;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Darius
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
            AntiGapcloser.OnEnemyGapcloser += Darius.OnEnemyGapcloser;
        }
    }
}