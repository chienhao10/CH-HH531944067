using System;
using EloBuddy;
using EloBuddy.SDK;

namespace AutoBuddy
{
    internal static class RandGen
    {
        private static int lastPath=1;
        public static Random r { get; private set; }

        static RandGen()
        {
            r=new Random();
            Core.DelayAction(ChangeSeed, 5000);
            Obj_AI_Base.OnNewPath += Obj_AI_Base_OnNewPath;
        }

        private static void Obj_AI_Base_OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            lastPath = DateTime.Now.Millisecond;
        }

        private static void ChangeSeed()
        {
            r = new Random(DateTime.Now.Millisecond * lastPath * (int)(Game.CursorPos.X+1000) * (int)(Game.CursorPos.Y+1000));
            Core.DelayAction(ChangeSeed, r.Next(10000, 20000));
        }

        public static void Start()
        {
        }
    }
}
