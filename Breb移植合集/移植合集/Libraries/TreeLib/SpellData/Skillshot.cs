using ClipperLib;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using EloBuddy;
using Font = SharpDX.Direct3D9.Font;
using LeagueSharp.Common.Data;
using LeagueSharp.Common;
using SharpDX.Direct3D9;
using SharpDX;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Security.AccessControl;
using System;
using System.Speech.Synthesis;

namespace TreeLib.SpellData
{
    public enum SkillShotType
    {
        SkillshotCircle,

        SkillshotLine,

        SkillshotMissileLine,

        SkillshotCone,

        SkillshotMissileCone,

        SkillshotRing,

        SkillshotArc
    }

    public enum DetectionType
    {
        RecvPacket,

        ProcessSpell
    }

    public struct SafePathResult
    {
        #region Constructors and Destructors

        public SafePathResult(bool isSafe, FoundIntersection intersection)
        {
            IsSafe = isSafe;
            Intersection = intersection;
        }

        #endregion

        #region Fields

        public FoundIntersection Intersection;

        public bool IsSafe;

        #endregion
    }

    public struct FoundIntersection
    {
        #region Constructors and Destructors

        public FoundIntersection(float distance, int time, Vector2 point, Vector2 comingFrom)
        {
            Distance = distance;
            ComingFrom = comingFrom;
            Valid = point.IsValid();
            Point = point + Config.GridSize * (ComingFrom - point).Normalized();
            Time = time;
        }

        #endregion

        #region Fields

        public Vector2 ComingFrom;

        public float Distance;

        public Vector2 Point;

        public int Time;

        public bool Valid;

        #endregion
    }

    public class Skillshot
    {
        #region Constructors and Destructors

        public Skillshot(DetectionType detectionType,
            SpellData spellData,
            int startT,
            Vector2 start,
            Vector2 end,
            Obj_AI_Base unit)
        {
            DetectionType = detectionType;
            SpellData = spellData;
            StartTick = startT;
            Start = start;
            End = end;
            Direction = (end - start).Normalized();
            Unit = unit;
            switch (spellData.Type)
            {
                case SkillShotType.SkillshotCircle:
                    Circle = new Geometry.Circle(CollisionEnd, spellData.Radius);
                    break;
                case SkillShotType.SkillshotLine:
                    Rectangle = new Geometry.Rectangle(Start, CollisionEnd, spellData.Radius);
                    break;
                case SkillShotType.SkillshotMissileLine:
                    Rectangle = new Geometry.Rectangle(Start, CollisionEnd, spellData.Radius);
                    break;
                case SkillShotType.SkillshotCone:
                    Sector = new Geometry.Sector(
                        start, CollisionEnd - start, spellData.Radius * (float) Math.PI / 180, spellData.Range);
                    break;
                case SkillShotType.SkillshotRing:
                    Ring = new Geometry.Ring(CollisionEnd, spellData.Radius, spellData.RingRadius);
                    break;
                case SkillShotType.SkillshotArc:
                    Arc = new Geometry.Arc(
                        start, end, Config.SkillShotsExtraRadius + (int) ObjectManager.Player.BoundingRadius);
                    break;
            }
            UpdatePolygon();
        }

        #endregion

        #region Fields

        public Geometry.Arc Arc;

        public Geometry.Circle Circle;

        public DetectionType DetectionType;

        public Vector2 Direction;

        public Geometry.Polygon DrawingPolygon;

        public Vector2 End;

        public bool ForceDisabled;

        public Geometry.Polygon Polygon;

        public Geometry.Rectangle Rectangle;

        public Geometry.Ring Ring;

        public Geometry.Sector Sector;

        public SpellData SpellData;

        public Vector2 Start;

        public int StartTick;

        private Vector2 collisionEnd;

        private int helperTick;

        private int lastCollisionCalc;

        #endregion

        #region Public Properties

        public Vector2 CollisionEnd
        {
            get
            {
                if (collisionEnd.IsValid())
                {
                    return collisionEnd;
                }
                if (IsGlobal)
                {
                    return GlobalGetMissilePosition(0) +
                           Direction * SpellData.MissileSpeed *
                           (0.5f + SpellData.Radius * 2 / ObjectManager.Player.MoveSpeed);
                }
                return End;
            }
        }

        public Geometry.Polygon EvadePolygon { get; set; }

        public bool IsActive
        {
            get
            {
                return SpellData.MissileAccel != 0
                    ? Utils.GameTimeTickCount <= StartTick + 5000
                    : Utils.GameTimeTickCount <=
                      StartTick + SpellData.Delay + SpellData.ExtraDuration +
                      1000 * (Start.LSDistance(End) / SpellData.MissileSpeed);
            }
        }

