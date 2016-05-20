// Copyright 2014 - 2015 Esk0r
// Collision.cs is part of Evade.
// 
// Evade is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Evade is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Evade. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;

namespace GosuMechanicsYasuo
{
    public enum CollisionObjectTypes
    {
        Minion,
        Champion,
        YasuoWall
    }

    public class FastPredictionResult
    {
        public Vector2 CurrentPosVector2;
        public bool IsMoving;
        public Vector2 PredictedPosVector2;
    }

    public class DetectedCollision
    {
        public float Difference;
        public float Distance;
        public Vector2 PositionVector2;
        public CollisionObjectTypes Type;
        public Obj_AI_Base UnitAiBase;
    }

    public static class Collision
    {
        private static int _wallCastTick;
        private static Vector2 _yasuoWallVector2;

        public static void Init()
        {
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsValid || sender.Team != GameObjectTeam.Neutral || !args.SData.Name.Equals("YasuoWMovingWall"))
            {
                return;
            }

            _wallCastTick = Environment.TickCount;
            _yasuoWallVector2 = sender.ServerPosition.LSTo2D();
        }

        public static FastPredictionResult FastPrediction(Vector2 fromVector2,
            Obj_AI_Base unitAiBase,
            int delay,
            int speed)
        {
            var tickDelay = delay / 1000f + (fromVector2.LSDistance(unitAiBase) / speed);
            var moveSpeedF = tickDelay * unitAiBase.MoveSpeed;
            var path = unitAiBase.GetWaypoints();

            if (path.PathLength() > moveSpeedF)
            {
                return new FastPredictionResult
                {
                    IsMoving = true,
                    CurrentPosVector2 = unitAiBase.ServerPosition.LSTo2D(),
                    PredictedPosVector2 = path.CutPath((int)moveSpeedF)[0]
                };
            }

            if (path.Count == 0)
            {
                return new FastPredictionResult
                {
                    IsMoving = false,
                    CurrentPosVector2 = unitAiBase.ServerPosition.LSTo2D(),
                    PredictedPosVector2 = unitAiBase.ServerPosition.LSTo2D()
                };
            }

            return new FastPredictionResult
            {
                IsMoving = false,
                CurrentPosVector2 = path[path.Count - 1],
                PredictedPosVector2 = path[path.Count - 1]
            };
        }

