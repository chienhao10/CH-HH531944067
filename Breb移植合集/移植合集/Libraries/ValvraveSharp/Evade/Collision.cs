namespace Valvrave_Sharp.Evade
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using EloBuddy;
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Core.Utils;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Menu;
    using SharpDX;

    #endregion

    internal class FastPredResult
    {
        #region Fields

        public Vector2 CurrentPos;

        public bool IsMoving;

        public Vector2 PredictedPos;

        #endregion
    }

    internal class DetectedCollision
    {
        #region Fields

        public float Diff;

        public float Distance;

        public Vector2 Position;

        public CollisionableObjects Type;

        public Obj_AI_Base Unit;

        #endregion
    }

    internal static class Collision
    {
        #region Static Fields

        private static Vector2 wallCastedPos;

        private static int wallCastT;

        #endregion

        #region Public Methods and Operators

        public static FastPredResult FastPrediction(Vector2 from, Obj_AI_Base unit, int delay, int speed)
        {
            var tDelay = delay / 1000f + (Math.Abs(speed - int.MaxValue) > 0 ? unit.Distance(@from) / speed : 0);
            var d = tDelay * unit.MoveSpeed;
            var path = unit.GetWaypoints();
            if (path.PathLength() > d)
            {
                return new FastPredResult
                           {
                               IsMoving = true, CurrentPos = unit.ServerPosition.ToVector2(),
                               PredictedPos = path.CutPath((int)d)[0]
                           };
            }
            return new FastPredResult
                       { IsMoving = false, CurrentPos = path[path.Count - 1], PredictedPos = path[path.Count - 1] };
        }

        public static Vector2 GetCollisionPoint(Skillshot skillshot)
        {
            var collisions = new List<DetectedCollision>();
            var from = skillshot.GetMissilePosition(0);
            skillshot.ForceDisabled = false;
            if (skillshot.SpellData.CollisionObjects.HasFlag(CollisionableObjects.Minions))
            {
                var minions = new List<Obj_AI_Minion>();
                minions.AddRange(GameObjects.Jungle.Where(i => i.LSIsValidTarget(1200, true, from.ToVector3())));
                minions.AddRange(
                    GameObjects.Minions.Where(
                        i =>
                        i.LSIsValidTarget(1200, false, @from.ToVector3())
                        && (skillshot.Unit.Team == Program.Player.Team
                                ? i.Team != Program.Player.Team
                                : i.Team == Program.Player.Team) && (i.IsMinion() || i.IsPet())));
                collisions.AddRange(
                    from minion in minions
                    let pred =
                        FastPrediction(
                            @from,
                            minion,
                            Math.Max(0, skillshot.SpellData.Delay - (Variables.TickCount - skillshot.StartTick)),
                            skillshot.SpellData.MissileSpeed)
                    let pos = pred.PredictedPos
                    let w =
                        skillshot.SpellData.RawRadius + (!pred.IsMoving ? minion.BoundingRadius - 15 : 0)
                        - pos.Distance(@from, skillshot.End, true)
                    where w > 0
                    select
                        new DetectedCollision
                            {
                                Position =
                                    pos.LSProjectOn(skillshot.End, skillshot.Start).LinePoint + skillshot.Direction * 30,
                                Unit = minion, Type = CollisionableObjects.Minions, Distance = pos.Distance(@from),
                                Diff = w
                            });
            }
            if (skillshot.SpellData.CollisionObjects.HasFlag(CollisionableObjects.Heroes))
            {
                collisions.AddRange(
                    from hero in GameObjects.AllyHeroes.Where(i => i.LSIsValidTarget(1200, false) && !i.IsMe)
                    let pred =
                        FastPrediction(
                            @from,
                            hero,
                            Math.Max(0, skillshot.SpellData.Delay - (Variables.TickCount - skillshot.StartTick)),
                            skillshot.SpellData.MissileSpeed)
                    let pos = pred.PredictedPos
                    let w = skillshot.SpellData.RawRadius + 30 - pos.Distance(@from, skillshot.End, true)
                    where w > 0
                    select
                        new DetectedCollision
                            {
                                Position =
                                    pos.LSProjectOn(skillshot.End, skillshot.Start).LinePoint + skillshot.Direction * 30,
                                Unit = hero, Type = CollisionableObjects.Heroes, Distance = pos.Distance(@from),
                                Diff = w
                            });
            }
            if (skillshot.SpellData.CollisionObjects.HasFlag(CollisionableObjects.YasuoWall)
                && GameObjects.AllyHeroes.Any(i => i.ChampionName == "Yasuo"))
            {
                var wall =
                    GameObjects.AllGameObjects.FirstOrDefault(
                        i => i.IsValid && Regex.IsMatch(i.Name, "_w_windwall.\\.troy", RegexOptions.IgnoreCase));
                if (wall != null)
                {
                    var level = wall.Name.Substring(wall.Name.Length - 6, 1);
                    var wallWidth = 300 + 50 * Convert.ToInt32(level);
                    var wallPos = wall.Position.ToVector2();
                    var wallDirection = (wallPos - wallCastedPos).LSNormalized().Perpendicular();
                    var wallStart = wallPos + wallWidth / 2f * wallDirection;
                    var wallEnd = wallStart - wallWidth * wallDirection;
                    var wallPolygon = new Geometry.Rectangle(wallStart, wallEnd, 75).ToPolygon();
                    var intersections = new List<Vector2>();
                    for (var i = 0; i < wallPolygon.Points.Count; i++)
                    {
                        var inter =
                            wallPolygon.Points[i].Intersection(
                                wallPolygon.Points[i != wallPolygon.Points.Count - 1 ? i + 1 : 0],
                                @from,
                                skillshot.End);
                        if (inter.Intersects)
                        {
                            intersections.Add(inter.Point);
                        }
                    }
                    if (intersections.Count > 0)
                    {
                        var intersection = intersections.OrderBy(item => item.Distance(@from)).ToList()[0];
                        var collisionT = Variables.TickCount
                                         + Math.Max(
                                             0,
                                             skillshot.SpellData.Delay - (Variables.TickCount - skillshot.StartTick))
                                         + 100
                                         + 1000
                                         * (Math.Abs(skillshot.SpellData.MissileSpeed - int.MaxValue) > 0
                                                ? intersection.Distance(@from) / skillshot.SpellData.MissileSpeed
                                                : 0);
                        if (collisionT - wallCastT < 4000)
                        {
                            if (skillshot.SpellData.Type != SkillShotType.SkillshotMissileLine)
                            {
                                skillshot.ForceDisabled = true;
                            }
                            return intersection;
                        }
                    }
                }
            }
            return collisions.Count > 0 ? collisions.OrderBy(i => i.Distance).First().Position : new Vector2();
        }

        public static void Init()
        {
            Obj_AI_Base.OnProcessSpellCast += (sender, args) =>
                {
                    if (!sender.LSIsValid() || sender.Team != Program.Player.Team || args.SData.Name != "YasuoWMovingWall")
                    {
                        return;
                    }
                    wallCastT = Variables.TickCount;
                    wallCastedPos = sender.ServerPosition.ToVector2();
                };
        }

        #endregion
    }
}