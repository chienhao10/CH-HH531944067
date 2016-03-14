using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using SharpDX;
using EloBuddy.SDK;

namespace WuAIO.Utilities
{
    public static class Vectors
    {
        static readonly AIHeroClient Player = EloBuddy.Player.Instance;

        public static Vector3 CorrectSpellRange(Vector3 toVector, uint range)
        {
            if (Player.Distance(toVector) <= range) return toVector;

            return Player.Position.Extend(toVector, range).To3D();
        }

        public static Vector3 BestCircularFarmLocation(int radius, int range, int minMinions = 3)
        {
            var minions = EntityManager.MinionsAndMonsters.CombinedAttackable.Where(it => !it.IsDead && it.IsValidTarget(range));

            if (minions.Any() && minions.Count() == 1) return default(Vector3);

            var hitsperminion = new List<int>();
            int hits = new int();

            for (int i = 0; i < minions.Count(); i++)
            {
                hits = 1;

                for (int j = 0; j < minions.Count(); j++)
                {
                    if (j == i) continue;

                    if (minions.ElementAt(i).Distance(minions.ElementAt(j)) <= radius) hits++;
                }

                hitsperminion.Add(hits);
            }

            if (hitsperminion.Any() && hitsperminion.Max() > minMinions)
            {
                var pos = minions.ElementAt(hitsperminion.IndexOf(hitsperminion.Max())).ServerPosition;

                return pos;
            }

            return default(Vector3);
        }
    }
}
