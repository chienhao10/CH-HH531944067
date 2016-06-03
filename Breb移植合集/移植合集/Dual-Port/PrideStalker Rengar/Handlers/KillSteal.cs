using EloBuddy;
using LeagueSharp.SDK;
using PrideStalker_Rengar.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrideStalker_Rengar.Handlers
{
    class KillSteal : Core
    {
       
        public static void Killsteal()
        {
            foreach (var target in GameObjects.EnemyHeroes.Where(x => x.LSIsValidTarget(800) && !x.IsDead && !x.IsZombie))
            {
                if(target != null && target.LSIsValidTarget())
                {
                    if(Spells.E.IsReady() && target.Health < Spells.E.GetDamage(target))
                    {
                        Spells.E.Cast(target);
                    }
                    if (Spells.W.IsReady() && target.Health < Spells.W.GetDamage(target))
                    {
                        Spells.W.Cast(target);
                    }
                }
            }
            if(MenuConfig.KillStealSummoner)
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(t => t.LSIsValidTarget(600f)))
                {
                    if (target.Health < Dmg.IgniteDmg && Spells.Ignite.IsReady())
                    {
                        GameObjects.Player.Spellbook.CastSpell(Spells.Ignite, target);
                    }
                }
            }
        }
    }
}
