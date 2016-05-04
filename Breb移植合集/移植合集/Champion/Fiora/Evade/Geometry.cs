using ClipperLib;

namespace FioraProject.Evade
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;
    using EloBuddy;
    public static class Geometry
    {
        #region Constants

        private const int CircleLineSegmentN = 22;

        #endregion

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
                var d = (int)to.LSDistance(from);
                if (d > distance)
                {
                    return from + distance * (to - from).LSNormalized();
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

        public class Arc
        {
            #region Fields

            public float Distance;

            public Vector2 End;

            public int HitBox;

            public Vector2 Start;

            #endregion

            #region Constructors and Destructors

            public Arc(Vector2 start, Vector2 end, int hitbox)
            {
                this.Start = start;
                this.End = end;
                this.HitBox = hitbox;
                this.Distance = this.Start.LSDistance(this.End);
            }

            #endregion

            #region Public Methods and Operators

            public Polygon ToPolygon(int offset = 0)
            {
                offset += this.HitBox;
                var result = new Polygon();
                var innerRadius = -0.1562f * this.Distance + 687.31f;
                var outerRadius = 0.35256f * this.Distance + 133f;
                outerRadius = outerRadius / (float)Math.Cos(2 * Math.PI / CircleLineSegmentN);
                var innerCenters = LeagueSharp.Common.Geometry.CircleCircleIntersection(this.Start,this.End, innerRadius, innerRadius);
                var outerCenters = LeagueSharp.Common.Geometry.CircleCircleIntersection(this.Start,this.End, outerRadius, outerRadius);
                var innerCenter = innerCenters[0];
                var outerCenter = outerCenters[0];
                var direction = (this.End - outerCenter).LSNormalized();
                var end = (this.Start - outerCenter).LSNormalized();
                var maxAngle = (float)(direction.LSAngleBetween(end) * Math.PI / 180);
                var step = -maxAngle / CircleLineSegmentN;
                for (var i = 0; i < CircleLineSegmentN; i++)
                {
                    var angle = step * i;
                    var point = outerCenter + (outerRadius + 15 + offset) * direction.LSRotated(angle);
                    result.Add(point);
                }
                direction = (this.Start - innerCenter).LSNormalized();
                end = (this.End - innerCenter).LSNormalized();
                maxAngle = (float)(direction.LSAngleBetween(end) * Math.PI / 180);
                step = maxAngle / CircleLineSegmentN;
                for (var i = 0; i < CircleLineSegmentN; i++)
                {
                    var angle = step * i;
                    var point = innerCenter + Math.Max(0, innerRadius - offset - 100) * direction.LSRotated(angle);
                    result.Add(point);
                }
                return result;
            }

            #endregion
        }

        public class Circle
        {
            #region Fields

            public Vector2 Center;

            public float Radius;

            #endregion

            #region Constructors and Destructors

            public Circle(Vector2 center, float radius)
            {
                this.Center = center;
                this.Radius = radius;
            }

            #endregion

            #region Public Methods and Operators

            public Polygon ToPolygon(int offset = 0, float overrideWidth = -1)
            {
                var result = new Polygon();
                var outRadius = overrideWidth > 0
                                    ? overrideWidth
                                    : (offset + this.Radius) / (float)Math.Cos(2 * Math.PI / CircleLineSegmentN);
                const double Step = 2 * Math.PI / CircleLineSegmentN;
                var angle = (double)this.Radius;
                for (var i = 0; i <= CircleLineSegmentN; i++)
                {
                    angle += Step;
                    var point = new Vector2(
                        this.Center.X + outRadius * (float)Math.Cos(angle),
                        this.Center.Y + outRadius * (float)Math.Sin(angle));
                    result.Add(point);
                }
                return result;
            }

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
                this.Points.Add(point);
            }

            public void Draw(Color color, int width = 1)
            {
                for (var i = 0; i <= this.Points.Count - 1; i++)
                {
                    var nextIndex = (this.Points.Count - 1 == i) ? 0 : (i + 1);
                    var from = Drawing.WorldToScreen(this.Points[i].To3D());
                    var to = Drawing.WorldToScreen(this.Points[nextIndex].To3D());
                    Drawing.DrawLine(from[0], from[1], to[0], to[1], width, color);
                }
            }

            public bool IsOutside(Vector2 point)
            {
                var p = new IntPoint(point.X, point.Y);
                return Clipper.PointInPolygon(p, this.ToClipperPath()) != 1;
            }

            public List<IntPoint> ToClipperPath()
            {
                var result = new List<IntPoint>(this.Points.Count);
                result.AddRange(this.Points.Select(i => new IntPoint(i.X, i.Y)));
                return result;
            }

            #endregion
        }

        public class Rectangle
        {
            #region Fields

            public Vector2 Direction;

            public Vector2 Perpendicular;

            public Vector2 REnd;

            public Vector2 RStart;

            public float Width;

            #endregion

            #region Constructors and Destructors

            public Rectangle(Vector2 start, Vector2 end, float width)
            {
                this.RStart = start;
                this.REnd = end;
                this.Width = width;
                this.Direction = (end - start).LSNormalized();
                this.Perpendicular = this.Direction.LSPerpendicular();
            }

            #endregion

            #region Public Methods and Operators

            public Polygon ToPolygon(int offset = 0, float overrideWidth = -1)
            {
                var result = new Polygon();
                result.Add(
                    this.RStart + (overrideWidth > 0 ? overrideWidth : this.Width + offset) * this.Perpendicular
                    - offset * this.Direction);
                result.Add(
                    this.RStart - (overrideWidth > 0 ? overrideWidth : this.Width + offset) * this.Perpendicular
                    - offset * this.Direction);
                result.Add(
                    this.REnd - (overrideWidth > 0 ? overrideWidth : this.Width + offset) * this.Perpendicular
                    + offset * this.Direction);
                result.Add(
                    this.REnd + (overrideWidth > 0 ? overrideWidth : this.Width + offset) * this.Perpendicular
                    + offset * this.Direction);
                return result;
            }

            #endregion
        }

        public class Ring
        {
            #region Fields

            public Vector2 Center;

            public float Radius;

            public float RingRadius;

            #endregion

            #region Constructors and Destructors

            public Ring(Vector2 center, float radius, float ringRadius)
            {
                this.Center = center;
                this.Radius = radius;
                this.RingRadius = ringRadius;
            }

            #endregion

            #region Public Methods and Operators

            public Polygon ToPolygon(int offset = 0)
            {
                var result = new Polygon();
                var outRadius = (offset + this.Radius + this.RingRadius)
                                / (float)Math.Cos(2 * Math.PI / CircleLineSegmentN);
                var innerRadius = this.Radius - this.RingRadius - offset;
                for (var i = 0; i <= CircleLineSegmentN; i++)
                {
                    var angle = i * 2 * Math.PI / CircleLineSegmentN;
                    var point = new Vector2(
                        this.Center.X - outRadius * (float)Math.Cos(angle),
                        this.Center.Y - outRadius * (float)Math.Sin(angle));
                    result.Add(point);
                }
                for (var i = 0; i <= CircleLineSegmentN; i++)
                {
                    var angle = i * 2 * Math.PI / CircleLineSegmentN;
                    var point = new Vector2(
                        this.Center.X + innerRadius * (float)Math.Cos(angle),
                        this.Center.Y - innerRadius * (float)Math.Sin(angle));
                    result.Add(point);
                }
                return result;
            }

            #endregion
        }

        public class Sector
        {
            #region Fields

            public float Angle;

            public Vector2 Center;

            public Vector2 Direction;

            public float Radius;

            #endregion

            #region Constructors and Destructors

            public Sector(Vector2 center, Vector2 direction, float angle, float radius)
            {
                this.Center = center;
                this.Direction = direction;
                this.Angle = angle;
                this.Radius = radius;
            }

            #endregion

            #region Public Methods and Operators

            public Polygon ToPolygon(int offset = 0)
            {
                var result = new Polygon();
                var outRadius = (this.Radius + offset) / (float)Math.Cos(2 * Math.PI / CircleLineSegmentN);
                result.Add(this.Center);
                var side1 = this.Direction.LSRotated(-this.Angle * 0.5f);
                for (var i = 0; i <= CircleLineSegmentN; i++)
                {
                    var cDirection = side1.LSRotated(i * this.Angle / CircleLineSegmentN).LSNormalized();
                    result.Add(
                        new Vector2(this.Center.X + outRadius * cDirection.X, this.Center.Y + outRadius * cDirection.Y));
                }
                return result;
            }

            #endregion
        }
    }
}