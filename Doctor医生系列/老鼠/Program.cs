using System;
using System.Drawing;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;

namespace Twitch
{
    public static class Program
    {
        public const string ChampName = "Twitch";

        public static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
        }

        private static void OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName != ChampName)
            {
                return;
            }
            Config.Initialize();
            SpellManager.Initialize();
            ModeManager.Initialize();
            Events.Initialize();
            Damages.InitDamageIndicator();
            WelcomeMsg();
        }

        private static void WelcomeMsg()
        {
            Chat.Print("Doctor{0} Loaded. Good Luck!", Color.GreenYellow, ChampName);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Doctor{0} Loaded. Good Luck!", ChampName);
            Console.ResetColor();
        }
    }
}
