using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;

namespace TheCheater
{
    class LeaguesharpOrbwalkDetector : IDetector
    {
        private const float RandomizerFactor = 2f / 3f; // Even though the distance in l# can be set, it is only randomized from 0.8x distance to 1.2x distance, which is a 2/3 relationship
        private int _scripting;
        private AIHeroClient _hero;
        private readonly List<DataSet> _recentData = new List<DataSet>();
        private readonly Dictionary<DetectorSetting, float[]> _settingValues = new Dictionary<DetectorSetting, float[]>() { { DetectorSetting.Safe, new[] { 0.125f, 90 } }, { DetectorSetting.AntiHumanizer, new[] { 0.25f, 80 } } };
        private DetectorSetting _currentSetting;

        public LeaguesharpOrbwalkDetector()
        {
            
        }

        struct DataSet
        {
            public Vector3 Position;
            public float Time;
            public float Distance;

            public static DataSet Create(Vector3 position)
            {
                return new DataSet { Position = position, Distance = ObjectManager.Player.Position.LSDistance(position), Time = Game.Time };
            }
        }

        public void FeedData(Vector3 targetPos)
        {
            if (_recentData.Count >= 5)
            {
                _recentData.RemoveAt(0);
                _recentData.Add(DataSet.Create(targetPos));
            }
            else
            {
                _recentData.Add(DataSet.Create(targetPos));
                return;
            }

            if (_recentData.Last().Time - _recentData.First().Time < _recentData.Count * _settingValues[_currentSetting][0]) // clicking intensifies, 0.125f doesnt catch movement if you just hold the right mouse button, but sometimes still catches orbwalker at 170 humanizer delay
            {

                var min = float.MaxValue;
                var max = float.MinValue;
                foreach (var data in _recentData)
                {
                    if (data.Distance < min)
                        min = data.Distance;
                    if (data.Distance > max)
                        max = data.Distance;
                }

                if (!(max*RandomizerFactor <= min))
                {
                    return;
                }

                var angleDiff = 0f;
                for (int i = 1; i < _recentData.Count - 1; i++)
                    angleDiff += GetAngleDifference(_recentData[i].Position - _recentData[i - 1].Position, _recentData[i + 1].Position - _recentData[i].Position);

                if (angleDiff > _settingValues[_currentSetting][1] * _recentData.Count) // since the length is randomized it clicks like a vibrating mouse ... results in lots of direction turns. A normal person does not change the click-direction 90° avg. per click -> 450° per 5 click, which means you changed your click direction ~3 times in a short time
                    _scripting++;
            }
        }

        private static float GetAngleDifference(Vector3 vec1, Vector3 vec2)
        {
            return (float)RadianToDegree(Math.Atan2(Vector3.Cross(vec1, vec2).Length(), Vector3.Dot(vec1, vec2)));

        }

        private static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        public int GetScriptDetections()
        {
            return _scripting;
        }

        public string GetName()
        {
            return "L# Common";
        }

        public void Initialize(AIHeroClient hero, DetectorSetting setting = DetectorSetting.Safe)
        {
            _hero = hero;
            ApplySetting(setting);
        }

        public void ApplySetting(DetectorSetting setting)
        {
            _currentSetting = setting == DetectorSetting.Preferred ? DetectorSetting.Safe : setting;
        }
    }
}
