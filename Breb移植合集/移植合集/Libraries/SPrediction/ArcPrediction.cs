/*
 Copyright 2015 - 2015 SPrediction
 ArcPrediction.cs is part of SPrediction
 
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
//using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using LeagueSharp.Common;
using SharpDX;

namespace SPrediction
{
    /// <summary>
    ///     Arc Prediction class
    /// </summary>
    public static class ArcPrediction
    {
        /// <summary>
        ///     Gets Prediction result
        /// </summary>
        /// <param name="input">Neccesary inputs for prediction calculations</param>
        /// <returns>Prediction result as <see cref="Prediction.Result" /></returns>
        public static Prediction.Result GetPrediction(Prediction.Input input)
        {
            return GetPrediction(input.Target, input.SpellWidth, input.SpellDelay, input.SpellMissileSpeed,
                input.SpellRange, input.SpellCollisionable, input.Path, input.AvgReactionTime, input.LastMovChangeTime,
                input.AvgPathLenght, input.LastAngleDiff, input.From.LSTo2D(), input.RangeCheckFrom.LSTo2D());
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
        public static Prediction.Result GetPrediction(AIHeroClient target, float width, float delay, float missileSpeed,
            float range, bool collisionable)
        {
            return GetPrediction(target, width, delay, missileSpeed, range, collisionable, target.GetWaypoints(),
                target.AvgMovChangeTime(), target.LastMovChangeTime(), target.AvgPathLenght(), target.LastAngleDiff(),
                ObjectManager.Player.ServerPosition.LSTo2D(), ObjectManager.Player.ServerPosition.LSTo2D());
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
        public static Prediction.Result GetPrediction(Obj_AI_Base target, float width, float delay, float missileSpeed,
            float range, bool collisionable, List<Vector2> path, float avgt, float movt, float avgp, float anglediff,
            Vector2 from, Vector2 rangeCheckFrom, bool arconly = true)
        {
            Prediction.AssertInitializationMode();

            if (arconly)
            {
                if (target.LSDistance(from) < width || target.LSDistance(from) > range*0.75f)
                    return CirclePrediction.GetPrediction(target, width, delay, missileSpeed, range, collisionable, path,
                        avgt, movt, avgp, anglediff, from, rangeCheckFrom);

                var pred = LinePrediction.GetPrediction(target, 80f, delay, missileSpeed, range, collisionable, path,
                    avgt, movt, avgp, anglediff, from, rangeCheckFrom);
                if (pred.HitChance >= HitChance.Low)
                {
                    pred.CastPosition = @from + (pred.CastPosition - @from).LSNormalized()*range
                        /*.RotateAroundPoint(from, (1 - pred.UnitPosition.LSDistance(ObjectManager.Player.ServerPosition.LSTo2D()) / 820f) * (float)Math.PI / 2f)*/;
                    var cos = (float) Math.Cos((1 - pred.UnitPosition.LSDistance(from)/820f)*Math.PI/2);
                    var sin = (float) Math.Sin((1 - pred.UnitPosition.LSDistance(from)/820f)*Math.PI/2);
                    var x = cos*(pred.CastPosition.X - from.X) - sin*(pred.CastPosition.Y - from.Y) + from.X;
                    var y = sin*(pred.CastPosition.X - from.X) + cos*(pred.CastPosition.Y - from.Y) + from.Y;
                    pred.CastPosition = new Vector2(x, y);
                }

                return pred;
            }
            var result = new Prediction.Result();

            if (path.Count <= 1) //if target is not moving, easy to hit
            {
                result.HitChance = HitChance.Immobile;
                result.CastPosition = target.ServerPosition.LSTo2D();
                result.UnitPosition = result.CastPosition;
                return result;
            }

            if (target is AIHeroClient && ((AIHeroClient) target).IsChannelingImportantSpell())
            {
                result.HitChance = HitChance.Immobile;
                result.CastPosition = target.ServerPosition.LSTo2D();
                result.UnitPosition = result.CastPosition;
                return result;
            }

            if (Utility.IsImmobileTarget(target))
                return Prediction.GetImmobilePrediction(target, width, delay, missileSpeed, range, collisionable,
                    SkillshotType.SkillshotCircle, @from, rangeCheckFrom);

            if (target.IsDashing())
                return Prediction.GetDashingPrediction(target, width, delay, missileSpeed, range, collisionable,
                    SkillshotType.SkillshotCircle, @from, rangeCheckFrom);

            var targetDistance = rangeCheckFrom.LSDistance(target.ServerPosition);
            var flyTime = 0f;

            if (missileSpeed != 0)
            {
                var Vt = (path[path.Count - 1] - path[0]).LSNormalized()*target.MoveSpeed;
                var Vs = (target.ServerPosition.LSTo2D() - rangeCheckFrom).LSNormalized()*missileSpeed;
                var Vr = Vs - Vt;

                flyTime = targetDistance/Vr.Length();

                if (path.Count > 5)
                    flyTime = targetDistance/missileSpeed;
            }

            var t = flyTime + delay + Game.Ping/2000f + ConfigMenu.SpellDelay/1000f;

            result.HitChance = Prediction.GetHitChance(t*1000f, avgt, movt, avgp, anglediff);

            #region arc collision test

            if (result.HitChance > HitChance.Low)
            {
                for (var i = 1; i < path.Count; i++)
                {
                    var senderPos = rangeCheckFrom;
                    var testPos = path[i];

                    var multp = testPos.LSDistance(senderPos)/875.0f;

                    var dianaArc = new Geometry.Polygon(
                        ClipperWrapper.DefineArc(senderPos - new Vector2(875/2f, 20), testPos, (float) Math.PI*multp,
                            410, 200*multp),
                        ClipperWrapper.DefineArc(senderPos - new Vector2(875/2f, 20), testPos, (float) Math.PI*multp,
                            410, 320*multp));

                    if (!dianaArc.IsOutside(target.ServerPosition.LSTo2D()))
                    {
                        result.HitChance = HitChance.VeryHigh;
                        result.CastPosition = testPos;
                        result.UnitPosition = testPos;
                        return result;
                    }
                }
            }

            #endregion

            return CirclePrediction.GetPrediction(target, width, delay, missileSpeed, range, collisionable, path, avgt,
                movt, avgp, anglediff, @from, rangeCheckFrom);
        }

        /// <summary>
        ///     Gets Aoe Prediction result
        /// </summary>
        /// <param name="width">Spell width</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="range">Spell range</param>
        /// <param name="from">Spell casted position</param>
        /// <param name="rangeCheckFrom"></param>
        /// <returns>Prediction result as <see cref="Prediction.AoeResult" /></returns>
        public static Prediction.AoeResult GetAoePrediction(float width, float delay, float missileSpeed, float range,
            Vector2 from, Vector2 rangeCheckFrom)
        {
            var result = new Prediction.AoeResult();
            var enemies =
                HeroManager.Enemies.Where(
                    p =>
                        p.LSIsValidTarget() &&
                        Prediction.GetFastUnitPosition(p, delay, 0, from).LSDistance(rangeCheckFrom) < range);

            foreach (var enemy in enemies)
            {
                var prediction = GetPrediction(enemy, width, delay, missileSpeed, range, false, enemy.GetWaypoints(),
                    enemy.AvgMovChangeTime(), enemy.LastMovChangeTime(), enemy.AvgPathLenght(), enemy.LastAngleDiff(),
                    from, rangeCheckFrom);
                if (prediction.HitChance > HitChance.Medium)
                {
                    var multp = result.CastPosition.LSDistance(@from)/875.0f;

                    var spellHitBox = new Geometry.Polygon(
                        ClipperWrapper.DefineArc(from - new Vector2(875/2f, 20), result.CastPosition,
                            (float) Math.PI*multp, 410, 200*multp),
                        ClipperWrapper.DefineArc(from - new Vector2(875/2f, 20), result.CastPosition,
                            (float) Math.PI*multp, 410, 320*multp));

                    var collidedEnemies =
                        HeroManager.Enemies.AsParallel()
                            .Where(
                                p =>
                                    ClipperWrapper.IsIntersects(
                                        ClipperWrapper.MakePaths(
                                            ClipperWrapper.DefineCircle(
                                                Prediction.GetFastUnitPosition(p, delay, missileSpeed), p.BoundingRadius)),
                                        ClipperWrapper.MakePaths(spellHitBox)));
                    var collisionCount = collidedEnemies.Count();
                    if (collisionCount > result.HitCount)
                        result = prediction.ToAoeResult(collisionCount,
                            new Collision.Result(collidedEnemies.ToList<Obj_AI_Base>(), Collision.Flags.EnemyChampions));
                }
            }

            return result;
        }
    }
}