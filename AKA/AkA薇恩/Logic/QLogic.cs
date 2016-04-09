using System;
using System.Collections.Generic;
using System.Linq;
using Aka_s_Vayne_reworked;
using Aka_s_Vayne_reworked.Functions;
using Aka_s_Vayne_reworked.Logic;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace AddonTemplate.Logic
{
    public static class QLogic
    {
        public static Vector3 TumbleOrderPos = Vector3.Zero;

        private static QProvider Provider = new QProvider();

        #region smart

        public static Vector3 NewQPrediction()
        {
            if (!MenuManager.Qsettings["QE"].Cast<CheckBox>().CurrentValue &&
    !Program.E.IsReady())
            {
                return Vector3.Zero;
            }

            const int currentStep = 30;
            var direction = Variables._Player.Direction.To2D().Perpendicular();
            for (var i = 0f; i < 360f; i += currentStep)
            {
                var angleRad = Geometry.DegreeToRadian(i);
                var rotatedPosition = Variables._Player.Position.To2D() + (300f * direction.Rotated(angleRad));
                if (NewELogic.GetCondemnTarget(rotatedPosition.To3D()).IsValidTarget() && rotatedPosition.To3D().IsSafe())
                {
                    return rotatedPosition.To3D();
                }
            }

            return Vector3.Zero;
        }

        public static List<Vector2> GetEnemyPoints(bool dynamic = true)
        {
            var staticRange = 360f;
            var polygonsList = Variables.EnemiesClose.Select(enemy => new VHRGeometry.Circle(enemy.ServerPosition.To2D(), (dynamic ? (enemy.IsMelee ? enemy.AttackRange * 1.5f : enemy.AttackRange) : staticRange) + enemy.BoundingRadius + 20).ToPolygon()).ToList();
            var pathList = VHRGeometry.ClipPolygons(polygonsList);
            var pointList = pathList.SelectMany(path => path, (path, point) => new Vector2(point.X, point.Y)).Where(currentPoint => !NavMesh.GetCollisionFlags(currentPoint).HasFlag(CollisionFlags.Wall) || !NavMesh.GetCollisionFlags(currentPoint).HasFlag(CollisionFlags.Building)).ToList();
            return pointList;
        }

        public static bool IsSafe(this Vector3 position, bool noQIntoEnemiesCheck = false)
        {
            if (other.UnderEnemyTower((Vector2)position) && !other.UnderEnemyTower((Vector2)Variables._Player.Position))
            {
                return false;
            }

            var allies = position.CountAlliesInRange(Variables._Player.AttackRange);
            var enemies = position.CountEnemiesInRange(Variables._Player.AttackRange);
            var lhEnemies = position.GetLhEnemiesNear(Variables._Player.AttackRange, 15).Count();

            if (enemies <= 1) ////It's a 1v1, safe to assume I can Q
            {
                return true;
            }

            if (position.UnderAllyTurret_Ex())
            {
                var nearestAllyTurret = ObjectManager.Get<Obj_AI_Turret>().Where(a => a.IsAlly).OrderBy(d => d.Distance(position, true)).FirstOrDefault();

                if (nearestAllyTurret != null)
                {
                    ////We're adding more allies, since the turret adds to the firepower of the team.
                    allies += 2;
                }
            }

            ////Adding 1 for my Player
            var normalCheck = (allies + 1 > enemies - lhEnemies);
            var QEnemiesCheck = true;

            if (MenuManager.Qsettings["UseQE"].Cast<CheckBox>().CurrentValue && noQIntoEnemiesCheck)
            {
                if (!MenuManager.Qsettings["UseQE"].Cast<CheckBox>().CurrentValue)
                {
                    var Vector2Position = position.To2D();
                    var enemyPoints = MenuManager.Qsettings["UseSafeQ"].Cast<CheckBox>().CurrentValue
                        ? GetEnemyPoints()
                        : GetEnemyPoints(false);
                    if (enemyPoints.Contains(Vector2Position) &&
                        !MenuManager.Qsettings["UseQspam"].Cast<CheckBox>().CurrentValue)
                    {
                        QEnemiesCheck = false;
                    }

                    var closeEnemies =
                    EntityManager.Heroes.Enemies.FindAll(en => en.IsValidTarget(1500f) && !(en.Distance(Variables._Player.ServerPosition) < en.AttackRange + 65f))
                    .OrderBy(en => en.Distance(position));

                    if (
                        !closeEnemies.All(
                            enemy =>
                                position.CountEnemiesInRange(
                                    MenuManager.Qsettings["UseSafeQ"].Cast<CheckBox>().CurrentValue
                                        ? enemy.AttackRange
                                        : 405f) <= 1))
                    {
                        QEnemiesCheck = false;
                    }
                }
                else
                {
                    var closeEnemies =
                    EntityManager.Heroes.Enemies.FindAll(en => en.IsValidTarget(1500f)).OrderBy(en => en.Distance(position));
                    if (closeEnemies.Any())
                    {
                        QEnemiesCheck =
                            !closeEnemies.All(
                                enemy =>
                                    position.CountEnemiesInRange(
                                        MenuManager.Qsettings["UseSafeQ"].Cast<CheckBox>().CurrentValue
                                            ? enemy.AttackRange
                                            : 405f) <= 1);
                    }
                }

            }

            return normalCheck && QEnemiesCheck;
        }

        public static void PreCastTumble(Obj_AI_Base target)
        {
            if (!target.IsValidTarget(Variables._Player.AttackRange + 65f + 65f + 300f))
            {
                return;
           }

            var smartQPosition = NewQPrediction();
            var smartQCheck = smartQPosition != Vector3.Zero;
            var QPosition = smartQCheck ? smartQPosition : Game.CursorPos;

            OnCastTumble(target, QPosition);
        }

        public static void OnCastTumble(Obj_AI_Base target, Vector3 position)
        {
            var mode = MenuManager.ComboMenu["Qmode"].Cast<ComboBox>().CurrentValue;
            var afterTumblePosition = Variables._Player.ServerPosition.Extend(position, 300f);
            var distanceToTarget = afterTumblePosition.Distance(target.ServerPosition, true);
            if ((distanceToTarget < Math.Pow(Variables._Player.AttackRange + 65, 2) && distanceToTarget > 110 * 110)
                || MenuManager.Qsettings["UseQspam"].Cast<CheckBox>().CurrentValue)
            {
                switch (mode)
                {
                    case 1:
                        //smart logic!
                        //var smartQPosition = NewQPrediction();
                        //var smartQCheck = smartQPosition != Vector3.Zero;
                        //var QPosition = smartQCheck ? smartQPosition : Game.CursorPos;
                        //var QPosition2 = Provider.GetQPosition() != Vector3.Zero ? Provider.GetQPosition() : QPosition;

                        //if (!other.UnderEnemyTower((Vector2)QPosition2) || (other.UnderEnemyTower((Vector2)QPosition2) && other.UnderEnemyTower((Vector2)Variables._Player.Position)))
                        //{
                        //CastQ(QPosition2);
                        //}
                        break;
                    case 0:
                        //To mouse
                        DefaultQCast(position, target);
                        break;
                    case 2:
                        //Away from melee enemies
                        if (Variables.MeleeEnemiesTowardsMe.Any() &&
                            !Variables.MeleeEnemiesTowardsMe.All(m => m.HealthPercent <= 15))
                        {
                            var Closest =
                                Variables.MeleeEnemiesTowardsMe.OrderBy(m => m.Distance(Variables._Player)).First();
                            var whereToQ = (Vector3)Closest.ServerPosition.Extend(
                                Variables._Player.ServerPosition, Closest.Distance(Variables._Player) + 300f);

                            if (whereToQ.IsSafe())
                            {
                                CastQ(whereToQ);
                            }
                        }
                        else
                        {
                            DefaultQCast(position, target);
                        }
                        break;
                    case 3:
                        //Prada
                        var Target = TargetSelector.GetTarget((int)Variables._Player.GetAutoAttackRange(), DamageType.Physical);
                        if (Target == null) return;
                        var tumblePosition = Target.GetTumblePos();
                        Cast(tumblePosition);
                        break;
                    case 4:
                        //Sebby
                        CastDash();
                        break;
                }
            }
        }

        public static Vector3 GetAfterTumblePosition(Vector3 endPosition)
        {
            return (Vector3)Variables._Player.ServerPosition.Extend(endPosition, 300f);
        }

        public static void DefaultQCast(Vector3 position, Obj_AI_Base Target)
        {
            var afterTumblePosition = GetAfterTumblePosition(Game.CursorPos);
            var CursorPos = Game.CursorPos;
            var EnemyPoints = GetEnemyPoints();
            if (afterTumblePosition.IsSafe(true) || (!EnemyPoints.Contains(Game.CursorPos.To2D())) || (Variables.EnemiesClose.Count() == 1))
            {
                if (afterTumblePosition.Distance(Target.ServerPosition) <= Variables._Player.GetAutoAttackRange(Target))
                {
                    CastQ(position);
                }
            }
        }

        private static void CastQ(Vector3 Position)
        {
            var endPosition = Position;

            if (MenuManager.Qsettings["Mirin"].Cast<CheckBox>().CurrentValue)
            {
                var qBurstModePosition = GetQBurstModePosition();
                if (qBurstModePosition != null)
                {
                    endPosition = (Vector3)qBurstModePosition;
                }
            }
            Player.CastSpell(SpellSlot.Q, endPosition);
        }

        private static Vector3? GetQBurstModePosition()
        {
            var positions =
                GetWallQPositions(70).ToList().OrderBy(pos => pos.Distance(Variables._Player.ServerPosition, true));

            foreach (var position in positions)
            {
                var collFlags = NavMesh.GetCollisionFlags(position);
                if (collFlags.HasFlag(CollisionFlags.Wall) || collFlags.HasFlag(CollisionFlags.Building) && position.IsSafe(true))
                {
                    return position;
                }
            }

            return null;
        }


        private static Vector3[] GetWallQPositions(float Range)
        {
            Vector3[] vList =
            {
                (Variables._Player.ServerPosition.To2D() + Range * Variables._Player.Direction.To2D()).To3D(),
                (Variables._Player.ServerPosition.To2D() - Range * Variables._Player.Direction.To2D()).To3D()

            };

            return vList;
        }

        #endregion smart

        #region old

        public static void Cast(Vector3 position)
        {
            TumbleOrderPos = position;
            if (position != Vector3.Zero)
            {
                Player.CastSpell(SpellSlot.Q, TumbleOrderPos);
            }
        }

        public static Vector3 GetAggressiveTumblePos(this Obj_AI_Base target)
        {
            var cursorPos = Game.CursorPos;

            if (!cursorPos.IsDangerousPosition()) return cursorPos;
            //if the target is not a melee and he's alone he's not really a danger to us, proceed to 1v1 him :^ )
            if (!target.IsMelee && Variables._Player.CountEnemiesInRange(800) == 1) return cursorPos;

            var aRC = new VHRGeometry.Circle(Variables._Player.ServerPosition.To2D(), 300).ToPolygon().ToClipperPath();
            var targetPosition = target.ServerPosition;


            foreach (var p in aRC)
            {
                var v3 = new Vector2(p.X, p.Y).To3D();
                var dist = v3.Distance(targetPosition);
                if (dist > 325 && dist < 450)
                {
                    return v3;
                }
            }
            return Vector3.Zero;
        }

        public static Vector3 GetTumblePos(this Obj_AI_Base target)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                return GetAggressiveTumblePos(target);

            var cursorPos = Game.CursorPos;

            if (!cursorPos.IsDangerousPosition()) return cursorPos;
            //if the target is not a melee and he's alone he's not really a danger to us, proceed to 1v1 him :^ )
            if (!target.IsMelee && Variables._Player.CountEnemiesInRange(800) == 1) return cursorPos;

            var aRC = new VHRGeometry.Circle(Variables._Player.ServerPosition.To2D(), 300).ToPolygon().ToClipperPath();
            var targetPosition = target.ServerPosition;
            var pList =(from p in aRC
                   select new Vector2(p.X, p.Y).To3D()
                    into v3
                   let dist = v3.Distance(targetPosition)
                   where !v3.IsDangerousPosition() && dist < 500
                   select v3).ToList();

            if (other.UnderEnemyTower((Vector2) Variables._Player.ServerPosition) || Variables._Player.CountEnemiesInRange(800) == 1 ||
                cursorPos.CountEnemiesInRange(450) <= 1)
            {
                return pList.Count > 1 ? pList.OrderBy(el => el.Distance(cursorPos)).FirstOrDefault() : Vector3.Zero;
            }
            return pList.Count > 1
                ? pList.OrderByDescending(el => el.Distance(cursorPos)).FirstOrDefault()
                : Vector3.Zero;
        }


        public static bool IsDangerousPosition(this Vector3 pos)
        {
            return
                EntityManager.Heroes.Enemies.Any(
                    e => e.IsValidTarget() && e.IsVisible &&
                        e.Distance(pos) < 375) ||
                Traps.EnemyTraps.Any(t => pos.Distance(t.Position) < 125) ||
                (other.UnderEnemyTower((Vector2)pos) && !other.UnderEnemyTower((Vector2)Variables._Player.ServerPosition) || (NavMesh.GetCollisionFlags(pos).HasFlag(CollisionFlags.Wall) || NavMesh.GetCollisionFlags(pos).HasFlag(CollisionFlags.Building)));
        }

        #endregion old

        #region new

        public static Vector3 CastDash()
        {
            int DashMode = MenuManager.Qsettings["QNmode"].Cast<ComboBox>().CurrentValue;

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
                        bestpoint = (Vector3)Variables._Player.Position.Extend(rEndPos, Program.Q.Range);
                        if (IsGoodPosition(bestpoint))
                            Cast(bestpoint);
                    }
                    else
                    {
                        bestpoint = (Vector3)Variables._Player.Position.Extend(lEndPos, Program.Q.Range);
                        if (IsGoodPosition(bestpoint))
                            Cast(bestpoint);
                    }
                }
            }
            else if (DashMode == 1)
            {
                var points = CirclePoints(12, Program.Q.Range, Variables._Player.Position);
                bestpoint = (Vector3)Variables._Player.Position.Extend(Game.CursorPos, Program.Q.Range);
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

            if (!bestpoint.IsZero && bestpoint.CountEnemiesInRange(Variables._Player.BoundingRadius + Variables._Player.AttackRange + 100) == 0)
                return Vector3.Zero;

            return bestpoint;
        }

        public static bool IsGoodPosition(Vector3 dashPos)
        {
            if (MenuManager.Qsettings["QNWall"].Cast<CheckBox>().CurrentValue)
            {
                float segment = Program.Q.Range / 5;
                for (int i = 1; i <= 5; i++)
                {
                    if (Variables._Player.Position.Extend(dashPos, i * segment).IsWall())
                        return false;
                }
            }

            if (MenuManager.Qsettings["QNTurret"].Cast<CheckBox>().CurrentValue)
            {
                if (other.UnderEnemyTower(dashPos.To2D()))
                    return false;
            }

            var enemyCheck = MenuManager.Qsettings["QNenemies"].Cast<Slider>().CurrentValue;
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

        #endregion new
    }
}