        public static Vector2 GetCollisionPoint(Skillshot skillshot)
        {
            var collisions = new List<DetectedCollision>();
            var from = skillshot.GetMissilePosition(0);
            skillshot.ForceDisabled = false;
            foreach (var cObject in skillshot.SpellData.CollisionObjects)
            {
                switch (cObject)
                {
                    case CollisionObjectTypes.Minion:

                        collisions.AddRange(
                            from minion in
                                MinionManager.GetMinions(
                                    @from.To3D(), 1200, MinionTypes.All,
                                    skillshot.Unit.Team == ObjectManager.Player.Team
                                        ? MinionTeam.NotAlly
                                        : MinionTeam.NotAllyForEnemy)
                            let pred =
                                FastPrediction(
                                    @from, minion,
                                    Math.Max(
                                        0, skillshot.SpellData.Delay - (Environment.TickCount - skillshot.StartTick)),
                                    skillshot.SpellData.MissileSpeed)
                            let pos = pred.PredictedPosVector2
                            let w =
                                skillshot.SpellData.RawRadius + (!pred.IsMoving ? (minion.BoundingRadius - 15) : 0) -
                                pos.LSDistance(@from, skillshot.End, true)
                            where w > 0
                            select
                                new DetectedCollision
                                {
                                    PositionVector2 =
                                        pos.LSProjectOn(skillshot.End, skillshot.Start).LinePoint +
                                        skillshot.Direction * 30,
                                    UnitAiBase = minion,
                                    Type = CollisionObjectTypes.Minion,
                                    Distance = pos.LSDistance(@from),
                                    Difference = w
                                });

                        break;

                    case CollisionObjectTypes.Champion:
                        collisions.AddRange(
                            from hero in
                                ObjectManager.Get<AIHeroClient>()
                                    .Where(
                                        h =>
                                            (h.LSIsValidTarget(1200, false) && h.Team == ObjectManager.Player.Team &&
                                             !h.IsMe || h.Team != ObjectManager.Player.Team))
                            let pred =
                                FastPrediction(
                                    @from, hero,
                                    Math.Max(
                                        0, skillshot.SpellData.Delay - (Environment.TickCount - skillshot.StartTick)),
                                    skillshot.SpellData.MissileSpeed)
                            let pos = pred.PredictedPosVector2
                            let w = skillshot.SpellData.RawRadius + 30 - pos.LSDistance(@from, skillshot.End, true)
                            where w > 0
                            select
                                new DetectedCollision
                                {
                                    PositionVector2 =
                                        pos.LSProjectOn(skillshot.End, skillshot.Start).LinePoint +
                                        skillshot.Direction * 30,
                                    UnitAiBase = hero,
                                    Type = CollisionObjectTypes.Minion,
                                    Distance = pos.LSDistance(@from),
                                    Difference = w
                                });
                        break;

                    case CollisionObjectTypes.YasuoWall:
                        if (
                            !ObjectManager.Get<AIHeroClient>()
                                .Any(
                                    hero =>
                                        hero.LSIsValidTarget(float.MaxValue, false) &&
                                        hero.Team == ObjectManager.Player.Team && hero.ChampionName == "Yasuo"))
                        {
                            break;
                        }
                        GameObject wall = null;
                        foreach (
                            var gameObject in
                                ObjectManager.Get<GameObject>()
                                    .Where(
                                        gameObject =>
                                            gameObject.IsValid &&
                                            Regex.IsMatch(
                                                gameObject.Name, "_w_windwall.\\.troy", RegexOptions.IgnoreCase)))
                        {
                            wall = gameObject;
                        }
                        if (wall == null)
                        {
                            break;
                        }
                        var level = wall.Name.Substring(wall.Name.Length - 6, 1);
                        var wallWidth = (300 + 50 * Convert.ToInt32(level));


                        var wallDirection = (wall.Position.LSTo2D() - _yasuoWallVector2).LSNormalized().LSPerpendicular();
                        var fraction = wallWidth / 0x2; // 0x2 = 2
                        var wallStart = wall.Position.LSTo2D() + fraction * wallDirection;
                        var wallEnd = wallStart - wallWidth * wallDirection;
                        var wallPolygon = new Geometry.Rectangle(wallStart, wallEnd, 75).ToPolygon();
                        var intersections = new List<Vector2>();

                        for (var i = 0; i < wallPolygon.Points.Count; i++)
                        {
                            var inter =
                                wallPolygon.Points[i].LSIntersection(
                                    wallPolygon.Points[i != wallPolygon.Points.Count - 1 ? i + 1 : 0], from,
                                    skillshot.End);
                            if (inter.Intersects)
                            {
                                intersections.Add(inter.Point);
                            }
                        }

                        if (intersections.Count > 0)
                        {
                            var intersection = intersections.OrderBy(item => item.LSDistance(from)).ToList()[0];
                            var collisionT = Environment.TickCount +
                                             Math.Max(
                                                 0,
                                                 skillshot.SpellData.Delay -
                                                 (Environment.TickCount - skillshot.StartTick)) + 100 +
                                             (1000 * intersection.LSDistance(from)) / skillshot.SpellData.MissileSpeed;
                            if (collisionT - _wallCastTick < 4000)
                            {
                                if (skillshot.SpellData.Type != SkillShotType.SkillshotMissileLine)
                                {
                                    skillshot.ForceDisabled = true;
                                }
                                return intersection;
                            }
                        }

                        break;
                }
            }

            return collisions.Count > 0
                ? collisions.OrderBy(c => c.Distance).ToList()[0].PositionVector2
                : new Vector2();
        }
    }
}