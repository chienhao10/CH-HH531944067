using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using SharpDX;
using EloBuddy;

namespace TheCheater
{
    public enum DetectorSetting { Preferred, Safe, AntiHumanizer}
    interface IDetector
    {
        void Initialize(AIHeroClient hero, DetectorSetting setting = DetectorSetting.Safe);
        void ApplySetting(DetectorSetting setting);
        void FeedData(Vector3 targetPos);
        int GetScriptDetections();
        string GetName();
    }
}
