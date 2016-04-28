using System;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using Utility = LeagueSharp.Common.Utility;

namespace NechritoRiven
{
    internal class Harass
    {
        public static void HarassLogic()
        {
            var target = TargetSelector.GetTarget(400, DamageType.Physical);
            if (Spells._q.IsReady() && Spells._w.IsReady() && Spells._e.IsReady() && Program._qstack == 1)
            {
                if (target.IsValidTarget() && !target.IsZombie)
                {
                    Logic.ForceCastQ(target);
                    Utility.DelayAction.Add(1, Logic.ForceW);
                }
            }
            if (Spells._q.IsReady() && Spells._e.IsReady() && Program._qstack == 3 && !Orbwalker.CanAutoAttack)
            {
                var epos = Program.Player.ServerPosition +
                           (Program.Player.ServerPosition - target.ServerPosition).Normalized() * 300;
                Spells._e.Cast(epos);
                Utility.DelayAction.Add(190, () => Spells._q.Cast(epos));
            }
        }
    }
}