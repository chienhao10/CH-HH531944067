using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using SharpDX;
using SoloVayne.Skills.Condemn;
using Vayne1;
using WardTracker;
using Geometry = LeagueSharp.Common.Geometry;
using WardType = WardTracker.WardType;

namespace SoloVayne.Skills.Tumble
{
    internal class TumbleLogicProvider
    {
        public static CondemnLogicProvider Provider = new CondemnLogicProvider();

        /// <summary>
        ///     Gets the SOLO Vayne Q position using a patented logic!
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetSOLOVayneQPosition()
        {
            #region The Required Variables

            var positions = TumbleHelper.GetRotatedQPositions();
            var enemyPositions = TumbleHelper.GetEnemyPoints();
            var safePositions = positions.Where(pos => !enemyPositions.Contains(pos.To2D())).ToList();
            var BestPosition = ObjectManager.Player.ServerPosition.LSExtend(Game.CursorPos, 300f);
            var AverageDistanceWeight = .60f;
            var ClosestDistanceWeight = .40f;

            var bestWeightedAvg = 0f;

            var highHealthEnemiesNear =
                HeroManager.Enemies.Where(m => !m.IsMelee && m.IsValidTarget(1300f) && m.HealthPercent > 7).ToList();

            var alliesNear = HeroManager.Allies.Count(ally => !ally.IsMe && ally.IsValidTarget(1500f));

            var enemiesNear =
                HeroManager.Enemies.Where(m => m.IsValidTarget(Orbwalking.GetRealAutoAttackRange(m) + 300f + 65f))
                    .ToList();

            #endregion

            #region 1 Enemy around only

            if (ObjectManager.Player.CountEnemiesInRange(1500f) <= 1)
            {
                //Logic for 1 enemy near
                var position = ObjectManager.Player.ServerPosition.LSExtend(Game.CursorPos, 300f);
                return position;
            }

            #endregion

            if (
                enemiesNear.Any(
                    t =>
                        t.Health + 15 <
                        ObjectManager.Player.LSGetAutoAttackDamage(t)*2 +
                        ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.Q) &&
                        t.LSDistance(ObjectManager.Player) < Orbwalking.GetRealAutoAttackRange(t) + 80f))
            {
                var QPosition =
                    ObjectManager.Player.ServerPosition.LSExtend(
                        enemiesNear.OrderBy(t => t.Health).First().ServerPosition, 300f);

                if (!LeagueSharp.Common.Utility.UnderTurret(QPosition, true))
                {
                    return QPosition;
                }
            }

            #region Alone, 2 Enemies, 1 Killable

            if (enemiesNear.Count() <= 2)
            {
                if (
                    enemiesNear.Any(
                        t =>
                            t.Health + 15 <
                            ObjectManager.Player.LSGetAutoAttackDamage(t)*2 +
                            ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.Q) &&
                            t.LSDistance(ObjectManager.Player) < Orbwalking.GetRealAutoAttackRange(t) + 80f))
                {
                    var QPosition =
                        ObjectManager.Player.ServerPosition.LSExtend(
                            highHealthEnemiesNear.OrderBy(t => t.Health).First().ServerPosition, 300f);

                    if (!LeagueSharp.Common.Utility.UnderTurret(QPosition, true))
                    {
                        return QPosition;
                    }
                }
            }

            #endregion

            #region Alone, 2 Enemies, None Killable

            if (alliesNear == 0 && highHealthEnemiesNear.Count() <= 2)
            {
                //Logic for 2 enemies Near and alone

                //If there is a killable enemy among those. 
                var backwardsPosition =
                    (ObjectManager.Player.ServerPosition.To2D() + 300f*ObjectManager.Player.Direction.To2D()).To3D();

                if (!LeagueSharp.Common.Utility.UnderTurret(backwardsPosition, true))
                {
                    return backwardsPosition;
                }
            }

            #endregion

            #region Already in an enemy's attack range. 

            var closeNonMeleeEnemy =
                TumbleHelper.GetClosestEnemy(ObjectManager.Player.ServerPosition.LSExtend(Game.CursorPos, 300f));

            if (closeNonMeleeEnemy != null
                && ObjectManager.Player.LSDistance(closeNonMeleeEnemy) <= closeNonMeleeEnemy.AttackRange - 85
                && !closeNonMeleeEnemy.IsMelee)
            {
                return ObjectManager.Player.ServerPosition.LSExtend(Game.CursorPos, 300f).IsSafeEx()
                    ? ObjectManager.Player.ServerPosition.LSExtend(Game.CursorPos, 300f)
                    : Vector3.Zero;
            }

            #endregion

            #region Logic for multiple enemies / allies around.

