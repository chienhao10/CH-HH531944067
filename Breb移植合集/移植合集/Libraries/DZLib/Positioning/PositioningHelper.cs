using System;
using System.Collections.Generic;
using System.Linq;
using DZLib.Geometry;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;

namespace DZLib.Positioning
{
    public class PositioningHelper
    {
        public static IEnumerable<AIHeroClient> EnemiesClose
        {
            get
            {
                return
                    HeroManager.Enemies.Where(
                        m =>
                            m.LSDistance(ObjectManager.Player, true) <= Math.Pow(1000, 2) && m.LSIsValidTarget(1500, false) &&
                            m.LSCountEnemiesInRange(m.IsMelee() ? m.AttackRange * 1.5f : m.AttackRange + 20 * 1.5f) > 0);
            }
        }

        public static float GetAvgDistanceFromEnemyTeam(Vector3 from, float MaxRange = 1200f)
        {
            var numberOfEnemies = from.LSCountEnemiesInRange(MaxRange);
            if (numberOfEnemies != 0)
            {
                var enemies = HeroManager.Enemies.Where(en => en.LSIsValidTarget(MaxRange, true, from)).ToList();
                var totalDistance = enemies.Sum(en => en.LSDistance(ObjectManager.Player.ServerPosition));

                return totalDistance / numberOfEnemies;
            }
            return -1;
        }

        public static float GetAvgDistanceFromAllyTeam(Vector3 from, float MaxRange = 1200f)
        {
            var numberOfAllies = from.CountAlliesInRange(MaxRange);
            if (numberOfAllies != 0)
            {
                var allies = HeroManager.Allies.Where(ally => ally.LSIsValidTarget(MaxRange, false, from) && !ally.IsMe).ToList();
                var totalDistance = allies.Sum(ally => ally.LSDistance(ObjectManager.Player.ServerPosition));

                return totalDistance / numberOfAllies;
            }
            return -1;
        }

        public static AIHeroClient GetClosestEnemy(float MaxRange, Vector3 from = default(Vector3))
        {
            return
                HeroManager.Enemies
                .FirstOrDefault(en => en.LSIsValidTarget(MaxRange, true, from == default(Vector3) ? ObjectManager.Player.ServerPosition : from));
        }


        public static AIHeroClient GetClosestAlly(float MaxRange, Vector3 from = default(Vector3))
        {
            return
               HeroManager.Allies
               .FirstOrDefault(ally => !ally.IsMe && ally.LSIsValidTarget(MaxRange, false, from == default(Vector3) ? ObjectManager.Player.ServerPosition : from));
        }

        public static List<Vector2> GetRotatedPositions(Vector3 from, float range, float angle, int step)
        {
            int currentStep = step;
            var direction = ObjectManager.Player.Direction.LSTo2D().LSPerpendicular();
            var currentList = new List<Vector2>();
            for (var i = 0f; i < angle; i += currentStep)
            {
                var angleRad = LeagueSharp.Common.Geometry.DegreeToRadian(i);
                var rotatedPosition = ObjectManager.Player.Position.LSTo2D() + (range * direction.LSRotated(angleRad));
                currentList.Add(rotatedPosition);
            }
            return currentList;
        }

        public static List<Vector2> GetEnemyZoneList(bool dynamic = true)
        {
            var staticRange = 360f;
            var polygonsList = EnemiesClose.ToList().Select(enemy => new DZGeometry.Circle(enemy.ServerPosition.LSTo2D(), (dynamic ? (enemy.IsMelee ? enemy.AttackRange * 1.5f : enemy.AttackRange) : staticRange) + enemy.BoundingRadius + 20).ToPolygon()).ToList();
            var pathList = DZGeometry.ClipPolygons(polygonsList);
            var pointList = pathList.SelectMany(path => path, (path, point) => new Vector2(point.X, point.Y)).Where(currentPoint => !currentPoint.LSIsWall()).ToList();
            return pointList;
        }
    }
}
