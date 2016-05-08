/*
 Copyright 2015 - 2015 SPrediction
 Prediction.cs is part of SPrediction
 
 SPrediction is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.
 
 SPrediction is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 GNU General Public License for more details.
 
 You should have received a copy of the GNU General Public License
 along with SPrediction. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using Spell = LeagueSharp.Common.Spell;

namespace SPrediction
{
    /// <summary>
    ///     Spacebar Prediction class
    /// </summary>
    public static class Prediction
    {
        #region Internal Properties

        internal static bool blInitialized;

        #endregion

        #region Private Methods

        /// <summary>
        ///     Initialization assert
        /// </summary>
        internal static void AssertInitializationMode()
        {
            if (!blInitialized)
                throw new InvalidOperationException("Prediction is not initalized");
        }

        #endregion

        #region Structures for prediction inputs/results

        /// <summary>
        ///     Neccesary input structure for prediction calculations
        /// </summary>
        public struct Input
        {
            #region Public Properties

            public Obj_AI_Base Target;
            public float SpellDelay;
            public float SpellMissileSpeed;
            public float SpellWidth;
            public float SpellRange;
            public bool SpellCollisionable;
            public SkillshotType SpellSkillShotType;
            public List<Vector2> Path;
            public float AvgReactionTime;
            public float LastMovChangeTime;
            public float AvgPathLenght;
            public float LastAngleDiff;
            public Vector3 From;
            public Vector3 RangeCheckFrom;

            #endregion

            #region Constructors and Destructors

            public Input(Obj_AI_Base _target, Spell s)
            {
                Target = _target;
                SpellDelay = s.Delay;
                SpellMissileSpeed = s.Speed;
                SpellWidth = s.Width;
                SpellRange = s.Range;
                SpellCollisionable = s.Collision;
                SpellSkillShotType = s.Type;
                Path = Target.GetWaypoints();
                if (Target is AIHeroClient)
                {
                    var t = Target as AIHeroClient;
                    AvgReactionTime = t.AvgMovChangeTime();
                    LastMovChangeTime = t.LastMovChangeTime();
                    AvgPathLenght = t.AvgPathLenght();
                    LastAngleDiff = t.LastAngleDiff();
                }
                else
                {
                    AvgReactionTime = 0;
                    LastMovChangeTime = 0;
                    AvgPathLenght = 0;
                    LastAngleDiff = 0;
                }
                From = s.From;
                RangeCheckFrom = s.RangeCheckFrom;
            }

            public Input(Obj_AI_Base _target, float delay, float speed, float radius, float range, bool collision,
                SkillshotType type, Vector3 _from, Vector3 _rangeCheckFrom)
            {
                Target = _target;
                SpellDelay = delay;
                SpellMissileSpeed = speed;
                SpellWidth = radius;
                SpellRange = range;
                SpellCollisionable = collision;
                SpellSkillShotType = type;
                Path = Target.GetWaypoints();
                if (Target is AIHeroClient)
                {
                    var t = Target as AIHeroClient;
                    AvgReactionTime = t.AvgMovChangeTime();
                    LastMovChangeTime = t.LastMovChangeTime();
                    AvgPathLenght = t.AvgPathLenght();
                    LastAngleDiff = t.LastAngleDiff();
                }
                else
                {
                    AvgReactionTime = 0;
                    LastMovChangeTime = 0;
                    AvgPathLenght = 0;
                    LastAngleDiff = 0;
                }
                From = _from;
                RangeCheckFrom = _rangeCheckFrom;
            }

            #endregion
        }

        /// <summary>
        ///     structure for prediction results
        /// </summary>
        public struct Result
        {
            #region Public Properties

            public Input Input;
            public Obj_AI_Base Unit;
            public Vector2 CastPosition;
            public Vector2 UnitPosition;
            public HitChance HitChance;
            public Collision.Result CollisionResult;

            #endregion

            #region Constructors and Destructors

            public Result(Input inp, Obj_AI_Base unit, Vector2 castpos, Vector2 unitpos, HitChance hc,
                Collision.Result col)
            {
                Input = inp;
                Unit = unit;
                CastPosition = castpos;
                UnitPosition = unitpos;
                HitChance = hc;
                CollisionResult = col;
            }

            #endregion

            #region Internal Methods

            internal void Lock(bool checkDodge = true)
            {
                CollisionResult = Collision.GetCollisions(Input.From.LSTo2D(), CastPosition, Input.SpellRange,
                    Input.SpellWidth, Input.SpellDelay, Input.SpellMissileSpeed);
                CheckCollisions();
                CheckOutofRange(checkDodge);
            }

            #endregion

            #region Private Methods

            private void CheckCollisions()
            {
                if (Input.SpellCollisionable &&
                    (CollisionResult.Objects.HasFlag(Collision.Flags.Minions) ||
                     CollisionResult.Objects.HasFlag(Collision.Flags.YasuoWall)))
                    HitChance = HitChance.Collision;
            }

            private void CheckOutofRange(bool checkDodge)
            {
                if (Input.RangeCheckFrom.LSTo2D().LSDistance(CastPosition) >
                    Input.SpellRange - (checkDodge ? GetArrivalTime(Input.From.LSTo2D().LSDistance(CastPosition), Input.SpellDelay, Input.SpellMissileSpeed)*Unit.MoveSpeed*(100 - ConfigMenu.MaxRangeIgnore)/100f: 0))
                    HitChance = HitChance.OutOfRange;
            }

            #endregion
        }

        /// <summary>
        ///     structure for aoe prediction result
        /// </summary>
        public struct AoeResult
        {
            #region Public Properties

            public Vector2 CastPosition;
            public Collision.Result CollisionResult;
            public int HitCount;

            #endregion

            #region Constructors and Destructors

            public AoeResult(Vector2 castpos, Collision.Result col, int hc)
            {
                CastPosition = castpos;
                CollisionResult = col;
                HitCount = hc;
            }

            #endregion
        }

        #endregion

        #region Initializer Methods

        /// <summary>
        ///     Initializes Prediction Services
        /// </summary>
        public static void Initialize(Menu mainMenu, string prefMenuName = "SPRED")
        {
            if (blInitialized)
                throw new Exception("SPrediction Already Initialized");

            if (mainMenu == null)
                throw new NullReferenceException("Menu cannot be null!");

            PathTracker.Initialize();
            Collision.Initialize();
            StasisPrediction.Initialize();
            ConfigMenu.Initialize(prefMenuName);
            Drawings.Initialize();

            blInitialized = true;
        }

        public static void Initialize()
        {
            try
            {
                PathTracker.Initialize();
                Collision.Initialize();
                StasisPrediction.Initialize();
                ConfigMenu.Initialize();
                Drawings.Initialize();

                blInitialized = true;
            }
            catch
            {
                var m = MainMenu.AddMenu("SPrediction", "SPREDX");
                m.Add("PREDICTONLIST",
                    new Slider("Prediction Method (0 : SPrediction | 1 : EB/Common Prediction)", 0, 0, 1));
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     Gets Prediction result
        /// </summary>
        /// <param name="input">Neccesary inputs for prediction calculations</param>
        /// <returns>Prediction result as <see cref="Prediction.Result" /></returns>
        internal static Result GetPrediction(Input input)
        {
            return GetPrediction(input.Target, input.SpellWidth, input.SpellDelay, input.SpellMissileSpeed,
                input.SpellRange, input.SpellCollisionable, input.SpellSkillShotType, input.Path, input.AvgReactionTime,
                input.LastMovChangeTime, input.AvgPathLenght, input.LastAngleDiff, input.From.LSTo2D(),
                input.RangeCheckFrom.LSTo2D());
        }

        /// <summary>
        ///     Gets Prediction result
        /// </summary>
        /// <param name="target">Target for spell</param>
        /// <param name="width">Spell width</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="range">Spell range</param>
        /// <param name="collisionable">Spell collisionable</param>
        /// <param name="type">Spell skillshot type</param>
        /// <param name="from">Spell casted position</param>
        /// <returns>Prediction result as <see cref="Prediction.Result" /></returns>
        internal static Result GetPrediction(AIHeroClient target, float width, float delay, float missileSpeed,
            float range, bool collisionable, SkillshotType type)
        {
            return GetPrediction(target, width, delay, missileSpeed, range, collisionable, type, target.GetWaypoints(), target.AvgMovChangeTime(), target.LastMovChangeTime(), target.AvgPathLenght(), target.LastAngleDiff(), ObjectManager.Player.ServerPosition.LSTo2D(), ObjectManager.Player.ServerPosition.LSTo2D());
        }

        /// <summary>
        ///     Gets Prediction result
        /// </summary>
        /// <param name="target">Target for spell</param>
        /// <param name="width">Spell width</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="range">Spell range</param>
        /// <param name="collisionable">Spell collisionable</param>
        /// <param name="type">Spell skillshot type</param>
        /// <param name="path">Waypoints of target</param>
        /// <param name="avgt">Average reaction time (in ms)</param>
        /// <param name="movt">Passed time from last movement change (in ms)</param>
        /// <param name="avgp">Average Path Lenght</param>
        /// <param name="from">Spell casted position</param>
        /// <param name="rangeCheckFrom"></param>
        /// <returns>Prediction result as <see cref="Prediction.Result" /></returns>
        internal static Result GetPrediction(Obj_AI_Base target, float width, float delay, float missileSpeed,
            float range, bool collisionable, SkillshotType type, List<Vector2> path, float avgt, float movt, float avgp,
            float anglediff, Vector2 from, Vector2 rangeCheckFrom)
        {
            AssertInitializationMode();

            var result = new Result
            {
                Input = new Input(target, delay, missileSpeed, width, range, collisionable, type, @from.To3D2(),
                    rangeCheckFrom.To3D2()),
                Unit = target
            };

            try
            {
                if (type == SkillshotType.SkillshotCircle)
                    range += width;

                //to do: hook logic ? by storing average movement direction etc
                if (path.Count <= 1 && movt > 100 && (Environment.TickCount - PathTracker.EnemyInfo[target.NetworkId].LastAATick > 300 || !ConfigMenu.CheckAAWindUp)) //if target is not moving, easy to hit (and not aaing)
                {
                    result.HitChance = HitChance.VeryHigh;
                    result.CastPosition = target.ServerPosition.LSTo2D();
                    result.UnitPosition = result.CastPosition;
                    result.Lock();

                    return result;
                }

                if (target is AIHeroClient)
                {
                    if (((AIHeroClient) target).IsChannelingImportantSpell())
                    {
                        result.HitChance = HitChance.VeryHigh;
                        result.CastPosition = target.ServerPosition.LSTo2D();
                        result.UnitPosition = result.CastPosition;
                        result.Lock();

                        return result;
                    }

                    if (Environment.TickCount - PathTracker.EnemyInfo[target.NetworkId].LastAATick < 300 && ConfigMenu.CheckAAWindUp)
                    {
                        if (target.AttackCastDelay*1000 + PathTracker.EnemyInfo[target.NetworkId].AvgOrbwalkTime + avgt - width/2f/target.MoveSpeed >= GetArrivalTime(target.ServerPosition.LSTo2D().LSDistance(from), delay, missileSpeed))
                        {
                            result.HitChance = HitChance.High;
                            result.CastPosition = target.ServerPosition.LSTo2D();
                            result.UnitPosition = result.CastPosition;
                            result.Lock();

                            return result;
                        }
                    }

                    //to do: find a fuking logic
                    if (avgp < 400 && movt < 100 && path.PathLength() <= avgp)
                    {
                        result.HitChance = HitChance.High;
                        result.CastPosition = path.Last();
                        result.UnitPosition = result.CastPosition;
                        result.Lock();

                        return result;
                    }
                }

                if (target.LSIsDashing()) //if unit is dashing
                    return GetDashingPrediction(target, width, delay, missileSpeed, range, collisionable, type, from, rangeCheckFrom);

                if (Utility.IsImmobileTarget(target)) //if unit is immobile
                    return GetImmobilePrediction(target, width, delay, missileSpeed, range, collisionable, type, from, rangeCheckFrom);

                result = WaypointAnlysis(target, width, delay, missileSpeed, range, collisionable, type, path, avgt, movt, avgp, anglediff, from);

                var d = result.CastPosition.LSDistance(target.ServerPosition.LSTo2D());
                if (d >= (avgt - movt)*target.MoveSpeed && d >= avgp)
                    result.HitChance = HitChance.Medium;

                result.Lock();

                return result;
            }
            finally
            {
                //check if movement changed while prediction calculations
                if (!target.GetWaypoints().SequenceEqual(path))
                    result.HitChance = HitChance.Medium;
            }
        }

        /// <summary>
        ///     Gets Prediction result while unit is dashing
        /// </summary>
        /// <param name="target">Target for spell</param>
        /// <param name="width">Spell width</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="range">Spell range</param>
        /// <param name="collisionable">Spell collisionable</param>
        /// <param name="type">Spell skillshot type</param>
        /// <param name="from">Spell casted position</param>
        /// <returns></returns>
        internal static Result GetDashingPrediction(Obj_AI_Base target, float width, float delay, float missileSpeed,
            float range, bool collisionable, SkillshotType type, Vector2 from, Vector2 rangeCheckFrom)
        {
            var result = new Result
            {
                Input = new Input(target, delay, missileSpeed, width, range, collisionable, type, @from.To3D2(), rangeCheckFrom.To3D2()),
                Unit = target
            };

            if (target.LSIsDashing())
            {
                var dashInfo = target.LSGetDashInfo();
                if (dashInfo.IsBlink)
                {
                    result.HitChance = HitChance.Impossible;
                    result.CastPosition = dashInfo.EndPos;
                    return result;
                }

                result.CastPosition = GetFastUnitPosition(target, dashInfo.Path, delay, missileSpeed, from, dashInfo.Speed);
                result.HitChance = HitChance.Dashing;

                result.Lock(false);
            }
            else
            {
                result = GetPrediction(target, width, delay, missileSpeed, range, collisionable, type, target.GetWaypoints(), 0, 0, 0, 0, from, rangeCheckFrom);
                result.Lock(false);
            }
            return result;
        }

        /// <summary>
        ///     Gets Prediction result while unit is immobile
        /// </summary>
        /// <param name="target">Target for spell</param>
        /// <param name="width">Spell width</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="range">Spell range</param>
        /// <param name="collisionable">Spell collisionable</param>
        /// <param name="type">Spell skillshot type</param>
        /// <param name="from">Spell casted position</param>
        /// <returns></returns>
        internal static Result GetImmobilePrediction(Obj_AI_Base target, float width, float delay, float missileSpeed,
            float range, bool collisionable, SkillshotType type, Vector2 from, Vector2 rangeCheckFrom)
        {
            var result = new Result
            {
                Input = new Input(target, delay, missileSpeed, width, range, collisionable, type, @from.To3D2(),
                    rangeCheckFrom.To3D2()),
                Unit = target,
                CastPosition = target.ServerPosition.LSTo2D()
            };
            result.UnitPosition = result.CastPosition;

            //calculate spell arrival time
            var t = delay + Game.Ping/2000f;
            if (missileSpeed != 0)
                t += from.LSDistance(target.ServerPosition)/missileSpeed;

            if (type == SkillshotType.SkillshotCircle)
                t += width/target.MoveSpeed/2f;

            if (t >= Utility.LeftImmobileTime(target))
            {
                result.HitChance = HitChance.Immobile;
                result.Lock();

                return result;
            }

            if (target is AIHeroClient)
                result.HitChance = GetHitChance(t - Utility.LeftImmobileTime(target),
                    ((AIHeroClient) target).AvgMovChangeTime(), 0, 0, 0);
            else
                result.HitChance = HitChance.High;

            result.Lock();

            return result;
        }

        /// <summary>
        ///     Get HitChance
        /// </summary>
        /// <param name="t">Arrive time to target (in ms)</param>
        /// <param name="avgt">Average reaction time (in ms)</param>
        /// <param name="movt">Passed time from last movement change (in ms)</param>
        /// <param name="avgp">Average Path Lenght</param>
        /// <returns>HitChance</returns>
        internal static HitChance GetHitChance(float t, float avgt, float movt, float avgp, float anglediff)
        {
            if (avgp > 400)
            {
                if (movt > 50)
                {
                    if (avgt >= t*1.25f)
                    {
                        if (anglediff < 30)
                            return HitChance.VeryHigh;
                        return HitChance.High;
                    }
                    if (avgt - movt >= t)
                        return HitChance.Medium;
                    return HitChance.Low;
                }
                return HitChance.VeryHigh;
            }
            return HitChance.High;
        }

        /// <summary>
        ///     Gets spell arrival time to cast position
        /// </summary>
        /// <param name="distance">Distance from to to</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <returns></returns>
        internal static float GetArrivalTime(float distance, float delay, float missileSpeed = 0)
        {
            if (missileSpeed != 0)
                return distance/missileSpeed + delay;

            return delay;
        }

        /// <summary>
        ///     Calculates cast position with target's path
        /// </summary>
        /// <param name="target">Target for spell</param>
        /// <param name="width">Spell width</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="range">Spell range</param>
        /// <param name="collisionable">Spell collisionable</param>
        /// <param name="type">Spell skillshot type</param>
        /// <param name="path">Waypoints of target</param>
        /// <param name="avgt">Average reaction time (in ms)</param>
        /// <param name="movt">Passed time from last movement change (in ms)</param>
        /// <param name="avgp">Average Path Lenght</param>
        /// <param name="from">Spell casted position</param>
        /// <returns></returns>
        internal static Result WaypointAnlysis(Obj_AI_Base target, float width, float delay, float missileSpeed,
            float range, bool collisionable, SkillshotType type, List<Vector2> path, float avgt, float movt, float avgp,
            float anglediff, Vector2 from, float moveSpeed = 0, bool isDash = false)
        {
            if (moveSpeed == 0)
                moveSpeed = target.MoveSpeed;

            var result = new Result {Unit = target};

            var flyTimeMax = 0f;

            if (missileSpeed != 0) //skillshot with a missile
                flyTimeMax = range/missileSpeed;

            var tMin = delay + Game.Ping/2000f + ConfigMenu.SpellDelay/1000f;
            var tMax = flyTimeMax + delay + Game.Ping/1000f + ConfigMenu.SpellDelay/1000f;
            var pathTime = 0f;
            int[] pathBounds = {-1, -1};

            //find bounds
            for (var i = 0; i < path.Count - 1; i++)
            {
                var t = path[i + 1].LSDistance(path[i])/moveSpeed;

                if (pathTime <= tMin && pathTime + t >= tMin)
                    pathBounds[0] = i;
                if (pathTime <= tMax && pathTime + t >= tMax)
                    pathBounds[1] = i;

                if (pathBounds[0] != -1 && pathBounds[1] != -1)
                    break;

                pathTime += t;
            }

            //calculate cast & unit position
            if (pathBounds[0] != -1 && pathBounds[1] != -1)
            {
                for (var k = pathBounds[0]; k <= pathBounds[1]; k++)
                {
                    var direction = (path[k + 1] - path[k]).LSNormalized();
                    var distance = width;
                    var extender = target.BoundingRadius;

                    if (type == SkillshotType.SkillshotLine)
                        extender = width;

                    var steps = (int) Math.Floor(path[k].LSDistance(path[k + 1])/distance);
                    //split & anlyse current path
                    for (var i = 1; i < steps - 1; i++)
                    {
                        var pCenter = path[k] + direction*distance*i;
                        var pA = pCenter - direction*extender;
                        var pB = pCenter + direction*extender;

                        var flytime = missileSpeed != 0 ? from.LSDistance(pCenter)/missileSpeed : 0f;
                        var t = flytime + delay + Game.Ping/2000f + ConfigMenu.SpellDelay/1000f;

                        var currentPosition = target.ServerPosition.LSTo2D();

                        var arriveTimeA = currentPosition.LSDistance(pA)/moveSpeed;
                        var arriveTimeB = currentPosition.LSDistance(pB)/moveSpeed;

                        if (Math.Min(arriveTimeA, arriveTimeB) <= t && Math.Max(arriveTimeA, arriveTimeB) >= t)
                        {
                            result.HitChance = GetHitChance(t, avgt, movt, avgp, anglediff);
                            result.CastPosition = pCenter;
                            result.UnitPosition = pCenter;
                                //+ (direction * (t - Math.Min(arriveTimeA, arriveTimeB)) * moveSpeed);
                            return result;
                        }
                    }
                }
            }

            result.HitChance = HitChance.Impossible;
            result.CastPosition = target.ServerPosition.LSTo2D();

            return result;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets fast-predicted unit position
        /// </summary>
        /// <param name="target">Target</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="from">Spell casted position</param>
        /// <returns></returns>
        public static Vector2 GetFastUnitPosition(Obj_AI_Base target, float delay, float missileSpeed = 0,
            Vector2? from = null, float distanceSet = 0)
        {
            var path = target.GetWaypoints();
            if (from == null)
                from = ObjectManager.Player.ServerPosition.LSTo2D();

            if (path.Count <= 1 || (target is AIHeroClient && target.Spellbook.IsChanneling) || Utility.IsImmobileTarget(target))
                return target.ServerPosition.LSTo2D();

            if (target.LSIsDashing())
                return target.LSGetDashInfo().Path.Last();

            var distance = distanceSet;

            if (distance == 0)
            {
                var targetDistance = from.Value.LSDistance(target.ServerPosition);
                var flyTime = targetDistance/missileSpeed;

                if (missileSpeed != 0 && path.Count == 2)
                {
                    var Vt = (path[1] - path[0]).LSNormalized()*target.MoveSpeed;
                    var Vs = (target.ServerPosition.LSTo2D() - from.Value).LSNormalized()*missileSpeed;
                    var Vr = Vt - Vs;

                    flyTime = targetDistance/Vr.Length();
                }

                var t = flyTime + delay + Game.Ping/2000f;
                distance = t*target.MoveSpeed;
            }

            for (var i = 0; i < path.Count - 1; i++)
            {
                var d = path[i + 1].LSDistance(path[i]);
                if (distance == d)
                    return path[i + 1];
                if (distance < d)
                    return path[i] + distance*(path[i + 1] - path[i]).LSNormalized();
                distance -= d;
            }

            return path.Last();
        }

        /// <summary>
        ///     Gets fast-predicted unit position
        /// </summary>
        /// <param name="target">Target</param>
        /// <param name="path">Path</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="from">Spell casted position</param>
        /// <param name="moveSpeed">Move speed</param>
        /// <param name="distanceSet"></param>
        /// <returns></returns>
        public static Vector2 GetFastUnitPosition(Obj_AI_Base target, List<Vector2> path, float delay, float missileSpeed = 0, Vector2? from = null, float moveSpeed = 0, float distanceSet = 0)
        {
            if (from == null)
                from = target.ServerPosition.LSTo2D();

            if (moveSpeed == 0)
                moveSpeed = target.MoveSpeed;

            if (path.Count <= 1 || (target is AIHeroClient && ((AIHeroClient) target).IsChannelingImportantSpell()) || Utility.IsImmobileTarget(target))
                return target.ServerPosition.LSTo2D();

            if (target.LSIsDashing())
                return target.LSGetDashInfo().Path.Last();

            var distance = distanceSet;

            if (distance == 0)
            {
                var targetDistance = from.Value.LSDistance(target.ServerPosition);
                var flyTime = 0f;

                if (missileSpeed != 0) //skillshot with a missile
                {
                    var Vt = (path[path.Count - 1] - path[0]).LSNormalized()*moveSpeed;
                    var Vs = (target.ServerPosition.LSTo2D() - from.Value).LSNormalized()*missileSpeed;
                    var Vr = Vs - Vt;

                    flyTime = targetDistance/Vr.Length();

                    if (path.Count > 5) //complicated movement
                        flyTime = targetDistance/missileSpeed;
                }

                var t = flyTime + delay + Game.Ping/2000f + ConfigMenu.SpellDelay/1000f;
                distance = t*moveSpeed;
            }

            for (var i = 0; i < path.Count - 1; i++)
            {
                var d = path[i + 1].LSDistance(path[i]);
                if (distance == d)
                    return path[i + 1];
                if (distance < d)
                    return path[i] + distance*(path[i + 1] - path[i]).LSNormalized();
                distance -= d;
            }

            return path.Last();
        }

        #endregion
    }
}