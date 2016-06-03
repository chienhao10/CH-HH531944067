#region

using System;
using LeagueSharp.SDK;

#endregion

namespace Spirit_Karma
{
    internal class Program
    {

        public static void Load()
        {
            if (GameObjects.Player.ChampionName != "Karma")
            {
                Console.WriteLine("Could not load Karma!"); return;
            }
            Spirit_Karma.Load.Load.LoadAssembly();
        }
    }
}
