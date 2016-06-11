using EloBuddy;
using EloBuddy.SDK.Events;

namespace TBlitzReworked
{
    class Program
    {
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoaded;
        }

        private static void OnLoaded(System.EventArgs args)
        {
            if (ObjectManager.Player.ChampionName == "Blitzcrank")
            new Blitzcrank();
            //Definitely do not check out my AIO as well, AIO's are much illegal :^) You'd get arrested by EB police
        }
    }
}
