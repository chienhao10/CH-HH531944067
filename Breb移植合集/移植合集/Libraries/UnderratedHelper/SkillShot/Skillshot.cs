#region

using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using SharpDX;
using Prediction = LeagueSharp.Common.Prediction;

#endregion

namespace UnderratedAIO.Helpers.SkillShot
{
    public enum SkillShotType
    {
        SkillshotCircle,
        SkillshotLine,
        SkillshotMissileLine,
        SkillshotCone,
        SkillshotMissileCone,
        SkillshotRing
    }

    public enum DetectionType
    {
        RecvPacket,
        ProcessSpell
    }

    public struct SafePathResult
    {
        public FoundIntersection Intersection;
        public bool IsSafe;

        public SafePathResult(bool isSafe, FoundIntersection intersection)
        {
            IsSafe = isSafe;
            Intersection = intersection;
        }
    }

    public struct FoundIntersection
    {
        public Vector2 ComingFrom;
        public float Distance;
        public Vector2 Point;
        public int Time;
        public bool Valid;

        public FoundIntersection(float distance, int time, Vector2 point, Vector2 comingFrom)
        {
            Distance = distance;
            ComingFrom = comingFrom;
            Valid = (point.X != 0) && (point.Y != 0);
            Point = point + 10*(ComingFrom - point).Normalized();
            Time = time;
        }
    }

    public class SkillshotData
    {
        public bool AddHitbox;
        public bool CanBeRemoved = false;
        public bool Centered;
        public string ChampionName;
        public CollisionObjectTypes[] CollisionObjects = {};
        public int DangerValue;
        public int Delay;
        public bool DisableFowDetection = false;
        public bool DontAddExtraDuration;
        public bool DontCross = false;
        public bool DontRemove = false;
        public int ExtraDuration;
        public string[] ExtraMissileNames = {};
        public int ExtraRange = -1;
        public string[] ExtraSpellNames = {};
        public bool FixedRange;
        public bool ForceRemove = false;
        public string FromObject = "";
        public string[] FromObjects = {};
        public int Id = -1;
        public bool Invert;
        public bool IsDangerous = false;
        public int MissileAccel = 0;
        public bool MissileDelayed;
        public bool MissileFollowsUnit;
        public int MissileMaxSpeed;
        public int MissileMinSpeed;
        public int MissileSpeed;
        public string MissileSpellName = "";
        public float MultipleAngle;
        public int MultipleNumber = -1;
        public List<int> PossibleTargets = new List<int>();
        public int RingRadius;
        public SpellSlot Slot;
        public string SpellName;
        public string ToggleParticleName = "";
        public SkillShotType Type;


        public SkillshotData()
        {
        }

        public SkillshotData(string championName,
            string spellName,
            SpellSlot slot,
            SkillShotType type,
            int delay,
            int range,
            int radius,
            int missileSpeed,
            bool addHitbox,
            bool fixedRange,
            int defaultDangerValue)
        {
            ChampionName = championName;
            SpellName = spellName;
            Slot = slot;
            Type = type;
            Delay = delay;
            Range = range;
            RawRadius = radius;
            MissileSpeed = missileSpeed;
            AddHitbox = addHitbox;
            FixedRange = fixedRange;
            DangerValue = defaultDangerValue;
        }

        public int Radius
        {
            get { return !AddHitbox ? RawRadius : RawRadius + (int) ObjectManager.Player.BoundingRadius; }
            set { RawRadius = value; }
        }

        public int RawRadius { get; private set; }

        public int RawRange { get; private set; }

        public int Range
        {
            get { return RawRange; }
            set { RawRange = value; }
        }
    }

    public class Skillshot
    {
        private Vector2 _collisionEnd;
        private int _lastCollisionCalc;
        public bool CheckStart = false;

