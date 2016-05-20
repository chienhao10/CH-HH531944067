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
    public static class Geometry
    {
        #region Constants

        private const int CircleLineSegmentN = 22;

        #endregion

        public class Arc
        {
            #region Constructors and Destructors

            public Arc(Vector2 start, Vector2 end, int hitbox)
            {
                Start = start;
                End = end;
                HitBox = hitbox;
                Distance = Start.LSDistance(End);
            }

            #endregion

            #region Public Methods and Operators

            public Polygon ToPolygon(int offset = 0)
            {
                offset += HitBox;
                var result = new Polygon();
                var innerRadius = -0.1562f * Distance + 687.31f;
                var outerRadius = 0.35256f * Distance + 133f;
                outerRadius = outerRadius / (float) Math.Cos(2 * Math.PI / CircleLineSegmentN);
                var innerCenters = LeagueSharp.Common.Geometry.CircleCircleIntersection(
                    Start, End, innerRadius, innerRadius);
                var outerCenters = LeagueSharp.Common.Geometry.CircleCircleIntersection(
                    Start, End, outerRadius, outerRadius);
                var innerCenter = innerCenters[0];
                var outerCenter = outerCenters[0];
                var direction = (End - outerCenter).Normalized();
                var end = (Start - outerCenter).Normalized();
                var maxAngle = (float) (direction.AngleBetween(end) * Math.PI / 180);
                var step = -maxAngle / CircleLineSegmentN;
                for (var i = 0; i < CircleLineSegmentN; i++)
                {
                    var angle = step * i;
                    var point = outerCenter + (outerRadius + 15 + offset) * direction.Rotated(angle);
                    result.Add(point);
                }
                direction = (Start - innerCenter).Normalized();
                end = (End - innerCenter).Normalized();
                maxAngle = (float) (direction.AngleBetween(end) * Math.PI / 180);
                step = maxAngle / CircleLineSegmentN;
                for (var i = 0; i < CircleLineSegmentN; i++)
                {
                    var angle = step * i;
                    var point = innerCenter + Math.Max(0, innerRadius - offset - 100) * direction.Rotated(angle);
                    result.Add(point);
                }
                return result;
            }

            #endregion

            #region Fields

            public float Distance;

            public Vector2 End;

            public int HitBox;

            public Vector2 Start;

            #endregion
        }

        public class Circle
        {
            #region Constructors and Destructors

            public Circle(Vector2 center, float radius)
            {
                Center = center;
                Radius = radius;
            }

            #endregion

            #region Public Methods and Operators

            public Polygon ToPolygon(int offset = 0, float overrideWidth = -1)
            {
                var result = new Polygon();
                var outRadius = overrideWidth > 0
                    ? overrideWidth
                    : (offset + Radius) / (float) Math.Cos(2 * Math.PI / CircleLineSegmentN);
                const double Step = 2 * Math.PI / CircleLineSegmentN;
                var angle = (double) Radius;
                for (var i = 0; i <= CircleLineSegmentN; i++)
                {
                    angle += Step;
                    var point = new Vector2(
                        Center.X + outRadius * (float) Math.Cos(angle), Center.Y + outRadius * (float) Math.Sin(angle));
                    result.Add(point);
                }
                return result;
            }

            #endregion

            #region Fields

            public Vector2 Center;

            public float Radius;

            #endregion
        }

        public class Polygon
        {
            #region Fields

            public List<Vector2> Points = new List<Vector2>();

            #endregion

            #region Public Methods and Operators

            public void Add(Vector2 point)
            {
                Points.Add(point);
            }

            public void Draw(Color color, int width = 1)
            {
                for (var i = 0; i <= Points.Count - 1; i++)
                {
                    var nextIndex = Points.Count - 1 == i ? 0 : i + 1;
                    var from = Drawing.WorldToScreen(Points[i].To3D());
                    var to = Drawing.WorldToScreen(Points[nextIndex].To3D());
                    Drawing.DrawLine(from[0], from[1], to[0], to[1], width, color);
                }
            }

            public bool IsOutside(Vector2 point)
            {
                var p = new IntPoint(point.X, point.Y);
                return Clipper.PointInPolygon(p, ToClipperPath()) != 1;
            }

            public List<IntPoint> ToClipperPath()
            {
                var result = new List<IntPoint>(Points.Count);
                result.AddRange(Points.Select(i => new IntPoint(i.X, i.Y)));
                return result;
            }

            #endregion
        }

        public class Rectangle
        {
            #region Constructors and Destructors

            public Rectangle(Vector2 start, Vector2 end, float width)
            {
                RStart = start;
                REnd = end;
                Width = width;
                Direction = (end - start).Normalized();
                Perpendicular = Direction.Perpendicular();
            }

            #endregion

            #region Public Methods and Operators

            public Polygon ToPolygon(int offset = 0, float overrideWidth = -1)
            {
                var result = new Polygon();
                result.Add(
                    RStart + (overrideWidth > 0 ? overrideWidth : Width + offset) * Perpendicular - offset * Direction);
                result.Add(
                    RStart - (overrideWidth > 0 ? overrideWidth : Width + offset) * Perpendicular - offset * Direction);
                result.Add(
                    REnd - (overrideWidth > 0 ? overrideWidth : Width + offset) * Perpendicular + offset * Direction);
                result.Add(
                    REnd + (overrideWidth > 0 ? overrideWidth : Width + offset) * Perpendicular + offset * Direction);
                return result;
            }

            #endregion

            #region Fields

            public Vector2 Direction;

            public Vector2 Perpendicular;

            public Vector2 REnd;

            public Vector2 RStart;

            public float Width;

            #endregion
        }

        public class Ring
        {
            #region Constructors and Destructors

            public Ring(Vector2 center, float radius, float ringRadius)
            {
                Center = center;
                Radius = radius;
                RingRadius = ringRadius;
            }

            #endregion

            #region Public Methods and Operators

            public Polygon ToPolygon(int offset = 0)
            {
                var result = new Polygon();
                var outRadius = (offset + Radius + RingRadius) / (float) Math.Cos(2 * Math.PI / CircleLineSegmentN);
                var innerRadius = Radius - RingRadius - offset;
                for (var i = 0; i <= CircleLineSegmentN; i++)
                {
                    var angle = i * 2 * Math.PI / CircleLineSegmentN;
                    var point = new Vector2(
                        Center.X - outRadius * (float) Math.Cos(angle), Center.Y - outRadius * (float) Math.Sin(angle));
                    result.Add(point);
                }
                for (var i = 0; i <= CircleLineSegmentN; i++)
                {
                    var angle = i * 2 * Math.PI / CircleLineSegmentN;
                    var point = new Vector2(
                        Center.X + innerRadius * (float) Math.Cos(angle),
                        Center.Y - innerRadius * (float) Math.Sin(angle));
                    result.Add(point);
                }
                return result;
            }

            #endregion

            #region Fields

            public Vector2 Center;

            public float Radius;

            public float RingRadius;

            #endregion
        }

        public class Sector
        {
            #region Constructors and Destructors

            public Sector(Vector2 center, Vector2 direction, float angle, float radius)
            {
                Center = center;
                Direction = direction;
                Angle = angle;
                Radius = radius;
            }

            #endregion

            #region Public Methods and Operators

            public Polygon ToPolygon(int offset = 0)
            {
                var result = new Polygon();
                var outRadius = (Radius + offset) / (float) Math.Cos(2 * Math.PI / CircleLineSegmentN);
                result.Add(Center);
                var side1 = Direction.Rotated(-Angle * 0.5f);
                for (var i = 0; i <= CircleLineSegmentN; i++)
                {
                    var cDirection = side1.Rotated(i * Angle / CircleLineSegmentN).Normalized();
                    result.Add(new Vector2(Center.X + outRadius * cDirection.X, Center.Y + outRadius * cDirection.Y));
                }
                return result;
            }

            #endregion

            #region Fields

            public float Angle;

            public Vector2 Center;

            public Vector2 Direction;

            public float Radius;

            #endregion
        }

        #region Public Methods and Operators

        public static List<List<IntPoint>> ClipPolygons(List<Polygon> polygons)
        {
            var subj = new List<List<IntPoint>>(polygons.Count);
            var clip = new List<List<IntPoint>>(polygons.Count);
            foreach (var polygon in polygons)
            {
                subj.Add(polygon.ToClipperPath());
                clip.Add(polygon.ToClipperPath());
            }
            var solution = new List<List<IntPoint>>();
            var c = new Clipper();
            c.AddPaths(subj, PolyType.ptSubject, true);
            c.AddPaths(clip, PolyType.ptClip, true);
            c.Execute(ClipType.ctUnion, solution, PolyFillType.pftPositive, PolyFillType.pftEvenOdd);
            return solution;
        }

        public static Vector2 PositionAfter(this List<Vector2> self, int t, int speed, int delay = 0)
        {
            var distance = Math.Max(0, t - delay) * speed / 1000;
            for (var i = 0; i <= self.Count - 2; i++)
            {
                var from = self[i];
                var to = self[i + 1];
                var d = (int) to.LSDistance(from);
                if (d > distance)
                {
                    return from + distance * (to - from).Normalized();
                }
                distance -= d;
            }
            return self[self.Count - 1];
        }

        public static Polygon ToPolygon(this List<IntPoint> v)
        {
            var polygon = new Polygon();
            foreach (var point in v)
            {
                polygon.Add(new Vector2(point.X, point.Y));
            }
            return polygon;
        }

        public static List<Polygon> ToPolygons(this List<List<IntPoint>> v)
        {
            return v.Select(i => i.ToPolygon()).ToList();
        }

        #endregion
    }
}