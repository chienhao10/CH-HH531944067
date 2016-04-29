using System;
using EloBuddy;
using EloBuddy.SDK.Events;
using SharpDX;
using EloBuddy.SDK;

namespace AntiAFK
{
    class Program
    {
        static int lastTick;
        static int currentTick;
        static Vector3 lastPos;
        static Vector3 currentPos;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += gameLoadEventArgs =>
            {
                Hacks.AntiAFK = false;
                lastTick = 0;
                Game.OnUpdate += OnUpdate;
            };
        }

        private static void OnUpdate(EventArgs args)
        {
            if (!ObjectManager.Player.IsDead)
            {
                currentTick = Environment.TickCount;
                if (lastTick == 0)
                    lastTick = Environment.TickCount;
                currentPos = Player.Instance.ServerPosition;
                if (lastPos.IsZero)
                    lastPos = Player.Instance.ServerPosition;
                if (currentPos.Distance(lastPos) > 200)
                {
                    lastTick = currentTick;
                    lastPos = currentPos;
                }
                if ((currentTick - lastTick) > 70000)
                    Game.QuitGame();
            }
        }
    }    
}

