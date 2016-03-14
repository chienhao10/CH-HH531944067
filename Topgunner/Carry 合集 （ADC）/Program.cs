using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using AshesToAshes;
namespace CarryAIO
{
    public static class Program
    {
        // Change this line to the champion you want to make the addon for,
        // watch out for the case being correct!

        public static void Main(string[] args)
        {
            // Wait till the loading screen has passed
            Loading.OnLoadingComplete += OnLoadingComplete;
        }

        private static void OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName == "MissFortune")
            {
                MissFortune.Program.OnLoadingComplete(args);
            }
            else if (Player.Instance.ChampionName == "Ashe")
            {
                AshesToAshes.Ashe.OnLoadingCompleteAshe(args);
            }
            else if (Player.Instance.ChampionName == "Caitlyn")
            {
                Kitelyn.Program.OnLoadingCompleteCait(args);
            }
            else if (Player.Instance.ChampionName == "Quinn")
            {
                RoamQueenQuinn.Program.OnLoadingComplete(args);
            }
            else if (Player.Instance.ChampionName == "Corki")
            {
                Corki.Program.OnLoadingComplete(args);
            }
            else if (Player.Instance.ChampionName == "Sivir")
            {
                SivirDamage.Program.OnLoadingComplete(args);
            }
            else if (Player.Instance.ChampionName == "Vayne")
            {
                PurifierVayne.Program.OnLoadingComplete(args);
            }
            else if (Player.Instance.ChampionName == "Varus")
            {
                CorruptedVarus.Program.OnLoadingComplete(args);
            }
            else if (Player.Instance.ChampionName == "Lucian")
            {
                MatrixLucian.Program.OnLoadingComplete(args);
            }
        }
    }
}