        public Skillshot(DetectionType detectionType,
            SkillshotData skillshotData,
            int startT,
            Vector2 startPosition,
            Vector2 endPosition,
            Obj_AI_Base caster)
        {
            DetectionType = detectionType;
            SkillshotData = skillshotData;
            StartTick = startT;
            StartPosition = startPosition;
            EndPosition = endPosition;
            MissilePosition = startPosition;
            Direction = (endPosition - startPosition).Normalized();
            Caster = caster;

            //Create the spatial object for each type of skillshot.
            switch (skillshotData.Type)
            {
                case SkillShotType.SkillshotCircle:
                    Circle = new SkillshotGeometry.Circle(CollisionEnd, skillshotData.Radius);
                    break;
                case SkillShotType.SkillshotLine:
                    Rectangle = new SkillshotGeometry.Rectangle(StartPosition, CollisionEnd, skillshotData.Radius);
                    break;
                case SkillShotType.SkillshotMissileLine:
                    Rectangle = new SkillshotGeometry.Rectangle(StartPosition, CollisionEnd, skillshotData.Radius);
                    break;
                case SkillShotType.SkillshotCone:
                    Sector = new SkillshotGeometry.Sector(
                        startPosition, CollisionEnd - startPosition, skillshotData.Radius*(float) Math.PI/180,
                        skillshotData.Range);
                    break;
                case SkillShotType.SkillshotRing:
                    Ring = new SkillshotGeometry.Ring(CollisionEnd, skillshotData.Radius, skillshotData.RingRadius);
                    break;
            }

            UpdatePolygon(); // Create the polygon
        }

        public DetectionType DetectionType { get; set; }
        public SkillshotGeometry.Circle Circle { get; set; }
        public SkillshotGeometry.Polygon Polygon { get; set; }
        public SkillshotGeometry.Rectangle Rectangle { get; set; }
        public SkillshotGeometry.Ring Ring { get; set; }
        public SkillshotGeometry.Sector Sector { get; set; }

        public Vector2 StartPosition { get; set; }
        public Vector2 EndPosition { get; set; }
        public Vector2 Direction { get; set; }

        public bool ForceDisabled { get; set; }
        public Vector2 MissilePosition { get; set; }
        public SkillshotData SkillshotData { get; set; }
        public int StartTick { get; set; }
        public Obj_AI_Base Caster { get; set; }

        public Vector2 CollisionEnd
        {
            get
            {
                if (_collisionEnd.IsValid())
                {
                    return _collisionEnd;
                }

                if (IsGlobal)
                {
                    return GlobalGetMissilePosition(0) +
                           Direction*SkillshotData.MissileSpeed*
                           (0.5f + SkillshotData.Radius*2/ObjectManager.Player.MoveSpeed);
                }

                return EndPosition;
            }
        }

        public bool IsGlobal
        {
            get { return SkillshotData.RawRange == 20000; }
        }

        public bool IsActive
        {
            get
            {
                if (SkillshotData.MissileAccel != 0)
                {
                    return System.Environment.TickCount <= StartTick + 5000;
                }

                return System.Environment.TickCount <=
                       StartTick + SkillshotData.Delay + SkillshotData.ExtraDuration +
                       1000*(StartPosition.LSDistance(EndPosition)/SkillshotData.MissileSpeed);
            }
        }

        public void Game_OnGameUpdate()
        {
            //Even if it doesnt consume a lot of resources with 20 updatest second works k
            if (SkillshotData.CollisionObjects.Count() > 0 && SkillshotData.CollisionObjects != null &&
                System.Environment.TickCount - _lastCollisionCalc > 50)
            {
                _lastCollisionCalc = System.Environment.TickCount;
                _collisionEnd = Collision.GetCollisionPoint(this);
            }

            //Update the missile position each time the game updates.
            if (SkillshotData.Type == SkillShotType.SkillshotMissileLine)
            {
                Rectangle = new SkillshotGeometry.Rectangle(GetMissilePosition(0), CollisionEnd, SkillshotData.Radius);
                UpdatePolygon();
            }

            //Spells that update to the Caster position.
            if (SkillshotData.MissileFollowsUnit)
            {
                if (Caster.IsVisible)
                {
                    EndPosition = Caster.ServerPosition.To2D();
                    Direction = (EndPosition - StartPosition).Normalized();
                    UpdatePolygon();
                }
            }
        }

        public void UpdatePolygon()
        {
            switch (SkillshotData.Type)
            {
                case SkillShotType.SkillshotCircle:
                    Polygon = Circle.ToPolygon();
                    break;
                case SkillShotType.SkillshotLine:
                    Polygon = Rectangle.ToPolygon();
                    break;
                case SkillShotType.SkillshotMissileLine:
                    Polygon = Rectangle.ToPolygon();
                    break;
                case SkillShotType.SkillshotCone:
                    Polygon = Sector.ToPolygon();
                    break;
                case SkillShotType.SkillshotRing:
                    Polygon = Ring.ToPolygon();
                    break;
            }
        }

