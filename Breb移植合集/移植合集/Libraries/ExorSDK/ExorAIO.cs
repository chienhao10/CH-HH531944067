using EloBuddy.SDK.Menu;
using ExorSDK.Core;
using ExorSDK.Utilities;
using LeagueSharp.SDK;

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
        public static void OnLoad()
        {
            /// <summary>
            ///     Loads the Main Menu.
            /// </summary>
            Vars.Menu = MainMenu.AddMenu($"aio.{GameObjects.Player.ChampionName.ToLower()}", $"[ExorAIO]: {GameObjects.Player.ChampionName}");

            /// <summary>
            ///     Tries to load the current Champion.
            /// </summary>
            Core.Bootstrap.LoadChampion();
        }
    }
}