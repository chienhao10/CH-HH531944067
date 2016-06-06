using System;
using EloBuddy;
using KappaLeBlanc;
using EloBuddy.SDK;
using System.Linq;

namespace Modes
{
    class CloneControl : Helper
    {
        public static void Execute()
        {
            if (Clone == null) return;

            switch (CastSlider(LBMenu.Misc, "CloneMode"))
            {
                case 0:
                    MoveToTurret();
                    break;
                case 1:
                    MoveToMouse();
                    break;
                case 2:
                    MoveToEnemy();
                    break;
            }
        }

        private static void MoveToEnemy()
        {
            var target = TargetSelector.GetTarget(int.MaxValue, DamageType.Magical);
            if (target == null)
                MoveToMouse();
            Player.IssueOrder(GameObjectOrder.MovePet, target.Position);
        }

        private static void MoveToMouse()
        {
            Player.IssueOrder(GameObjectOrder.MovePet, Game.CursorPos);
        }

        private static void MoveToTurret()
        {
            Obj_AI_Turret turret = EntityManager.Turrets.Allies.Where(x => !x.IsDead).OrderBy(x => x.Distance(Clone)).FirstOrDefault();
            if (turret == null)
                MoveToMouse();
            Player.IssueOrder(GameObjectOrder.MovePet, turret.Position.Extend(myHero, new Random().Next((int)turret.BoundingRadius, 400)).To3D());
        }
    }
}