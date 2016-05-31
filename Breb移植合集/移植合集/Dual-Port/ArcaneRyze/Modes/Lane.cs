using Arcane_Ryze.Handler;
using Arcane_Ryze.Main;
using EloBuddy;
using LeagueSharp.SDK;
using System;
using System.Linq;
using static Arcane_Ryze.Core;

namespace Arcane_Ryze.Modes
{
    class Lane
    {
        public static void LaneLogic()
        {
            var minions = GameObjects.EnemyMinions.Where(m => m.IsMinion && m.IsEnemy && m.Team != GameObjectTeam.Neutral && m.LSIsValidTarget(ObjectManager.Player.AttackRange)).ToList();
            if (PassiveStack > 4)
            {
                return;
            }
            if (!(ObjectManager.Player.ManaPercent >= MenuConfig.LaneMana)) return;
            {
                foreach (var m in minions)
                {
                    if (!m.LSIsValidTarget() || m.IsZombie || m.IsDead) continue;
                    if (m.Health < Spells.Q.GetDamage(m) && !ObjectManager.Player.Spellbook.IsAutoAttacking)
                    {
                        Spells.Q.Cast(m);
                    }
                    if (Spells.Q.IsReady() && m.Health > Spells.Q.GetDamage(m) && ObjectManager.Player.GetAutoAttackDamage(m) > m.Health)
                    {
                        Spells.Q.Cast(m);
                    }
                    if (Spells.E.IsReady())
                    {
                        Spells.E.Cast(m);
                    }
                    if (Spells.W.IsReady() && m.Health < Spells.W.GetDamage(m))
                    {
                        Spells.W.Cast(m);
                    }
                    if (Spells.R.IsReady() && MenuConfig.LaneR)
                    {
                        Spells.R.Cast();
                    }
                }
            }
        }
    }
}
