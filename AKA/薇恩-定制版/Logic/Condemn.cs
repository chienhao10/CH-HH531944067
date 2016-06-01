using SharpDX;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using System.Linq;
using System.Collections.Generic;
using System;
using Auto_Carry_Vayne.Manager;

namespace Auto_Carry_Vayne.Logic
{
    class Condemn
    {
        //The First One ~Aka

        public static long LastCheck;
        public static List<Vector2> Points = new List<Vector2>();

        public static void Execute()
        {
            foreach (
                var enemy in
                    EntityManager.Heroes.Enemies.Where(
                        x =>
                            x.IsValidTarget(SpellManager.E.Range) && !x.HasBuffOfType(BuffType.SpellShield) &&
                            !x.HasBuffOfType(BuffType.SpellImmunity) &&
                            IsCondemable(x)))

                SpellManager.E.Cast(enemy);
        }



        public static bool IsCondemable(AIHeroClient unit, Vector2 pos = new Vector2())
        {
            if (unit.HasBuffOfType(BuffType.SpellImmunity) || unit.HasBuffOfType(BuffType.SpellShield) || LastCheck + 50 > Environment.TickCount || Variables._Player.IsDashing()) return false;
            var prediction = SpellManager.E2.GetPrediction(unit);
            var predictionsList = pos.IsValid() ? new List<Vector3>() { pos.To3D() } : new List<Vector3>
                        {
                            unit.ServerPosition,
                            unit.Position,
                            prediction.CastPosition,
                            prediction.UnitPosition
                        };

            var wallsFound = 0;
            Points = new List<Vector2>();
            foreach (var position in predictionsList)
            {
                for (var i = 0; i < MenuManager.CondemnPushDistance; i += (int)unit.BoundingRadius)
                {
                    var cPos = ObjectManager.Player.Position.Extend(position, ObjectManager.Player.Distance(position) + i).To3D();
                    Points.Add(cPos.To2D());
                    if (NavMesh.GetCollisionFlags(cPos).HasFlag(CollisionFlags.Wall) || NavMesh.GetCollisionFlags(cPos).HasFlag(CollisionFlags.Building))
                    {
                        wallsFound++;
                        break;
                    }
                }
            }
            if ((wallsFound / predictionsList.Count) >= 33 / 100f)
            {
                return true;
            }
            return false;
        }

    }
}

