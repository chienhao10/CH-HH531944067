using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZLib.Core;
using LeagueSharp;
using iDZEzreal.MenuHelper;
using EloBuddy;

namespace iDZEzreal
{
    class EzrealBootstrap
    {

        public static void OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != "Ezreal")
            {
                return;
            }
            MenuGenerator.Generate();
            DZAntigapcloser.BuildMenu(Variables.Menu, "[Ez] Antigapcloser", "ezreal.antigapcloser");

            SPrediction.Prediction.Initialize(Variables.Menu);
            Ezreal.OnLoad();
        }
    }
}
