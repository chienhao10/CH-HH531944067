using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common;
using SharpDX;

namespace SAutoCarry.Champions.Helpers
{
    public static class Tumble
    {
        public static Vector3 FindTumblePosition(AIHeroClient target)
        {
            if ((Vayne.Program.Only2W) && target.GetBuffCount("vaynesilvereddebuff") == 1) // == 1 cuz calling this after attack which is aa missile still flying
                return Vector3.Zero;

            if (Vayne.Program.Wall)
            {
                var outRadius = ObjectManager.Player.BoundingRadius / (float)Math.Cos(2 * Math.PI / 8);

                for (var i = 1; i <= 8; i++)
                {
                    var angle = i * 2 * Math.PI / 8;
                    float x = ObjectManager.Player.Position.X + outRadius * (float)Math.Cos(angle);
                    float y = ObjectManager.Player.Position.Y + outRadius * (float)Math.Sin(angle);
                    var colFlags = NavMesh.GetCollisionFlags(x, y);
                    if (colFlags.HasFlag(CollisionFlags.Wall) || colFlags.HasFlag(CollisionFlags.Building))
                        return new Vector3(x, y, 0);
                }
            }

            if (Vayne.Program.sacMode == 0)
            {
                Vector3 vec = target.ServerPosition;

                if (target.Path.Length > 0)
                {
                    if (ObjectManager.Player.LSDistance(vec) < ObjectManager.Player.LSDistance(target.Path.Last()))
                        return IsSafe(target, Game.CursorPos);
                    else
                        return IsSafe(target, Game.CursorPos.To2D().LSRotated(LeagueSharp.Common.Geometry.DegreeToRadian((vec - ObjectManager.Player.ServerPosition).To2D().LSAngleBetween((Game.CursorPos - ObjectManager.Player.ServerPosition).To2D()) % 90)).To3D());
                }
                else
                {
                    if (target.IsMelee)
                        return IsSafe(target, Game.CursorPos);
                }

                return IsSafe(target, ObjectManager.Player.ServerPosition + (target.ServerPosition - ObjectManager.Player.ServerPosition).LSNormalized().To2D().LSRotated(LeagueSharp.Common.Geometry.DegreeToRadian(90 - (vec - ObjectManager.Player.ServerPosition).To2D().LSAngleBetween((Game.CursorPos - ObjectManager.Player.ServerPosition).To2D()))).To3D() * 300f);
            }
            else if (Vayne.Program.sacMode == 1)
            {
                return Game.CursorPos;
            }

            return Vector3.Zero;
        }

        public static Vector3 IsSafe(AIHeroClient target, Vector3 vec, bool checkTarget = true)
        {
            if (Vayne.Program.DontSafeCheck)
                return vec;

            if (checkTarget)
            {
                if (target.ServerPosition.To2D().Distance(vec) <= target.AttackRange)
                {
                    if (vec.CountEnemiesInRange(1000) > 1)
                        return Vector3.Zero;
                    else if (target.ServerPosition.To2D().LSDistance(vec) <= target.AttackRange / 2f)
                        return SCommon.Maths.Geometry.Deviation(ObjectManager.Player.ServerPosition.To2D(), target.ServerPosition.To2D(), 60).To3D();
                }

                if (((Vayne.Program.DontQIntoEnemies || target.IsMelee) && HeroManager.Enemies.Any(p => p.ServerPosition.To2D().Distance(vec) <= p.AttackRange + ObjectManager.Player.BoundingRadius + (p.IsMelee ? 100 : 0))) || vec.UnderTurret(true))
                    return Vector3.Zero;
            }
            if (HeroManager.Enemies.Any(p => p.NetworkId != target.NetworkId && p.ServerPosition.To2D().Distance(vec) <= p.AttackRange + (p.IsMelee ? 50 : 0)) || vec.UnderTurret(true))
                return Vector3.Zero;

            return vec;
        }
    }
}