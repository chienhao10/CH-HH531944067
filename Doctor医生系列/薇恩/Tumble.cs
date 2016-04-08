using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrVayne.Common;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX.Direct3D;

namespace DrVayne
{
    public static class Tumble
    {
        public static Vector3 TumbleOrderPos = Vector3.Zero;

        public static void Cast(Vector3 position)
        {
            if (!Program.SubMenu["Combo"]["Q"].Cast<CheckBox>().CurrentValue) return;

            TumbleOrderPos = position;
            if (position != Vector3.Zero)
            {
                Program.Q.Cast(TumbleOrderPos);
            }
        }
        public static bool IsDangerousPosition(this Vector3 pos)
        {
            return
                EntityManager.Heroes.Enemies.Any(
                    e => e.IsValidTarget(600) && e.IsVisible &&
                        e.Distance(pos) < 375) ||
                Program.EnemyTraps.Any(t => pos.Distance6(t.Position) < 125);
        }

        public static Vector3 GetTumblePos(this Obj_AI_Base target)
        {
            var cursorPos = Game.CursorPos;

            if (!cursorPos.IsDangerousPosition()) return cursorPos;
            //if the target is not a melee and he's alone he's not really a danger to us, proceed to 1v1 him :^ )
            if (!target.IsMelee && Program.myHero.CountEnemiesInRange2(800) == 1) return cursorPos;

            var aRC = new QGeometry.Circle(Program.myHero.ServerPosition.To2D2(), 300).ToPolygon().ToClipperPath();
            var targetPosition = target.ServerPosition;
            var pList = new List<Vector3>();
            var additionalDistance = (0.106 + Game.Ping / 2000f) * target.MoveSpeed;


            foreach (var p in aRC)
            {
                var v3 = new Vector2(p.X, p.Y).To3D();

                if (target.IsFacing2(Program.myHero))
                {
                    if (!v3.IsDangerousPosition() && v3.Distance6(targetPosition) < 550) pList.Add(v3);
                }
                else
                {
                    if (!v3.IsDangerousPosition() && v3.Distance6(targetPosition) < 550 - additionalDistance) pList.Add(v3);
                }
            }
            if (Program.myHero.UnderTurret() || Program.myHero.CountEnemiesInRange2(800) == 1)
            {
                return pList.Count > 1 ? pList.OrderBy(el => el.Distance6(cursorPos)).FirstOrDefault() : Vector3.Zero;
            }
            if (!cursorPos.IsDangerousPosition())
            {
                return pList.Count > 1 ? pList.OrderBy(el => el.Distance6(cursorPos)).FirstOrDefault() : Vector3.Zero;
            }
            return pList.Count > 1 ? pList.OrderByDescending(el => el.Distance6(cursorPos)).FirstOrDefault() : Vector3.Zero;
        }
    }
}