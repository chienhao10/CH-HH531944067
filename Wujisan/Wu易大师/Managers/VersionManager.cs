using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Net;
using EloBuddy;
using Version = System.Version;

namespace WuAIO.Managers
{
    static class VersionManager
    {
        public static Version AssVersion { get { return Assembly.GetExecutingAssembly().GetName().Version; } }

        public static void CheckVersion()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var champ = Player.Instance.ChampionName == "MasterYi" ? "Yi" : Player.Instance.ChampionName;

                    string Text = new WebClient().DownloadString("https://raw.githubusercontent.com/WujuSan/EloBuddy/master/Wu" + champ + "/Wu" + champ + "/Properties/AssemblyInfo.cs");

                    var Match = new Regex(@"\[assembly\: AssemblyVersion\(""(\d+)\.(\d+)\.(\d+)\.(\d+)""\)\]").Match(Text);

                    if (Match.Success)
                    {
                        var CorrectVersion = new Version(string.Format("{0}.{1}.{2}.{3}", Match.Groups[1], Match.Groups[2], Match.Groups[3], Match.Groups[4]));

                        if (CorrectVersion > AssVersion)
                        {
                            Chat.Print("<font color='#FFFF00'>Your Wu{0} is </font><font color='#FF0000'>OUTDATED</font><font color='#FFFF00'>, The correct version is: " + CorrectVersion + "</font>", champ);
                            Chat.Print("<font color='#FFFF00'>Your Wu{0} is </font><font color='#FF0000'>OUTDATED</font><font color='#FFFF00'>, The correct version is: " + CorrectVersion + "</font>", champ);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e + "\n");
                }
            });
        }
    }
}
