using System;
using EloBuddy.SDK.Events;

namespace KA_Lux
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            Lux.Initialize();
        }
    }
}
