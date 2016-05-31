#region

using System.Linq;
using Arcane_Ryze.Main;
using LeagueSharp.SDK;
using static Arcane_Ryze.Core;

#endregion

namespace Arcane_Ryze.Handler
{
    internal class KillSteal
    {
        public static void Killsteal()
        {
            foreach (var target in GameObjects.EnemyHeroes.Where(x => x.LSIsValidTarget(Spells.Q.Range) && !x.IsDead && !x.IsZombie))
            {
                if (target != null && target.LSIsValidTarget() && !target.IsInvulnerable)
                {
                    if (Spells.Q.IsReady() && target.Health < Spells.Q.GetDamage(target))
                    {
                        Spells.Q.Cast(target);
                    }
                    if (Spells.E.IsReady() && target.Health < Spells.E.GetDamage(target))
                    {
                        Spells.E.Cast(target);
                    }
                    if (Spells.W.IsReady() && target.Health < Spells.W.GetDamage(target))
                    {
                        Spells.W.Cast(target);
                    }
                }
            }
            if (MenuConfig.KillStealSummoner)
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
