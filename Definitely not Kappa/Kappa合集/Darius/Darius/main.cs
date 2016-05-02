namespace Darius
{
    using System;

    using EloBuddy.SDK.Events;

    class main
    {
        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            KappaDarius.Execute();
        }
    }
}
