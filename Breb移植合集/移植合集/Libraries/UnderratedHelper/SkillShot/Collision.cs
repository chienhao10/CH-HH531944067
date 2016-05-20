#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using SharpDX;

#endregion

namespace UnderratedAIO.Helpers.SkillShot
{
    public enum CollisionObjectTypes
    {
        Minion,
        Champions,
        YasuoWall,
        Null
    }

    internal class FastPredResult
    {
        public Vector2 CurrentPos;
        public bool IsMoving;
        public Vector2 PredictedPos;
    }

    internal class DetectedCollision
    {
        public float Diff;
        public float Distance;
        public Vector2 Position;
        public CollisionObjectTypes Type;
        public Obj_AI_Base Unit;
    }

    internal static class Collision
    {
        private static int _wallCastT;
        private static Vector2 _yasuoWallCastedPos;

        public static void Init()
        {
            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
        }


        private static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsValid && sender.Team == ObjectManager.Player.Team && args.SData.Name == "YasuoWMovingWall")
            {
                _wallCastT = System.Environment.TickCount;
                _yasuoWallCastedPos = sender.ServerPosition.To2D();
            }
        }

        public static FastPredResult FastPrediction(Vector2 from, Obj_AI_Base unit, int delay, int speed)
        {
            var tDelay = delay/1000f + @from.LSDistance(unit)/speed;
            var d = tDelay*unit.MoveSpeed;
            var path = unit.GetWaypoints();

            if (path.PathLength() > d)
            {
                return new FastPredResult
                {
                    IsMoving = true,
                    CurrentPos = unit.ServerPosition.To2D(),
                    PredictedPos = path.CutPath((int) d)[0]
                };
            }
            if (path.Count == 0)
            {
                return new FastPredResult
                {
                    IsMoving = false,
                    CurrentPos = unit.ServerPosition.To2D(),
                    PredictedPos = unit.ServerPosition.To2D()
                };
            }
            return new FastPredResult
            {
                IsMoving = false,
                CurrentPos = path[path.Count - 1],
                PredictedPos = path[path.Count - 1]
            };
        }

        public static Vector2 GetCollisionPoint(Skillshot skillshot)
        {
            var collisions = new List<DetectedCollision>();
            var from = skillshot.GetMissilePosition(0);
            skillshot.ForceDisabled = false;
            foreach (var cObject in skillshot.SkillshotData.CollisionObjects)
            {
                switch (cObject)
                {
                    case CollisionObjectTypes.Minion:

                        collisions.AddRange(from minion in MinionManager.GetMinions(@from.To3D(), 1200, MinionTypes.All, skillshot.Caster.Team == ObjectManager.Player.Team ? MinionTeam.NotAlly : MinionTeam.NotAllyForEnemy)
                            let pred = FastPrediction(@from, minion, Math.Max(0, skillshot.SkillshotData.Delay - (System.Environment.TickCount - skillshot.StartTick)), skillshot.SkillshotData.MissileSpeed)
                            let pos = pred.PredictedPos
                            let w = skillshot.SkillshotData.RawRadius + (!pred.IsMoving ? minion.BoundingRadius - 15 : 0) - pos.LSDistance(@from, skillshot.EndPosition, true)
                            where w > 0
                            select new DetectedCollision
                            {
                                Position = pos.ProjectOn(skillshot.EndPosition, skillshot.StartPosition).LinePoint + skillshot.Direction*30, Unit = minion, Type = CollisionObjectTypes.Minion, Distance = pos.LSDistance(@from), Diff = w
                            });

                        break;

                    case CollisionObjectTypes.Champions:
                        collisions.AddRange(from hero in ObjectManager.Get<AIHeroClient>().Where(h => h.IsValidTarget(1200) && h.Team == ObjectManager.Player.Team && !h.IsMe || h.Team != ObjectManager.Player.Team)
                            let pred = FastPrediction(@from, hero, Math.Max(0, skillshot.SkillshotData.Delay - (System.Environment.TickCount - skillshot.StartTick)), skillshot.SkillshotData.MissileSpeed)
                            let pos = pred.PredictedPos
                            let w = skillshot.SkillshotData.RawRadius + 30 - pos.LSDistance(@from, skillshot.EndPosition, true)
                            where w > 0
                            select new DetectedCollision
                            {
                                Position = pos.ProjectOn(skillshot.EndPosition, skillshot.StartPosition).LinePoint + skillshot.Direction*30, Unit = hero, Type = CollisionObjectTypes.Minion, Distance = pos.LSDistance(@from), Diff = w
                            });
                        break;

                    case CollisionObjectTypes.YasuoWall:
                        if (
                            !ObjectManager.Get<AIHeroClient>()
                                .Any(
                                    hero =>
                                        hero.IsValidTarget(float.MaxValue) &&
                                        hero.Team == ObjectManager.Player.Team && hero.ChampionName == "Yasuo"))
                        {
                            break;
                        }
                        GameObject wall = null;
                        foreach (var gameObject in ObjectManager.Get<GameObject>())
                        {
                            if (gameObject.IsValid &&
                                Regex.IsMatch(gameObject.Name, "_w_windwall.\\.troy", RegexOptions.IgnoreCase))
                            {
                                wall = gameObject;
                            }
                        }
                        if (wall == null)
                        {
                            break;
                        }
                        var level = wall.Name.Substring(wall.Name.Length - 6, 1);
                        var wallWidth = 300 + 50*Convert.ToInt32(level);


                        var wallDirection = (wall.Position.To2D() - _yasuoWallCastedPos).Normalized().Perpendicular();
                        var wallStart = wall.Position.To2D() + wallWidth/2*wallDirection;
                        var wallEnd = wallStart - wallWidth*wallDirection;
                        var wallPolygon = new SkillshotGeometry.Rectangle(wallStart, wallEnd, 75).ToPolygon();
                        var intersection = new Vector2();
                        var intersections = new List<Vector2>();

                        for (var i = 0; i < wallPolygon.Points.Count; i++)
                        {
                            var inter =
                                wallPolygon.Points[i].Intersection(
                                    wallPolygon.Points[i != wallPolygon.Points.Count - 1 ? i + 1 : 0], from,
                                    skillshot.EndPosition);
                            if (inter.Intersects)
                            {
                                intersections.Add(inter.Point);
                            }
                        }

                        if (intersections.Count > 0)
                        {
                            intersection = intersections.OrderBy(item => item.LSDistance(from)).ToList()[0];
                            var collisionT = System.Environment.TickCount +
                                             Math.Max(
                                                 0,
                                                 skillshot.SkillshotData.Delay -
                                                 (System.Environment.TickCount - skillshot.StartTick)) + 100 +
                                             1000*intersection.LSDistance(@from)/skillshot.SkillshotData.MissileSpeed;
                            if (collisionT - _wallCastT < 4000)
                            {
                                if (skillshot.SkillshotData.Type != SkillShotType.SkillshotMissileLine)
                                {
                                    skillshot.ForceDisabled = true;
                                }
                                return intersection;
                            }
                        }

                        break;
                }
            }

            Vector2 result;
            result = collisions.Count > 0 ? collisions.OrderBy(c => c.Distance).ToList()[0].Position : new Vector2();

            return result;
        }
    }
}