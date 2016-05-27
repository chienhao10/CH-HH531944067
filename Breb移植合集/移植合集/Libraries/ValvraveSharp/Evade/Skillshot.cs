namespace Valvrave_Sharp.Evade
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using EloBuddy;
    using SharpDX;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Menu;
    using Color = System.Drawing.Color;

    #endregion

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
            this.Point = point + Config.GridSize * (this.ComingFrom - point).LSNormalized();
            this.Time = time;
        }

        #endregion
    }

    public class Skillshot
    {
        #region Fields

        public Geometry.Arc Arc;

        public Geometry.Circle Circle;

        public DetectionType DetectionType;

        public Vector2 Direction;

        public Geometry.Polygon DrawingPolygon;

        public Vector2 End;

        public Geometry.Polygon EvadePolygon;

        public bool ForceDisabled;

        public MissileClient Missile;

        public Vector2 OriginalEnd;

        public Geometry.Polygon Polygon;

        public Geometry.Rectangle Rectangle;

        public Geometry.Ring Ring;

        public Geometry.Sector Sector;

        public SpellData SpellData;

        public Vector2 Start;

        public int StartTick;

        public Obj_AI_Base Unit;

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
            Obj_AI_Base unit,
            MissileClient missile = null)
        {
            this.DetectionType = detectionType;
            this.SpellData = spellData;
            this.StartTick = startT;
            this.Start = start;
            this.End = end;
            this.Direction = (end - start).LSNormalized();
            this.Unit = unit;
            this.Missile = missile;
            switch (spellData.Type)
            {
                case SkillShotType.SkillshotCircle:
                    this.Circle = new Geometry.Circle(this.CollisionEnd, spellData.Radius);
                    break;
                case SkillShotType.SkillshotLine:
                    this.Rectangle = new Geometry.Rectangle(this.Start, this.CollisionEnd, spellData.Radius);
                    break;
                case SkillShotType.SkillshotMissileLine:
                    this.Rectangle = new Geometry.Rectangle(this.Start, this.CollisionEnd, spellData.Radius);
                    break;
                case SkillShotType.SkillshotCone:
                    this.Sector = new Geometry.Sector(
                        start,
                        this.CollisionEnd - start,
                        spellData.Radius * (float)Math.PI / 180,
                        spellData.Range);
                    break;
                case SkillShotType.SkillshotRing:
                    this.Ring = new Geometry.Ring(this.CollisionEnd, spellData.Radius, spellData.RingRadius);
                    break;
                case SkillShotType.SkillshotArc:
                    this.Arc = new Geometry.Arc(
                        start,
                        end,
                        Config.SkillShotsExtraRadius + (int)Program.Player.BoundingRadius);
                    break;
            }
            this.UpdatePolygon();
        }

        #endregion

        #region Public Properties

        public static bool getCheckBoxItem(string item)
        {
            return Config.evadeMenu[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return Config.evadeMenu[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(string item)
        {
            return Config.evadeMenu[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(string item)
        {
            return Config.evadeMenu[item].Cast<ComboBox>().CurrentValue;
        }

        public bool CanDodge
        {
            get
            {
                if (this.DetectionType == DetectionType.ProcessSpell)
                {
                    return true;
                }
                var missileValid = this.Missile != null && this.Missile.IsValid && this.Missile.IsVisible;
                return getCheckBoxItem("DisableFoW") ? missileValid : !getCheckBoxItem(this.SpellData.SpellName + "DisableFoW") || missileValid;
            }
        }

        public Vector2 CollisionEnd
        {
            get
            {
                if (this.collisionEnd.IsValid())
                {
                    return this.collisionEnd;
                }
                if (this.IsGlobal)
                {
                    return this.GlobalGetMissilePosition(0)
                           + this.Direction * this.SpellData.MissileSpeed
                           * (0.5f + this.SpellData.Radius * 2 / Program.Player.MoveSpeed);
                }
                return this.End;
            }
        }

        public int DangerLevel => getSliderItem(this.SpellData.SpellName + "DangerLevel");

        public bool Enable
        {
            get
            {
                if (this.ForceDisabled)
                {
                    return false;
                }
                if (Variables.TickCount - this.cachedValueTick < 100)
                {
                    return this.cachedValue;
                }


                if (!getCheckBoxItem(this.SpellData.SpellName + "IsDangerous") && getKeyBindItem("OnlyDangerous"))
                {
                    this.cachedValue = false;
                    this.cachedValueTick = Variables.TickCount;
                    return this.cachedValue;
                }
                this.cachedValue = getCheckBoxItem(this.SpellData.SpellName + "Enabled");
                this.cachedValueTick = Variables.TickCount;
                return this.cachedValue;
            }
        }

        public bool IsActive
            =>
                this.SpellData.MissileAccel != 0
                    ? Variables.TickCount <= this.StartTick + 5000
                    : Variables.TickCount
                      <= this.StartTick + this.SpellData.ExtraDuration + this.SpellData.Delay
                      + (int)
                        (1000
                         * (Math.Abs(this.SpellData.MissileSpeed - int.MaxValue) > 0
                                ? this.Start.Distance(this.End) / this.SpellData.MissileSpeed
                                : 0));

        public bool IsGlobal => this.SpellData.RawRange == 20000;

        #endregion

        #region Public Methods and Operators

        public void Draw(Color color, Color missileColor, int width = 1)
        {
            if (Config.evadeMenu[this.SpellData.SpellName + "Draw"] == null)
            {
                return;
            }
            if (!getCheckBoxItem(this.SpellData.SpellName + "Draw"))
            {
                return;
            }
            this.DrawingPolygon.Draw(color, width);
            if (this.SpellData.Type == SkillShotType.SkillshotMissileLine)
            {
                var position = this.GetMissilePosition(0);
                var from =
                    Drawing.WorldToScreen(
                        (position + this.SpellData.Radius * this.Direction.Perpendicular()).ToVector3());
                var to =
                    Drawing.WorldToScreen(
                        (position - this.SpellData.Radius * this.Direction.Perpendicular()).ToVector3());
                Drawing.DrawLine(from[0], from[1], to[0], to[1], 2, missileColor);
            }
        }

        public Vector2 GetMissilePosition(int time)
        {
            var t = Math.Max(0, Variables.TickCount + time - this.StartTick - this.SpellData.Delay);
            int x;
            if (this.SpellData.MissileAccel == 0)
            {
                x = t * (Math.Abs(this.SpellData.MissileSpeed - int.MaxValue) > 0 ? this.SpellData.MissileSpeed : 0)
                    / 1000;
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
            t = (int)Math.Max(0, Math.Min(this.CollisionEnd.Distance(this.Start), x));
            return this.Start + this.Direction * t;
        }

        public Vector2 GlobalGetMissilePosition(int time)
        {
            var t = Math.Max(0, Variables.TickCount + time - this.StartTick - this.SpellData.Delay);
            t = (int)Math.Max(0, Math.Min(this.End.Distance(this.Start), t * this.SpellData.MissileSpeed / 1000f));
            return this.Start + this.Direction * t;
        }

        public bool IsAboutToHit(int time, Obj_AI_Base unit)
        {
            if (this.SpellData.Type == SkillShotType.SkillshotMissileLine)
            {
                var missilePos = this.GetMissilePosition(0);
                var missilePosAfterT = this.GetMissilePosition(time);
                var projection = unit.ServerPosition.ToVector2().LSProjectOn(missilePos, missilePosAfterT);
                return projection.IsOnSegment
                       && projection.SegmentPoint.Distance(unit.ServerPosition) < this.SpellData.Radius;
            }
            if (!this.IsSafe(unit.ServerPosition.ToVector2()))
            {
                var timeToExplode = this.SpellData.ExtraDuration + this.SpellData.Delay
                                    + (int)
                                      (1000
                                       * (Math.Abs(this.SpellData.MissileSpeed - int.MaxValue) > 0
                                              ? this.Start.Distance(this.End) / this.SpellData.MissileSpeed
                                              : 0)) - (Variables.TickCount - this.StartTick);
                if (timeToExplode <= time)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsDanger(Vector2 point)
        {
            return !this.IsSafe(point);
        }

        public bool IsSafe(Vector2 point)
        {
            return this.Polygon.IsOutside(point);
        }

        public SafePathResult IsSafePath(List<Vector2> path, int timeOffset, int speed = -1, int delay = 0)
        {
            var distance = 0f;
            timeOffset += Game.Ping / 2;
            speed = speed == -1 ? (int)Program.Player.MoveSpeed : speed;
            var allIntersections = new List<FoundIntersection>();
            for (var i = 0; i <= path.Count - 2; i++)
            {
                var from = path[i];
                var to = path[i + 1];
                var segmentIntersections = new List<FoundIntersection>();
                for (var j = 0; j <= this.Polygon.Points.Count - 1; j++)
                {
                    var sideStart = this.Polygon.Points[j];
                    var sideEnd = this.Polygon.Points[j == this.Polygon.Points.Count - 1 ? 0 : j + 1];
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
                if (segmentIntersections.Count > 0)
                {
                    allIntersections.AddRange(segmentIntersections.OrderBy(o => o.Distance).ToList());
                }
                distance += from.Distance(to);
            }
            if (this.SpellData.Type == SkillShotType.SkillshotMissileLine
                || this.SpellData.Type == SkillShotType.SkillshotMissileCone
                || this.SpellData.Type == SkillShotType.SkillshotArc)
            {
                if (this.IsSafe(Evade.PlayerPosition))
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
                            enterIntersection.Point.LSProjectOn(this.Start, this.End).SegmentPoint;
                        if (i == allIntersections.Count - 1)
                        {
                            var missilePositionOnIntersection =
                                this.GetMissilePosition(enterIntersection.Time - timeOffset);
                            return
                                new SafePathResult(
                                    (this.End.Distance(missilePositionOnIntersection) + 50
                                     <= this.End.Distance(enterIntersectionProjection))
                                    && Program.Player.MoveSpeed < this.SpellData.MissileSpeed,
                                    allIntersections[0]);
                        }
                        var exitIntersection = allIntersections[i + 1];
                        var exitIntersectionProjection =
                            exitIntersection.Point.LSProjectOn(this.Start, this.End).SegmentPoint;
                        var missilePosOnEnter = this.GetMissilePosition(enterIntersection.Time - timeOffset);
                        var missilePosOnExit = this.GetMissilePosition(exitIntersection.Time + timeOffset);
                        if (missilePosOnEnter.Distance(this.End) + 50 > enterIntersectionProjection.Distance(this.End)
                            && missilePosOnExit.Distance(this.End) <= exitIntersectionProjection.Distance(this.End))
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
                    var exitIntersectionProjection = exitIntersection.Point.LSProjectOn(this.Start, this.End).SegmentPoint;
                    var missilePosOnExit = this.GetMissilePosition(exitIntersection.Time + timeOffset);
                    if (missilePosOnExit.Distance(this.End) <= exitIntersectionProjection.Distance(this.End))
                    {
                        return new SafePathResult(false, allIntersections[0]);
                    }
                }
            }
            if (this.IsSafe(Evade.PlayerPosition))
            {
                if (allIntersections.Count == 0)
                {
                    return new SafePathResult(true, new FoundIntersection());
                }
                if (this.SpellData.DontCross)
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
            var timeToExplode = (this.SpellData.DontAddExtraDuration ? 0 : this.SpellData.ExtraDuration)
                                + this.SpellData.Delay
                                + (int)
                                  (1000
                                   * (Math.Abs(this.SpellData.MissileSpeed - int.MaxValue) > 0
                                          ? this.Start.Distance(this.End) / this.SpellData.MissileSpeed
                                          : 0)) - (Variables.TickCount - this.StartTick);
            var myPositionWhenExplodes = path.PositionAfter(timeToExplode, speed, delay);
            if (!this.IsSafe(myPositionWhenExplodes))
            {
                return new SafePathResult(false, allIntersections[0]);
            }
            var myPositionWhenExplodesWithOffset = path.PositionAfter(timeToExplode, speed, timeOffset);
            return new SafePathResult(this.IsSafe(myPositionWhenExplodesWithOffset), allIntersections[0]);
        }

        public bool IsSafeToBlink(Vector2 point, int timeOffset, int delay)
        {
            timeOffset /= 2;
            if (this.IsSafe(point))
            {
                return true;
            }
            if (this.SpellData.Type == SkillShotType.SkillshotMissileLine)
            {
                var missilePositionAfterBlink = this.GetMissilePosition(delay + timeOffset);
                var myPositionProjection = Evade.PlayerPosition.LSProjectOn(this.Start, this.End);
                return missilePositionAfterBlink.Distance(this.End)
                       >= myPositionProjection.SegmentPoint.Distance(this.End);
            }
            var timeToExplode = (this.SpellData.DontAddExtraDuration ? 0 : this.SpellData.ExtraDuration)
                                + this.SpellData.Delay
                                + (int)
                                  (1000
                                   * (Math.Abs(this.SpellData.MissileSpeed - int.MaxValue) > 0
                                          ? this.Start.Distance(this.End) / this.SpellData.MissileSpeed
                                          : 0)) - (Variables.TickCount - this.StartTick);
            return timeToExplode > timeOffset + delay;
        }

        public void OnUpdate()
        {
            if (this.SpellData.CollisionObjects.GetFlags().Any() && Variables.TickCount - this.lastCollisionCalc > 50)
            {
                this.lastCollisionCalc = Variables.TickCount;
                this.collisionEnd = Collision.GetCollisionPoint(this);
            }
            if (this.SpellData.Type == SkillShotType.SkillshotMissileLine)
            {
                this.Rectangle = new Geometry.Rectangle(
                    this.GetMissilePosition(0),
                    this.CollisionEnd,
                    this.SpellData.Radius);
                this.UpdatePolygon();
            }
            if (this.SpellData.MissileFollowsUnit && this.Unit.IsVisible)
            {
                this.End = this.Unit.ServerPosition.ToVector2();
                this.Direction = (this.End - this.Start).LSNormalized();
                this.UpdatePolygon();
            }
            if (this.SpellData.SpellName == "TaricE")
            {
                this.Start = this.Unit.ServerPosition.ToVector2();
                this.End = this.Start + this.Direction * this.SpellData.Range;
                this.Rectangle = new Geometry.Rectangle(this.Start, this.End, this.SpellData.Radius);
                this.UpdatePolygon();
            }
            if (this.SpellData.SpellName == "SionR")
            {
                if (this.helperTick == 0)
                {
                    this.helperTick = this.StartTick;
                }
                this.SpellData.MissileSpeed = (int)this.Unit.MoveSpeed;
                if (this.Unit.LSIsValidTarget(float.MaxValue, false))
                {
                    if (!this.Unit.HasBuff("SionR") && Variables.TickCount - this.helperTick > 600)
                    {
                        this.StartTick = 0;
                    }
                    else
                    {
                        this.StartTick = Variables.TickCount - this.SpellData.Delay;
                        this.Start = this.Unit.ServerPosition.ToVector2();
                        this.End = this.Unit.ServerPosition.ToVector2()
                                   + 1000 * this.Unit.Direction.ToVector2().Perpendicular();
                        this.Direction = (this.End - this.Start).LSNormalized();
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
                this.Circle.Center = this.Unit.ServerPosition.ToVector2();
                this.UpdatePolygon();
            }
        }

        public void UpdatePolygon()
        {
            switch (this.SpellData.Type)
            {
                case SkillShotType.SkillshotCircle:
                    this.Polygon = this.Circle.ToPolygon();
                    this.DrawingPolygon = this.Circle.ToPolygon(
                        0,
                        !this.SpellData.AddHitbox
                            ? this.SpellData.Radius
                            : this.SpellData.Radius - Program.Player.BoundingRadius);
                    this.EvadePolygon = this.Circle.ToPolygon(Config.ExtraEvadeDistance);
                    break;
                case SkillShotType.SkillshotLine:
                    this.Polygon = this.Rectangle.ToPolygon();
                    this.DrawingPolygon = this.Rectangle.ToPolygon(
                        0,
                        !this.SpellData.AddHitbox
                            ? this.SpellData.Radius
                            : this.SpellData.Radius - Program.Player.BoundingRadius);
                    this.EvadePolygon = this.Rectangle.ToPolygon(Config.ExtraEvadeDistance);
                    break;
                case SkillShotType.SkillshotMissileLine:
                    this.Polygon = this.Rectangle.ToPolygon();
                    this.DrawingPolygon = this.Rectangle.ToPolygon(
                        0,
                        !this.SpellData.AddHitbox
                            ? this.SpellData.Radius
                            : this.SpellData.Radius - Program.Player.BoundingRadius);
                    this.EvadePolygon = this.Rectangle.ToPolygon(Config.ExtraEvadeDistance);
                    break;
                case SkillShotType.SkillshotCone:
                    this.Polygon = this.Sector.ToPolygon();
                    this.DrawingPolygon = this.Polygon;
                    this.EvadePolygon = this.Sector.ToPolygon(Config.ExtraEvadeDistance);
                    break;
                case SkillShotType.SkillshotRing:
                    this.Polygon = this.Ring.ToPolygon();
                    this.DrawingPolygon = this.Polygon;
                    this.EvadePolygon = this.Ring.ToPolygon(Config.ExtraEvadeDistance);
                    break;
                case SkillShotType.SkillshotArc:
                    this.Polygon = this.Arc.ToPolygon();
                    this.DrawingPolygon = this.Polygon;
                    this.EvadePolygon = this.Arc.ToPolygon(Config.ExtraEvadeDistance);
                    break;
            }
        }

        #endregion
    }
}