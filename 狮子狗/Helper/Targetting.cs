using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace RengarPro_Revamped.Helper
{
    internal class Targetting
    {
        public static AIHeroClient RjumpTarget, PassiveJumpTarget;

        public static void Initialize()
        {
            Game.OnTick += Game_OnTick;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
            Player.OnIssueOrder += Player_OnIssueOrder;
        }

        private static void Player_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            try
            {
                if (Standarts.Rengar.IsDead)
                {
                    return;
                }

                if (!sender.IsMe
                    || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)
                    || !args.Order.HasFlag(GameObjectOrder.AttackUnit)
                    || !Standarts.RengarHasUltimate
                    || args.Target == null || !args.Target.IsValid || !(args.Target is AIHeroClient))
                    return;
                UltimateTargetingOnIssue(sender, args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            try
            {
                if (Standarts.Rengar.IsDead)
                {
                    return;
                }

                if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                    return;
                if (Standarts.RengarHasPassive && !Standarts.RengarHasUltimate)
                {
                    args.Process = false;
                    BushTargeting();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            try
            {
                if (Standarts.Rengar.IsDead)
                {
                    return;
                }

                if (Standarts.RengarHasUltimate)
                {
                    RjumpTarget = GetUltimateTarget();
                }
                else if (Standarts.RengarHasPassive)
                {
                    PassiveJumpTarget = GetBushTarget();
                }
                if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                    return;
                UltimateTargetingOnTick();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void UltimateTargetingOnIssue(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            try
            {
                var target = args.Target as AIHeroClient;
                var ultTarget = GetUltimateTarget();
                if (!target.IsValid() || !ultTarget.IsValidTarget() || target.NetworkId != ultTarget.NetworkId)
                    args.Process = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void UltimateTargetingOnTick()
        {
            try
            {
                if (!Standarts.RengarHasUltimate)
                    return;
                var ultTarget = GetUltimateTarget();
                if (!ultTarget.IsValid() || !Player.Instance.IsInAutoAttackRange(ultTarget) || !Orbwalker.CanAutoAttack)
                    return;
                Player.IssueOrder(GameObjectOrder.AttackUnit, ultTarget);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void BushTargeting()
        {
            try
            {
                var target = GetBushTarget();
                if (!target.IsValid() || !Player.Instance.IsInAutoAttackRange(target) || !Orbwalker.CanAutoAttack)
                    return;
                Player.IssueOrder(GameObjectOrder.AttackUnit, target);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static AIHeroClient GetUltimateTarget()
        {
            if (TargetSelector.SelectedTarget.IsValid())
            {
                return TargetSelector.SelectedTarget;
            }
            var target =
                EntityManager.Heroes.Enemies.Where(hero => hero.IsValid())
                    .OrderBy(hero => hero.Distance(Player.Instance))
                    .FirstOrDefault();
            return target != null ? target : TargetSelector.SelectedTarget;
        }

        private static AIHeroClient GetBushTarget()
        {
            if (TargetSelector.SelectedTarget.IsValid()
                && Player.Instance.IsInAutoAttackRange(TargetSelector.SelectedTarget))
            {
                return TargetSelector.SelectedTarget;
            }
            var target =
                EntityManager.Heroes.Enemies.Where(hero => hero.IsValid() && Player.Instance.IsInAutoAttackRange(hero))
                    .OrderByDescending(hero => EntityManager.Heroes.Enemies)
                    .ThenBy(hero => hero.Health)
                    .FirstOrDefault();
            return target;
        }
    }
}