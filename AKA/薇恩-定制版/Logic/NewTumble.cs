using SharpDX;
using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using System.Collections.Generic;
using System;
using Auto_Carry_Vayne.Manager;

namespace Auto_Carry_Vayne.Logic
{
    static class NewTumble
    {
        #region AkaTumble Extensions
        #region SafeCheck
        static bool IsSafe(Vector3 position)
        {
            var closeEnemies =
                    EntityManager.Heroes.Enemies.FindAll(en => en.IsValidTarget(1500f) && !(en.Distance(ObjectManager.Player.ServerPosition) < en.AttackRange + 65f))
                    .OrderBy(en => en.Distance(position));

            return closeEnemies.All(
                                enemy =>
                                    position.CountEnemiesInRange(enemy.AttackRange) <= 1);
        }
        public static bool IsSafeCheck(this Vector3 position)
        {
            return IsSafe(position)
                && !(NavMesh.GetCollisionFlags(position).HasFlag(CollisionFlags.Wall)
                            || NavMesh.GetCollisionFlags(position).HasFlag(CollisionFlags.Building))
                //&& position.IsNotIntoTraps()
                && position.IsNotIntoEnemies()
                && EntityManager.Heroes.Enemies.All(m => m.Distance(position) > 350f)
                && (!Variables.UnderEnemyTower((Vector2)position) || (Variables.UnderEnemyTower((Vector2)Variables._Player.Position) && Variables.UnderEnemyTower((Vector2)position) && ObjectManager.Player.HealthPercent > 10));
            //Either it is not under turret or both the player and the position are under turret already and the health percent is greater than 10.
        }

        static List<Vector2> GetEnemyPoints(bool dynamic = true)
        {
            var staticRange = 360f;
            var polygonsList = Variables.EnemiesClose.Select(enemy => new Geometry.Polygon.Circle(enemy.ServerPosition.To2D(), (dynamic ? (enemy.IsMelee ? enemy.AttackRange * 1.5f : enemy.AttackRange) : staticRange) + enemy.BoundingRadius + 20)).ToList();
            var pathList = Geometry.ClipPolygons(polygonsList);
            var pointList = pathList.SelectMany(path => path, (path, point) => new Vector2(point.X, point.Y)).Where(currentPoint => !currentPoint.IsWall()).ToList();
            return pointList;
        }

        static bool IsNotIntoTraps(this Vector3 position)
        {
            var tarpsnear = Traps.EnemyTraps.FindAll(t => t.Distance(position) <= 5);

            if (tarpsnear != null)
            {
                return false;
            }
            return true;
        }


