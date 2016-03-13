// Copyright 2001 softSurfer, 2012 Dan Sunday
// This code may be freely used and modified for any purpose
// providing that this copyright notice is included with it.
// SoftSurfer makes no warranty for this code, and cannot be held
// liable for any real or imagined damage resulting from its use.
// Users of this code must verify correctness for their application.


// Assume that classes are already given for the objects:
//    Point and Vector with
//        coordinates {float x, y, z;}
//        operators for:
//            Point   = Point ± Vector
//            Vector =  Point - Point
//            Vector =  Vector ± Vector
//            Vector =  Scalar * Vector
//    Line and Segment with defining points {Point  P0, P1;}
//    Track with initial position and velocity vector
//            {Point P0;  Vector v;}
//===================================================================

using System;
using SharpDX;

namespace AdEvade.Utils
{

    class MathUtilsCpa
    {

        public class Line
        {
            public Vector2 P0;
            public Vector2 P1;

            public Line(Vector2 p0, Vector2 p1)
            {
                P0 = p0;
                P1 = p1;
            }
        }

        public class Segment
        {
            public Vector2 P0;
            public Vector2 P1;

            public Segment(Vector2 p0, Vector2 p1)
            {
                P0 = p0;
                P1 = p1;
            }
        }

        public class Track
        {
            public Vector2 P0;
            public Vector2 V;

            public Track(Vector2 p0, Vector2 v)
            {
                P0 = p0;
                V = v;
            }
        }


        const float SmallNum = 0.00000001f; // anything that avoids division overflow

        // dot product (3D) which allows vector operations in arguments
        public static float Dot(Vector2 u, Vector2 v)
        {
            return Vector2.Dot(u, v);//((u).x * (v).x + (u).y * (v).y + (u).z * (v).z)
        }
        public static float Norm(Vector2 v)
        {
            return (float)Math.Sqrt(Dot(v, v));// norm = length of  vector
        }
        public static float D(Vector2 u, Vector2 v)
        {
            return Norm(u - v);        // distance = norm of difference
        }

        public static float Abs(float x)
        {
            return Math.Abs(x); //  absolute value
        }

        // dist3D_Line_to_Line(): get the 3D minimum distance between 2 lines
        //    Input:  two 3D lines L1 and L2
        //    Return: the shortest distance between L1 and L2
        public static float dist3D_Line_to_Line(Line l1, Line l2)
        {

            Vector2 u = l1.P1 - l1.P0;
            Vector2 v = l2.P1 - l2.P0;
            Vector2 w = l1.P0 - l2.P0;
            float a = Dot(u, u);         // always >= 0
            float b = Dot(u, v);
            float c = Dot(v, v);         // always >= 0
            float d = Dot(u, w);
            float e = Dot(v, w);
            float D = a * c - b * b;        // always >= 0
            float sc, tc;

            // compute the line parameters of the two closest points
            if (D < SmallNum)
            {          // the lines are almost parallel
                sc = 0.0f;
                tc = (b > c ? d / b : e / c);    // use the largest denominator
            }
            else
            {
                sc = (b * e - c * d) / D;
                tc = (a * e - b * d) / D;
            }

            // get the difference of the two closest points
            Vector2 dP = w + (sc * u) - (tc * v);  // =  L1(sc) - L2(tc)

            return Norm(dP);   // return the closest distance
        }
        //===================================================================


        // dist3D_Segment_to_Segment(): get the 3D minimum distance between 2 segments
        //    Input:  two 3D line segments S1 and S2
        //    Return: the shortest distance between S1 and S2
        public static float dist3D_Segment_to_Segment(Segment s1, Segment s2)
        {
            Vector2 u = s1.P1 - s1.P0;
            Vector2 v = s2.P1 - s2.P0;
            Vector2 w = s1.P0 - s2.P0;
            float a = Dot(u, u);         // always >= 0
            float b = Dot(u, v);
            float c = Dot(v, v);         // always >= 0
            float d = Dot(u, w);
            float e = Dot(v, w);
            float D = a * c - b * b;        // always >= 0
            float sc, sN, sD = D;       // sc = sN / sD, default sD = D >= 0
            float tc, tN, tD = D;       // tc = tN / tD, default tD = D >= 0

            // compute the line parameters of the two closest points
            if (D < SmallNum)
            { // the lines are almost parallel
                sN = 0.0f;         // force using point P0 on segment S1
                sD = 1.0f;         // to prevent possible division by 0.0 later
                tN = e;
                tD = c;
            }
            else
            {                 // get the closest points on the infinite lines
                sN = (b * e - c * d);
                tN = (a * e - b * d);
                if (sN < 0.0)
                {        // sc < 0 => the s=0 edge is visible
                    sN = 0.0f;
                    tN = e;
                    tD = c;
                }
                else if (sN > sD)
                {  // sc > 1  => the s=1 edge is visible
                    sN = sD;
                    tN = e + b;
                    tD = c;
                }
            }

            if (tN < 0.0)
            {            // tc < 0 => the t=0 edge is visible
                tN = 0.0f;
                // recompute sc for this edge
                if (-d < 0.0)
                    sN = 0.0f;
                else if (-d > a)
                    sN = sD;
                else
                {
                    sN = -d;
                    sD = a;
                }
            }
            else if (tN > tD)
            {      // tc > 1  => the t=1 edge is visible
                tN = tD;
                // recompute sc for this edge
                if ((-d + b) < 0.0)
                    sN = 0;
                else if ((-d + b) > a)
                    sN = sD;
                else
                {
                    sN = (-d + b);
                    sD = a;
                }
            }
            // finally do the division to get sc and tc
            sc = (Abs(sN) < SmallNum ? 0.0f : sN / sD);
            tc = (Abs(tN) < SmallNum ? 0.0f : tN / tD);

            // get the difference of the two closest points
            Vector2 dP = w + (sc * u) - (tc * v);  // =  S1(sc) - S2(tc)

            return Norm(dP);   // return the closest distance
        }
        //===================================================================


