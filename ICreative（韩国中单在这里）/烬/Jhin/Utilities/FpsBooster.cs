using System.Linq;
using EloBuddy;

namespace Jhin.Utilities
{
    public enum CalculationType
    {
        Damage,
        Distance,
        Prediction,
        HealthPrediction,
        IsValidTarget
    }

    public static class FpsBooster
    {
        private const int MaxIndex = 6;
        private static int _currentIndex;

        public static void Initialize()
        {
            Game.OnTick += delegate
            {
                _currentIndex++;
                if (_currentIndex > MaxIndex)
                {
                    _currentIndex = 0;
                }
            };
        }

        public static bool CanBeExecuted(params int[] indexs)
        {
            return indexs.Contains(_currentIndex);
        }

        public static bool CanBeExecuted(CalculationType type)
        {
            return (int) type == _currentIndex;
        }
    }
}