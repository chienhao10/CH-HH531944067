using SharpDX;
using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using System.Collections.Generic;
using System;
using Auto_Carry_Vayne.Manager;

namespace Auto_Carry_Vayne.Logic
{
    public static class Tumble
    {
        #region new

        public static Vector3 CastDash()
        {
            int DashMode = MenuManager.UseQMode;

            Vector3 bestpoint = Vector3.Zero;
            if (DashMode == 0)
            {
                var orbT = TargetSelector.GetTarget((int)Variables._Player.GetAutoAttackRange(),
    DamageType.Physical);
                if (orbT != null)
                {
                    Vector2 start = Variables._Player.Position.To2D();
                    Vector2 end = orbT.Position.To2D();
                    var dir = (end - start).Normalized();
                    var pDir = dir.Perpendicular();

                    var rightEndPos = end + pDir * Variables._Player.Distance(orbT);
                    var leftEndPos = end - pDir * Variables._Player.Distance(orbT);

                    var rEndPos = new Vector3(rightEndPos.X, rightEndPos.Y, Variables._Player.Position.Z);
                    var lEndPos = new Vector3(leftEndPos.X, leftEndPos.Y, Variables._Player.Position.Z);

                    if (Game.CursorPos.Distance(rEndPos) < Game.CursorPos.Distance(lEndPos))
                    {
                        bestpoint = (Vector3)Variables._Player.Position.Extend(rEndPos, Manager.SpellManager.Q.Range);
                        if (IsGoodPosition(bestpoint))
                            Cast(bestpoint);
                    }
                    else
                    {
                        bestpoint = (Vector3)Variables._Player.Position.Extend(lEndPos, Manager.SpellManager.Q.Range);
                        if (IsGoodPosition(bestpoint))
                            Cast(bestpoint);
                    }
                }
            }
            else if (DashMode == 1)
            {
                var points = CirclePoints(12, Manager.SpellManager.Q.Range, Variables._Player.Position);
                bestpoint = (Vector3)Variables._Player.Position.Extend(Game.CursorPos, Manager.SpellManager.Q.Range);
                int enemies = bestpoint.CountEnemiesInRange(400);
                foreach (var point in points)
                {
                    int count = point.CountEnemiesInRange(400);
                    if (count < enemies)
                    {
                        enemies = count;
                        bestpoint = point;
                    }
                    else if (count == enemies && Game.CursorPos.Distance(point) < Game.CursorPos.Distance(bestpoint))
                    {
                        enemies = count;
                        bestpoint = point;
                    }
                }
                if (IsGoodPosition(bestpoint))
                    Cast(bestpoint);
            }
            else if (DashMode == 2)
            {
                bestpoint = Game.CursorPos;
                if (IsGoodPosition(bestpoint))
                Cast(bestpoint);
            }

            if (!bestpoint.IsZero && bestpoint.CountEnemiesInRange(Variables._Player.BoundingRadius + Variables._Player.AttackRange + 100) == 0)
                return Vector3.Zero;

            return bestpoint;
        }

        public static bool IsGoodPosition(Vector3 dashPos)
        {
            if (MenuManager.UseQWall)
            {
                float segment = Manager.SpellManager.Q.Range / 5;
                for (int i = 1; i <= 5; i++)
                {
                    if (Variables._Player.Position.Extend(dashPos, i * segment).IsWall())
                        return false;
                }
            }

            var enemyCheck = MenuManager.UseQEnemies;
            var enemyCountDashPos = dashPos.CountEnemiesInRange(600);

            if (enemyCheck > enemyCountDashPos)
                return true;

            var enemyCountPlayer = Variables._Player.CountEnemiesInRange(400);

            if (enemyCountDashPos <= enemyCountPlayer)
                return true;

            return false;
        }

        public static List<Vector3> CirclePoints(float CircleLineSegmentN, float radius, Vector3 position)
        {
            List<Vector3> points = new List<Vector3>();
            for (var i = 1; i <= CircleLineSegmentN; i++)
            {
                var angle = i * 2 * Math.PI / CircleLineSegmentN;
                var point = new Vector3(position.X + radius * (float)Math.Cos(angle), position.Y + radius * (float)Math.Sin(angle), position.Z);
                points.Add(point);
            }
            return points;
        }

        public static void Cast(Vector3 position)
        {
            if (position != Vector3.Zero)
            {
                Player.CastSpell(SpellSlot.Q, position);
            }
        }

        #endregion new
    }
}
