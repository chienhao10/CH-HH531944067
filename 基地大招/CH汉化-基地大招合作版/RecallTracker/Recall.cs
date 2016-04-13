using System;
using EloBuddy;

namespace HumanziedBaseUlt.RecallTracker
{
    class Recall
    {
        public Recall(AIHeroClient unit, int recallStart, int recallEnd, int duration)
        {
            Unit = unit;
            RecallStart = recallStart;
            Duration = duration;
            RecallEnd = recallEnd;
            ExpireTime = RecallEnd + 2000;
        }

        private readonly int RecallEnd;
        private readonly int Duration;
        private readonly int RecallStart;
        public int ExpireTime;
        private int CancelT;

        public readonly AIHeroClient Unit;

        public bool GetsBaseUlted = false;
        private float _BaseUltArriveTimePercentComplete = 0;
        /// <summary>
        /// PercentComplete when baseUlt gets fired
        /// </summary>
        public float BaseUltArriveTimePercentComplete
        {
            get { return _BaseUltArriveTimePercentComplete; }
            set
            {
                GetsBaseUlted = true;
                _BaseUltArriveTimePercentComplete = value;
            }
        }

        public void SetBaseUltShootTime(float shootTime)
        {
            //shootTime - startTime = elapsed
            float elapsed = shootTime /*offset*/ - RecallStart;
            BaseUltArriveTimePercentComplete =
                (float) Math.Round((elapsed / Duration)*100) > 100 ? 100 : (float) Math.Round((elapsed / Duration)*100);
        }

        public bool IsAborted;

        public void Abort()
        {
            CancelT = Environment.TickCount;
            ExpireTime = Environment.TickCount + 2000;
            IsAborted = true;
            GetsBaseUlted = false;
        }

        private float Elapsed { get { return ((CancelT > 0 ? CancelT : Environment.TickCount) - RecallStart); } }

        public float PercentComplete()
        {
            return (float)Math.Round((Elapsed / Duration) * 100) > 100 ? 100 : (float)Math.Round((Elapsed / Duration) * 100);
        }
    }
}
