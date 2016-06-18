using System;
using EloBuddy;
using EloBuddy.SDK.Events;
using WuAIO.Managers;

namespace WuAIO
{
    static class Program
    {
        static void Main(string[] args) { Loading.OnLoadingComplete += OnLoadingComplete; }

        static void OnLoadingComplete(EventArgs args)
        {
            VersionManager.CheckVersion();

            try
            {
                Activator.CreateInstance(null, "WuAIO." + Player.Instance.ChampionName);
                Chat.Print("Wu{0} Loaded, [By WujuSan], Version: {1}", Player.Instance.ChampionName == "MasterYi" ? "Yi" : Player.Instance.ChampionName, VersionManager.AssVersion);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}
