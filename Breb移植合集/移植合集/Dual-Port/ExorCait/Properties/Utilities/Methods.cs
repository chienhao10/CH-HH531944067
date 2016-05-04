using EloBuddy;
using LeagueSharp;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Caitlyn
{
    /// <summary>
    ///     The methods class.
    /// </summary>
    class Methods
    {
        /// <summary>
        ///     Initializes the methods.
        /// </summary>
        public static void Initialize()
        {
            Game.OnUpdate += Caitlyn.OnUpdate;
            Obj_AI_Base.OnPlayAnimation += Caitlyn.OnPlayAnimation;
            AntiGapcloser.OnEnemyGapcloser += Caitlyn.OnEnemyGapcloser;
        }
    }
}
