using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using AutoBuddy.Humanizers;

namespace AutoBuddy.MainLogics
{
    internal class MiscEvents
    {
        public MiscEvents()
        {
            Game.OnNotify += Game_OnNotify;
        }

        private void Game_OnNotify(GameNotifyEventArgs args)
        {
            if (args.EventId == GameEventId.OnSurrenderVote)
            {
                switch (MainMenu.GetMenu("AB").Get<Slider>("ff").CurrentValue)
                {
                    case 1:
                        Core.DelayAction(() => SafeFunctions.SayChat("/ff"), RandGen.r.Next(2000, 5000));
                        break;
                    case 2:
                        Core.DelayAction(() => SafeFunctions.SayChat("/noff"), RandGen.r.Next(2000, 5000));
                        break;
                }
            }
        }
    }
}
