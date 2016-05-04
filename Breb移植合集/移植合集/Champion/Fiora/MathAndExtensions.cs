using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common.Data;
using ItemData = LeagueSharp.Common.Data.ItemData;
using FioraProject.Evade;

namespace FioraProject
{
    public static class MathAndExtensions
    {
        #region Math And Extensions
        public static int CountMinionsInRange(this Vector3 Position, float Range, bool JungleTrueEnemyFalse)
        {
            return
                MinionManager.GetMinions(Range, MinionTypes.All, JungleTrueEnemyFalse ? MinionTeam.Neutral : MinionTeam.Enemy).Count;
        }
        public static float AngleToRadian(this int Angle)
        {
            return Angle * (float)Math.PI / 180f;
        }
        public static bool InTheCone(this Vector2 pos, Vector2 centerconePolar, Vector2 centerconeEnd, double coneAngle)
        {
            return AngleBetween(pos, centerconePolar, centerconeEnd) < coneAngle / 2;
        }
        public static double AngleBetween(Vector2 a, Vector2 center, Vector2 c)
        {
            float a1 = c.LSDistance(center);
            float b1 = a.LSDistance(c);
            float c1 = center.LSDistance(a);
            if (a1 == 0 || c1 == 0) { return 0; }
            else
            {
                return Math.Acos((a1 * a1 + c1 * c1 - b1 * b1) / (2 * a1 * c1)) * (180 / Math.PI);
            }
        }
        public static Vector2 RotateAround(this Vector2 pointToRotate, Vector2 centerPoint, float angleInRadians)
        {
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Vector2
            {
                X =
                    (float)
                    (cosTheta * (pointToRotate.X - centerPoint.X) -
                    sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y =
                    (float)
                    (sinTheta * (pointToRotate.X - centerPoint.X) +
                    cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }
        public static double AngleBetween(Vector2 a, Vector2 b)
        {
            var Theta1 = Math.Atan2(a.Y, a.X);
            var Theta2 = Math.Atan2(b.Y, b.X);
            var Theta = Math.Abs(Theta1 - Theta2);
            return
                Theta > 180 ? 360 - Theta : Theta;
        }
        #endregion  Math And Extensions
    }
}
