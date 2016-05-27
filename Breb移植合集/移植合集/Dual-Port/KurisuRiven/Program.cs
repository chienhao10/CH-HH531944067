using System;
using System.Reflection;
using LeagueSharp.Common;

namespace KurisuRiven
{
    internal static class Program
    {
        public static void Game_OnGameLoad()
        {
            new KurisuRiven();
        }
    }
}