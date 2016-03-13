using System;

namespace AdEvade.Randomizer
{
    public class Randomizer
    {
        public System.Random Random { get; set; }

        public Randomizer()
        {
            Random = new Random(Guid.NewGuid().GetHashCode());
        }

        public bool Bool()
        {
            return Random.Next(201) > 100;
        }

        public int Int(int min = 0, int max = 100)
        {
            return Random.Next(min, max);
        }

        public float Percentage()
        {
            var value = Int();
            return value / 100f;
        }

        public bool IsAbovePercentage(float minPercentage)
        {
            return Percentage() > minPercentage;
        }

    }
}