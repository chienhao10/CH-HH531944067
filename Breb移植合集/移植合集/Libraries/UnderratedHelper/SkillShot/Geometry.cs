#region

using System;
using System.Collections.Generic;
using System.Linq;
using ClipperLib;
using EloBuddy.SDK;
using SharpDX;
using LeagueSharp.Common;

#endregion

namespace UnderratedAIO.Helpers.SkillShot
{
    /// <summary>
    ///     Class that contains the geometry related methods.
    /// </summary>
    public static class SkillshotGeometry
    {
        private const int CircleLineSegmentN = 22;

        /// <summary>
        ///     Returns the position on the path after t milliseconds at speed speed.
        /// </summary>
        public static Vector2 PositionAfter(this List<Vector2> self, int t, int speed, int delay = 0)
        {
            var distance = Math.Max(0, t - delay)*speed/1000;
            for (var i = 0; i <= self.Count - 2; i++)
            {
                var from = self[i];
                var to = self[i + 1];
                var d = (int) to.LSDistance(from);
                if (d > distance)
                {
                    return from + distance*(to - from).Normalized();
                }
                distance -= d;
            }
            return self[self.Count - 1];
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
                var outRadius = overrideWidth > 0
                    ? overrideWidth
                    : (offset + Radius)/(float) Math.Cos(2*Math.PI/CircleLineSegmentN);

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

            public List<IntPoint> ToClipperPath()
            {
                var result = new List<IntPoint>(Points.Count);
                result.AddRange(Points.Select(point => new IntPoint(point.X, point.Y)));

                return result;
            }

            public bool IsOutside(Vector2 point)
            {
                var p = new IntPoint(point.X, point.Y);
                return Clipper.PointInPolygon(p, ToClipperPath()) != 1;
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
                var side1 = Direction.Rotated(-Angle*0.5f);

                for (var i = 0; i <= CircleLineSegmentN; i++)
                {
                    var cDirection = side1.Rotated(i*Angle/CircleLineSegmentN).Normalized();
                    result.Add(new Vector2(Center.X + outRadius*cDirection.X, Center.Y + outRadius*cDirection.Y));
                }

                return result;
            }
        }
    }
}