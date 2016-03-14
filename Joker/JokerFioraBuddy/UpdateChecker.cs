using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using JokerFioraBuddy.Misc;
using SharpDX;

namespace JokerFioraBuddy
{
    public class UpdateChecker
    {
        public static System.Version gitVersion = new System.Version("0.0.0.0");
        public static void CheckForUpdates()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (var webClient = new WebClient())
                    {
                        var rawVersion = webClient.DownloadString("https://raw.githubusercontent.com/JokerArtLoL/EloBuddyAddons/master/JokerFioraBuddy/JokerFioraBuddy/Properties/AssemblyInfo.cs");
                        var match = new Regex(@"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]").Match(rawVersion);

                        if (match.Success)
                        {
                            gitVersion = new System.Version(string.Format("{0}.{1}.{2}.{3}", match.Groups[1], match.Groups[2], match.Groups[3], match.Groups[4]));

                            Chat.Print("<font color='#15C3AC'>Joker Fiora - The Grand Duelist: </font>" + "<font color='#C0C0C0'>Thanks for using Joker Fiora <3!" + "</font>");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }
    }
}
