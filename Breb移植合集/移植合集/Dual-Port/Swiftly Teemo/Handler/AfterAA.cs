using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.UI;
using LeagueSharp.SDK.Core.Utils;
using SharpDX;
using System;
using System.Linq;

namespace Swiftly_Teemo.Handler
{
    class AfterAA : Core
    {
        public static void Orbwalker_OnPostAttack(AttackableUnit dgfg, EventArgs args)
        {
            if (dgfg is AIHeroClient)
            {
                var Target = dgfg as AIHeroClient;

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))

                {
                    if (Spells.Q.IsReady())
                    {
                        Spells.Q.Cast(Target);
                    }
                }
            }

            if(Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                {
                    var mob = ObjectManager.Get<Obj_AI_Minion>().Where(m => !m.IsDead && !m.IsZombie && m.Team == GameObjectTeam.Neutral && m.LSIsValidTarget(Spells.Q.Range)).ToList();
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
