using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hJhin.Champions;
using EloBuddy;
using LeagueSharp.SDK;

namespace hJhin
{
    class Program
    {
        public static void Load()
        {
            if (ObjectManager.Player.ChampionName == "Jhin")
            {
                // ReSharper disable once ObjectCreationAsStatement
                new Jhin();
            }
            else
            {
                Console.WriteLine("{0} not supported :roto2: ", ObjectManager.Player.ChampionName);
            }
        }
    }
}
