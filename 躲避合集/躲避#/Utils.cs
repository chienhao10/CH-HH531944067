using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Color = System.Drawing.Color;

namespace EvadePlus
{
    internal static class Utils
    {
        private static Random _random;

        public static Random Random
        {
            get
            {
                if (_random == null)
                {
                    _random = new Random();
                }

                return _random;
            }
        }

        #region "Extensions"

        public static float HitBoxRadius(this Obj_AI_Base unit)
        {
            return unit.BoundingRadius;
        }

        public static bool IndexEquals(this GameObject obj1, GameObject obj2)
        {
            return obj1.Index == obj2.Index;
        }

        public static bool IsWalking(this Obj_AI_Base unit)
        {
            return unit.IsMoving;
        }

        public static Vector3 Destination(this Obj_AI_Base unit)
        {
            return unit.Path.Last();
        }

        public static int WalkingTime(this Obj_AI_Base unit, Vector2 point)
        {
            return (int) (1000 * unit.ServerPosition.Distance(point) / unit.MoveSpeed);
        }

        public static int WalkingTime(this Obj_AI_Base unit, Vector2 start, Vector2 end)
        {
            return (int) (1000 * start.Distance(end) / unit.MoveSpeed);
        }

        public static bool IsMovingTowards(this Obj_AI_Base unit, Vector3 position)
        {
            return unit.IsWalking() && unit.Path.Last().Distance(position, true) <= 50.Pow();
        }

        public static bool IsMovingTowards(this Obj_AI_Base unit, Vector2 position)
        {
            return unit.IsMovingTowards(position.To3DWorld());
        }

        public static Vector3 To3DPlayer(this Vector2 vector)
        {
            return new Vector3(vector.X, vector.Y, Player.Instance.Position.Z);
        }

        public static bool IsWall(this Vector2 vector)
        {
            return NavMesh.GetCollisionFlags(vector.X, vector.Y).HasFlag(CollisionFlags.Wall);
        }

        public static Vector3 ExtendVector3(this Vector3 vector, Vector3 direction, float distance)
        {
            if (vector.To2D().Distance(direction.To2D(), true) == 0)
            {
                return vector;
            }

            var edge = direction.To2D() - vector.To2D();
            edge.Normalize();

            var v = vector.To2D() + edge * distance;
            return new Vector3(v.X, v.Y, vector.Z);
        }

        public static Vector2[] ToVector2(this IEnumerable<Vector3> source)
        {
            return source.Select(v => v.To2D()).ToArray();
        }

        public static Vector3[] ToVector3(this IEnumerable<Vector2> source)
        {
            return source.Select(v => v.To3DWorld()).ToArray();
        }

        public static float Distance(this Vector2 point, Vector2 segmentStart, Vector2 segmentEnd, bool squared = false)
        {
            var a =
                Math.Abs((segmentEnd.Y - segmentStart.Y) * point.X - (segmentEnd.X - segmentStart.X) * point.Y +
                         segmentEnd.X * segmentStart.Y - segmentEnd.Y * segmentStart.X);
            return (squared ? a * a : a) / segmentStart.Distance(segmentEnd, squared);
        }

        public static bool IsInLineSegment(this Vector2 point, Vector2 segmentStart, Vector2 segmentEnd,
            float tolerance = 0)
        {
            var d = segmentStart.Distance(segmentEnd, true);
            return point.Distance(segmentEnd, true) <= d + tolerance &&
                   point.Distance(segmentStart, true) <= d + tolerance;
        }

        public static Geometry.Polygon ToPolygon(this IEnumerable<Vector2> points)
        {
            var polygon = new Geometry.Polygon();
            polygon.Points.AddRange(points);
            return polygon;
        }

        public static Geometry.Polygon ToDetailedPolygon(this Geometry.Polygon polygon, float tolerance = 80)
        {
            var list = new List<Vector2>();

            for (var i = 0; i < polygon.Points.Count; i++)
            {
                var lineStart = polygon.Points[i];
                var lineEnd = (i + 1 == polygon.Points.Count) ? polygon.Points[0] : polygon.Points[i + 1];

                if (lineStart.Distance(lineEnd, true) > tolerance.Pow())
                {
                    var condition = (int) lineStart.Distance(lineEnd) / tolerance;
                   
                    for (var u = 0; u < condition; u++)
                    {
                        list.Add(lineStart.Extend(lineEnd, u * tolerance));
                    }
                }
                else
                {
                    list.Add(lineStart);
                }
            }
            return list.ToPolygon();
        }

