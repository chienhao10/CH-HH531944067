using System;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using Utility = LeagueSharp.Common.Utility;

namespace NechritoRiven
{
    internal class FastHarass
    {
        public static void FastHarassLogic()
        {
            var target = TargetSelector.GetTarget(400, DamageType.Physical);
            if (Spells._q.IsReady() && Spells._w.IsReady() && Program._qstack == 1)
            {
                if (target.IsValidTarget() && !target.IsZombie)
                {
                    Logic.ForceCastQ(target);
                    Utility.DelayAction.Add(1, Logic.ForceW);
                }
            }
            if (Spells._q.IsReady() && Program._qstack == 3)
            {
                if (target.IsValidTarget() && !target.IsZombie)
                {
                    Logic.ForceCastQ(target);
                    Utility.DelayAction.Add(1, Logic.ForceW);
                }
            }
        }
    }
}