using EloBuddy;
using LeagueSharp;
using LeagueSharp.SDK;
using System;
using System.Linq;

namespace SDK_SkinChanger
{
    class Program
    {
        public static AIHeroClient Player => ObjectManager.Player;

        public static void Load()
        {
            MenuConfig.Load();
        }
    }
}
