using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ARAMDetFull.Champions;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

namespace ARAMDetFull
{
    class ARAMDetFull
    {
        /* TODO:
         * ##- Tower range higher dives a lot
         * ##- before level 6/7 play safer dont go so close stay behind other players or 800/900 units away from closest enemy champ
         * ##- Target selector based on invincible enemies
         * ##- IF invincible or revive go full in
         * ##- if attacking enemy and it is left 3 or less aa to kill then follow to kill (check movespeed dif)
         *  - bush invis manager player death
         *  - fixx gankplank plays like retard
         *  - this weeks customs
         *  - WPF put to allways take mark
         * ##- nami auto level
         *  - Some skills make aggresivity for time and how much to put in balance ignore minsions on/off
         * ## - LeeSin
         * ## - Nocturn
         *  - Gnar
         *  -Katarina error
         *  - Gangplank error
         *  ##- healing relics
         *  -Make velkoz
         */

        public ARAMDetFull()
        {
            Console.WriteLine("Aram det full started!");
            CustomEvents.Game.OnGameLoad += onLoad;
        }

        public static int gameStart = 0;

        public static Menu Config;

        public static int now
        {
            get { return (int)DateTime.Now.TimeOfDay.TotalMilliseconds; }
        }

        private static void onLoad(EventArgs args)
        {
            gameStart = now;

            Chat.Print("ARAm - Sharp by DeTuKs");

            try
            {

                Config = MainMenu.AddMenu("ARAM", "Yasuo");

                //Combo
                var combo = Config.AddSubMenu("Combo Sharp", "combo");
                combo.Add("comboItems", new CheckBox("Use Items"));

                //LastHit
                Config.AddSubMenu("LastHit Sharp", "lHit");

                //LaneClear
                Config.AddSubMenu("LaneClear Sharp", "lClear");

                //Harass
                Config.AddSubMenu("Harass Sharp", "harass");

                //Extra
                Config.AddSubMenu("Extra Sharp", "extra");


                //Debug
                var debug = Config.AddSubMenu("Debug", "debug");
                debug.Add("db_targ", new KeyBind("Debug Target", false, KeyBind.BindTypes.HoldActive, 'T'));


                Drawing.OnDraw += onDraw;
                Game.OnUpdate += OnGameUpdate;
                Drawing.OnDraw += onDraw;
                CustomEvents.Game.OnGameEnd += OnGameEnd;
                ARAMSimulator.setupARMASimulator();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void OnGameEnd(EventArgs args)
        {
            LeagueSharp.Common.Utility.DelayAction.Add(10000, closeGame);
        }

        private static void closeGame()
        {
            Game.QuitGame();
        }

        private static void onDraw(EventArgs args)
        {
            Drawing.DrawText(100, 100, Color.Red, "bal: " + ARAMSimulator.balance + " time: " );
            return;
        }
        public static void getAllBuffs()
        {
            foreach (var aly in HeroManager.Enemies)
            {
                foreach (var buffs in aly.Buffs)
                {
                    Console.WriteLine(aly.ChampionName + " - Buf: " + buffs.Name);
                }
            }
        }

        public static int lastTick = now;

        private static void OnGameUpdate(EventArgs args)
        {
            if (lastTick + 200 > now)
                return;
            lastTick = now;
            ARAMSimulator.updateArmaPlay();
        }
    }
}
