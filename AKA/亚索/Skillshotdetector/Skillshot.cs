using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using EloBuddy.SDK.Menu.Values;

namespace AkaYasuo
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
        #region Fields

        public FoundIntersection Intersection;

        public bool IsSafe;

        #endregion

        #region Constructors and Destructors

        public SafePathResult(bool isSafe, FoundIntersection intersection)
        {
            this.IsSafe = isSafe;
            this.Intersection = intersection;
        }

        #endregion
    }

    public struct FoundIntersection
    {
        #region Fields

        public Vector2 ComingFrom;

        public float Distance;

        public Vector2 Point;

        public int Time;

        public bool Valid;

        #endregion

        #region Constructors and Destructors

        public FoundIntersection(float distance, int time, Vector2 point, Vector2 comingFrom)
        {
            this.Distance = distance;
            this.ComingFrom = comingFrom;
            this.Valid = point.IsValid();
            this.Point = point + Configs.GridSize * (this.ComingFrom - point).Normalized();
            this.Time = time;
        }

        #endregion
    }

    public class Skillshot
    {
        #region Fields

        public Geometry.Polygon.Arc Arc;

        public Geometry.Polygon.Circle Circle;

        public DetectionType DetectionType;

        public Vector2 Direction;

        public Vector2 End;

        public bool ForceDisabled;

        public Geometry.Polygon Polygon;

        public Geometry.Polygon.Rectangle Rectangle;

        public Geometry.Polygon.Ring Ring;

        public Geometry.Polygon.Sector Sector;

        public SpellData SpellData;

        public Vector2 Start;

        public int StartTick;

        private bool cachedValue;

        private int cachedValueTick;

        private Vector2 collisionEnd;

        private int helperTick;

        private int lastCollisionCalc;

        #endregion

        #region Constructors and Destructors

        public Skillshot(
            DetectionType detectionType,
            SpellData spellData,
            int startT,
            Vector2 start,
            Vector2 end,
            Obj_AI_Base unit)
        {
            this.DetectionType = detectionType;
            this.SpellData = spellData;
            this.StartTick = startT;
            this.Start = start;
            this.End = end;
            this.Direction = (end - start).Normalized();
            this.Unit = unit;
            switch (spellData.Type)
            {
                case SkillShotType.SkillshotCircle:
                    this.Circle = new Geometry.Polygon.Circle(this.CollisionEnd, spellData.Radius, 22);
                    break;
                case SkillShotType.SkillshotLine:
                case SkillShotType.SkillshotMissileLine:
                    this.Rectangle = new Geometry.Polygon.Rectangle(this.Start, this.CollisionEnd, spellData.Radius);
                    break;
                case SkillShotType.SkillshotCone:
                    this.Sector = new Geometry.Polygon.Sector(
                        start,
                        this.CollisionEnd - start,
                        spellData.Radius * (float)Math.PI / 180,
                        spellData.Range,
                        22);
                    break;
                case SkillShotType.SkillshotRing:
                    this.Ring = new Geometry.Polygon.Ring(this.CollisionEnd, spellData.Radius, spellData.RingRadius, 22);
                    break;
                case SkillShotType.SkillshotArc:
                    this.Arc = new Geometry.Polygon.Arc(
                        start,
                        end,
                        Configs.SkillShotsExtraRadius + (int)ObjectManager.Player.BoundingRadius,
                        22);
                    break;
            }
            this.UpdatePolygon();
        }

        #endregion

        #region Public Properties

        public bool Evade
        {
            get
            {
                if (this.ForceDisabled)
                {
                    return false;
                }
                if (Utils.GameTimeTickCount - this.cachedValueTick < 100)
                {
                    return this.cachedValue;
                }
                this.cachedValue = EvadeManager.EvadeSkillshot.championmenu["Enabled"].Cast<CheckBox>().CurrentValue;
                this.cachedValueTick = Utils.GameTimeTickCount;
                return this.cachedValue;
            }
        }

        public bool IsActive
        {
            get
            {
                return this.SpellData.MissileAccel != 0
                           ? Utils.GameTimeTickCount <= this.StartTick + 5000
                           : Utils.GameTimeTickCount
                             <= this.StartTick + this.SpellData.Delay + this.SpellData.ExtraDuration
                             + 1000 * (this.Start.Distance(this.End) / this.SpellData.MissileSpeed);
            }
        }

        public Vector2 Perpendicular
        {
            get
            {
                return this.Direction.Perpendicular();
            }
        }

        public Obj_AI_Base Unit { get; set; }

        #endregion

        #region Properties

        private Vector2 CollisionEnd
        {
            get
            {
                return this.collisionEnd.IsValid()
                           ? this.collisionEnd
                           : (this.SpellData.RawRange == 20000
                                  ? this.GetGlobalMissilePosition(0)
                                    + this.Direction * this.SpellData.MissileSpeed
                                    * (0.5f + this.SpellData.Radius * 2 / ObjectManager.Player.MoveSpeed)
                                  : this.End);
            }
        }

        #endregion

        #region Public Methods and Operators

        public Vector2 GetMissilePosition(int time)
        {
            var t = Math.Max(0, Utils.GameTimeTickCount + time - this.StartTick - this.SpellData.Delay);
            int x;
            if (this.SpellData.MissileAccel == 0)
            {
                x = t * this.SpellData.MissileSpeed / 1000;
            }
            else
            {
                var t1 = (this.SpellData.MissileAccel > 0
                              ? this.SpellData.MissileMaxSpeed
                              : this.SpellData.MissileMinSpeed - this.SpellData.MissileSpeed) * 1000f
                         / this.SpellData.MissileAccel;
                x = t <= t1
                        ? (int)
                          (t * this.SpellData.MissileSpeed / 1000d
                           + 0.5d * this.SpellData.MissileAccel * Math.Pow(t / 1000d, 2))
                        : (int)
                          (t1 * this.SpellData.MissileSpeed / 1000d
                           + 0.5d * this.SpellData.MissileAccel * Math.Pow(t1 / 1000d, 2)
                           + (t - t1) / 1000d
                           * (this.SpellData.MissileAccel < 0
                                  ? this.SpellData.MissileMaxSpeed
                                  : this.SpellData.MissileMinSpeed));
            }
            return this.Start + this.Direction * (int)Math.Max(0, Math.Min(this.CollisionEnd.Distance(this.Start), x));
        }

        public bool IsAboutToHit(int time, Obj_AI_Base unit)
        {
            if (this.SpellData.Type != SkillShotType.SkillshotMissileLine)
            {
                return !this.IsSafePoint(unit.ServerPosition.To2D())
                       && this.SpellData.ExtraDuration + this.SpellData.Delay
                       + (int)(1000 * this.Start.Distance(this.End) / this.SpellData.MissileSpeed)
                       - (Utils.GameTimeTickCount - this.StartTick) <= time;
            }
            var project = unit.ServerPosition.To2D()
                .ProjectOn(this.GetMissilePosition(0), this.GetMissilePosition(time));
            return project.IsOnSegment && unit.Distance(project.SegmentPoint) < this.SpellData.Radius;
        }

        public SafePathResult IsSafePath(List<Vector2> path, int timeOffset, int speed = -1, int delay = 0)
        {
            var distance = 0f;
            timeOffset += Game.Ping / 2;
            speed = speed == -1 ? (int)ObjectManager.Player.MoveSpeed : speed;
            var allIntersections = new List<FoundIntersection>();
            for (var i = 0; i <= path.Count - 2; i++)
            {
                var from = path[i];
                var to = path[i + 1];
                var segmentIntersections = new List<FoundIntersection>();
                for (var j = 0; j <= this.Polygon.Points.Count - 1; j++)
                {
                    var sideStart = this.Polygon.Points[j];
                    var sideEnd = this.Polygon.Points[j == (this.Polygon.Points.Count - 1) ? 0 : j + 1];
                    var intersection = from.Intersection(to, sideStart, sideEnd);
                    if (intersection.Intersects)
                    {
                        segmentIntersections.Add(
                            new FoundIntersection(
                                distance + intersection.Point.Distance(from),
                                (int)((distance + intersection.Point.Distance(from)) * 1000 / speed),
                                intersection.Point,
                                from));
                    }
                }
                allIntersections.AddRange(segmentIntersections.OrderBy(o => o.Distance));
                distance += from.Distance(to);
            }
            if (this.SpellData.Type == SkillShotType.SkillshotMissileLine
                || this.SpellData.Type == SkillShotType.SkillshotMissileCone
                || this.SpellData.Type == SkillShotType.SkillshotArc)
            {
                if (this.IsSafePoint(ObjectManager.Player.ServerPosition.To2D()))
                {
                    if (allIntersections.Count == 0)
                    {
                        return new SafePathResult(true, new FoundIntersection());
                    }
                    if (this.SpellData.DontCross)
                    {
                        return new SafePathResult(false, allIntersections[0]);
                    }
                    for (var i = 0; i <= allIntersections.Count - 1; i = i + 2)
                    {
                        var enterIntersection = allIntersections[i];
                        var enterIntersectionProjection =
                            enterIntersection.Point.ProjectOn(this.Start, this.End).SegmentPoint;
                        if (i == allIntersections.Count - 1)
                        {
                            return
                                new SafePathResult(
                                    (this.End.Distance(this.GetMissilePosition(enterIntersection.Time - timeOffset))
                                     + 50 <= this.End.Distance(enterIntersectionProjection))
                                    && ObjectManager.Player.MoveSpeed < this.SpellData.MissileSpeed,
                                    allIntersections[0]);
                        }
                        var exitIntersection = allIntersections[i + 1];
                        var exitIntersectionProjection =
                            exitIntersection.Point.ProjectOn(this.Start, this.End).SegmentPoint;
                        if (this.GetMissilePosition(enterIntersection.Time - timeOffset).Distance(this.End) + 50
                            > enterIntersectionProjection.Distance(this.End)
                            && this.GetMissilePosition(exitIntersection.Time + timeOffset).Distance(this.End)
                            <= exitIntersectionProjection.Distance(this.End))
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
                    var exitIntersectionProjection = exitIntersection.Point.ProjectOn(this.Start, this.End).SegmentPoint;
                    if (this.GetMissilePosition(exitIntersection.Time + timeOffset).Distance(this.End)
                        <= exitIntersectionProjection.Distance(this.End))
                    {
                        return new SafePathResult(false, allIntersections[0]);
                    }
                }
            }
            if (allIntersections.Count == 0)
            {
                return new SafePathResult(false, new FoundIntersection());
            }
            if (this.IsSafePoint(ObjectManager.Player.ServerPosition.To2D()) && this.SpellData.DontCross)
            {
                return new SafePathResult(false, allIntersections[0]);
            }
            var timeToExplode = (this.SpellData.DontAddExtraDuration ? 0 : this.SpellData.ExtraDuration)
                                + this.SpellData.Delay
                                + (int)(1000 * this.Start.Distance(this.End) / this.SpellData.MissileSpeed)
                                - (Utils.GameTimeTickCount - this.StartTick);
            return !this.IsSafePoint(path.PositionAfter(timeToExplode, speed, delay))
                       ? new SafePathResult(false, allIntersections[0])
                       : new SafePathResult(
                             this.IsSafePoint(path.PositionAfter(timeToExplode, speed, timeOffset)),
                             allIntersections[0]);
        }

        public bool IsSafePoint(Vector2 point)
        {
            return this.Polygon.IsOutside(point);
        }

        public void OnUpdate()
        {
            if (this.SpellData.CollisionObjects.Count() > 0 && this.SpellData.CollisionObjects != null
                && Utils.GameTimeTickCount - this.lastCollisionCalc > 50)
            {
                this.lastCollisionCalc = Utils.GameTimeTickCount;
                this.collisionEnd = this.GetCollisionPoint();
            }
            if (this.SpellData.Type == SkillShotType.SkillshotMissileLine)
            {
                this.Rectangle = new Geometry.Polygon.Rectangle(
                    this.GetMissilePosition(0),
                    this.CollisionEnd,
                    this.SpellData.Radius);
                this.UpdatePolygon();
            }
            if (this.SpellData.MissileFollowsUnit && this.Unit.IsVisible)
            {
                this.End = this.Unit.ServerPosition.To2D();
                this.Direction = (this.End - this.Start).Normalized();
                this.UpdatePolygon();
            }
            if (this.SpellData.SpellName == "SionR")
            {
                if (this.helperTick == 0)
                {
                    this.helperTick = this.StartTick;
                }
                this.SpellData.MissileSpeed = (int)this.Unit.MoveSpeed;
                if (this.Unit.IsValidTarget(float.MaxValue, false))
                {
                    if (!this.Unit.HasBuff("SionR") && Utils.GameTimeTickCount - this.helperTick > 600)
                    {
                        this.StartTick = 0;
                    }
                    else
                    {
                        this.StartTick = Utils.GameTimeTickCount - this.SpellData.Delay;
                        this.Start = this.Unit.ServerPosition.To2D();
                        this.End = this.Unit.ServerPosition.To2D() + 1000 * this.Unit.Direction.To2D().Perpendicular();
                        this.Direction = (this.End - this.Start).Normalized();
                        this.UpdatePolygon();
                    }
                }
                else
                {
                    this.StartTick = 0;
                }
            }
            if (this.SpellData.FollowCaster)
            {
                this.Circle.Center = this.Unit.ServerPosition.To2D();
                this.UpdatePolygon();
            }
        }

        #endregion


        private Vector2 GetGlobalMissilePosition(int time)
        {
            return this.Start
                   + this.Direction
                   * (int)
                     Math.Max(
                         0,
                         Math.Min(
                             this.End.Distance(this.Start),
                             (float)Math.Max(0, Utils.GameTimeTickCount + time - this.StartTick - this.SpellData.Delay)
                             * this.SpellData.MissileSpeed / 1000));
        }

        private void UpdatePolygon()
        {
            switch (this.SpellData.Type)
            {
                case SkillShotType.SkillshotCircle:
                    this.Circle.UpdatePolygon();
                    this.Polygon = this.Circle;
                    break;
                case SkillShotType.SkillshotLine:
                case SkillShotType.SkillshotMissileLine:
                    this.Rectangle.UpdatePolygon();
                    this.Polygon = this.Rectangle;
                    break;
                case SkillShotType.SkillshotCone:
                    this.Sector.UpdatePolygon();
                    this.Polygon = this.Sector;
                    break;
                case SkillShotType.SkillshotRing:
                    this.Ring.UpdatePolygon();
                    this.Polygon = this.Ring;
                    break;
                case SkillShotType.SkillshotArc:
                    this.Arc.UpdatePolygon();
                    this.Polygon = this.Arc;
                    break;
            }
        }
    }
}