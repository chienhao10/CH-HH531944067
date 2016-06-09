
using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;

namespace PuppyStandaloneOrbwalker
{
    internal class Program
    {
        public static void Game_OnGameLoad()
        {
            //xSLx_Orbwalker.xSLxOrbwalker.AddToMenu();
            //new LeagueSharp.Common.Orbwalking.Orbwalker();

            Chat.Print("In the EB Orbwalker make sure all the keys are the same as the standalone orbwalker.");
            Chat.Print("- Disable everything in EB's Orbwalker drawings.");
            Chat.Print("Some things do NOT work (yet) such as resetting auto attack.");
        }
    }
}