using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.SDK;
using Swiftly_Teemo.Main;
using SharpDX;
using System;
using System.Linq;

namespace Swiftly_Teemo.Handler
{
    internal class AfterAa : Core
    {
        public static void Orbwalker_OnPostAttack(AttackableUnit dgfg, EventArgs args)
        {
            if (dgfg is AIHeroClient)
            {
                   var Target = dgfg as AIHeroClient;

                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                    {
                        if (Target == null || Target.IsDead || Target.IsInvulnerable || !Target.IsValidTarget(Spells.Q.Range)) return;
                        {
                            if (Spells.Q.IsReady())
                            {
                                Spells.Q.Cast(Target);
                            }
                        }
                    
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)) return;

                    var mob = GameObjects.Jungle.Where(m => m != null && m.IsValidTarget(Player.AttackRange) && !GameObjects.JungleSmall.Contains(m));

                    foreach (var m in mob)
                    {
                        if (Spells.Q.IsReady())
                        {
                            Spells.Q.Cast(m);
                        }
                    }
                }
            }
        }
    }
}