using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Protype_Viktor;
using SharpDX;

namespace Prototype_Viktor
{
   internal class Laser
    {
        public static Vector2 LaserLocation(Vector2 sourcepos, List<Vector2> minionPositions, float width, float range)
        {
            var result = new Vector2();
            var minionCount = 0;
            var startPos = sourcepos;

            var max = minionPositions.Count;
            for (var i = 0; i < max; i++)
            {
                for (var j = 0; j < max; j++)
                {
                    if (minionPositions[j] != minionPositions[i])
                    {
                        minionPositions.Add((minionPositions[j] + minionPositions[i]) / 2);
                    }
                }
            }

            foreach (var pos in minionPositions)
            {
                if (pos.Distance(startPos, true) <= range * range)
                {
                    var endPos = startPos + range * (pos - startPos).Normalized();

                    var count =
                        minionPositions.Count(pos2 => pos2.Distance(startPos, endPos, true, true) <= width * width);

                    if (count >= minionCount)
                    {
                        result = endPos;
                        minionCount = count;
                    }
                }
            }

            return result;
        }

        public static FarmLocation GetBestLaserFarmLocation(bool jungle)
        {
            var bestendpos = new Vector2();
            var beststartpos = new Vector2();
            var minionCount = 0;
            List<Obj_AI_Minion> allminions;

            allminions =
                EntityManager.MinionsAndMonsters.Get(EntityManager.MinionsAndMonsters.EntityType.Minion,
                    EntityManager.UnitTeam.Enemy, Program._Player.Position, Program.EMaxRange, false).ToList();

            var minionslist = (from mnion in allminions select mnion.Position.To2D()).ToList();
            var posiblePositions = new List<Vector2>();
            posiblePositions.AddRange(minionslist);
            var max = posiblePositions.Count;
            for (var i = 0; i < max; i++)
            {
                for (var j = 0; j < max; j++)
                {
                    if (posiblePositions[j] != posiblePositions[i])
                    {
                        posiblePositions.Add((posiblePositions[j] + posiblePositions[i]) / 2);
                    }
                }
            }

            foreach (var startposminion in allminions.Where(m => Program._Player.Distance(m) < 525))
            {
                var startPos = startposminion.Position.To2D();

                foreach (var pos in posiblePositions)
                {
                    if (pos.Distance(startPos, true) <= 700 * 700)
                    {
                        var endPos = startPos + 700 * (pos - startPos).Normalized();

                        var count =
                            minionslist.Count(pos2 => pos2.Distance(startPos, endPos, true, true) <= 140 * 140);

                        if (count >= minionCount)
                        {
                            bestendpos = endPos;
                            minionCount = count;
                            beststartpos = startPos;
                        }
                    }
                }
            }

            //Console.WriteLine("Startpos: " + beststartpos + " Count : " + minionCount);
            return new FarmLocation(beststartpos, bestendpos, minionCount);
        }

        public struct FarmLocation
        {
            public int MinionsHit;
            public Vector2 Position1;
            public Vector2 Position2;

            public FarmLocation(Vector2 startpos, Vector2 endpos, int minionsHit)
            {
                Position1 = startpos;
                Position2 = endpos;
                MinionsHit = minionsHit;
            }
        }
    }
}