            foreach (var position in safePositions)
            {
                var enemy = TumbleHelper.GetClosestEnemy(position);
                if (!enemy.LSIsValidTarget())
                {
                    continue;
                }

                var avgDist = TumbleHelper.GetAvgDistance(position);

                if (avgDist > -1)
                {
                    var closestDist = ObjectManager.Player.ServerPosition.LSDistance(enemy.ServerPosition);
                    var weightedAvg = closestDist*ClosestDistanceWeight + avgDist*AverageDistanceWeight;
                    if (weightedAvg > bestWeightedAvg && position.IsSafeEx())
                    {
                        bestWeightedAvg = weightedAvg;
                        BestPosition = position;
                    }
                }
            }

            #endregion

            var endPosition = BestPosition.IsSafe() ? BestPosition : Game.CursorPos;

            #region Couldn't find a suitable position, tumble to nearest ally logic

            if (endPosition == Vector3.Zero)
            {
                //Try to find another suitable position. This usually means we are already near too much enemies turrets so just gtfo and tumble
                //to the closest ally ordered by most health.
                var alliesClose =
                    HeroManager.Allies.Where(ally => !ally.IsMe && ally.IsValidTarget(1500f)).ToList();
                if (alliesClose.Any() && enemiesNear.Any())
                {
                    var closestMostHealth =
                        alliesClose.OrderBy(m => m.LSDistance(ObjectManager.Player))
                            .ThenByDescending(m => m.Health)
                            .FirstOrDefault();

                    if (closestMostHealth != null
                        &&
                        closestMostHealth.LSDistance(
                            enemiesNear.OrderBy(m => m.LSDistance(ObjectManager.Player)).FirstOrDefault())
                        >
                        ObjectManager.Player.LSDistance(
                            enemiesNear.OrderBy(m => m.LSDistance(ObjectManager.Player)).FirstOrDefault()))
                    {
                        var tempPosition =
                            ObjectManager.Player.ServerPosition.LSExtend(closestMostHealth.ServerPosition, 300f);
                        if (tempPosition.IsSafeEx())
                        {
                            endPosition = tempPosition;
                        }
                    }
                }
            }

            #endregion

            #region Couldn't find an ally, tumble inside bush

            var AmInBush = NavMesh.IsWallOfGrass(ObjectManager.Player.ServerPosition, 33);
            var closeEnemies = TumbleVariables.EnemiesClose.ToList();
            if (!AmInBush && endPosition == Vector3.Zero)
            {
                var PositionsComplete = TumbleHelper.GetCompleteRotatedQPositions();
                foreach (var position in PositionsComplete)
                {
                    if (NavMesh.IsWallOfGrass(position, 33)
                        &&
                        closeEnemies.All(
                            m => m.LSDistance(position) > 340f && !NavMesh.IsWallOfGrass(m.ServerPosition, 40))
                        &&
                        !WardTrackerVariables.detectedWards.Any(
                            m =>
                                NavMesh.IsWallOfGrass(m.Position, 33) &&
                                m.Position.LSDistance(position) < m.WardTypeW.WardVisionRange &&
                                !(m.WardTypeW.WardType == WardType.ShacoBox ||
                                  m.WardTypeW.WardType == WardType.TeemoShroom)))
                    {
                        if (position.IsSafe())
                        {
                            endPosition = position;
                            break;
                        }
                    }
                }
            }

            #endregion

            #region Couldn't even tumble to ally, just go to mouse

            if (endPosition == Vector3.Zero)
            {
                var mousePosition = ObjectManager.Player.ServerPosition.LSExtend(Game.CursorPos, 300f);
                if (mousePosition.IsSafe())
                {
                    endPosition = mousePosition;
                }
            }

            #endregion

            if (ObjectManager.Player.HealthPercent < 10 && ObjectManager.Player.CountEnemiesInRange(1500) > 1)
            {
                var position = ObjectManager.Player.ServerPosition.LSExtend(Game.CursorPos, 300f);
                return position.IsSafeEx() ? position : endPosition;
            }

            return endPosition;
        }

        /// <summary>
        ///     Gets the QE position for the Tumble-Condemn combo.
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetQEPosition()
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || !Program.E.IsReady())
            {
                return Vector3.Zero;
            }

            var direction = ObjectManager.Player.Direction.To2D().LSPerpendicular();
            for (var i = 0f; i < 360f; i += 45)
            {
                var angleRad = Geometry.DegreeToRadian(i);
                var rotatedPosition = ObjectManager.Player.Position.To2D() + 300f*direction.LSRotated(angleRad);
                if (Provider.GetTarget(rotatedPosition.To3D()).IsValidTarget() && rotatedPosition.To3D().IsSafe())
                {
                    return rotatedPosition.To3D();
                }
            }

            return Vector3.Zero;
        }
    }
}