        public static void DrawPolygon(this Geometry.Polygon polygon, Color color, int lineWidth = 1)
        {
            for (var i = 0; i < polygon.Points.Count; i++)
            {
                var lineStart = polygon.Points[i];
                var lineEnd = (i + 1 == polygon.Points.Count) ? polygon.Points[0] : polygon.Points[i + 1];

                Line.DrawLine(color, lineWidth + 1, lineStart.To3DPlayer(), lineEnd.To3DPlayer());
            }
        }

        public static bool IsIntersectingWithLineSegment(this Geometry.Polygon polygon, Vector2 segmentStart, Vector2 segmentEnd)
        {
            for (var i = 0; i < polygon.Points.Count; i++)
            {
                var lineStart = polygon.Points[i];
                var lineEnd = (i + 1 == polygon.Points.Count) ? polygon.Points[0] : polygon.Points[i + 1];

                bool intersection;
                GetLineSegmentsIntersectionPoint(lineStart, lineEnd, segmentStart, segmentEnd, out intersection);

                if (intersection)
                {
                    return true;
                }
            }

            return false;
        }

        public static Vector2[] GetIntersectionPointsWithLineSegment(this Geometry.Polygon polygon, Vector2 segmentStart, Vector2 segmentEnd)
        {
            var points = new List<Vector2>();

            for (var i = 0; i < polygon.Points.Count; i++)
            {
                var lineStart = polygon.Points[i];
                var lineEnd = (i + 1 == polygon.Points.Count) ? polygon.Points[0] : polygon.Points[i + 1];

                bool intersection;
                var point = GetLineSegmentsIntersectionPoint(lineStart, lineEnd, segmentStart, segmentEnd, out intersection);

                if (intersection)
                {
                    points.Add(point);
                }
            }

            return points.ToArray();
        }

        public static IEnumerable<Vector2> GetSortedPath(this List<Vector2> path, Vector2 start)
        {
            var list = new List<Vector2>();
            var current = start;

            while (path.Any())
            {
                if (path.Count > 1)
                    path.Sort((p1, p2) => p1.Distance(current, true).CompareTo(p2.Distance(current, true)));

                var next = path.First();
                list.Add(next);
                current = next;
                path.RemoveAt(0);
            }

            return list;
        }

        public static void SortPath(this List<Vector2> path, Vector2 start)
        {
            var list = path.GetSortedPath(start);
            path.Clear();
            path.AddRange(list);
        }

        #endregion

        public static GameObjectTeam PlayerTeam()
        {
            return Player.Instance.Team;
        }

        public static string GetGameObjectName(GameObject obj)
        {
            switch (obj.Type)
            {
                case GameObjectType.MissileClient:
                    return ((MissileClient) obj).SData.Name;
                case GameObjectType.obj_AI_Minion:
                    return ((Obj_AI_Minion) obj).BaseSkinName;              
                default:
                    return obj.Name;
            }
        }

        public static GameObjectTeam GetGameObjectTeam(GameObject obj)
        {
            var missile = obj as MissileClient;
            return missile == null ? obj.Team : missile.SpellCaster.Team;
        }

        public static void Draw3DRect(Vector3 start, Vector3 end, float width, Color color, int lineWidth = 2, bool drawStartLine = true)
        {
            var halfWidth = width / 2;
            var d1 = start.To2D();
            var d2 = end.To2D();
            var direction = (d1 - d2).Perpendicular().Normalized();

            Line.DrawLine(color, lineWidth, (d1 + direction * halfWidth).To3DPlayer(),
                (d2 + direction * halfWidth).To3DPlayer());
            Line.DrawLine(color, lineWidth, (d1 + direction * -halfWidth).To3DPlayer(),
                (d2 + direction * -halfWidth).To3DPlayer());
            Line.DrawLine(color, lineWidth, (d1 + direction * halfWidth).To3DPlayer(),
                (d1 + direction * -halfWidth).To3DPlayer());
            Line.DrawLine(color, lineWidth, (d2 + direction * halfWidth).To3DPlayer(),
                (d2 + direction * -halfWidth).To3DPlayer());

            if (drawStartLine)
            {
                Line.DrawLine(Color.LawnGreen, lineWidth,
                    (d1 + direction * (halfWidth + Player.Instance.BoundingRadius)).To3DPlayer(),
                    (d1 + direction * -(halfWidth + Player.Instance.BoundingRadius)).To3DPlayer());
            }
        }

        public static void DrawPath(Vector2[] path, Color color, int lineWidth = 1)
        {
            for (var i = 0; i < path.Length - 1; i++)
            {
                var lineStart = path[i];
                var lineEnd = path[i + 1];

                Line.DrawLine(color, lineWidth, lineStart.To3DPlayer(), lineEnd.To3DPlayer());
            }
        }

