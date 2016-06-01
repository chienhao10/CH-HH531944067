using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using NabbTracker;
using SharpDX;
using SOLOVayne;
using Program = Vayne1.Program;

namespace SoloVayne.Skills.Condemn
{
    internal class CondemnLogicProvider
    {
        /// <summary>
        ///     Gets the target.
        /// </summary>
        /// <param name="position">The starting position.</param>
        /// <returns>The condemn target or null</returns>
        internal AIHeroClient GetTarget(Vector3 position = default(Vector3))
        {
            var HeroList =
                HeroManager.Enemies.Where(
                    h =>
                        h.LSIsValidTarget(Program.E.Range) && !h.HasBuffOfType(BuffType.SpellShield) &&
                        !h.HasBuffOfType(BuffType.SpellImmunity));

            var Accuracy = 38;
            var PushDistance = 425;

            if (ObjectManager.Player.ServerPosition.UnderTurret(true))
            {
                return null;
            }

            if (HeroManager.Allies.Count(ally => !ally.IsMe && ally.LSIsValidTarget(1500f, false)) == 0 &&
                ObjectManager.Player.LSCountEnemiesInRange(1500f) == 1)
            {
                Accuracy = 33;
                PushDistance = 460;
            }

            var startPosition = position != default(Vector3) ? position : ObjectManager.Player.ServerPosition;

            foreach (var Hero in HeroList)
            {
                if (Hero.Health + 10 <= ObjectManager.Player.GetAutoAttackDamage(Hero)*2)
                {
                    continue;
                }
                var prediction = Program.E2.GetPrediction(Hero);
                var targetPosition = prediction.UnitPosition;
                var finalPosition = targetPosition.LSExtend(startPosition, -PushDistance);
                var finalPosition_ex = Hero.ServerPosition.LSExtend(startPosition, -PushDistance);
                var finalPosition_3 = prediction.CastPosition.LSExtend(startPosition, -PushDistance);

                //Yasuo Wall Logic
                if (YasuoWall.CollidesWithWall(startPosition, Hero.ServerPosition.LSExtend(startPosition, -450f)))
                {
                    continue;
                }

                //Condemn to turret logic
                if (
                    GameObjects.AllyTurrets.Any(
                        m => m.IsValidTarget(float.MaxValue) && m.LSDistance(finalPosition) <= 450f))
                {
                    var turret =
                        GameObjects.AllyTurrets.FirstOrDefault(
                            m => m.IsValidTarget(float.MaxValue) && m.LSDistance(finalPosition) <= 450f);
                    if (turret != null)
                    {
                        var enemies = GameObjects.Enemy.Where(m => m.LSDistance(turret) < 775f && m.IsValidTarget());

                        if (!enemies.Any())
                        {
                            return Hero;
                        }
                    }
                }

                //Condemn To Wall Logic
                var condemnRectangle =
                    new SOLOPolygon(SOLOPolygon.Rectangle(targetPosition.To2D(), finalPosition.LSTo2D(), Hero.BoundingRadius));
                var condemnRectangle_ex =
                    new SOLOPolygon(SOLOPolygon.Rectangle(Hero.ServerPosition.To2D(), finalPosition_ex.LSTo2D(),
                        Hero.BoundingRadius));
                var condemnRectangle_3 =
                    new SOLOPolygon(SOLOPolygon.Rectangle(prediction.CastPosition.To2D(), finalPosition_3.LSTo2D(),
                        Hero.BoundingRadius));

                if (IsBothNearWall(Hero))
                {
                    return null;
                }

                if (
                    condemnRectangle.Points.Count(
                        point => NavMesh.GetCollisionFlags(point.X, point.Y).HasFlag(CollisionFlags.Wall)) >=
                    condemnRectangle.Points.Count()*(Accuracy/100f)
                    ||
                    condemnRectangle_ex.Points.Count(
                        point => NavMesh.GetCollisionFlags(point.X, point.Y).HasFlag(CollisionFlags.Wall)) >=
                    condemnRectangle_ex.Points.Count()*(Accuracy/100f)
                    ||
                    condemnRectangle_3.Points.Count(
                        point => NavMesh.GetCollisionFlags(point.X, point.Y).HasFlag(CollisionFlags.Wall)) >=
                    condemnRectangle_ex.Points.Count()*(Accuracy/100f))
                {
                    return Hero;
                }
            }
            return null;
        }

        /// <summary>
        ///     Determines whether whether or not both the players and the target are near a wall.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        private static bool IsBothNearWall(Obj_AI_Base target)
        {
            var positions =
                GetWallQPositions(target, 110).ToList().OrderBy(pos => pos.LSDistance(target.ServerPosition, true));
            var positions_ex =
                GetWallQPositions(ObjectManager.Player, 110)
                    .ToList()
                    .OrderBy(pos => pos.LSDistance(ObjectManager.Player.ServerPosition, true));

            if (positions.Any(p => p.LSIsWall()) && positions_ex.Any(p => p.LSIsWall()))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Gets the wall q positions (Sideways positions to the players).
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="Range">The range.</param>
        /// <returns></returns>
        private static Vector3[] GetWallQPositions(Obj_AI_Base player, float Range)
        {
            Vector3[] vList =
            {
                (player.ServerPosition.To2D() + Range*player.Direction.To2D()).To3D(),
                (player.ServerPosition.To2D() - Range*player.Direction.To2D()).To3D()
            };

            return vList;
        }

        /// <summary>
        ///     Determines whether the specified target is condemnable.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="fromPosition">From position.</param>
        /// <returns></returns>
        public bool IsCondemnable(Obj_AI_Base target, Vector3 fromPosition)
        {
            var pushDistance = 420f;
            var targetPosition = target.ServerPosition;
            for (var i = 0; i < pushDistance; i += 40)
            {
                var tempPos = targetPosition.LSExtend(fromPosition, -i);
                if (tempPos.LSIsWall())
                {
                    return true;
                }
            }
            return false;
        }
    }
}