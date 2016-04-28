using System;
using LeagueSharp.Common;
using EloBuddy.SDK;

namespace NechritoRiven
{
    internal class Jungle
    {

        public static void JungleLogic()
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                var mobs = MinionManager.GetMinions(190 + Program.Player.AttackRange, MinionTypes.All, MinionTeam.Neutral,
               MinionOrderTypes.MaxHealth);
                if (mobs.Count == 0)
                    return;

                if (Spells._e.IsReady() && !Orbwalking.InAutoAttackRange(mobs[0]))
                {
                    Spells._e.Cast(mobs[0].Position);
                }
            }
        }
    }
}