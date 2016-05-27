using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.SDK;
using SharpDX;
using EloBuddy;

namespace Challenger_Series.Utils
{
    public class VectorHelper
    {
        // Credits to furikuretsu from Stackoverflow (http://stackoverflow.com/a/10772759)

        #region ConeCalculations

        public static bool IsLyingInCone(Vector2 position, Vector2 apexPoint, Vector2 circleCenter, double aperture)
        {
            // This is for our convenience
            var halfAperture = aperture / 2;

            // Vector pointing to X point from apex
            var apexToXVect = apexPoint - position;

            // Vector pointing from apex to circle-center point.
            var axisVect = apexPoint - circleCenter;

            // X is lying in cone only if it's lying in 
            // infinite version of its cone -- that is, 
            // not limited by "round basement".
            // We'll use dotProd() to 
            // determine angle between apexToXVect and axis.
            var isInInfiniteCone = DotProd(apexToXVect, axisVect) / Magn(apexToXVect) / Magn(axisVect) >
                // We can safely compare cos() of angles 
                // between vectors instead of bare angles.
                                   Math.Cos(halfAperture);

            if (!isInInfiniteCone)
                return false;

            // X is contained in cone only if projection of apexToXVect to axis
            // is shorter than axis. 
            // We'll use dotProd() to figure projection length.
            var isUnderRoundCap = DotProd(apexToXVect, axisVect) / Magn(axisVect) < Magn(axisVect);

            return isUnderRoundCap;
        }

        private static float DotProd(Vector2 a, Vector2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        private static float Magn(Vector2 a)
        {
            return (float)(Math.Sqrt(a.X * a.X + a.Y * a.Y));
        }

        #endregion

        public static Vector2? GetFirstWallPoint(Vector3 from, Vector3 to, float step = 25)
        {
            return GetFirstWallPoint(from.ToVector2(), to.ToVector2(), step);
        }

        public static Vector2? GetFirstWallPoint(Vector2 from, Vector2 to, float step = 25)
        {
            var direction = (to - from).LSNormalized();

            for (float d = 0; d < from.Distance(to); d = d + step)
            {
                var testPoint = from + d * direction;
                var flags = NavMesh.GetCollisionFlags(testPoint.X, testPoint.Y);
                if (flags.HasFlag(CollisionFlags.Wall) || flags.HasFlag(CollisionFlags.Building))
                {
                    return from + (d - step) * direction;
                }
            }

            return null;
        }

        public static List<Obj_AI_Base> GetDashObjects(IEnumerable<Obj_AI_Base> predefinedObjectList = null)
        {
            var objects = predefinedObjectList != null ? predefinedObjectList.ToList() : ObjectManager.Get<Obj_AI_Base>().Where(o => o.LSIsValidTarget(ObjectManager.Player.AttackRange)).ToList();
            var apexPoint = ObjectManager.Player.ServerPosition.ToVector2() + (ObjectManager.Player.ServerPosition.ToVector2() - Game.CursorPos.ToVector2()).LSNormalized() * ObjectManager.Player.AttackRange;

            return objects.Where(o => IsLyingInCone(o.ServerPosition.ToVector2(), apexPoint, ObjectManager.Player.ServerPosition.ToVector2(), Math.PI)).OrderBy(o => o.DistanceSquared(apexPoint)).ToList();
        }
    }
}
