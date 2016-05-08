#region License
/* Copyright (c) LeagueSharp 2016
 * No reproduction is allowed in any way unless given written consent
 * from the LeagueSharp staff.
 * 
 * Author: imsosharp
 * Date: 2/20/2016
 * File: Program.cs
 */
#endregion License

using Challenger_Series;
using EloBuddy;
using LeagueSharp;
using LeagueSharp.SDK;

namespace Challenger_Series
{
    class Program
    {
        public static void Main()
        {
            Events.OnLoad += (sender, eventArgs) =>
            {
                switch (ObjectManager.Player.ChampionName)
                {
                    case "Soraka":
                        new Soraka();
                        break;
                    case "KogMaw":
                        new Plugins.KogMaw();
                        break;
                    case "Kalista":
                        new Plugins.Kalista();
                        break;
                    case "Lucian":
                        new Plugins.Lucian();
                        break;
                    case "Ashe":
                        new Plugins.Ashe();
                        break;
                    default:
                        //Variables.Orbwalker.Enabled = false;
                        break;
                }
            };
        }
    }
}
