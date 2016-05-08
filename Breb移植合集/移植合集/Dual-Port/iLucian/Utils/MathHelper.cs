using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;

namespace iLucian.Utils
{
    public class MathHelper
    {
        public class CircInter
        {
            public bool None;
            public bool One;
            public Vector2 Inter1;
            public Vector2 Inter2;

            public CircInter()
            {
                One = false;
                None = false;
                Inter1 = new Vector2();
                Inter2 = new Vector2();
            }

            public Vector2 GetBestInter(Obj_AI_Base target)
            {
                if (None)
                    return new Vector2(0, 0);
                if (One)
                    return Inter1;
                var dist1 = target.LSDistance(Inter1, true);
                var dist2 = target.LSDistance(Inter2, true);

                return dist1 > dist2 ? Inter2 : Inter1;
            }
        }

        public static CircInter GetCicleLineInteraction(Vector2 from, Vector2 to, Vector2 cPos, float radius)
        {
            var res = new CircInter();

            var dx = from.X - to.X;
            var dy = from.Y - to.Y;

            var A = dx * dx + dy * dy;
            var B = 2 * (dx * (to.X - cPos.X) + dy * (to.Y - cPos.Y));
            var C = (to.X - cPos.X) * (to.X - cPos.X) +
                (to.Y - cPos.Y) * (to.Y - cPos.Y) -
                radius * radius;

            var det = B * B - 4 * A * C;
            if ((A <= 0.0000001) || (det < 0))
            {
                res.None = true;
                // No real solutions.
            }
            else if (det == 0)
            {
                res.One = true;
                // One solution.
                var t = -B / (2 * A);
                res.Inter1 =
                    new Vector2(to.X + t * dx, to.Y + t * dy);
            }
            else
            {
                // Two solutions.
                var t = (float)((-B + Math.Sqrt(det)) / (2 * A));
                res.Inter1 =
                    new Vector2(to.X + t * dx, to.Y + t * dy);
                t = (float)((-B - Math.Sqrt(det)) / (2 * A));
                res.Inter2 =
                    new Vector2(to.X + t * dx, to.Y + t * dy);
            }


            return res;
        }
    }
}