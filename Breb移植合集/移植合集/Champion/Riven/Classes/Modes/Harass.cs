using System;
using LeagueSharp.Common;
using EloBuddy.SDK;
using EloBuddy;

namespace NechritoRiven
{
    class Harass
    {
        private static void Game_OnUpdate(EventArgs args)
        {
            HarassLogic();
        }
       public static void HarassLogic()
        {
            var target = TargetSelector.GetTarget(400, DamageType.Physical);
            if (Spells._q.IsReady() && Spells._w.IsReady() && Spells._e.IsReady() && Program._qstack == 1)
            {
                if (target.IsValidTarget() && !target.IsZombie)
                {
                    Logic.ForceCastQ(target);
                    LeagueSharp.Common.Utility.DelayAction.Add(1, Logic.ForceW);
                }
            }
            if (Spells._q.IsReady() && Spells._e.IsReady() && Program._qstack == 3 && !Orbwalker.CanAutoAttack && Orbwalker.CanMove)
            {
                var epos = Program.Player.ServerPosition +
                          (Program.Player.ServerPosition - target.ServerPosition).Normalized() * 300;
                Spells._e.Cast(epos);
                LeagueSharp.Common.Utility.DelayAction.Add(190, () => Spells._q.Cast(epos));
            }
        }
    }
}
