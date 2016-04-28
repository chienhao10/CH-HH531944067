using Color = System.Drawing.Color;
using EloBuddy.SDK.Menu;
using EloBuddy;
using System;
using RandomUlt.Helpers;

namespace RandomUlt
{
    internal class Program
    {
        public static Menu config, RandomUltM;
        public static LastPositions positions;
        public static readonly AIHeroClient player = ObjectManager.Player;

        public static void Game_OnGameLoad()
        {
            config = MainMenu.AddMenu("RandomUlt Beta", "RandomUlt Beta");
            RandomUltM = config.AddSubMenu("Options", "Options");
            positions = new LastPositions(RandomUltM);
        }
    }
}