        /// <summary>
        ///     Returns the missile position after time time.
        /// </summary>
        public Vector2 GlobalGetMissilePosition(int time)
        {
            var t = Math.Max(0, System.Environment.TickCount + time - StartTick - SkillshotData.Delay);
            t = (int) Math.Max(0, Math.Min(EndPosition.LSDistance(StartPosition), t*SkillshotData.MissileSpeed/1000));
            return StartPosition + Direction*t;
        }

        /// <summary>
        ///     Returns the missile position after time time.
        /// </summary>
        public Vector2 GetMissilePosition(int time)
        {
            var t = Math.Max(0, System.Environment.TickCount + time - StartTick - SkillshotData.Delay);


            var x = 0;

            //Missile with acceleration = 0.
            if (SkillshotData.MissileAccel == 0)
            {
                x = t*SkillshotData.MissileSpeed/1000;
            }

            //Missile with constant acceleration.
            else
            {
                var t1 = (SkillshotData.MissileAccel > 0
                    ? SkillshotData.MissileMaxSpeed
                    : SkillshotData.MissileMinSpeed - SkillshotData.MissileSpeed)*1000f/SkillshotData.MissileAccel;

                if (t <= t1)
                {
                    x =
                        (int)
                            (t*SkillshotData.MissileSpeed/1000d +
                             0.5d*SkillshotData.MissileAccel*Math.Pow(t/1000d, 2));
                }
                else
                {
                    x =
                        (int)
                            (t1*SkillshotData.MissileSpeed/1000d +
                             0.5d*SkillshotData.MissileAccel*Math.Pow(t1/1000d, 2) +
                             (t - t1)/1000d*
                             (SkillshotData.MissileAccel < 0
                                 ? SkillshotData.MissileMaxSpeed
                                 : SkillshotData.MissileMinSpeed));
                }
            }

            t = (int) Math.Max(0, Math.Min(CollisionEnd.LSDistance(StartPosition), x));
            return StartPosition + Direction*t;
        }