        public bool IsGlobal
        {
            get { return SpellData.RawRange == 20000; }
        }

        public Obj_AI_Base Unit { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Draw(Color color, Color missileColor, int width = 1)
        {
            /*if (
                !Config.Menu.SubMenu(SpellData.ChampionName.ToLowerInvariant())
                    .SubMenu(SpellData.SpellName)
                    .Item("Draw")
                    .GetValue<bool>())
            {
                return;
            }
            DrawingPolygon.Draw(color, width);
            if (SpellData.Type == SkillShotType.SkillshotMissileLine)
            {
                var position = GetMissilePosition(0);
                var from = Drawing.WorldToScreen((position + SpellData.Radius * Direction.Perpendicular()).To3D());
                var to = Drawing.WorldToScreen((position - SpellData.Radius * Direction.Perpendicular()).To3D());
                Drawing.DrawLine(from[0], from[1], to[0], to[1], 2, missileColor);
            }*/
        }

        public Vector2 GetMissilePosition(int time)
        {
            var t = Math.Max(0, Utils.GameTimeTickCount + time - StartTick - SpellData.Delay);
            int x;
            if (SpellData.MissileAccel == 0)
            {
                x = t * SpellData.MissileSpeed / 1000;
            }
            else
            {
                var t1 = (SpellData.MissileAccel > 0
                    ? SpellData.MissileMaxSpeed
                    : SpellData.MissileMinSpeed - SpellData.MissileSpeed) * 1000f / SpellData.MissileAccel;
                x = t <= t1
                    ? (int)
                        (t * SpellData.MissileSpeed / 1000d + 0.5d * SpellData.MissileAccel * Math.Pow(t / 1000d, 2))
                    : (int)
                        (t1 * SpellData.MissileSpeed / 1000d + 0.5d * SpellData.MissileAccel * Math.Pow(t1 / 1000d, 2) +
                         (t - t1) / 1000d *
                         (SpellData.MissileAccel < 0 ? SpellData.MissileMaxSpeed : SpellData.MissileMinSpeed));
            }
            t = (int) Math.Max(0, Math.Min(CollisionEnd.LSDistance(Start), x));
            return Start + Direction * t;
        }

        public Vector2 GlobalGetMissilePosition(int time)
        {
            var t = Math.Max(0, Utils.GameTimeTickCount + time - StartTick - SpellData.Delay);
            t = (int) Math.Max(0, Math.Min(End.LSDistance(Start), t * SpellData.MissileSpeed / 1000f));
            return Start + Direction * t;
        }

        public bool IsAboutToHit(int time, Obj_AI_Base unit)
        {
            if (SpellData.Type == SkillShotType.SkillshotMissileLine)
            {
                var missilePos = GetMissilePosition(0);
                var missilePosAfterT = GetMissilePosition(time);
                var projection = unit.ServerPosition.To2D().ProjectOn(missilePos, missilePosAfterT);
                return projection.IsOnSegment &&
                       projection.SegmentPoint.LSDistance(unit.ServerPosition) < SpellData.Radius;
            }
            if (!IsSafe(unit.ServerPosition.To2D()))
            {
                var timeToExplode = SpellData.ExtraDuration + SpellData.Delay +
                                    (int) (1000 * Start.LSDistance(End) / SpellData.MissileSpeed) -
                                    (Utils.GameTimeTickCount - StartTick);
                if (timeToExplode <= time)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsDanger(Vector2 point)
        {
            return !IsSafe(point);
        }

        public bool IsSafe(Vector2 point)
        {
            return Polygon.IsOutside(point);
        }

        public SafePathResult IsSafePath(List<Vector2> path, int timeOffset, int speed = -1, int delay = 0)
        {
            var distance = 0f;
            timeOffset += Game.Ping / 2;
            speed = speed == -1 ? (int) ObjectManager.Player.MoveSpeed : speed;
            var allIntersections = new List<FoundIntersection>();
            for (var i = 0; i <= path.Count - 2; i++)
            {
                var from = path[i];
                var to = path[i + 1];
                var segmentIntersections = new List<FoundIntersection>();
                for (var j = 0; j <= Polygon.Points.Count - 1; j++)
                {
                    var sideStart = Polygon.Points[j];
                    var sideEnd = Polygon.Points[j == Polygon.Points.Count - 1 ? 0 : j + 1];
                    var intersection = from.Intersection(to, sideStart, sideEnd);
                    if (intersection.Intersects)
                    {
                        segmentIntersections.Add(
                            new FoundIntersection(
                                distance + intersection.Point.LSDistance(from),
                                (int) ((distance + intersection.Point.LSDistance(from)) * 1000 / speed),
                                intersection.Point, from));
                    }
                }
                var sortedList = segmentIntersections.OrderBy(o => o.Distance).ToList();
                allIntersections.AddRange(sortedList);
                distance += from.LSDistance(to);
            }
            if (SpellData.Type == SkillShotType.SkillshotMissileLine ||
                SpellData.Type == SkillShotType.SkillshotMissileCone || SpellData.Type == SkillShotType.SkillshotArc)
            {
                if (IsSafe(Evade.PlayerPosition))
                {
                    if (allIntersections.Count == 0)
                    {
                        return new SafePathResult(true, new FoundIntersection());
                    }
                    if (SpellData.DontCross)
                    {
                        return new SafePathResult(false, allIntersections[0]);
                    }
                    for (var i = 0; i <= allIntersections.Count - 1; i = i + 2)
                    {
                        var enterIntersection = allIntersections[i];
                        var enterIntersectionProjection = enterIntersection.Point.ProjectOn(Start, End).SegmentPoint;
                        if (i == allIntersections.Count - 1)
                        {
                            var missilePositionOnIntersection = GetMissilePosition(enterIntersection.Time - timeOffset);
                            return
                                new SafePathResult(
                                    (End.LSDistance(missilePositionOnIntersection) + 50 <=
                                     End.LSDistance(enterIntersectionProjection)) &&
                                    ObjectManager.Player.MoveSpeed < SpellData.MissileSpeed, allIntersections[0]);
                        }
                        var exitIntersection = allIntersections[i + 1];
                        var exitIntersectionProjection = exitIntersection.Point.ProjectOn(Start, End).SegmentPoint;
                        var missilePosOnEnter = GetMissilePosition(enterIntersection.Time - timeOffset);
                        var missilePosOnExit = GetMissilePosition(exitIntersection.Time + timeOffset);
                        if (missilePosOnEnter.LSDistance(End) + 50 > enterIntersectionProjection.LSDistance(End) &&
                            missilePosOnExit.LSDistance(End) <= exitIntersectionProjection.LSDistance(End))
                        {
                            return new SafePathResult(false, allIntersections[0]);
                        }
                    }
                    return new SafePathResult(true, allIntersections[0]);
                }
                if (allIntersections.Count == 0)
                {
                    return new SafePathResult(false, new FoundIntersection());
                }
                if (allIntersections.Count > 0)
                {
                    var exitIntersection = allIntersections[0];
                    var exitIntersectionProjection = exitIntersection.Point.ProjectOn(Start, End).SegmentPoint;
                    var missilePosOnExit = GetMissilePosition(exitIntersection.Time + timeOffset);
                    if (missilePosOnExit.LSDistance(End) <= exitIntersectionProjection.LSDistance(End))
                    {
                        return new SafePathResult(false, allIntersections[0]);
                    }
                }
            }
            if (IsSafe(Evade.PlayerPosition))
            {
                if (allIntersections.Count == 0)
                {
                    return new SafePathResult(true, new FoundIntersection());
                }
                if (SpellData.DontCross)
                {
                    return new SafePathResult(false, allIntersections[0]);
                }
            }
            else
            {
                if (allIntersections.Count == 0)
                {
                    return new SafePathResult(false, new FoundIntersection());
                }
            }
            var timeToExplode = (SpellData.DontAddExtraDuration ? 0 : SpellData.ExtraDuration) + SpellData.Delay +
                                (int) (1000 * Start.LSDistance(End) / SpellData.MissileSpeed) -
                                (Utils.GameTimeTickCount - StartTick);
            var myPositionWhenExplodes = path.PositionAfter(timeToExplode, speed, delay);
            if (!IsSafe(myPositionWhenExplodes))
            {
                return new SafePathResult(false, allIntersections[0]);
            }
            var myPositionWhenExplodesWithOffset = path.PositionAfter(timeToExplode, speed, timeOffset);
            return new SafePathResult(IsSafe(myPositionWhenExplodesWithOffset), allIntersections[0]);
        }

        public bool IsSafeToBlink(Vector2 point, int timeOffset, int delay = 0)
        {
            timeOffset /= 2;
            if (IsSafe(Evade.PlayerPosition))
            {
                return true;
            }
            if (SpellData.Type == SkillShotType.SkillshotMissileLine)
            {
                var missilePositionAfterBlink = GetMissilePosition(delay + timeOffset);
                var myPositionProjection = Evade.PlayerPosition.ProjectOn(Start, End);
                return !(missilePositionAfterBlink.LSDistance(End) < myPositionProjection.SegmentPoint.LSDistance(End));
            }
            var timeToExplode = SpellData.ExtraDuration + SpellData.Delay +
                                (int) (1000 * Start.LSDistance(End) / SpellData.MissileSpeed) -
                                (Utils.GameTimeTickCount - StartTick);
            return timeToExplode > timeOffset + delay;
        }

        public void OnUpdate()
        {
            if (SpellData.CollisionObjects.Length > 0 && SpellData.CollisionObjects != null &&
                Utils.GameTimeTickCount - lastCollisionCalc > 50)
            {
                lastCollisionCalc = Utils.GameTimeTickCount;
                collisionEnd = Collision.GetCollisionPoint(this);
            }

            if (SpellData.Type == SkillShotType.SkillshotMissileLine)
            {
                Rectangle = new Geometry.Rectangle(GetMissilePosition(0), CollisionEnd, SpellData.Radius);
                UpdatePolygon();
            }
            if (SpellData.MissileFollowsUnit && Unit.IsVisible)
            {
                End = Unit.ServerPosition.To2D();
                Direction = (End - Start).Normalized();
                UpdatePolygon();
            }
            if (SpellData.SpellName == "SionR")
            {
                if (helperTick == 0)
                {
                    helperTick = StartTick;
                }
                SpellData.MissileSpeed = (int) Unit.MoveSpeed;
                if (Unit.IsValidTarget(float.MaxValue, false))
                {
                    if (!Unit.HasBuff("SionR") && Utils.GameTimeTickCount - helperTick > 600)
                    {
                        StartTick = 0;
                    }
                    else
                    {
                        StartTick = Utils.GameTimeTickCount - SpellData.Delay;
                        Start = Unit.ServerPosition.To2D();
                        End = Unit.ServerPosition.To2D() + 1000 * Unit.Direction.To2D().Perpendicular();
                        Direction = (End - Start).Normalized();
                        UpdatePolygon();
                    }
                }
                else
                {
                    StartTick = 0;
                }
            }
            if (SpellData.FollowCaster)
            {
                Circle.Center = Unit.ServerPosition.To2D();
                UpdatePolygon();
            }
        }

        public void UpdatePolygon()
        {
            switch (SpellData.Type)
            {
                case SkillShotType.SkillshotCircle:
                    Polygon = Circle.ToPolygon();
                    DrawingPolygon = Circle.ToPolygon(
                        0,
                        !SpellData.AddHitbox ? SpellData.Radius : SpellData.Radius - ObjectManager.Player.BoundingRadius);
                    EvadePolygon = Circle.ToPolygon(Config.ExtraEvadeDistance);
                    break;
                case SkillShotType.SkillshotLine:
                    Polygon = Rectangle.ToPolygon();
                    DrawingPolygon = Rectangle.ToPolygon(
                        0,
                        !SpellData.AddHitbox ? SpellData.Radius : SpellData.Radius - ObjectManager.Player.BoundingRadius);
                    EvadePolygon = Rectangle.ToPolygon(Config.ExtraEvadeDistance);
                    break;
                case SkillShotType.SkillshotMissileLine:
                    Polygon = Rectangle.ToPolygon();
                    DrawingPolygon = Rectangle.ToPolygon(
                        0,
                        !SpellData.AddHitbox ? SpellData.Radius : SpellData.Radius - ObjectManager.Player.BoundingRadius);
                    EvadePolygon = Rectangle.ToPolygon(Config.ExtraEvadeDistance);
                    break;
                case SkillShotType.SkillshotCone:
                    Polygon = Sector.ToPolygon();
                    DrawingPolygon = Polygon;
                    EvadePolygon = Sector.ToPolygon(Config.ExtraEvadeDistance);
                    break;
                case SkillShotType.SkillshotRing:
                    Polygon = Ring.ToPolygon();
                    DrawingPolygon = Polygon;
                    EvadePolygon = Ring.ToPolygon(Config.ExtraEvadeDistance);
                    break;
                case SkillShotType.SkillshotArc:
                    Polygon = Arc.ToPolygon();
                    DrawingPolygon = Polygon;
                    EvadePolygon = Arc.ToPolygon(Config.ExtraEvadeDistance);
                    break;
            }
        }

        #endregion
    }
}