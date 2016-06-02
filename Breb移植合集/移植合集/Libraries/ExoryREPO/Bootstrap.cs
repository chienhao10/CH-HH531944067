using EloBuddy;
using EloBuddy.SDK.Menu;
using ExorAIO.Champions.Renekton;
using ExorAIO.Champions.Ryze;
using ExorAIO.Champions.Tryndamere;
using ExorAIO.Utilities;

namespace ExorAIO.Core
{
    /// <summary>
    ///     The bootstrap class.
    /// </summary>
    internal class Bootstrap
    {
        /// <summary>
        ///     Builds the general Menu and loads the Common Orbwalker.
        /// </summary>
        public static void BuildMenu()
        {
            /// <summary>
            ///     The main Menu.
            /// </summary>
            Variables.Menu = MainMenu.AddMenu("[ExorAIO]: " + ObjectManager.Player.ChampionName, "EXORY");
        }

        /// <summary>
        ///     Tries to load the champion which is being currently played.
        /// </summary>
        public static void LoadChampion()
        {
            switch (ObjectManager.Player.ChampionName)
            {
                case "Renekton":
                    Renekton.OnLoad();
                    break;
                case "Tryndamere":
                    Tryndamere.OnLoad();
                    break;
                case "Ryze":
                    Ryze.OnLoad();
                    break;
                default:
                    Variables.IsLoaded = false;
                    break;
            }
        }
    }
}