        /// <summary>
        ///     Returns if the skillshot will hit the unit if the unit follows the path.
        /// </summary>
        public SafePathResult IsSafePath(int timeOffset, int speed = -1, int delay = 0, Obj_AI_Base unit = null)
        {
            var Distance = 0f;
            timeOffset += Game.Ping/2;


            if (unit == null)
            {
                unit = ObjectManager.Player;
            }
            speed = speed == -1 ? (int) unit.MoveSpeed : speed;
            var path = unit.Path.ToList().LSTo2D();
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
                                Distance + intersection.Point.LSDistance(from),
                                (int) ((Distance + intersection.Point.LSDistance(from))*1000/speed),
                                intersection.Point, from));
                    }
                }

                var sortedList = segmentIntersections.OrderBy(o => o.Distance).ToList();
                allIntersections.AddRange(sortedList);

                Distance += from.LSDistance(to);
            }

            //Skillshot with missile.
            if (SkillshotData.Type == SkillShotType.SkillshotMissileLine ||
                SkillshotData.Type == SkillShotType.SkillshotMissileCone)
            {
                return new SafePathResult(false, new FoundIntersection());
            }


            if (IsSafe(ObjectManager.Player.ServerPosition.To2D()))
            {
                if (allIntersections.Count == 0)
                {
                    return new SafePathResult(true, new FoundIntersection());
                }

                if (SkillshotData.DontCross)
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

            var timeToExplode = (SkillshotData.DontAddExtraDuration ? 0 : SkillshotData.ExtraDuration) +
                                SkillshotData.Delay +
                                (int) (1000*StartPosition.LSDistance(EndPosition)/SkillshotData.MissileSpeed) -
                                (System.Environment.TickCount - StartTick);


            var myPositionWhenExplodes = path.PositionAfter(timeToExplode, speed, delay);

            if (!IsSafe(myPositionWhenExplodes))
            {
                return new SafePathResult(false, allIntersections[0]);
            }

            var myPositionWhenExplodesWithOffset = path.PositionAfter(timeToExplode, speed, timeOffset);

            return new SafePathResult(IsSafe(myPositionWhenExplodesWithOffset), allIntersections[0]);
        }

        public bool IsSafe(Vector2 point)
        {
            return Polygon.IsOutside(point);
        }

        public bool IsDanger(Vector2 point)
        {
            return !IsSafe(point);
        }

        //Returns if the skillshot is about to hit the unit in the next time seconds.
        public bool IsAboutToHit(int time, Obj_AI_Base unit, Obj_AI_Base caster)
        {
            if (SkillshotData.Type == SkillShotType.SkillshotMissileLine)
            {
                if (!SkillshotData.IsDangerous)
                {
                    time = time/2;
                }
                var missilePos = GetMissilePosition(0);
                var missilePosAfterT = GetMissilePosition(time);

                /*


                var collision = false;
                foreach (var prob in
                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(
                            o =>
                                o.Health > 5 && o.NetworkId != unit.NetworkId && !o.IsDead && o.Team == unit.Team &&
                                o.IsValidTarget(caster.LSDistance(unit), false, missilePos.To3D2()) &&
                                CheckUnitCollision(o) != CollisionObjectTypes.Null &&
                                SkillshotData.CollisionObjects.Any(c => c == CheckUnitCollision(o))))
                {
                    var mp = prob.ServerPosition.To2D().ProjectOn(missilePos, unit.ServerPosition.To2D());
                    if (mp.IsOnSegment && mp.SegmentPoint.LSDistance(unit.ServerPosition) < SkillshotData.Radius)
                    {
                        collision = true;
                        break;
                    }
                }*/
                var input = new PredictionInput
                {
                    Collision = true,
                    Aoe = false,
                    Delay = missilePos.LSDistance(StartPosition) < 1 ? SkillshotData.Delay : 0,
                    From = missilePos.To3D2(),
                    Radius = SkillshotData.Radius,
                    Range = missilePos.LSDistance(missilePosAfterT),
                    Speed = SkillshotData.MissileSpeed,
                    Type = SkillshotType.SkillshotLine,
                    Unit = unit,
                    CollisionObjects = GetColisionObj()
                };
                var collisions =
                    LeagueSharp.Common.Collision.GetCollision(
                        new List<Vector3> {StartPosition.To3D2(), missilePosAfterT.To3D2()}, input);

                var projection = unit.ServerPosition.To2D().ProjectOn(missilePos, missilePosAfterT);
                var pred = Prediction.GetPrediction(input);
                var poly = CombatHelper.GetPolyFromVector(
                    missilePos.To3D2(), missilePosAfterT.To3D2(), SkillshotData.Radius);
                if (((projection.IsOnSegment &&
                      projection.SegmentPoint.LSDistance(unit.ServerPosition) < SkillshotData.Radius) ||
                     poly.IsInside(pred.UnitPosition)) && collisions.Count(c => c.NetworkId != unit.NetworkId) == 0)
                {
                    return true;
                }

                return false;
            }

            if (!IsSafe(unit.ServerPosition.To2D()))
            {
                var timeToExplode = SkillshotData.ExtraDuration + SkillshotData.Delay +
                                    (int) (1000*StartPosition.LSDistance(EndPosition)/SkillshotData.MissileSpeed) -
                                    (System.Environment.TickCount - StartTick);
                if (timeToExplode <= time)
                {
                    return true;
                }
            }

            return false;
        }

        public CollisionableObjects[] GetColisionObj()
        {
            var list = new List<CollisionableObjects>();
            foreach (var col in SkillshotData.CollisionObjects)
            {
                if (col == CollisionObjectTypes.Champions)
                {
                    list.Add(CollisionableObjects.Heroes);
                }
                if (col == CollisionObjectTypes.Minion)
                {
                    list.Add(CollisionableObjects.Minions);
                }
                if (col == CollisionObjectTypes.YasuoWall)
                {
                    list.Add(CollisionableObjects.YasuoWall);
                }
            }
            return list.ToArray();
        }

        public static CollisionObjectTypes CheckUnitCollision(Obj_AI_Base o)
        {
            if (o is AIHeroClient)
            {
                return CollisionObjectTypes.Champions;
            }
            if (o is Obj_AI_Minion)
            {
                return CollisionObjectTypes.Minion;
            }
            return CollisionObjectTypes.Null;
        }

        internal SkillshotType GetSkillShotType()
        {
            switch (SkillshotData.Type)
            {
                case SkillShotType.SkillshotLine:
                    return SkillshotType.SkillshotLine;
                case SkillShotType.SkillshotCircle:
                    return SkillshotType.SkillshotCircle;
                case SkillShotType.SkillshotCone:
                    return SkillshotType.SkillshotCone;
                case SkillShotType.SkillshotMissileCone:
                    return SkillshotType.SkillshotCone;
                case SkillShotType.SkillshotMissileLine:
                    return SkillshotType.SkillshotLine;
                case SkillShotType.SkillshotRing:
                    return SkillshotType.SkillshotCircle;
            }
            return SkillshotType.SkillshotLine;
        }
    }
}