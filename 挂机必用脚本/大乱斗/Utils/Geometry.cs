using System;
using System.Collections.Generic;
using ClipperLib;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using Color = System.Drawing.Color;
using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;
using GamePath = System.Collections.Generic.List<SharpDX.Vector2>;

namespace AutoSharp.Utils
{
    public static class Geometry
    {

        public static void DrawLineInWorld(Vector3 start, Vector3 end, int width, Color color)
        {
            var from = Drawing.WorldToScreen(start);
            var to = Drawing.WorldToScreen(end);
            Drawing.DrawLine(from[0], from[1], to[0], to[1], width, color);
            //Drawing.DrawLine(from.X, from.Y, to.X, to.Y, width, color);
        }

        public static float RadianToDegree(double angle)
        {
            return (float) (angle*(180.0/Math.PI));
        }

        public static float DegreeToRadian(double angle)
        {
            return (float) (Math.PI*angle/180.0);
        }

        private const int CircleLineSegmentN = 22;

        // ReSharper disable once InconsistentNaming
        public static Vector3 SwitchYZ(this Vector3 v)
        {
            return new Vector3(v.X, v.Z, v.Y);
        }

//Clipper
        public static List<Polygon> ToPolygons(this Paths v)
        {
            var result = new List<Polygon>();
            foreach (var path in v)
            {
                result.Add(path.ToPolygon());
            }
            return result;
        }

        /// <summary>
        /// Returns the position on the path after t milliseconds at speed speed.
        /// </summary>
        public static Vector2 PositionAfter(this GamePath self, int t, int speed, int delay = 0)
        {
            var distance = Math.Max(0, t - delay)*speed/1000;
            for (var i = 0; i <= self.Count - 2; i++)
            {
                var from = self[i];
                var to = self[i + 1];
                var d = (int) to.Distance(from);
                if (d > distance)
                {
                    return from + distance*(to - from).Normalized();
                }
                distance -= d;
            }
            return self[self.Count - 1];
        }

        public static Polygon ToPolygon(this Path v)
        {
            var polygon = new Polygon();
            foreach (var point in v)
            {
                polygon.Add(new Vector2(point.X, point.Y));
            }
            return polygon;
        }

        public static Paths ClipPolygons(List<Polygon> polygons)
        {
            var subj = new Paths(polygons.Count);
            var clip = new Paths(polygons.Count);
            foreach (var polygon in polygons)
            {
                subj.Add(polygon.ToClipperPath());
                clip.Add(polygon.ToClipperPath());
            }
            var solution = new Paths();
            var c = new Clipper();
            c.AddPaths(subj, PolyType.ptSubject, true);
            c.AddPaths(clip, PolyType.ptClip, true);
            c.Execute(ClipType.ctUnion, solution, PolyFillType.pftPositive, PolyFillType.pftEvenOdd);
            return solution;
        }

        public class Circle
        {
            public Vector2 Center;
            public float Radius;

            public Circle(Vector2 center, float radius)
            {
                Center = center;
                Radius = radius;
            }

            public Polygon ToPolygon(int offset = 0, float overrideWidth = -1)
            {
                var result = new Polygon();
                var outRadius = (overrideWidth > 0
                    ? overrideWidth
                    : (offset + Radius)/(float) Math.Cos(2*Math.PI/CircleLineSegmentN));
                for (var i = 1; i <= CircleLineSegmentN; i++)
                {
                    var angle = i*2*Math.PI/CircleLineSegmentN;
                    var point = new Vector2(
                        Center.X + outRadius*(float) Math.Cos(angle), Center.Y + outRadius*(float) Math.Sin(angle));
                    result.Add(point);
                }
                return result;
            }
        }

        public class Polygon
        {
            public List<Vector2> Points = new List<Vector2>();

            public void Add(Vector2 point)
            {
                Points.Add(point);
            }

            public Path ToClipperPath()
            {
                var result = new Path(Points.Count);
                foreach (var point in Points)
                {
                    result.Add(new IntPoint(point.X, point.Y));
                }
                return result;
            }

            public bool IsOutside(Vector2 point)
            {
                var p = new IntPoint(point.X, point.Y);
                return Clipper.PointInPolygon(p, ToClipperPath()) != 1;
            }

            public void Draw(Color color, int width = 1)
            {
                for (var i = 0; i <= Points.Count - 1; i++)
                {
                    var nextIndex = (Points.Count - 1 == i) ? 0 : (i + 1);
                    DrawLineInWorld(Points[i].To3D(), Points[nextIndex].To3D(), width, color);
                }
            }
        }

