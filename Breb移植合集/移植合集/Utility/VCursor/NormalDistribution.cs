using System;
using LeagueSharp.Common;

namespace VCursor
{
    // This class was derived from code posted here:
    // http://stackoverflow.com/questions/218060/random-gaussian-variables

    public class NormalDistribution
    {
        private readonly Random gaussian = new Random(Utils.TickCount);

        /// <summary>
        ///     Uses a Box-Muller polar method to generate a value from within a gaussian distribution with mean of 0 and standard
        ///     deviation of 1
        /// </summary>
        /// <returns></returns>
        public double NextGaussian()
        {
            double v1;
            double s;
            do
            {
                v1 = 2 * gaussian.NextDouble() - 1;
                var v2 = 2 * gaussian.NextDouble() - 1;

                s = v1 * v1 + v2 * v2;
            } while (s >= 1 || s == 0);

            var multiplier = Math.Sqrt(-2 * Math.Log(s) / s);
            return v1 * multiplier;
        }
    }
}