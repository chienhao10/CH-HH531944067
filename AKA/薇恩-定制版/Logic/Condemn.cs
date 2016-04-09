using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using SharpDX;


namespace Auto_Carry_Vayne.Logic
{
    class Condemn
    {
        //my condemn logic so far
        public static Obj_AI_Base GetTarget(Vector3 fromPosition)
        {
            var targetList =
                EntityManager.Heroes.Enemies.Where(
                    h =>
                    h.IsValidTarget(Manager.SpellManager.E.Range) && !h.HasBuffOfType(BuffType.SpellShield)
                    && !h.HasBuffOfType(BuffType.SpellImmunity)
                    && h.Health > ObjectManager.Player.GetAutoAttackDamage(h, true) * 2).ToList();

            if (!targetList.Any())
            {
                return null;
            }

            foreach (var enemy in targetList)
            {
                var prediction = Manager.SpellManager.E2.GetPrediction(enemy);
                var predictionsList = new List<Vector3>
                                          {
                                              enemy.ServerPosition,
                                              enemy.Position,
                                              prediction.CastPosition,
                                              prediction.UnitPosition
                                          };

                var wallsFound = 0;

                foreach (var position in predictionsList)
                {
                    var distance = fromPosition.Distance(position);

                    for (var i = 0; i < Manager.MenuManager.CondemnPushDistance; i += (int)enemy.BoundingRadius)
                    {
                        var finalPosition = fromPosition.Extend(position, distance + i).To3D();
                        var j4Flag = Manager.MenuManager.J4Flag && (Variables.IsJ4Flag(finalPosition, enemy));
                        if (NavMesh.GetCollisionFlags(finalPosition).HasFlag(CollisionFlags.Wall)
                            || NavMesh.GetCollisionFlags(finalPosition).HasFlag(CollisionFlags.Building) || j4Flag)
                        {
                            wallsFound++;
                            break;
                        }
                    }
                }

                if (wallsFound >= Manager.MenuManager.CondemnHitchance)
                {
                    return enemy;
                }
            }

            return null;
        }
    }
}


