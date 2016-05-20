using EloBuddy;
using LeagueSharp;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Anivia
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
            Game.OnUpdate += Anivia.OnUpdate;
            GameObject.OnCreate += Anivia.OnCreate;
            GameObject.OnDelete += Anivia.OnDelete;
            AntiGapcloser.OnEnemyGapcloser += Anivia.OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Anivia.OnInterruptableTarget;
        }
    }
}
