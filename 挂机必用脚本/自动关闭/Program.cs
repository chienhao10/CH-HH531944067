using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK.Events;

namespace AutoClose
{
    class Program
    {
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
            Loading.OnLoadingCompleteSpectatorMode += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            Game.OnEnd += Game_OnEnd;
            Chat.OnInput += Chat_OnInput;
        }

        private static void Chat_OnInput(ChatInputEventArgs args)
        {
            Process.GetCurrentProcess().Kill();
        }

        private static void Game_OnEnd(GameEndEventArgs args)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
