using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;
using LS = LeagueSharp.Common;

namespace SoloVayne.Skills.Tumble
{
    class TumbleHelper
    {
        /// <summary>
        /// Gets the rotated q positions.
        /// </summary>
        /// <returns></returns>
        public static List<Vector3> GetRotatedQPositions()
        {
            const int currentStep = 30;
           // var direction = ObjectManager.Player.Direction.To2D().Perpendicular();
            var direction = (Game.CursorPos - ObjectManager.Player.ServerPosition).Normalized().To2D();

            var list = new List<Vector3>();
            for (var i = -105; i <= 105; i += currentStep)
            {
                var angleRad = Geometry.DegreeToRadian(i);
                var rotatedPosition = ObjectManager.Player.Position.To2D() + (300f * direction.Rotated(angleRad));
                list.Add(rotatedPosition.To3D());
            }
            return list;
        }

        /// <summary>
        /// Gets the rotated q positions.
        /// </summary>
        /// <returns></returns>
        public static List<Vector3> GetCompleteRotatedQPositions()
        {
            const int currentStep = 30;
            // var direction = ObjectManager.Player.Direction.To2D().Perpendicular();
            var direction = (Game.CursorPos - ObjectManager.Player.ServerPosition).Normalized().To2D();

            var list = new List<Vector3>();
            for (var i = -0; i <= 360; i += currentStep)
            {
                var angleRad = Geometry.DegreeToRadian(i);
                var rotatedPosition = ObjectManager.Player.Position.To2D() + (300f * direction.Rotated(angleRad));
                list.Add(rotatedPosition.To3D());
            }
            return list;
        }

        /// <summary>
        /// Gets the closest enemy.
        /// </summary>
        /// <param name="from">From.</param>
        /// <returns></returns>
        public static AIHeroClient GetClosestEnemy(Vector3 from)
        {
            if (TargetSelector.SelectedTarget is AIHeroClient)
            {
                var owAI = TargetSelector.SelectedTarget as AIHeroClient;
                if (owAI.IsValidTarget(LS.Orbwalking.GetRealAutoAttackRange(null) + 120f, true, from))
                {
                    return owAI;
                }
            }

            return null;
        }

        /// <summary>
        /// Determines whether the specified position is Safe using AA ranges logic.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public static bool IsSafeEx(Vector3 position)
        {
            var closeEnemies = LS.HeroManager.Enemies.FindAll(en => en.IsValidTarget(1500f) && !(en.Distance(ObjectManager.Player.ServerPosition) < en.AttackRange + 65f)).OrderBy(en => en.Distance(position));

            return closeEnemies.All(enemy => position.CountEnemiesInRange(enemy.AttackRange) <= 1);
        }

        /// <summary>
        /// Gets the average distance of a specified position to the enemies.
        /// </summary>
        /// <param name="from">From.</param>
        /// <returns></returns>
        public static float GetAvgDistance(Vector3 from)
        {
            var numberOfEnemies = from.CountEnemiesInRange(1200f);
            if (numberOfEnemies != 0)
            {
                var enemies = LS.HeroManager.Enemies.Where(en => en.IsValidTarget(1200f, true, from) && en.Health > ObjectManager.Player.GetAutoAttackDamage(en) * 3 + ObjectManager.Player.GetSpellDamage(en, SpellSlot.W) + ObjectManager.Player.GetSpellDamage(en, SpellSlot.Q)).ToList();
                var enemiesEx = LS.HeroManager.Enemies.Where(en => en.IsValidTarget(1200f, true, from)).ToList();
                var LHEnemies = enemiesEx.Count() - enemies.Count();

                var totalDistance = (LHEnemies > 1 && enemiesEx.Count() > 2) ?
                    enemiesEx.Sum(en => en.Distance(ObjectManager.Player.ServerPosition)) :
                    enemies.Sum(en => en.Distance(ObjectManager.Player.ServerPosition));

                return totalDistance / numberOfEnemies;
            }
            return -1;
        }

        /// <summary>
        /// Gets the enemy points.
        /// </summary>
        /// <param name="dynamic">if set to <c>true</c> [dynamic].</param>
        /// <returns></returns>
        public static List<Vector2> GetEnemyPoints(bool dynamic = true)
        {
            var staticRange = 360f;
            var polygonsList = TumbleVariables.EnemiesClose.Select(enemy => new SOLOVayne.SOLOGeometry.Circle(enemy.ServerPosition.To2D(), (dynamic ? (enemy.IsMelee ? enemy.AttackRange * 1.5f : enemy.AttackRange) : staticRange) + enemy.BoundingRadius + 20).ToPolygon()).ToList();
            var pathList = SOLOVayne.SOLOGeometry.ClipPolygons(polygonsList);
            var pointList = pathList.SelectMany(path => path, (path, point) => new Vector2(point.X, point.Y)).Where(currentPoint => !currentPoint.IsWall()).ToList();
            return pointList;
        }

        /// <summary>
        /// Gets the Q burst mode position.
        /// </summary>
        /// <returns></returns>
        public static Vector3? GetQBurstModePosition()
        {
            var positions =
                GetWallQPositions(70).ToList().OrderBy(pos => pos.Distance(ObjectManager.Player.ServerPosition, true));

            foreach (var position in positions)
            {
                if (position.IsWall() && position.IsSafe())
                {
                    return position;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the wall Q positions.
        /// </summary>
        /// <param name="Range">The range.</param>
        /// <returns></returns>
        public static Vector3[] GetWallQPositions(float Range)
        {
            Vector3[] vList =
            {
                (ObjectManager.Player.ServerPosition.To2D() + Range * ObjectManager.Player.Direction.To2D()).To3D(),
                (ObjectManager.Player.ServerPosition.To2D() - Range * ObjectManager.Player.Direction.To2D()).To3D()
            };

            return vList;
        }

    }
}