        // cpa_time(): compute the time of CPA for two tracks
        //    Input:  two tracks Tr1 and Tr2
        //    Return: the time at which the two tracks are closest
        public static float cpa_time(Track tr1, Track tr2)
        {
            Vector2 dv = tr1.V - tr2.V;

            float dv2 = Dot(dv, dv);
            if (dv2 < SmallNum)      // the  tracks are almost parallel
                return 0.0f;             // any time is ok.  Use time 0.

            Vector2 w0 = tr1.P0 - tr2.P0;
            float cpatime = -Dot(w0, dv) / dv2;

            return cpatime;             // time of CPA
        }
        //===================================================================


        // cpa_distance(): compute the distance at CPA for two tracks
        //    Input:  two tracks Tr1 and Tr2
        //    Return: the distance for which the two tracks are closest
        public static float cpa_distance(Track tr1, Track tr2)
        {
            float ctime = cpa_time(tr1, tr2);
            Vector2 p1 = tr1.P0 + (ctime * tr1.V);
            Vector2 p2 = tr2.P0 + (ctime * tr2.V);

            return D(p1, p2);            // distance at CPA
        }
        //===================================================================

        public static float cpa_distance(Vector2 p1, Vector2 v1, Vector2 p2, Vector2 v2)
        {
            return cpa_distance(new Track(p1, v1), new Track(p2, v2));
        }

        public static float CpaPoints(Vector2 p1, Vector2 v1, Vector2 p2, Vector2 v2, out Vector2 ret1, out Vector2 ret2)
        {
            Track tr1 = new Track(p1, v1);
            Track tr2 = new Track(p2, v2);

            float ctime = cpa_time(tr1, tr2);

            Vector2 P1 = tr1.P0 + (ctime * tr1.V);
            Vector2 P2 = tr2.P0 + (ctime * tr2.V);

            if (ctime <= 0)
            {
                P1 = tr1.P0;
                P2 = tr2.P0;
            }

            ret1 = P1;
            ret2 = P2;

            return D(P1, P2);
        }

        public static float CPAPointsEx(Vector2 p1, Vector2 v1, Vector2 p2, Vector2 v2, Vector2 p1End, Vector2 p2End)
        {
            Track tr1 = new Track(p1, v1);
            Track tr2 = new Track(p2, v2);

            float ctime = Math.Max(0, cpa_time(tr1, tr2));

            Vector2 P1 = tr1.P0 + (ctime * tr1.V);
            Vector2 P2 = tr2.P0 + (ctime * tr2.V);

            P1 = D(p1, P1) > D(p1, p1End) ? p1End : P1;
            P2 = D(p2, P2) > D(p2, p2End) ? p2End : P2;

            return D(P1, P2);
        }

        public static float CPAPointsEx(Vector2 p1, Vector2 v1, Vector2 p2, Vector2 v2, Vector2 p1End, Vector2 p2End,
            out Vector2 p1Out, out Vector2 p2Out)
        {
            Track tr1 = new Track(p1, v1);
            Track tr2 = new Track(p2, v2);

            float ctime = cpa_time(tr1, tr2);

            if (ctime == 0)
            {
                bool collision;
                var collisionTime = MathUtils.GetCollisionTime(p1, p2, v1, v2, 10, 10, out collision);

                if (collision)
                {
                    ctime = collisionTime;
                }
            }

            Vector2 P1 = tr1.P0 + (ctime * tr1.V);
            Vector2 P2 = tr2.P0 + (ctime * tr2.V);

            //P1 = d(p1, P1) > d(p1, p1end) ? p1end : P1;
            //P2 = d(p2, P2) > d(p2, p2end) ? p2end : P2;

            p1Out = P1;//P1.ProjectOn(p1, p1end).SegmentPoint;//P1;
            p2Out = P2;//P2.ProjectOn(p2, p2end).SegmentPoint;

            return D(P1, P2);
        }

        public static float CpaTime(Vector2 p1, Vector2 v1, Vector2 p2, Vector2 v2)
        {
            Track tr1 = new Track(p1, v1);
            Track tr2 = new Track(p2, v2);

            return cpa_time(tr1, tr2);
        }
    }
}
