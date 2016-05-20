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
    class SacOrbwalkerDetector : IDetector
    {
        private int _scripting;
        private AIHeroClient _hero;
        private readonly List<DataSet> _recentData = new List<DataSet>();
        private readonly Dictionary<DetectorSetting, float[]> _settingValues = new Dictionary<DetectorSetting, float[]> { { DetectorSetting.Safe, new[] { 0.125f, 90 } }, { DetectorSetting.AntiHumanizer, new[] { 0.5f, 80 } } };
        private DetectorSetting _currentSetting;


        struct DataSet
        {
            public Vector3 Position;
            public float Time;
            public float Distance;

            public static DataSet Create(Vector3 position, AIHeroClient sender)
            {
                return new DataSet { Position = position, Distance = sender.Position.LSDistance(position), Time = Game.Time };
            }
        }

        public void FeedData(Vector3 targetPos)
        {
            if (_recentData.Count >= 5)
            {
                _recentData.RemoveAt(0);
                _recentData.Add(DataSet.Create(targetPos, _hero));
            }
            else
            {
                _recentData.Add(DataSet.Create(targetPos, _hero));
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

                if (min >= 445 && max <= 475 && max - min < 30) // sac always clicks in a certain range
                {
                    _scripting++;
                }
            }
        }

        public int GetScriptDetections()
        {
            return _scripting;
        }

        public string GetName()
        {
            return "BoL SAC";
        }

        public void Initialize(AIHeroClient hero, DetectorSetting setting = DetectorSetting.Preferred)
        {
            _hero = hero;
            ApplySetting(setting);
        }

        public void ApplySetting(DetectorSetting setting)
        {
            _currentSetting = setting == DetectorSetting.Preferred ? DetectorSetting.AntiHumanizer : setting;
        }
    }
}
