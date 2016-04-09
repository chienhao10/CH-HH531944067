using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Jhin.Utilities;

namespace Jhin.Managers
{
    public static class CacheManager
    {
        private static readonly Dictionary<int, Dictionary<int, float>> CachedDistanceSqr =
            new Dictionary<int, Dictionary<int, float>>();

        private static readonly Dictionary<int, Dictionary<int, float>> CachedDistance =
            new Dictionary<int, Dictionary<int, float>>();

        private static readonly Dictionary<int, Dictionary<int, Dictionary<int, bool>>> CachedInRange =
            new Dictionary<int, Dictionary<int, Dictionary<int, bool>>>();

        private static readonly Dictionary<int, Dictionary<int, bool>> CachedInAutoAttackRange =
            new Dictionary<int, Dictionary<int, bool>>();

        private static readonly Dictionary<string, Dictionary<int, bool>> CachedHasBuff =
            new Dictionary<string, Dictionary<int, bool>>();

        private static readonly Dictionary<int, Dictionary<int, float>> CachedAutoAttackDamage =
            new Dictionary<int, Dictionary<int, float>>();

        public static void Initialize()
        {
            Game.OnTick += delegate
            {
                if (FpsBooster.CanBeExecuted(CalculationType.Distance))
                {
                }
                foreach (var pair in CachedDistanceSqr)
                {
                    pair.Value.Clear();
                }
                foreach (var pair in CachedDistance)
                {
                    pair.Value.Clear();
                }
                foreach (var pair in CachedInRange.SelectMany(pair => pair.Value))
                {
                    pair.Value.Clear();
                }
                foreach (var pair in CachedInAutoAttackRange)
                {
                    pair.Value.Clear();
                }
                if (FpsBooster.CanBeExecuted(CalculationType.IsValidTarget))
                {
                }
                foreach (var pair in CachedHasBuff)
                {
                    pair.Value.Clear();
                }
                if (FpsBooster.CanBeExecuted(CalculationType.Damage))
                {
                }
                foreach (var pair in CachedAutoAttackDamage)
                {
                    pair.Value.Clear();
                }
            };
        }

        public static float GetDistanceSqr(this GameObject target1, GameObject target2)
        {
            if (!CachedDistanceSqr.ContainsKey(target1.NetworkId))
            {
                CachedDistanceSqr.Add(target1.NetworkId, new Dictionary<int, float>());
            }
            if (!CachedDistanceSqr[target1.NetworkId].ContainsKey(target2.NetworkId))
            {
                CachedDistanceSqr[target1.NetworkId].Add(target2.NetworkId, target1.Distance(target2, true));
            }
            if (!CachedDistanceSqr.ContainsKey(target2.NetworkId))
            {
                CachedDistanceSqr.Add(target2.NetworkId, new Dictionary<int, float>());
            }
            if (!CachedDistanceSqr[target2.NetworkId].ContainsKey(target1.NetworkId))
            {
                CachedDistanceSqr[target2.NetworkId].Add(target1.NetworkId,
                    CachedDistanceSqr[target1.NetworkId][target2.NetworkId]);
            }
            return CachedDistanceSqr[target1.NetworkId][target2.NetworkId];
        }

        public static float GetDistance(this GameObject target1, GameObject target2)
        {
            if (!CachedDistance.ContainsKey(target1.NetworkId))
            {
                CachedDistance.Add(target1.NetworkId, new Dictionary<int, float>());
            }
            if (!CachedDistance[target1.NetworkId].ContainsKey(target2.NetworkId))
            {
                CachedDistance[target1.NetworkId].Add(target2.NetworkId,
                    (float) Math.Sqrt(target1.GetDistanceSqr(target2)));
            }
            if (!CachedDistance.ContainsKey(target2.NetworkId))
            {
                CachedDistance.Add(target2.NetworkId, new Dictionary<int, float>());
            }
            if (!CachedDistance[target2.NetworkId].ContainsKey(target1.NetworkId))
            {
                CachedDistance[target2.NetworkId].Add(target1.NetworkId,
                    CachedDistance[target1.NetworkId][target2.NetworkId]);
            }
            return CachedDistance[target1.NetworkId][target2.NetworkId];
        }

