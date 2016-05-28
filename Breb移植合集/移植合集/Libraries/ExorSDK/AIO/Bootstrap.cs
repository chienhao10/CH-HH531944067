using ExorSDK.Champions.Caitlyn;
using ExorSDK.Champions.Sivir;
using ExorSDK.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;

namespace ExorSDK.Core
{
    /// <summary>
    ///     The bootstrap class.
    /// </summary>
    internal class Bootstrap
    {
        /// <summary>
        ///     Tries to load the champion which is being currently played.
        /// </summary>
        public static void LoadChampion()
        {
            switch (GameObjects.Player.ChampionName)
            {
                case "Sivir":
                    new Sivir().OnLoad();
                    break;
                case "Caitlyn":
                    new Caitlyn().OnLoad();
                    break;
                default:
                    Vars.IsLoaded = false;
                    break;
            }
        }
    }
}