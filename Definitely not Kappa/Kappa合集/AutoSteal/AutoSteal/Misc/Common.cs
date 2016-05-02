using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSteal.Misc
{
    using EloBuddy;

    internal static class Common
    {
        public static bool IsKillable(this Obj_AI_Base target)
        {
            if (target != null)
            {
                if (target.HasBuff("kindredrnodeathbuff") || target.HasBuff("JudicatorIntervention")
                    || target.HasBuff("ChronoShift") || target.HasBuff("UndyingRage") || target.IsInvulnerable
                    || target.IsZombie)
                {
                    return false;
                }
                return true;
            }
            return true;
        }
    }
}
