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
using LeagueSharp.Common;
using WardTracker;
using SoloVayne.Skills.Condemn;

namespace SoloVayne.Skills.Tumble
{
    class TumbleLogicProvider
    {
        public static CondemnLogicProvider Provider = new CondemnLogicProvider();

        /// <summary>
        /// Gets the SOLO Vayne Q position using a patented logic!
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetSOLOVayneQPosition()
        {
            #region The Required Variables
            var positions = TumbleHelper.GetRotatedQPositions();
            var enemyPositions = TumbleHelper.GetEnemyPoints();
            var safePositions = positions.Where(pos => !enemyPositions.Contains(pos.To2D())).ToList();
            var BestPosition = ObjectManager.Player.ServerPosition.Extend(Game.CursorPos, 300f);
            var AverageDistanceWeight = .60f;
            var ClosestDistanceWeight = .40f;

            var bestWeightedAvg = 0f;

            var highHealthEnemiesNear = HeroManager.Enemies.Where(m => !m.IsMelee && m.IsValidTarget(1300f) && m.HealthPercent > 7).ToList();

            var alliesNear = HeroManager.Allies.Count(ally => !ally.IsMe && ally.IsValidTarget(1500f, false));

            var enemiesNear = HeroManager.Enemies.Where(m => m.IsValidTarget(Orbwalking.GetRealAutoAttackRange(m) + 300f + 65f)).ToList();
            #endregion


            #region 1 Enemy around only
            if (ObjectManager.Player.CountEnemiesInRange(1500f) <= 1)
            {
                //Logic for 1 enemy near
                var position = ObjectManager.Player.ServerPosition.Extend(Game.CursorPos, 300f).To3D();
                return position;
            }
            #endregion

            if (enemiesNear.Any(t => t.Health + 15 < ObjectManager.Player.LSGetAutoAttackDamage(t) * 2 + ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.Q) && t.Distance(ObjectManager.Player) < Orbwalking.GetRealAutoAttackRange(t) + 80f))
            {
                var QPosition = ObjectManager.Player.ServerPosition.LSExtend(enemiesNear.OrderBy(t => t.Health).First().ServerPosition, 300f);

                if (!QPosition.UnderTurret(true))
                {
                    return QPosition;
                }
            }

            #region Alone, 2 Enemies, 1 Killable
            if (enemiesNear.Count() <= 2)
            {
                if (enemiesNear.Any(t => t.Health + 15 < ObjectManager.Player.LSGetAutoAttackDamage(t) * 2 + ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.Q) && t.Distance(ObjectManager.Player) < Orbwalking.GetRealAutoAttackRange(t) + 80f))
                {
                    var QPosition = ObjectManager.Player.ServerPosition.Extend(highHealthEnemiesNear.OrderBy(t => t.Health).First().ServerPosition, 300f).To3D();

                    if (!QPosition.UnderTurret(true))
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
                var backwardsPosition = (ObjectManager.Player.ServerPosition.To2D() + 300f * ObjectManager.Player.Direction.To2D()).To3D();

                if (!backwardsPosition.UnderTurret(true))
                {
                    return backwardsPosition;
                }
            }
            #endregion

            #region Already in an enemy's attack range. 
            var closeNonMeleeEnemy = TumbleHelper.GetClosestEnemy(ObjectManager.Player.ServerPosition.LSExtend(Game.CursorPos, 300f));

            if (closeNonMeleeEnemy != null 
                && ObjectManager.Player.Distance(closeNonMeleeEnemy) <= closeNonMeleeEnemy.AttackRange - 85 
                && !closeNonMeleeEnemy.IsMelee)
            {
                return ObjectManager.Player.ServerPosition.LSExtend(Game.CursorPos, 300f).IsSafeEx() ? ObjectManager.Player.ServerPosition.LSExtend(Game.CursorPos, 300f) : Vector3.Zero;
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
                    var weightedAvg = closestDist * ClosestDistanceWeight + avgDist * AverageDistanceWeight;
                    if (weightedAvg > bestWeightedAvg && position.IsSafeEx())
                    {
                        bestWeightedAvg = weightedAvg;
                        BestPosition = position.To2D();
                    }
                }
            }
            #endregion

            var endPosition = ((BestPosition).To3D().IsSafe()) ? BestPosition.To3D() : Vector3.Zero;

            #region Couldn't find a suitable position, tumble to nearest ally logic
            if (endPosition == Vector3.Zero)
            {
                //Try to find another suitable position. This usually means we are already near too much enemies turrets so just gtfo and tumble
                //to the closest ally ordered by most health.
                var alliesClose = HeroManager.Allies.Where(ally => !ally.IsMe && ally.IsValidTarget(1500f, false)).ToList();
                if (alliesClose.Any() && enemiesNear.Any())
                {
                    var closestMostHealth =
                    alliesClose.OrderBy(m => m.Distance(ObjectManager.Player)).ThenByDescending(m => m.Health).FirstOrDefault();

                    if (closestMostHealth != null 
                        && closestMostHealth.Distance(enemiesNear.OrderBy(m => m.Distance(ObjectManager.Player)).FirstOrDefault()) 
                        > ObjectManager.Player.Distance(enemiesNear.OrderBy(m => m.Distance(ObjectManager.Player)).FirstOrDefault()))
                    {
                        var tempPosition = ObjectManager.Player.ServerPosition.Extend(closestMostHealth.ServerPosition, 300f).To3D();
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
                        && closeEnemies.All(m => m.Distance(position) > 340f && !NavMesh.IsWallOfGrass(m.ServerPosition, 40))
                        && !WardTrackerVariables.detectedWards.Any(m => NavMesh.IsWallOfGrass(m.Position, 33) && m.Position.Distance(position) < m.WardTypeW.WardVisionRange && !(m.WardTypeW.WardType == WardTracker.WardType.ShacoBox || m.WardTypeW.WardType == WardTracker.WardType.TeemoShroom)))
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
                var mousePosition = ObjectManager.Player.ServerPosition.Extend(Game.CursorPos, 300f).To3D();
                if (mousePosition.IsSafe())
                {
                    endPosition = mousePosition;
                }
            }
            #endregion

            if (ObjectManager.Player.HealthPercent < 10 && ObjectManager.Player.CountEnemiesInRange(1500) > 1)
            {
                var position = ObjectManager.Player.ServerPosition.Extend(Game.CursorPos, 300f).To3D();
                return position.IsSafeEx() ? position : endPosition;
            }

            return endPosition;
        }

        /// <summary>
        /// Gets the QE position for the Tumble-Condemn combo.
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetQEPosition()
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || !Vayne.Program.E.IsReady())
            {
                return Vector3.Zero;
            }

            const int currentStep = 45;
            var direction = ObjectManager.Player.Direction.To2D().LSPerpendicular();
            for (var i = 0f; i < 360f; i += 45)
            {
                var angleRad = LeagueSharp.Common.Geometry.DegreeToRadian(i);
                var rotatedPosition = ObjectManager.Player.Position.To2D() + (300f * direction.LSRotated(angleRad));
                if (Provider.GetTarget(rotatedPosition.To3D()).IsValidTarget() && rotatedPosition.To3D().IsSafe())
                {
                    return rotatedPosition.To3D();
                }
            }

            return Vector3.Zero;
        }
    }
}
