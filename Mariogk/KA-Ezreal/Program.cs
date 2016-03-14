using System;
using EloBuddy.SDK.Events;

namespace KA_Ezreal
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
           Ezreal.Initialize();
        }
    }
}
