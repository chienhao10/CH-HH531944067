#region
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OlafxQx.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Geometry = OlafxQx.Common.CommonGeometry;
using EloBuddy;

#endregion

namespace OlafxQx
{
    internal class Olaf
    {
        public static string ChampionName => "Olaf";
        public static void Init()
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.CharData.BaseSkinName != ChampionName)
            {
                return;
            }

            Champion.PlayerSpells.Init();
            Modes.ModeConfig.Init();
            Common.CommonItems.Init();

            Chat.Print(
                "<font color='#ff3232'>Successfully Loaded: </font><font color='#d4d4d4'><font color='#FFFFFF'>" +
                ChampionName + "</font>");

            Console.Clear();
        }
    }
}