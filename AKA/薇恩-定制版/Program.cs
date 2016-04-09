using EloBuddy.SDK.Events;
using System;
using EloBuddy;

namespace Auto_Carry_Vayne
{
    class Program
    {
        private static void Main(string[] args1)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Variables._Player.ChampionName != "Vayne") return;
            Chat.Print("Auto Carry Vayne loaded! Made by Aka.");
            Manager.Manager.Load();
        }
    }
}