        public static bool InRange(this GameObject target1, GameObject target2, float range)
        {
            var intRange = (int) range;
            if (!CachedInRange.ContainsKey(target1.NetworkId))
            {
                CachedInRange.Add(target1.NetworkId, new Dictionary<int, Dictionary<int, bool>>());
            }
            if (!CachedInRange[target1.NetworkId].ContainsKey(target2.NetworkId))
            {
                CachedInRange[target1.NetworkId].Add(target2.NetworkId, new Dictionary<int, bool>());
            }
            if (!CachedInRange[target1.NetworkId][target2.NetworkId].ContainsKey(intRange))
            {
                CachedInRange[target1.NetworkId][target2.NetworkId].Add(intRange,
                    target1.GetDistanceSqr(target2) <= intRange.Pow());
            }
            if (!CachedInRange.ContainsKey(target2.NetworkId))
            {
                CachedInRange.Add(target2.NetworkId, new Dictionary<int, Dictionary<int, bool>>());
            }
            if (!CachedInRange[target2.NetworkId].ContainsKey(target1.NetworkId))
            {
                CachedInRange[target2.NetworkId].Add(target1.NetworkId, new Dictionary<int, bool>());
            }
            if (!CachedInRange[target2.NetworkId][target1.NetworkId].ContainsKey(intRange))
            {
                CachedInRange[target2.NetworkId][target1.NetworkId].Add(intRange,
                    CachedInRange[target1.NetworkId][target2.NetworkId][intRange]);
            }
            return CachedInRange[target1.NetworkId][target2.NetworkId][intRange];
        }

        public static bool InAutoAttackRange(this Obj_AI_Base target1, Obj_AI_Base target2)
        {
            if (!CachedInAutoAttackRange.ContainsKey(target1.NetworkId))
            {
                CachedInAutoAttackRange.Add(target1.NetworkId, new Dictionary<int, bool>());
            }
            if (!CachedInAutoAttackRange[target1.NetworkId].ContainsKey(target2.NetworkId))
            {
                CachedInAutoAttackRange[target1.NetworkId].Add(target2.NetworkId,
                    target1.InRange(target2, target1.AttackRange + target1.BoundingRadius + target2.BoundingRadius - 20));
            }
            return CachedInAutoAttackRange[target1.NetworkId][target2.NetworkId];
        }

        public static bool TargetHaveBuff(this Obj_AI_Base target, string buffName)
        {
            if (target != null)
            {
                if (!CachedHasBuff.ContainsKey(buffName))
                {
                    CachedHasBuff.Add(buffName, new Dictionary<int, bool>());
                }
                if (!CachedHasBuff[buffName].ContainsKey(target.NetworkId))
                {
                    CachedHasBuff[buffName].Add(target.NetworkId, target.HasBuff(buffName));
                }
                return CachedHasBuff[buffName][target.NetworkId];
            }
            return false;
        }

        public static float GetAttackDamage(this Obj_AI_Base from, Obj_AI_Base target, bool respectPassive = false)
        {
            if (!CachedAutoAttackDamage.ContainsKey(from.NetworkId))
            {
                CachedAutoAttackDamage.Add(from.NetworkId, new Dictionary<int, float>());
            }
            if (!CachedAutoAttackDamage[from.NetworkId].ContainsKey(target.NetworkId))
            {
                CachedAutoAttackDamage[from.NetworkId].Add(target.NetworkId,
                    from.GetAutoAttackDamage(target, respectPassive));
            }
            return CachedAutoAttackDamage[from.NetworkId][target.NetworkId];
        }
    }
}