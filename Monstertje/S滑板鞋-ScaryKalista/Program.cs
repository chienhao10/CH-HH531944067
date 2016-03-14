
using EloBuddy.SDK.Events;

namespace ScaryKalista
{
    class Program
    {
        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Kalista.OnLoadingComplete;
        }
    }
}
