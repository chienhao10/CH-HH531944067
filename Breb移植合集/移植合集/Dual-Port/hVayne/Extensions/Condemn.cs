using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using LeagueSharp.SDK;
using SharpDX;

namespace hVayne.Extensions
{
    class Condemn : Spells
    {
        // ReSharper disable once CollectionNeverQueried.Local
        private static List<Vector2> _points = new List<Vector2>();

        public static void ShineCondemn(AIHeroClient target, int push)
        {
            var pushDistance = push;
            var targetPosition = E.GetPrediction(target).UnitPosition;
            var pushDirection = (targetPosition - ObjectManager.Player.ServerPosition).LSNormalized();
            float checkDistance = pushDistance / 40f;
            for (int i = 0; i < 40; i++)
            {
                Vector3 finalPosition = targetPosition + (pushDirection * checkDistance * i);
                var collFlags = NavMesh.GetCollisionFlags(finalPosition);
                if (collFlags.HasFlag(CollisionFlags.Wall) || collFlags.HasFlag(CollisionFlags.Building)) //not sure about building, I think its turrets, nexus etc
                {
                    E.Cast(target);
                }
            }
        }

        public static void AsunaCondemn(AIHeroClient target, int push)
        {
            var ePred = E.GetPrediction(target);
            int pushDist = push;
            for (var i = 1; i < pushDist; i += (int)target.BoundingRadius)
            {
                var loc3 = ePred.UnitPosition.ToVector2().Extend(ObjectManager.Player.ServerPosition.ToVector2(), -i).ToVector3();
                var collFlags = NavMesh.GetCollisionFlags(loc3);
                if (loc3.LSIsWall() || collFlags.HasFlag(CollisionFlags.Wall) || collFlags.HasFlag(CollisionFlags.Building))
                {
                    E.Cast(target);
                }   
            }
        }

        public static bool Condemn360(AIHeroClient unit, int push, Vector2 pos = new Vector2())
        {
            if (unit.HasBuffOfType(BuffType.SpellImmunity) || unit.HasBuffOfType(BuffType.SpellShield) || ObjectManager.Player.IsDashing())
            {
                return false;
            }

            var prediction = E.GetPrediction(unit);
            var predictionsList = pos.IsValid() ? new List<Vector3>() { pos.ToVector3() } : new List<Vector3>
                        {
                            unit.ServerPosition,
                            unit.Position,
                            prediction.CastPosition,
                            prediction.UnitPosition
                        };

            var wallsFound = 0;
            _points = new List<Vector2>();
            foreach (var position in predictionsList)
            {
                for (var i = 0; i < push; i += (int)unit.BoundingRadius) // 420 = push distance
                {
                    var cPos = ObjectManager.Player.Position.LSExtend(position, ObjectManager.Player.Distance(position) + i).ToVector2();
                    _points.Add(cPos);
                    if (NavMesh.GetCollisionFlags(cPos.ToVector3()).HasFlag(CollisionFlags.Wall) || NavMesh.GetCollisionFlags(cPos.ToVector3()).HasFlag(CollisionFlags.Building))
                    {
                        wallsFound++;
                        break;
                    }
                }
            }

            // ReSharper disable once PossibleLossOfFraction
            if ((wallsFound / predictionsList.Count) >= 33 / 100f)
            {
                return true;
            }

            return false;
        }

        internal static void JungleCondemn(Obj_AI_Minion mob, int push)
        {
            var pushDistance = push;
            var targetPosition = E.GetPrediction(mob).UnitPosition;
            var pushDirection = (targetPosition - ObjectManager.Player.ServerPosition).LSNormalized();
            float checkDistance = pushDistance / 40f;
            for (int i = 0; i < 40; i++)
            {
                Vector3 finalPosition = targetPosition + (pushDirection * checkDistance * i);
                var collFlags = NavMesh.GetCollisionFlags(finalPosition);
                if (collFlags.HasFlag(CollisionFlags.Wall) || collFlags.HasFlag(CollisionFlags.Building)) //not sure about building, I think its turrets, nexus etc
                {
                    E.Cast(mob);
                }
            }
        }
    }
}
