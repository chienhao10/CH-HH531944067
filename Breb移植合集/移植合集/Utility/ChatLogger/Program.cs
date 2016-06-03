//This code is copyright (c) LeagueSharp 2015. Please do not remove this line.

using System;
using System.Diagnostics;
using System.IO;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;
using System.Security.Permissions;

namespace Chat_Logger
{
    public static class Program
    {
        public static Stopwatch Stopwatch;

        public static StreamWriter Writer { get; set; }
        public static string LogFile { get; set; }

        public static void Init()
        {
            //Define the logfile location
            LogFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\EloBuddy\", DateTime.Now.ToString("yy -MM-dd") + " " + DateTime.Now.ToString("HH-mm-ss") + " - " + ObjectManager.Player.ChampionName + ".txt");
            File.CreateText(LogFile);

            //Create a stopwatch which we will use to emulate in-game time.
            Stopwatch = new Stopwatch();
            Stopwatch.Start();

            //Show the user a message
            Chat.Print("The chat log for this game can be found at " + LogFile);
            Chat.Print(Game.IP + ":" + Game.Port);

            //Subscribe to OnChat to do the magic
            Chat.OnMessage += Chat_OnMessage;
        }

        public static string FormatTime(double time)
        {
            var t = TimeSpan.FromSeconds(time);
            return string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
        }

        private static void Chat_OnMessage(AIHeroClient sender, ChatMessageEventArgs args)
        {
            using (var sw = new StreamWriter(LogFile, true))
            {
                //store the current stopwatch millisecond for accurate results
                long elapsedTime = Stopwatch.ElapsedMilliseconds;
                //compute elapsed minutes
                long elapsedMinutes = elapsedTime / 60000;
                //create a variable to store the seconds in
                long elapsedSeconds = 0;
                //compute the elapsed seconds and store it in the variable previously created
                Math.DivRem(elapsedTime, 60000, out elapsedSeconds);
                elapsedSeconds /= 1000;

                //write everything to the stream
                sw.WriteLine("[" + elapsedMinutes + ":" + (elapsedSeconds < 10 ? "0" : "") + elapsedSeconds + "] " + args.Sender.Name + " (" + args.Sender.ChampionName + "): " + args.Message);
                //close the stream
                sw.Close();
            }
        }
    }
}