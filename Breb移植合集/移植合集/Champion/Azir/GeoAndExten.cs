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

namespace HeavenStrikeAzir
{
    public static class GeoAndExten
    {
        public static AIHeroClient Player { get{ return ObjectManager.Player; } }
        public static Vector2? GetFirstWallPoint(Vector2 from, Vector2 to, float step = 25)
        {
            var direction = (to - from).Normalized();

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
        public static Vector2? GetLastWallPoint(Vector2 from, Vector2 to, float step = 25)
        {
            var direction = (to - from).Normalized();
            var Fstwall = GetFirstWallPoint(from, to);
            if (Fstwall != null)
            {
                var firstwall = ((Vector2)Fstwall);
                for (float d = step; d < firstwall.Distance(to) + 1000; d = d + step)
                {
                    var testPoint = firstwall + d * direction;
                    var flags = NavMesh.GetCollisionFlags(testPoint.X, testPoint.Y);
                    if (!flags.HasFlag(CollisionFlags.Wall) && !flags.HasFlag(CollisionFlags.Building))
                    //if (!testPoint.IsWall())
                    {
                        return firstwall + d * direction;
                    }
                }
            }

            return null;
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
        public static Vector2? GetWPosition(Vector2 to)
        {
            var posW = Player.Position.To2D().LSExtend(to, Program._w.Range);
            var FstWall = GetFirstWallPoint(Player.Position.To2D(), posW);
            if (FstWall == null)
                return posW;
            var LstWall = GetLastWallPoint(Player.Position.To2D(), posW);
            if (LstWall == null)
                return posW;
            if (posW.Distance((Vector2)LstWall) / ((Vector2)FstWall).Distance((Vector2)LstWall) <= 0.5f)
                return (Vector2)LstWall;
            return null;
        }
        public static List<Vector2?> GetWsPosition(Vector2 to)
        {
            var posW = Player.Position.To2D().LSExtend(to, Program._w.Range);
            var rad = new double[] { -Math.PI / 2, Math.PI / 2, -Math.PI / 4, Math.PI / 4, 0 };
            var result = new List<Vector2?>();
            foreach (var i in rad)
            {
                result.Add(GetWPosition(GeoAndExten.RotateAround(posW, Player.Position.To2D(), (float)i)));
            }
            return result;
        }
    }
}
