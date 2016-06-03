#region

using System.Linq;
using LeagueSharp.SDK.Core.Utils;
using LeagueSharp.SDK;
using EloBuddy;


#endregion

namespace Swiftly_Teemo.Main
{
    internal class Mode : Core
    {   
        public static void Combo()
        {
            if(Target.LSIsValidTarget() && Target != null && !Target.IsZombie)
            {
                if (Spells.R.IsReady() && Target.Distance(Player) <= Spells.R.Range - 50)
                {
                    Spells.R.Cast(Target);
                }
                if (Target.IsInvulnerable && Target != null)
                {
                    Spells.R.Cast(Target.Position);
                }
                if (Spells.W.IsReady() && Player.ManaPercent > 22.5)
                {
                    Spells.W.Cast();
                }
            }
        }
       
        public static void Lane()
        {
            var minions = GameObjects.EnemyMinions.Where(m => m.IsMinion && m.IsEnemy && m.Team != GameObjectTeam.Neutral && m.LSIsValidTarget(Player.AttackRange)).ToList();
            var ammo = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo;
            if (minions == null)
            {
                return;
            }
            foreach (var m in minions)
            {
                if (m.Health < Spells.Q.GetDamage(m) && Player.ManaPercent > 35 && MenuConfig.LaneQ)
                {
                    Spells.Q.Cast(m);
                }
                if (m.Health < Spells.R.GetDamage(m) && Player.ManaPercent > 40 && ammo >= 3)
                {
                    Spells.R.Cast(m);
                }
            }
        }
        public static void Jungle()
        {
            var mob = ObjectManager.Get<Obj_AI_Minion>().Where(m => !m.IsDead && !m.IsZombie && m.Team == GameObjectTeam.Neutral && m.LSIsValidTarget(Spells.Q.Range)).ToList();
            var ammo = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo;

            if (mob == null)
            {
                return;
            }
            foreach (var m in mob)
            {
                if(Spells.R.IsReady() && m.Distance(Player) <= Spells.R.Range && m.Health > Spells.R.GetDamage(m))
                {
                    if(!m.BaseSkinName.Contains("Sru_Crab"))
                    {
                        if(ammo >= 3)
                        {
                            Spells.R.Cast(m);
                        }
                    }
                   
                }
            }
        }
        public static void Flee()
        {
            if(!MenuConfig.Flee)
            {
                return;
            }

                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

                if(Spells.W.IsReady())
                {
                    Spells.W.Cast();
                }
                if(Target.Distance(Player) <= Spells.R.Range && Target.LSIsValidTarget() && Target != null)
                {
                if(Spells.R.IsReady())
                  {
                    Spells.R.Cast(Player.Position);
                  }
                }
            }
    }
}
