using System;
using LeagueSharp.Common;
using LeagueSharp;
using System.Linq;
using EloBuddy.SDK;
using EloBuddy;

namespace NechritoRiven
{
    class FastHarass
    {
        private static void Game_OnUpdate(EventArgs args)
        {
            FastHarassLogic();
        }

        public static void FastHarassLogic()
        {
            var target = TargetSelector.GetTarget(400, DamageType.Physical);
            if (Spells._q.IsReady() && Spells._w.IsReady() && Program._qstack == 1)
            {
                if (target.IsValidTarget() && !target.IsZombie)
                {
                    Logic.ForceCastQ(target);
                    LeagueSharp.Common.Utility.DelayAction.Add(1, Logic.ForceW);
                }
            }
            if (Spells._q.IsReady() && Program._qstack == 3)
            {
                if (target.IsValidTarget() && !target.IsZombie)
                {
                    Logic.ForceCastQ(target);
                    LeagueSharp.Common.Utility.DelayAction.Add(1, Logic.ForceW);
                }
            }
        }
    }
}
