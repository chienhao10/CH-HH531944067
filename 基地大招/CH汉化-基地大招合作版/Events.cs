using System;
using EloBuddy;
using SharpDX;

namespace HumanziedBaseUlt
{
    class Events
    {
        public class InvisibleEventArgs : EventArgs
        {
            /// <summary>
            /// in ms
            /// </summary>
            public int StartTime { get; set; }
            public AIHeroClient sender { get; set; }

            public Vector3[] LastRealPath { get; set; }
        }

        //public delegate void OnEnemyInvisibleH(InvisibleEventArgs args);
        //public static event OnEnemyInvisibleH OnEnemyInvisible;
        //protected void FireOnEnemyInvisible(InvisibleEventArgs args)
        //{
        //    if (OnEnemyInvisible != null) OnEnemyInvisible(args);
        //}

        //public delegate void OnEnemyVisibleH(AIHeroClient sender);
        //public static event OnEnemyVisibleH OnEnemyVisible;
        //protected void FireOnEnemyVisible(AIHeroClient sender)
        //{
        //    if (OnEnemyVisible != null) OnEnemyVisible(sender);
        //}
    }
}