        public class Rectangle
        {
            public Vector2 Direction;
            public Vector2 Perpendicular;
            public Vector2 REnd;
            public Vector2 RStart;
            public float Width;

            public Rectangle(Vector2 start, Vector2 end, float width)
            {
                RStart = start;
                REnd = end;
                Width = width;
                Direction = (end - start).Normalized();
                Perpendicular = Direction.Perpendicular();
            }

            public Polygon ToPolygon(int offset = 0, float overrideWidth = -1)
            {
                var result = new Polygon();
                result.Add(
                    RStart + (overrideWidth > 0 ? overrideWidth : Width + offset)*Perpendicular - offset*Direction);
                result.Add(
                    RStart - (overrideWidth > 0 ? overrideWidth : Width + offset)*Perpendicular - offset*Direction);
                result.Add(
                    REnd - (overrideWidth > 0 ? overrideWidth : Width + offset)*Perpendicular + offset*Direction);
                result.Add(
                    REnd + (overrideWidth > 0 ? overrideWidth : Width + offset)*Perpendicular + offset*Direction);
                return result;
            }
        }

        public class Ring
        {
            public Vector2 Center;
            public float Radius;
            public float RingRadius; //actually radius width.

            public Ring(Vector2 center, float radius, float ringRadius)
            {
                Center = center;
                Radius = radius;
                RingRadius = ringRadius;
            }

            public Polygon ToPolygon(int offset = 0)
            {
                var result = new Polygon();
                var outRadius = (offset + Radius + RingRadius)/(float) Math.Cos(2*Math.PI/CircleLineSegmentN);
                var innerRadius = Radius - RingRadius - offset;
                for (var i = 0; i <= CircleLineSegmentN; i++)
                {
                    var angle = i*2*Math.PI/CircleLineSegmentN;
                    var point = new Vector2(
                        Center.X - outRadius*(float) Math.Cos(angle), Center.Y - outRadius*(float) Math.Sin(angle));
                    result.Add(point);
                }
                for (var i = 0; i <= CircleLineSegmentN; i++)
                {
                    var angle = i*2*Math.PI/CircleLineSegmentN;
                    var point = new Vector2(
                        Center.X + innerRadius*(float) Math.Cos(angle),
                        Center.Y - innerRadius*(float) Math.Sin(angle));
                    result.Add(point);
                }
                return result;
            }
        }

        public class Sector
        {
            public float Angle;
            public Vector2 Center;
            public Vector2 Direction;
            public float Radius;

            public Sector(Vector2 center, Vector2 direction, float angle, float radius)
            {
                Center = center;
                Direction = direction;
                Angle = angle;
                Radius = radius;
            }

            public Polygon ToPolygon(int offset = 0)
            {
                var result = new Polygon();
                var outRadius = (Radius + offset)/(float) Math.Cos(2*Math.PI/CircleLineSegmentN);
                result.Add(Center);
                // ReSharper disable once InconsistentNaming
                var Side1 = Direction.Rotated(-Angle*0.5f);
                for (var i = 0; i <= CircleLineSegmentN; i++)
                {
                    var cDirection = Side1.Rotated(i*Angle/CircleLineSegmentN).Normalized();
                    result.Add(new Vector2(Center.X + outRadius*cDirection.X, Center.Y + outRadius*cDirection.Y));
                }
                return result;
            }


        }
        public class Arc : Polygon
        {
            public float Angle;
            public Vector2 EndPos;
            public float Radius;
            public Vector2 StartPos;
            private readonly int _quality;

            public Arc(Vector3 start, Vector3 direction, float angle, float radius, int quality = 20)
                : this(start.To2D(), direction.To2D(), angle, radius, quality) { }

            public Arc(Vector2 start, Vector2 direction, float angle, float radius, int quality = 20)
            {
                StartPos = start;
                EndPos = (direction - start).Normalized();
                Angle = angle;
                Radius = radius;
                _quality = quality;
                UpdatePolygon();
            }

            public void UpdatePolygon(int offset = 0)
            {
                Points.Clear();
                var outRadius = (Radius + offset) / (float)Math.Cos(2 * Math.PI / _quality);
                var side1 = EndPos.Rotated(-Angle * 0.5f);
                for (var i = 0; i <= _quality; i++)
                {
                    var cDirection = side1.Rotated(i * Angle / _quality).Normalized();
                    Points.Add(
                        new Vector2(StartPos.X + outRadius * cDirection.X, StartPos.Y + outRadius * cDirection.Y));
                }
            }
        }
    }
}