        public static Vector2[] GetLineCircleIntersectionPoints(Vector2 center, float radius, Vector2 segmentStart, Vector2 segmentEnd)
        {
            float t;

            var dx = segmentEnd.X - segmentStart.X;
            var dy = segmentEnd.Y - segmentStart.Y;

            var a = dx * dx + dy * dy;
            var b = 2 * (dx * (segmentStart.X - center.X) + dy * (segmentStart.Y - center.Y));
            var c = (segmentStart.X - center.X) * (segmentStart.X - center.X) +
                    (segmentStart.Y - center.Y) * (segmentStart.Y - center.Y) -
                    radius * radius;

            var det = b * b - 4 * a * c;
            if ((a <= 0.0000001) || (det < 0))
            {
                return new Vector2[] {};
            }
            if (det == 0)
            {
                t = -b / (2 * a);
                return new[]
                {new Vector2(segmentStart.X + t * dx, segmentStart.Y + t * dy)};
            }
            t = (float) ((-b + Math.Sqrt(det)) / (2 * a));
            var t2 = (float) ((-b - Math.Sqrt(det)) / (2 * a));
            return new[]
            {
                new Vector2(segmentStart.X + t * dx, segmentStart.Y + t * dy),
                new Vector2(segmentStart.X + t2 * dx, segmentStart.Y + t2 * dy)
            };
        }

        public static Vector2[] GetPointsWithDistance(Vector2 segmentStart, Vector2 segmentEnd, Vector2 point, float distance)
        {
            var d = distance.Pow();
            var a = (segmentEnd.Y - segmentStart.Y) / (segmentEnd.X - segmentStart.X);
            var b = segmentStart.Y - a * segmentStart.X;

            var x =
                (float)
                    (-Math.Sqrt(a.Pow() * d - a.Pow() * point.X - 2 * a * b * point.X + 2 * a * point.X * point.Y -
                                b.Pow() +
                                2 * b * point.Y + d - point.Y.Pow()) - a * b + a * point.Y + point.X) / (a.Pow() + 1);
            var y = (float) (-a *
                             Math.Sqrt(a.Pow() * d - a.Pow() * point.X.Pow() - 2 * a * b * point.X +
                                       2 * a * point.X * point.Y - b.Pow() +
                                       2 * b * point.Y + d - point.Y.Pow()) + a.Pow() * point.Y + a * point.X + b) /
                    (a.Pow() + 1);

            return new[] {new Vector2(x, y), new Vector2(-x, -y)};
            //.Where(p => p.IsInLineSegment(segmentStart, segmentEnd)).ToArray()
        }

        public static Vector2 GetLinesIntersectionPoint(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2, out bool intersection)
        {
            var a1 = end1.Y - start1.Y;
            var a2 = end2.Y - start2.Y;
            var b1 = start1.X - end1.X;
            var b2 = start2.X - end2.X;
            var c1 = start1.X * a1 + b1 * start1.Y;
            var c2 = start2.X * a2 + b2 * start2.Y;

            var det = a1 * b2 - a2 * b1;
            if (det == 0)
            {
                intersection = false;
                return Vector2.Zero;
            }

            intersection = true;
            return new Vector2((b2 * c1 - b1 * c2) / det, (a1 * c2 - a2 * c1) / det);
        }

        public static Vector2 GetLineSegmentsIntersectionPoint(Vector2 segmentStart1, Vector2 segmentEnd1,
            Vector2 segmentStart2, Vector2 segmentEnd2, out bool intersection)
        {
            bool intersect;
            var point = GetLinesIntersectionPoint(segmentStart1, segmentEnd1, segmentStart2, segmentEnd2, out intersect);

            if (intersect && point.IsInLineSegment(segmentStart1, segmentEnd1) &&
                point.IsInLineSegment(segmentStart2, segmentEnd2))
            {
                intersection = true;
                return point;
            }

            intersection = false;
            return point;
        }
    }

    public class LinkedList<T>
    {
        private readonly List<T> _list = new List<T>();

        public int Index;

        public LinkedList(IEnumerable<T> elements, int index = 0)
        {
            _list.AddRange(elements);
            Index = index;
        }

        public T Next()
        {
            var element = _list[Index];

            if (Index + 1 < _list.Count)
            {
                Index++;
            }
            else
            {
                Index = 0;
            }

            return element;
        }

        public T Previous()
        {
            var element = _list[Index];

            if (Index > 0)
            {
                Index--;
            }
            else
            {
                Index = _list.Count - 1;
            }

            return element;
        }
    }
}