        static bool IsNotIntoEnemies(this Vector3 position)
        {

            var enemyPoints = GetEnemyPoints();
            if (enemyPoints.ToList().Contains(position.To2D()) && !enemyPoints.Contains(ObjectManager.Player.ServerPosition.To2D()))
            {
                return false;
            }

            var closeEnemies =
                EntityManager.Heroes.Enemies.FindAll(
                    en =>
                        en.IsValidTarget(1500f) &&
                        !(en.Distance(ObjectManager.Player.ServerPosition) < en.AttackRange + 65f));
            if (!closeEnemies.All(enemy => position.CountEnemiesInRange(enemy.AttackRange) <= 1))
            {
                return false;
            }

            return true;
        }
        #endregion
        #region DistanceCheck
        static AIHeroClient GetClosestEnemy(Vector3 from)
        {
            if (Orbwalker.LastTarget is AIHeroClient)
            {
                var owAI = TargetSelector.GetTarget((int)Variables._Player.GetAutoAttackRange(), DamageType.Physical);
                if (owAI.IsValidTarget(Variables._Player.GetAutoAttackRange(null) + 120f, true, from))
                {
                    return owAI;
                }
            }

            return null;
        }
        static float GetAvgDistance(Vector3 from)
        {
            var numberOfEnemies = from.CountEnemiesInRange(1200f);
            if (numberOfEnemies != 0)
            {
                var enemies = EntityManager.Heroes.Enemies.Where(en => en.IsValidTarget(1200f, true, from)
                                                                       &&
                                                                       en.Health >
                                                                       Variables._Player.GetAutoAttackDamage(en) * 3 +
                                                                       Variables._Player.GetSpellDamage(en, SpellSlot.W) +
                                                                       Variables._Player.GetSpellDamage(en, SpellSlot.Q))
                    ;
                var enemiesEx = EntityManager.Heroes.Enemies.Where(en => en.IsValidTarget(1200f, true, from));
                var LHEnemies = enemiesEx.Count() - enemies.Count();

                var totalDistance = (LHEnemies > 1 && enemiesEx.Count() > 2)
                    ? enemiesEx.Sum(en => en.Distance(Variables._Player.ServerPosition))
                    : enemies.Sum(en => en.Distance(Variables._Player.ServerPosition));

                return totalDistance / numberOfEnemies;
            }
            return -1;
        }
        public static List<Vector3> GetRotatedQPositions()
        {
            const int currentStep = 30;
            // var direction = Variables._Player.Direction.To2D().Perpendicular();
            var direction = (Game.CursorPos - Variables._Player.ServerPosition).Normalized().To2D();

            var list = new List<Vector3>();
            for (var i = -70; i <= 70; i += currentStep)
            {
                var angleRad = Geometry.DegreeToRadian(i);
                var rotatedPosition = Variables._Player.Position.To2D() + (300f * direction.Rotated(angleRad));
                list.Add(rotatedPosition.To3D());
            }
            return list;
        }
        #endregion
        #endregion
        public static Vector3 AkaQPosition()
        {
            #region Variables
            var positions = GetRotatedQPositions();
            var enemyPositions = GetEnemyPoints();
            var safePositions = positions.Where(pos => !enemyPositions.Contains(pos.To2D()));
            var BestPosition = Variables._Player.ServerPosition.Extend(Game.CursorPos, 300f);
            var AverageDistanceWeight = .60f;
            var ClosestDistanceWeight = .40f;

            var bestWeightedAvg = 0f;

            var enemiesNear =
    EntityManager.Heroes.Enemies.Where(
        m => m.IsValidTarget(Variables._Player.GetAutoAttackRange(m) + 300f + 65f));
            var highHealthEnemiesNear =
    EntityManager.Heroes.Enemies.Where(m => !m.IsMelee && m.IsValidTarget(1300f) && m.HealthPercent > 7)
        ;
            var closeNonMeleeEnemy = GetClosestEnemy((Vector3)Variables._Player.ServerPosition.Extend(Game.CursorPos, 300f));
            #endregion

            if (Variables._Player.CountEnemiesInRange(1500f) <= 1)
            {
                //Logic for 1 enemy near
                var position = (Vector3)Variables._Player.ServerPosition.Extend(Game.CursorPos, 300f);
                return position.IsSafeCheck() ? position : Vector3.Zero;
            }

            if (
        enemiesNear.Any(
            t =>
                t.Health + 15 <
                ObjectManager.Player.GetAutoAttackDamage(t) * 2 + Variables._Player.GetSpellDamage(t, SpellSlot.Q)
                && t.Distance(ObjectManager.Player) < Variables._Player.GetAutoAttackRange(t) + 80f))
            {
                var QPosition =
                    ObjectManager.Player.ServerPosition.Extend(
                        enemiesNear.OrderBy(t => t.Health).First().ServerPosition, 300f);

                if (!Variables.UnderEnemyTower(QPosition))
                {
                    return (Vector3)QPosition;
                }
            }

            if (enemiesNear.Count() <= 2)
            {
                if (
                    enemiesNear.Any(
                        t =>
                            t.Health + 15 <
                            Variables._Player.GetAutoAttackDamage(t) +
                            Variables._Player.GetSpellDamage(t, SpellSlot.Q)
                            && t.Distance(Variables._Player) < Variables._Player.GetAutoAttackRange(t) + 80f))
                {
                    var QPosition =
                        Variables._Player.ServerPosition.Extend(
                            highHealthEnemiesNear.OrderBy(t => t.Health).FirstOrDefault().ServerPosition, 300f);

                    if (!Variables.UnderEnemyTower(QPosition))
                    {
                        return (Vector3)QPosition;
                    }
                }
            }

            if (closeNonMeleeEnemy != null
                && Variables._Player.Distance(closeNonMeleeEnemy) <= closeNonMeleeEnemy.AttackRange - 85
                && !closeNonMeleeEnemy.IsMelee)
            {
                return ((Vector3)Variables._Player.ServerPosition.Extend(Game.CursorPos, 300f)).IsSafeCheck()
                    ? Variables._Player.ServerPosition.Extend(Game.CursorPos, 300f).To3D()
                    : Vector3.Zero;
            }

            foreach (var position in safePositions)
            {
                var enemy = GetClosestEnemy(position);
                if (!enemy.IsValidTarget())
                {
                    continue;
                }

                var avgDist = GetAvgDistance(position);

                if (avgDist > -1)
                {
                    var closestDist = Variables._Player.ServerPosition.Distance(enemy.ServerPosition);
                    var weightedAvg = closestDist * ClosestDistanceWeight + avgDist * AverageDistanceWeight;
                    if (weightedAvg > bestWeightedAvg && position.IsSafeCheck())
                    {
                        bestWeightedAvg = weightedAvg;
                        BestPosition = position.To2D();
                    }
                }
            }
            var endPosition = (BestPosition.To3D().IsSafeCheck()) ? BestPosition.To3D() : Vector3.Zero;

            if (endPosition == Vector3.Zero)
            {
                //Try to find another suitable position. This usually means we are already near too much enemies turrets so just gtfo and tumble
                //to the closest ally ordered by most health.
                var alliesClose =
                    EntityManager.Heroes.Allies.Where(ally => !ally.IsMe && ally.IsValidTarget(1500, false));
                if (alliesClose.Any() && enemiesNear.Any())
                {
                    var closestMostHealth =
                        alliesClose.OrderBy(m => m.Distance(Variables._Player))
                            .ThenByDescending(m => m.Health)
                            .FirstOrDefault();

                    if (closestMostHealth != null
                        &&
                        closestMostHealth.Distance(
                            enemiesNear.OrderBy(m => m.Distance(Variables._Player)).FirstOrDefault())
                        >
                        Variables._Player.Distance(
                            enemiesNear.OrderBy(m => m.Distance(Variables._Player)).FirstOrDefault()))
                    {
                        var tempPosition = (Vector3)Variables._Player.ServerPosition.Extend(closestMostHealth.ServerPosition,
                            300f);
                        if (tempPosition.IsSafeCheck())
                        {
                            endPosition = tempPosition;
                        }
                    }

                }

            }

            if (endPosition == Vector3.Zero)
            {
                var mousePosition = Variables._Player.ServerPosition.Extend(Game.CursorPos, 300f);
                if (mousePosition.To3D().IsSafeCheck())
                {
                    endPosition = mousePosition.To3D();
                }
            }


            return endPosition;
        }
    }
}
