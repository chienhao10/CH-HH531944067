using LeagueSharp.SDK;
using EloBuddy.SDK;
using EloBuddy;
using System;

namespace Arcane_Ryze.Modes
{
    class BeforeAA : Core
    {
        public static void OnAction(AttackableUnit fdf, EventArgs args)
        {
            if (fdf is AIHeroClient)
            {
                var target = fdf as AIHeroClient;

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                if(Spells.Q.IsReady() && target.LSIsValidTarget() && !target.IsZombie && PassiveStack < 4)
                {
                    Spells.Q.Cast(target);  
                }
            }
        }
    }
  }
}
