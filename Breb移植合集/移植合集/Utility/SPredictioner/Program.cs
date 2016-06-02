using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SPrediction;
using EloBuddy;

namespace SPredictioner
{
    class Program
    {
        public static void Init()
        {
            CustomEvents.Game.OnGameLoad += EventHandlers.Game_OnGameLoad;
        }
    }
}