using System;
using LeagueSharp;
using LeagueSharp.Common;
using VayneHunter_Reborn.Utility;
using EloBuddy;

namespace VayneHunter_Reborn
{
    class Program
    {
        private static string ChampionName = "Vayne";

        public static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != ChampionName)
            {
                return;
            }
            VHRBootstrap.OnLoad();

            Chat.Print("<font color='#FF0000'><b>[VHR - Rewrite!]</b></font> By Asuna Loaded!");
            Chat.Print("<font color='#FF0000'><b>Berb : </b></font> WARNING : There is some FPS drop included.");
        }
    }
}
