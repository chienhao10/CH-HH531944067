using Arcane_Ryze.Handler;
using Arcane_Ryze.Main;
using EloBuddy;
using LeagueSharp.SDK;
using System;
using System.Linq;
using static Arcane_Ryze.Core;

namespace Arcane_Ryze.Modes
{
    class LastHit
    {
        public static void LastHitLogic()
        {
            var minions = GameObjects.EnemyMinions.Where(m => m.IsMinion && m.IsEnemy && m.Team != GameObjectTeam.Neutral && m.LSIsValidTarget(ObjectManager.Player.AttackRange)).ToList();
            if (!MenuConfig.LaneQ || minions == null)
            {
                return;
            }
            if(PassiveStack < 4)
            {
                foreach (var m in minions)
                {
                    if(m.Health < Spells.Q.GetDamage(m) && m.Health > (float)ObjectManager.Player.GetAutoAttackDamage(m) && !ObjectManager.Player.Spellbook.IsAutoAttacking)
                    {
                        Spells.Q.Cast(m);
                    }
                    if (m.Health < Spells.W.GetDamage(m) && m.Health > (float)ObjectManager.Player.GetAutoAttackDamage(m) && !ObjectManager.Player.Spellbook.IsAutoAttacking)
                    {
                        Spells.W.Cast(m);
                    }
                }
            }
        }
    }
}
