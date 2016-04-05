using System;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace HumanziedBaseUlt
{
    class Events
    {
        public class InvisibleEventArgs : EventArgs
        {
            public int StartTime { get; set; }
            public AIHeroClient sender { get; set; }
            /// <summary>
            /// per second without potions
            /// </summary>
            public float StdHealthRegen { get; set; }

            public Vector3[] LastRealPath { get; set; }
        }

        public delegate void OnEnemyInvisibleH(InvisibleEventArgs args);
        public static event OnEnemyInvisibleH OnEnemyInvisible;
        protected void FireOnEnemyInvisible(InvisibleEventArgs args)
        {
            if (OnEnemyInvisible != null) OnEnemyInvisible(args);
        }

        public delegate void OnEnemyVisibleH(AIHeroClient sender);
        public static event OnEnemyVisibleH OnEnemyVisible;
        protected void FireOnEnemyVisible(AIHeroClient sender)
        {
            if (OnEnemyVisible != null) OnEnemyVisible(sender);
        }

        public class Messaging
        {
            private static int lastMsg;
            /// <summary>
            /// Prints ult delay in chat
            /// </summary>
            /// <param name="delay"></param>
            /// <param name="args">Champion name</param>
            public static void ProcessInfo(float delay, string args)
            {
                if (Core.GameTickCount - lastMsg <= 2000)
                    return;
               

                Chat.Print("<font color=\"#0cf006\">" + args + " ult delay: " + delay + " ms</font>");
                lastMsg = Core.GameTickCount;
            } 
        }
    }
}
