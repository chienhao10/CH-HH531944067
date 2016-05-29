#region
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Irelia.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Geometry = Irelia.Common.CommonGeometry;
using EloBuddy;

#endregion

namespace Irelia
{
    internal class Irelia
    {
        public static string ChampionName => "Irelia";
        public static void Init()
        {
            Game_OnGameLoad();
        }

        private static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.CharData.BaseSkinName != ChampionName)
            {
                return;
            }

            Champion.PlayerSpells.Init();
            Modes.ModeConfig.Init();
            Common.CommonItems.Init();

            Chat.Print("<font color='#ff3232'>Successfully Loaded: </font><font color='#d4d4d4'><font color='#FFFFFF'>" + ChampionName + "</font>");

            //Console.Clear();
        }
    }
}