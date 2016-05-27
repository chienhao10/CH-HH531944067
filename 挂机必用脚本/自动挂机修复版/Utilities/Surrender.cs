using AutoBuddy.Humanizers;
using EloBuddy;
using EloBuddy.SDK;

namespace AutoBuddy.Utilities
{
    internal class Surrender
    {
        public Surrender()
        {
            Game.OnNotify += Game_OnNotify;
        }

        //auto agree to surrender
        private void Game_OnNotify(GameNotifyEventArgs args)
        {
            if (args.EventId == GameEventId.OnSurrenderVote)
                Core.DelayAction(() => SafeFunctions.SayChat("/ff"), RandGen.r.Next(2000, 5000));
        }
    }
}