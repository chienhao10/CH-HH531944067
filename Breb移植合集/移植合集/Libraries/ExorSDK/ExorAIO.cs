using EloBuddy.SDK.Menu;
using ExorSDK.Core;
using ExorSDK.Utilities;
using LeagueSharp.SDK;
using System.Security.Permissions;

namespace ExorSDK
{
    /// <summary>
    ///     The AIO class.
    /// </summary>
    internal class AIO
    {
        /// <summary>
        ///     Loads the Assembly's core processes.
        /// </summary>
        /// 
        public static void OnLoad()
        {

            /// <summary>
            ///     Loads the Main Menu.
            /// </summary>
            Vars.Menu = MainMenu.AddMenu($"[ExorAIO]: {GameObjects.Player.ChampionName}", $"aio.{GameObjects.Player.ChampionName.ToLower()}");

            /// <summary>
            ///     Tries to load the current Champion.
            /// </summary>
            Core.Bootstrap.LoadChampion();
        }
    }
}