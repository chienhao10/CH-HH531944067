using System;
using EloBuddy.SDK;
using SharpDX;

namespace AdEvade.Utils
{
    class MathUtils
    {
        public static bool CheckLineIntersection(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            return a.Intersection(b, c, d).Intersects;
        }

        public static bool CheckLineIntersectionEx(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            Tuple<float, float> ret = LineToLineIntersection(a.X, a.Y, b.X, b.Y, c.X, c.Y, d.X, d.Y);

            var t1 = ret.Item1;
            var t2 = ret.Item2;

            if (t1 >= 0 && t1 <= 1 && t2 >= 0 && t2 <= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Vector2 CheckLineIntersectionEx2(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            Tuple<float, float> ret = LineToLineIntersection(a.X, a.Y, b.X, b.Y, c.X, c.Y, d.X, d.Y);

            var t1 = ret.Item1;
            var t2 = ret.Item2;

            if (t1 >= 0 && t1 <= 1 && t2 >= 0 && t2 <= 1)
            {
                return new Vector2(t1, t2);
            }
            else
            {
                return Vector2.Zero;
            }
        }

        public static Vector2 RotateVector(Vector2 start, Vector2 end, float angle)
        {
            angle = angle * ((float)(Math.PI / 180));
            Vector2 ret = end;
            ret.X = ((float)Math.Cos(angle) * (end.X - start.X) -
                (float)Math.Sin(angle) * (end.Y - start.Y) + start.X);
            ret.Y = ((float)Math.Sin(angle) * (end.X - start.X) +
                (float)Math.Cos(angle) * (end.Y - start.Y) + start.Y);
            return ret;
        }

        public static Tuple<float, float> LineToLineIntersection(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            var d = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);

            if (d == 0)
            {
                return Tuple.Create(float.MaxValue, float.MaxValue); //lines are parallel or coincidental
            }
            else
            {
                return Tuple.Create(((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / d,
                    ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / d);
            }
        }

        /*
         * 
         * //from leaguesharp.commons
                var spellPos = spell.GetCurrentSpellPosition(true);
                var sol = Geometry.VectorMovementCollision(spellPos, spell.endPos, spell.info.projectileSpeed, heroPos, ObjectCache.myHeroCache.moveSpeed);

                var startTime = 0f;
                var endTime = spellPos.Distance(spell.endPos) / spell.info.projectileSpeed;

                var time = (float) sol[0];
                var pos = (Vector2) sol[1];

                if (pos.IsValid() && time >= startTime && time <= startTime + endTime)
                {
                    return true;
                }
         * 
         */


        public static float VectorMovementCollisionEx(Vector2 targetPos, Vector2 targetDir, float targetSpeed, Vector2 sourcePos, float projSpeed, out bool collision, float extraDelay = 0, float extraDist = 0)
        {
            Vector2 velocity = targetDir * targetSpeed;
            targetPos = targetPos - velocity * (extraDelay / 1000);

            float velocityX = velocity.X;
            float velocityY = velocity.Y;

            Vector2 relStart = targetPos - sourcePos;

            float relStartX = relStart.X;
            float relStartY = relStart.Y;

            float a = velocityX * velocityX + velocityY * velocityY - projSpeed * projSpeed;
            float b = 2 * velocityX * relStartX + 2 * velocityY * relStartY;
            float c = Math.Max(0, relStartX * relStartX + relStartY * relStartY + extraDist * extraDist);

            float disc = b * b - 4 * a * c;

            if (disc >= 0)
            {
                float t1 = -(b + (float)Math.Sqrt(disc)) / (2 * a);
                float t2 = -(b - (float)Math.Sqrt(disc)) / (2 * a);

                collision = true;

                if (t1 > 0 && t2 > 0)
                {
                    return (t1 > t2) ? t2 : t1;

                }
                else if (t1 > 0)
                    return t1;
                else if (t2 > 0)
                    return t2;
            }

            collision = false;

            return 0;
        }

        public static bool PointOnLineSegment(Vector2 point, Vector2 start, Vector2 end)
        {
            var dotProduct = Vector2.Dot((end - start), (point - start));
            if (dotProduct < 0)
                return false;

            var lengthSquared = Vector2.DistanceSquared(start, end);
            if (dotProduct > lengthSquared)
                return false;

            return true;
        }

        public static bool IsPointOnLineSegment(Vector2 point, Vector2 start, Vector2 end)
        {
            if (Math.Max(start.X, end.X) > point.X && point.X > Math.Min(start.X, end.X)
                && Math.Max(start.Y, end.Y) > point.Y && point.Y > Math.Min(start.Y, end.Y))
            {
                return true;
            }

            return false;
        }

        //https://code.google.com/p/xna-circle-collision-detection/downloads/detail?name=Circle%20Collision%20Example.zip&can=2&q=

        public static float GetCollisionTime(Vector2 pa, Vector2 pb, Vector2 va, Vector2 vb, float ra, float rb, out bool collision)
        {
            Vector2 pab = pa - pb;
            Vector2 vab = va - vb;
            float a = Vector2.Dot(vab, vab);
            float b = 2 * Vector2.Dot(pab, vab);
            float c = Vector2.Dot(pab, pab) - (ra + rb) * (ra + rb);

            float discriminant = b * b - 4 * a * c;

            float t;
            if (discriminant < 0)
            {
                t = -b / (2 * a);
                collision = false;
            }
            else
            {
                float t0 = (-b + (float)Math.Sqrt(discriminant)) / (2 * a);
                float t1 = (-b - (float)Math.Sqrt(discriminant)) / (2 * a);

                if (t0 >= 0 && t1 >= 0)
                    t = Math.Min(t0, t1);
                else
                    t = Math.Max(t0, t1);

                if (t < 0)
                    collision = false;
                else
                    collision = true;
            }

            if (t < 0)
                t = 0;

            return t;
        }

        public static float GetCollisionDistanceEx(Vector2 pa, Vector2 va, float ra,
                                                   Vector2 pb, Vector2 vb, float rb,
                                                   out Vector2 PA, out Vector2 PB)
        {
            bool collision;
            var collisionTime = GetCollisionTime(pa, pb, va, vb, ra, rb, out collision);

            if (collision)
            {
                PA = pa + (collisionTime * va);
                PB = pb + (collisionTime * vb);

                return PA.Distance(PB);
            }

            PA = Vector2.Zero;
            PB = Vector2.Zero;

            return float.MaxValue;
        }

        public static float GetCollisionDistance(Vector2 pa, Vector2 paEnd, Vector2 va, float ra,
                                                 Vector2 pb, Vector2 pbEnd, Vector2 vb, float rb)
        {
            bool collision;
            var collisionTime = GetCollisionTime(pa, pb, va, vb, ra, rb, out collision);

            if (collision)
            {
                Vector2 PA = pa + (collisionTime * va);
                Vector2 PB = pb + (collisionTime * vb);

                PA = PA.ProjectOn(pa, paEnd).SegmentPoint;
                PB = PB.ProjectOn(pb, pbEnd).SegmentPoint;

                return PA.Distance(PB);
            }

            return float.MaxValue;
        }
        
        //http://csharphelper.com/blog/2014/09/determine-where-a-line-intersects-a-circle-in-c/
        // Find the points of intersection.
        public static int FindLineCircleIntersections(
            Vector2 center, float radius,
            Vector2 from, Vector2 to,
            out Vector2 intersection1, out Vector2 intersection2)
        {
            float cx = center.X;
            float cy = center.Y;
            float dx, dy, a, b, c, det, t;

            dx = to.X - from.X;
            dy = to.Y - from.Y;

            a = dx * dx + dy * dy;
            b = 2 * (dx * (from.X - cx) + dy * (from.Y - cy));
            c = (from.X - cx) * (from.X - cx) +
                (from.Y - cy) * (from.Y - cy) -
                radius * radius;

            det = b * b - 4 * a * c;
            if ((a <= 0.0000001) || (det < 0))
            {
                // No real solutions.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 0;
            }
            else if (det == 0)
            {
                // One solution.
                t = -b / (2 * a);
                intersection1 =
                    new Vector2(from.X + t * dx, from.Y + t * dy);
                intersection2 = new Vector2(float.NaN, float.NaN);

                var projection1 = intersection1.ProjectOn(from, to);
                if (projection1.IsOnSegment)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                // Two solutions.
                t = (float)((-b + Math.Sqrt(det)) / (2 * a));
                intersection1 =
                    new Vector2(from.X + t * dx, from.Y + t * dy);
                t = (float)((-b - Math.Sqrt(det)) / (2 * a));
                intersection2 =
                    new Vector2(from.X + t * dx, from.Y + t * dy);

                var projection1 = intersection1.ProjectOn(from, to);
                var projection2 = intersection2.ProjectOn(from, to);

                if (projection1.IsOnSegment && projection2.IsOnSegment)
                {
                    return 2;
                }
                else if (projection1.IsOnSegment && !projection2.IsOnSegment)
                {
                    return 1;
                }
                else if (!projection1.IsOnSegment && projection2.IsOnSegment)
                {
                    intersection1 = intersection2;
                    return 1;
                }

                return 0;
            }
        }
    }
}
